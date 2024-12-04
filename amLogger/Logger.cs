namespace amLogger;

public enum Level
{
    Trace = 0,
    Debug,
    Info,
    Warn,
    Error,
    Fatal,
    None
}
public class Log
{
    public int id;          // Some ID
    public int type;        // Type of message
    public Level lvl;
    public string src = ""; // Source
    public string msg = ""; // Message
    public void Send()
    {
        Logger.Write(this);
    }
    public override string ToString()
    {
        var d = DateTime.Now.ToShortTimeString();
        return d + " - " + src + " - " + msg;
    }
    public static void Trace(string source, string message)
    {
        Logger.Write(new Log() { src = source, msg = message });
    }
    public static void Trace(int exch_id, string source, string message)
    {
        Logger.Write(new Log() { id = exch_id, src = source, msg = message });
    }
    public static void Info(string source, string message)
    {
        Logger.Write(new Log() { lvl = Level.Info, src = source, msg = message });
    }
    public static void Info(int exch_id, string source, string message)
    {
        Logger.Write(new Log() { id = exch_id, lvl = Level.Info, src = source, msg = message });
    }
    public static void Warn(string source, string message)
    {
        Logger.Write(new Log() { lvl = Level.Info, src = source, msg = message });
    }
    public static void Warn(int exch_id, string source, string message)
    {
        Logger.Write(new Log() { id = exch_id, lvl = Level.Warn, src = source, msg = message });
    }
    public static void Error(string source, string message)
    {
        Logger.Write(new Log() { lvl = Level.Error, src = source, msg = message });
    }
    public static void Error(int exch_id, string source, string message)
    {
        Logger.Write(new Log() { id = exch_id, lvl = Level.Error, src = source, msg = message });
    }
    public static void Fatal(string source, string message)
    {
        Logger.Write(new Log() { lvl = Level.Fatal, src = source, msg = message });
    }
    public static void Fatal(int exch_id, string source, string message)
    {
        Logger.Write(new Log() { id = exch_id, lvl = Level.Fatal, src = source, msg = message });
    }
}
public class Logger
{
    static Action<Log>? _write;
    public static void Write(Log msg) => _write?.Invoke(msg);

    static object _lockFlag = new object();
    static Logger? _instance;

    public static Logger Instance
    {
        get
        {
            lock (_lockFlag)
            {
                if (_instance == null)
                    _instance = new Logger();
            }
            return _instance;
        }
    }
    public void Init(Action<Log> write)
    {
        _write = write;
    }
}

