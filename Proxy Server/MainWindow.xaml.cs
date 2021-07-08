using CsvHelper;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proxy_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public const int INTERNET_OPTION_PROXY_SETTINGS_CHANGED = 95;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var setting = new ProxyServerSettingDialog();

            setting.Owner = this;
            log.Info("ttaahsdasda");
            setting.ShowDialog();
        }

        private void SchoolButton_Click(object sender, RoutedEventArgs e)
        {
            var status = SchoolFunction();
            if (status)
            {
                SchoolSuccessIcon.Visibility = Visibility.Visible;
                HomeSuccessIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var status = HomeFunction();
            if (status)
            {
                HomeSuccessIcon.Visibility = Visibility.Visible;
                SchoolSuccessIcon.Visibility = Visibility.Collapsed;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool SchoolFunction()
        {
            var setting = ReadSetting();
            bool isSetProxy = false;
            if (setting != null)
            {
                isSetProxy = SetProxy(setting.ProxyServer, "");
            }
            bool isSetIP = SetStaticIP(Constant.WIFI_ADAPTER_NAME, "192.168.10.10", "255.255.255.0", "192.168.1.1");
            return (isSetProxy && isSetIP);
        }

        private bool HomeFunction()
        {
            var setting = ReadSetting();
            bool isSetProxy = false;
            if (setting != null)
            {
                isSetProxy = SetProxy(Constant.FAKE_PROXY_SERVER, setting.URLList);
            }
            bool isSetDHCP = SetDHCP(Constant.WIFI_ADAPTER_NAME);
            return (isSetProxy && isSetDHCP);
        }

        private bool SetProxy(string proxy, string domainList)
        {
            try
            {
                const string userRoot = "HKEY_CURRENT_USER";
                const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
                const string keyName = userRoot + "\\" + subkey;

                Registry.SetValue(keyName, "ProxyEnable", "1");
                Registry.SetValue(keyName, "ProxyServer", proxy);
                Registry.SetValue(keyName, "ProxyOverride", domainList);


                // These lines implement the Interface in the beginning of program 
                // They cause the OS to refresh the settings, causing IP to realy update


                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                return false;
            }
            
        }

        public bool SetStaticIP(string network, string ip, string subnet, string dns)
        {
            return SetIP("/c netsh interface ip set address \"" + network + "\" static " + ip + " " + subnet + " " + dns + " & netsh interface ip set dns \"" + network + "\" static " + dns);
        }
        public bool SetDHCP(string network)
        {
            return SetIP("/c netsh interface ip set address \"" + network + "\" dhcp & netsh interface ip set dns \"" + network + "\" dhcp");
        }

        private bool SetIP(string arg)  //To set IP with elevated cmd prompt
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                psi.UseShellExecute = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.Verb = "runas";
                psi.Arguments = arg;
                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                return false;
            }
        }

        private ProxySetting ReadSetting()
        {
            ProxySetting setting = null;
            var file = Constant.SETTING_PATH;
            if (File.Exists(file) != false)
            {
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<ProxySetting>();

                    setting = records.FirstOrDefault();
                }
            }
            return setting;
        }
    }
}
