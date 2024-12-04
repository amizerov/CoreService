using amLogger;
using amSecrets;

namespace amTelebot;

public class Db 
{
    static string usersDb = GetFile();
    public static string server = Secrets.AmRdpManagerTelegramBot_Server;
    public static List<long> GetTelebotUsers()
    {
        List<long> users = new();
        List<string> ls = File.ReadAllLines(usersDb).ToList();
        foreach(string line in ls)
        {
            try
            {
                var a = line.Split('|');
                var cid = long.Parse(a[0]);
                users.Add(cid);
            }
            catch { Log.Error("GetTelebotUsers", "Error in usersDb"); }
        }
        return users;
    }
    public static List<long> AddTelebotUser(long chatId, string userName = "")
    {
        List<long> users = GetTelebotUsers();
        if(!users.Contains(chatId))
            File.AppendAllLines(usersDb, new string[] { chatId + "|" + userName });

        return GetTelebotUsers();
    }
    public static List<long> RemoveTelebotUser(long chatId)
    {
        List<string> lines = File.ReadAllLines(usersDb).ToList();
        var linetoDelete = lines.Find(l => l.Contains(chatId.ToString()));
        if (linetoDelete != null)
        {
            lines.Remove(linetoDelete);
            File.WriteAllLines(usersDb, lines);
        }
        return GetTelebotUsers();
    }
    static string GetFile()
    {
        var f = Secrets.TelebotUsersDbPath + "Users.txt";

        return f;
    }
}
