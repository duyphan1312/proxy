using CsvHelper;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StrongProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        static HttpClient client = new HttpClient();

        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public const int INTERNET_OPTION_PROXY_SETTINGS_CHANGED = 95;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            InitializeComponent();
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                return;
            AppData appData = GetAppData();
            UpdateSettingFromAPI();
            UpdateStaticIPFromAPI();
            if (IsEnableProxy() && appData!= null && appData.IsHome)
            {
                HomeFunction();
                HomeSuccessIcon.Visibility = Visibility.Visible;
            }

            if (!IsEnableProxy() && appData != null && appData.IsSchool)
            {
                SchoolFunction();
                SchoolSuccessIcon.Visibility = Visibility.Visible;
            }

            ProxySetting setting = ReadSetting();
            if (setting != null)
            {
                if (!string.IsNullOrEmpty(setting.HomeLabel))
                {
                    if (setting.HomeLabel.Length <= 6)
                    {
                        txtHomeLabel.Text = setting.HomeLabel;
                    }
                    else
                    {
                        txtHomeLabel.Text = setting.HomeLabel.Substring(0, 6);
                    }
                }
                if (!string.IsNullOrEmpty(setting.SchoolLabel))
                {
                    if (setting.SchoolLabel.Length <= 6)
                    {
                        txtSchoolLabel.Text = setting.SchoolLabel;
                    }
                    else
                    {
                        txtSchoolLabel.Text = setting.SchoolLabel.Substring(0, 6);
                    }
                }
            }
            
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            ProxyServerSettingDialog setting = new ProxyServerSettingDialog();

            setting.Owner = this;
            setting.ShowDialog();
        }

        private void SchoolButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdministrator())
            {
                //bool status = SchoolFunction();

                StaticIP staticIP = ReadStaticIP();
                bool isSetIP = false;
                bool isDisableProxy = DisableProxy();

                if (!isDisableProxy)
                {
                    MessageBox.Show("" + Constant.DISPLAY_ERROR_BUTTON, "" + Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (staticIP != null)
                {
                    isSetIP = SetStaticIP(Constant.WIFI_ADAPTER_NAME, staticIP.IP, staticIP.SubnetMask, staticIP.Gateway, staticIP.DNS1, staticIP.DNS2);
                }
                else
                {
                    MessageBox.Show("" + Constant.DISPLAY_ERROR_NAMEPC, "" + Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (isDisableProxy && isSetIP)
                {
                    SchoolSuccessIcon.Visibility = Visibility.Visible;
                    HomeSuccessIcon.Visibility = Visibility.Collapsed;
                    //MessageBox.Show("" + Constant.DISPLAY_SUCCESS_CONNECT, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);

                    tbNofication.Text = Constant.DISPLAY_SUCCESS_CONNECT;
                    tbNofication.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#048caf");
                    tbNofication.Visibility = Visibility.Visible;

                    SetAppData(new AppData() { IsHome = false, IsSchool = true });
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
                bool status = HomeFunction();
                if (status)
                {
                    HomeSuccessIcon.Visibility = Visibility.Visible;
                    SchoolSuccessIcon.Visibility = Visibility.Collapsed;
                    //MessageBox.Show("" + Constant.DISPLAY_SUCCESS_CONNECT, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);

                    tbNofication.Text = Constant.DISPLAY_SUCCESS_CONNECT;
                    tbNofication.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#048caf");
                    tbNofication.Visibility = Visibility.Visible;

                    SetAppData(new AppData() { IsHome = true, IsSchool = false });
                }
                else
                {
                    MessageBox.Show("" + Constant.DISPLAY_ERROR_BUTTON, "" + Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);

                    //    setting.Owner = this;
                    //    setting.ShowDialog();
                    //}
                }
            }
            else
            {
                MessageBox.Show("" + Constant.DISPLAY_REQUIRED_ADMIN_ROLE_MESSAGE, "" + Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void UpdateSettingFromAPI()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["APIServer"] + Constant.URL_PROXY_SETTING;
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    ProxySetting proxySetting = JsonConvert.DeserializeObject<ProxySetting>(jsonResponse);

                    StreamWriter writer = new StreamWriter(Constant.SETTING_PATH);

                    CsvWriter csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                    {
                        csv.WriteHeader<ProxySetting>();
                        csv.NextRecord();

                        csv.WriteRecord(proxySetting);
                        csv.NextRecord();
                    }

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
            }
            
        }

        private void UpdateStaticIPFromAPI()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["APIServer"] + Constant.URL_STATIC_IP;
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    List<StaticIP> staticIPList = JsonConvert.DeserializeObject<List<StaticIP>>(jsonResponse);

                    StreamWriter writer = new StreamWriter(Constant.STATIC_IP_PATH);

                    CsvWriter csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                    {
                        csv.WriteHeader<StaticIP>();
                        csv.NextRecord();
                        foreach (var item in staticIPList)
                        {
                            csv.WriteRecord(item);
                            csv.NextRecord();
                        }
                    }

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
            }

        }

        private bool SchoolFunction()
        {
            StaticIP staticIP = ReadStaticIP();
            bool isSetIP = false;
            bool isDisableProxy = DisableProxy();
            if (staticIP != null)
            {
                isSetIP = SetStaticIP(Constant.WIFI_ADAPTER_NAME, staticIP.IP, staticIP.SubnetMask, staticIP.Gateway, staticIP.DNS1, staticIP.DNS2);
            }
            else
            {
                MessageBox.Show("" + Constant.DISPLAY_ERROR_NAMEPC, "" + Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isDisableProxy && isSetIP;
        }

        private bool HomeFunction()
        {
            ProxySetting setting = ReadSetting();
            bool isSetProxy = false;
            if (setting != null)
            {
                isSetProxy = SetProxy(setting.ProxyServer, setting.URLList);
            }
            bool isSetDHCP = SetDHCP(Constant.WIFI_ADAPTER_NAME);
            return isSetProxy && isSetDHCP;
        }

        private bool IsEnableProxy()
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            const string keyName = userRoot + "\\" + subkey;
            int x = (int) Registry.GetValue(keyName, "ProxyEnable", "1");
            return x == 1;
        }

        private bool DisableProxy()
        {
            try
            {
                const string userRoot = "HKEY_CURRENT_USER";
                const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
                const string keyName = userRoot + "\\" + subkey;

                Registry.SetValue(keyName, "ProxyEnable", 0);

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

        private bool SetProxy(string proxy, string domainList)
        {
            try
            {
                const string userRoot = "HKEY_CURRENT_USER";
                const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
                const string keyName = userRoot + "\\" + subkey;

                Registry.SetValue(keyName, "ProxyEnable", 1);
                Registry.SetValue(keyName, "ProxyServer", proxy);
                Registry.SetValue(keyName, "ProxyOverride", domainList);

                const string subKeyDisable = "Software\\Policies\\Microsoft\\Internet Explorer\\Control Panel";
                const string keyNameDisable = userRoot + "\\" + subKeyDisable;
                Registry.SetValue(keyNameDisable, "Proxy", 1);
                Registry.SetValue(keyNameDisable, "AdvancedTab", 1);

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

        public bool SetStaticIP(string network, string ip, string subnetMask, string gateway, string dns1, string dns2)
        {
            return SetIP("/c netsh interface ip set address \"" + network + "\" static " + ip + " " + subnetMask + " " + gateway + " & netsh interface ip set dns \"" + network + "\" static " + dns1 + " & netsh interface ip set dns \"" + network + "\" static " + dns2 + " index=2");
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
            try
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
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                return null;
            }
        }

        private StaticIP ReadStaticIP()
        {
            try
            {
                StaticIP staticIP = null;
                string file = Constant.STATIC_IP_PATH;
                if (File.Exists(file) != false)
                {
                    using (StreamReader reader = new StreamReader(file))
                    using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        IEnumerable<StaticIP> records = csv.GetRecords<StaticIP>();
                        staticIP = records.FirstOrDefault(x => x.PCName.ToUpper() == Environment.MachineName.ToUpper());
                    }
                }
                return staticIP;
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                return null;
            }
        }

        public static bool IsAdministrator()
        {
            return true;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private AppData GetAppData()
        {
            AppData appData = new AppData();
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
