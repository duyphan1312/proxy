using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ResetProxy
{
    class Program
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public const int INTERNET_OPTION_PROXY_SETTINGS_CHANGED = 95;
        static void Main(string[] args)
        {
            SetProxy();
            SetDHCP("Wi-Fi");
            Console.WriteLine("All done! Press any key to exit.");
            Console.ReadKey();
        }

        private static bool SetProxy()
        {
            try
            {
                const string userRoot = "HKEY_CURRENT_USER";
                const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
                const string keyName = userRoot + "\\" + subkey;

                Registry.SetValue(keyName, "ProxyEnable", 0);

                const string subKeyDisable = "Software\\Policies\\Microsoft\\Internet Explorer\\Control Panel";
                const string keyNameDisable = userRoot + "\\" + subKeyDisable;
                Registry.SetValue(keyNameDisable, "Proxy", 0);
                Registry.SetValue(keyNameDisable, "AdvancedTab", 0);

                // These lines implement the Interface in the beginning of program 
                // They cause the OS to refresh the settings, causing IP to realy update


                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool SetStaticIP(string network, string ip, string subnet, string dns)
        {
            return SetIP("/c netsh interface ip set address \"" + network + "\" static " + ip + " " + subnet + " " + dns + " & netsh interface ip set dns \"" + network + "\" static " + dns);
        }
        public static bool SetDHCP(string network)
        {
            return SetIP("/c netsh interface ip set address \"" + network + "\" dhcp & netsh interface ip set dns \"" + network + "\" dhcp");
        }

        private static bool SetIP(string arg)  //To set IP with elevated cmd prompt
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas",
                    Arguments = arg
                };
                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
