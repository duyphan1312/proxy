using CsvHelper;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
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
            var appData = GetAppData();
            if (appData != null)
            {
                SchoolSuccessIcon.Visibility = appData.IsSchool ? Visibility.Visible : Visibility.Collapsed;
                HomeSuccessIcon.Visibility = appData.IsHome ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var setting = new ProxyServerSettingDialog();

            setting.Owner = this;
            setting.ShowDialog();
        }

        private void SchoolButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdministrator())
            {
                var status = SchoolFunction();
                if (status)
                {
                    SchoolSuccessIcon.Visibility = Visibility.Visible;
                    HomeSuccessIcon.Visibility = Visibility.Collapsed;
                    MessageBox.Show("" + Constant.DISPLAY_SUCCESS_CONNECT, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);
                    SetAppData(new AppData() { IsHome = false, IsSchool = true });
                }
                else
                {
                    var result = MessageBox.Show("" + Constant.DISPLAY_ERROR_BUTTON, "" + Constant.ERROR, MessageBoxButton.YesNo, MessageBoxImage.Error);

                    if (result == MessageBoxResult.Yes)
                    {
                        var setting = new ProxyServerSettingDialog();

                        setting.Owner = this;
                        setting.ShowDialog();
                    }
                }
            }
            else
            {
                MessageBox.Show(Constant.DISPLAY_REQUIRED_ADMIN_ROLE_MESSAGE, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdministrator())
            {
                var status = HomeFunction();
                if (status)
                {
                    HomeSuccessIcon.Visibility = Visibility.Visible;
                    SchoolSuccessIcon.Visibility = Visibility.Collapsed;
                    MessageBox.Show("" + Constant.DISPLAY_SUCCESS_CONNECT, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);
                    SetAppData(new AppData() { IsHome = true, IsSchool = false });
                }
                else
                {
                    var result = MessageBox.Show("" + Constant.DISPLAY_ERROR_BUTTON, "" + Constant.ERROR, MessageBoxButton.YesNo, MessageBoxImage.Error);

                    if (result == MessageBoxResult.Yes)
                    {
                        var setting = new ProxyServerSettingDialog();

                        setting.Owner = this;
                        setting.ShowDialog();
                    }
                }
            }
            else
            {
                MessageBox.Show(Constant.DISPLAY_REQUIRED_ADMIN_ROLE_MESSAGE, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool SchoolFunction()
        {
            var setting = ReadSetting();
            var staticIP = ReadStaticIP();
            bool isSetProxy = false;
            bool isSetIP = false;
            if (setting != null)
            {
                isSetProxy = SetProxy(setting.ProxyServer, "");
            }
            if (staticIP != null)
            {
                isSetIP = SetStaticIP(Constant.WIFI_ADAPTER_NAME, staticIP.IP, staticIP.Gateway, staticIP.DNS);
            }
            
            return isSetProxy && isSetIP;
        }

        private bool HomeFunction()
        {
            ProxySetting setting = ReadSetting();
            bool isSetProxy = false;
            if (setting != null)
            {
                isSetProxy = SetProxy(Constant.FAKE_PROXY_SERVER, setting.URLList);
            }
            bool isSetDHCP = SetDHCP(Constant.WIFI_ADAPTER_NAME);
            return isSetProxy && isSetDHCP;
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
                log.Error(ex.StackTrace);
                return false;
            }
        }

        private ProxySetting ReadSetting()
        {
            ProxySetting setting = null;
            string file = Constant.SETTING_PATH;
            if (File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file))
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    IEnumerable<ProxySetting> records = csv.GetRecords<ProxySetting>();

                    setting = records.FirstOrDefault();
                }
            }
            return setting;
        }

        private StaticIP ReadStaticIP()
        {
            StaticIP staticIP = null;
            var file = Constant.STATIC_IP_PATH;
            if (File.Exists(file) != false)
            {
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<StaticIP>();
                    staticIP = records.FirstOrDefault(x => x.PCName.ToUpper() == Environment.MachineName.ToUpper());
                }
            }
            return staticIP;
        }

        public static bool IsAdministrator()
        {
            return true;
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private AppData GetAppData()
        {
            var appData = new AppData();
            using (StreamReader r = new StreamReader(Constant.APP_DATA_PATH))
            {
                string json = r.ReadToEnd();
                appData = JsonConvert.DeserializeObject<AppData>(json);
            }
            return appData;
        }

        private void SetAppData(AppData appData)
        {
            string json = JsonConvert.SerializeObject(appData);

            //write string to file
            File.WriteAllText(Constant.APP_DATA_PATH, json);
        }
    }
}
