namespace amSecrets;

public class Secrets
{
    static public string TelebotUsersDbPath
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\AmTelebotUsersDbPath.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File with Sql Connection is not found");

            return cs;
        }
    }
    static public string AmRdpManagerTelegramBot_Token
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\AmRdpManagerTelegramBot_Token.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File with Sql Connection is not found");

            return cs;
        }
    }
    static public string AmRdpManagerTelegramBot_Server
    {
        get
        {
            string server;
            string path = "D:\\Projects\\Common\\Secrets\\AmRdpManagerTelegramBot_Server.txt";
            if (File.Exists(path))
            {
                server = File.ReadAllText(path);
            }
            else
                throw new Exception("File with Sql Connection is not found");

            return server;
        }
    }
    static public string FireWallScriptPath
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\AmFireWallScriptPath.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File with FireWall Script Path is not found");

            return cs;
        }
    }
    static public string RemoteDesktopRuleName
    {
        get
        {
            string cs = "";
            string path = "D:\\Projects\\Common\\Secrets\\AmRemoteDesktopRuleName.txt";
            if (File.Exists(path))
            {
                cs = File.ReadAllText(path);
            }
            else
                throw new Exception("File with RemoteDesktop Rule Name is not found");

            return cs;
        }
    }
}
