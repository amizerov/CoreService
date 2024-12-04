using amLogger;
using amSecrets;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace amTelebot;

public class Worker
{
    static (string, int) command = ("", 0);
    static TelegramBotClient bot = new (Secrets.AmRdpManagerTelegramBot_Token);
    static List<long> ChatIds = new List<long>();
    static CancellationTokenSource cts = new CancellationTokenSource();

    public static async Task Start(string msgForStart)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>(), // receive all update types
            DropPendingUpdates = true // do not receive pending updates
        };

        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );

        ChatIds = Db.GetTelebotUsers();
        await SetupAlarmButton(msgForStart);
    }
    static async Task SetupAlarmButton(string msg)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {new KeyboardButton[] { "Alarm" }})
        {
            ResizeKeyboard = true // Уменьшает размер клавиатуры для соответствия количеству кнопок
        };

        foreach (var chatId in ChatIds)
            await bot.SendMessage(chatId, msg, replyMarkup: keyboard);
    }
    public static async Task SendMessageToAll(string msg)
    {
        foreach (var chatId in ChatIds)
            await bot.SendMessage(chatId, msg);
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.Message)
        {
            if (update.Message != null
             && update.Message.Type == MessageType.Text)
            {
                var message = update.Message;
                var cid = message.Chat.Id;
                var txt = message.Text;
                var msg = "";

                if (command.Item1.Length == 5)
                {
                    msg = ProcessIpAddDellCommands(txt);
                }
                else if (txt == "/start")
                {
                    ChatIds = Db.AddTelebotUser(cid, message.Chat.Username ?? "incognito");
                    var ver = Assembly.GetEntryAssembly()!.GetName().Version;
                    msg = $"Hi, you are connected to bot ver.{ver} on {Db.server}";
                }
                else if (txt == "/stop")
                {
                    ChatIds = Db.RemoveTelebotUser(cid);
                    msg = "Goodbye";
                }
                else if (txt == "/help")
                {
                    msg = "Можно написать [1.1.1.1 add] или [1.1.1.1 del], чтобы добавить или удалить IP";
                }
                else if (txt == "Alarm")
                {
                    msg = Alarm();
                }
                else if (txt == "/iplist")
                {
                    await MakeIpListButtons(cid);
                    return;
                }
                else if (txt == "/ipadd")
                {
                    command.Item1 = "ipadd";
                    msg = "Напишите IP адрес который надо добавить";
                }
                else if (txt == "/ipdel")
                {
                    command.Item1 = "ipdel";
                    msg = "Напишите IP адрес который надо удалить";
                }
                else
                {
                    if (TryIpAddDel(txt))
                        msg = "Ok";
                    else
                    {
                        msg = "Unknown command";
                        Log.Error(2, "HandleUpdateAsync", msg);
                    }
                }
                await bot.SendMessage(cid, msg, cancellationToken: cts.Token);
            }
        }
        else if (update.Type == UpdateType.InlineQuery)
        {
            if (update.InlineQuery != null)
            {
                var query = update.InlineQuery.Query;
                var cid = update.InlineQuery.From.Id;
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            if (update.CallbackQuery != null)
            {
                var data = update.CallbackQuery.Data;
                var cid = update.CallbackQuery.From.Id;
                var ip = data;

                if (ip != null)
                {
                    await bot.SendMessage(cid, "Удаляю ...", cancellationToken: cts.Token);
                    FireWall.Rules.RdpDelIP(ip);
                    var msg = $"{ip} адрес удален из белого списка";
                    await bot.SendMessage(cid, msg, cancellationToken: cts.Token);
                }
            }
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        Log.Fatal("Telebot error", $"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }

    public static async Task Stop(string stopMsg)
    {
        await SendMessageToAll(stopMsg);
        cts.Cancel();
    }

    static string Alarm()
    {
        string msg = $"Сервер {Db.server} "
            + (FireWall.Rules.RdpToggle() ? "" : "НЕ")
            + " доступен";

        return msg;
    }
    static bool TryIpAddDel(string? txt)
    {
        if (txt == null) return false;

        var a = txt.ToLower().Split(' ');
        if (a.Length != 2) return false;
        if (!System.Net.IPAddress.TryParse(a[0], out _)) return false;
        if (a[1] != "add" && a[1] != "del") return false;

        if (a[1] == "add")
            FireWall.Rules.RdpAddIP(a[0]);
        else
            FireWall.Rules.RdpDelIP(a[0]);

        return true;
    }
    static string ProcessIpAddDellCommands(string? txt)
    {
        if (txt == null) return "Empty message";
        var msg = "";
        if (command.Item1.Length == 5)
        {
            if (System.Net.IPAddress.TryParse(txt, out _))
            {
                if (command.Item1 == "ipadd")
                {
                    FireWall.Rules.RdpAddIP(txt);
                    msg = $"{txt} адрес добавлен в белый список";
                }
                else if (command.Item1 == "ipdel")
                {
                    FireWall.Rules.RdpDelIP(txt);
                    msg = $"{txt} адрес удален из белого списка";
                }
                command = ("", 0);
            }
            else
            {
                if (command.Item2 < 3)
                {
                    msg = "Неправильный формат IP адреса, попробуй еще раз";
                    command.Item2++;
                }
                else
                {
                    command = ("", 0);
                    msg = "Не получается";
                }
            }
        }
        return msg;
    }
    static async Task MakeIpListButtons(long chatId)
    {
        List<string> btns = FireWall.Rules.RdpIpList;
        List<InlineKeyboardButton> inlineBtnRow = new();
        List<List<InlineKeyboardButton>> inlineBtns = new();
        foreach (var btn in btns)
        {
            if (inlineBtnRow.Count % 2 == 0)
            {
                List<InlineKeyboardButton> r = new();
                foreach (var b in inlineBtnRow) r.Add(b);

                inlineBtns.Add(r);
                inlineBtnRow.Clear();
            }
            var kb = new InlineKeyboardButton(btn);
            kb.CallbackData = btn;
            inlineBtnRow.Add(kb);
        }
        if (inlineBtnRow.Count > 0) inlineBtns.Add(inlineBtnRow);

        InlineKeyboardMarkup inlineCoinBtns = new(inlineBtns);
        await bot.SendMessage(chatId, "При нажатии удалит", replyMarkup: inlineCoinBtns);
    }
}
