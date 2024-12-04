using System.Diagnostics;
using System.Net;
using amLogger;
using amSecrets;
using WindowsFirewallHelper;
using WindowsFirewallHelper.Addresses;

namespace FireWall
{
    public class Rules
    {
        static IFirewallRule? getRdp()
        {
            return FirewallManager.Instance
                .Rules.FirstOrDefault(r => r.Name == Secrets.RemoteDesktopRuleName);
        }
        static void RdpDisable()
        {
            var rdp = getRdp();
            if (rdp != null)
                rdp.IsEnable = false;
        }
        static void RdpEnable()
        {
            var rdp = getRdp();
            if (rdp != null)
                rdp.IsEnable = true;
        }
        static bool IsEnable { get
            {
                var rdp = getRdp();
                return rdp?.IsEnable ?? false;
            }
        }
        public static bool RdpToggle()
        {
            if (IsEnable)
                RdpDisable();
            else
                RdpEnable();

            return IsEnable;
        }
        public static List<string> RdpIpList
        {
            get
            {
                var list = new List<string>();
                var rdp = getRdp();
                if (rdp != null)
                    foreach (var addr in rdp.RemoteAddresses)
                        list.Add(addr.ToString());
                
                return list;
            }
        }
        public static void RdpAddIP2(string ip)
        {
            int iip = BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes().Reverse().ToArray(), 0);
            var rdp = getRdp();
            if (rdp != null)
            {
                var las = rdp.RemoteAddresses;
                SingleIP ipa = new(iip);
                las.Append(ipa);
                rdp.RemoteAddresses = las;
            }
        }
        public static void RdpAddIP(string ip)
        {
            var s = File.ReadAllText(Secrets.FireWallScriptPath + "IpAdd.ps1");
            s = s.Replace("{rule}", Secrets.RemoteDesktopRuleName);
            s = s.Replace("{ip}", ip);
            var r = ExecuteCommand(s);
            if (r.Length > 0) Log.Error("RdpAddIP", r);
        }
        public static void RdpDelIP(string ip)
        {
            var s = File.ReadAllText(Secrets.FireWallScriptPath + "IpDel.ps1");
            s = s.Replace("{rule}", Secrets.RemoteDesktopRuleName);
            s = s.Replace("{ip}", ip);
            var r = ExecuteCommand(s);
            if (r.Length > 0) Log.Error("RdpAddIP", r);

        }
        static string ExecuteCommand(string command)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.Arguments = $"-Command \"{command}\"";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;

            var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            return output;
        }
    }
}
