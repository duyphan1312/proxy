using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Proxy_Server
{
    /// <summary>
    /// Interaction logic for MainWindow_3.xaml
    /// </summary>
    public partial class ProxyServerSettingDialog : Window
    {

        public ProxyServerSettingDialog()
        {
            InitializeComponent();

            Loaded += ProxyServerSettingDialog_Loaded;
        }

        private void ProxyServerSettingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var file = Constant.SETTING_PATH;

            if (File.Exists(file) == false)
            {
                StreamWriter stream = File.CreateText(file);
                stream.Close();
            }

            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<ProxySetting>();

                ProxySetting setting = records.FirstOrDefault();

                if (setting != null)
                {
                    txtProxyServer.Text = setting.ProxyServer;
                    txtURLList.Text = setting.URLList; ;
                }
            }
        }


        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            string filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";

            save.Filter = filter;
            save.InitialDirectory = @"D:\";
            save.FileName = "ProxyServer";

            const string header = "ProxyServer,URLList";

            string proxyServer = txtProxyServer.Text;
            string urlList = txtURLList.Text;

            string value = proxyServer + "," + urlList;

            StreamWriter writer = null;
            

            if (save.ShowDialog() == true)
            {
                filter = save.FileName;

                writer = new StreamWriter(filter);

                writer.WriteLine(header);

                writer.WriteLine(value);

                writer.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputProxyServer(txtProxyServer.Text) == false)
            {
                MessageBox.Show("Giá trị của Proxy Server lỗi!", "Thông báo", MessageBoxButton.OK);
            }
            else
            {
                if (CheckInputURLList(txtURLList.Text) == false)
                {
                    MessageBox.Show("Giá trị của URL List lỗi!", "Thông báo", MessageBoxButton.OK);
                }
                else
                {
                    //File csv không tồn tại, tạo một File csv chứa header
                    var file = Constant.SETTING_PATH;

                    var writer = new StreamWriter(file);

                    var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                    {
                        csv.WriteHeader<ProxySetting>();
                        csv.NextRecord();

                        ProxySetting newSetting = new ProxySetting()
                        {
                            URLList = txtURLList.Text,
                            ProxyServer = txtProxyServer.Text
                        };

                        csv.WriteRecord(newSetting);
                        csv.NextRecord();
                    }

                    writer.Close();
                }
            }        
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.ShowDialog();

            string file = open.FileName;

            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<ProxySetting>();

                ProxySetting setting = records.FirstOrDefault();

                if (setting != null)
                {
                    if (CheckInputProxyServer(setting.ProxyServer) == false)
                    {
                        MessageBox.Show("Giá trị của Proxy Server lỗi!", "Thông báo", MessageBoxButton.OK);
                    }
                    else
                    {
                        if (CheckInputURLList(setting.URLList) == false)
                        {
                            MessageBox.Show("Giá trị của URL List lỗi!", "Thông báo", MessageBoxButton.OK);
                        }
                        else
                        {
                            txtProxyServer.Text = setting.ProxyServer;
                            txtURLList.Text = setting.URLList; ;

                            var fileOld = Constant.SETTING_PATH;

                            var writer = new StreamWriter(fileOld);

                            var csvOld = new CsvWriter(writer, CultureInfo.CurrentCulture);
                            {
                                csvOld.WriteHeader<ProxySetting>();
                                csvOld.NextRecord();

                                ProxySetting newSetting = new ProxySetting()
                                {
                                    URLList = txtURLList.Text,
                                    ProxyServer = txtProxyServer.Text
                                };

                                csvOld.WriteRecord(newSetting);
                                csvOld.NextRecord();
                            }

                            writer.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File không chứa giá trị Proxy Server và URL List!", "Thông báo", MessageBoxButton.OK);
                    txtProxyServer.Text = "";
                    txtURLList.Text = "";
                }
            }

        }

        private bool CheckInputProxyServer(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var regex = @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):?(\d{ 1,5})?";

            Match match = Regex.Match(input, regex);

            if (match.Success)
            {
                return true;              
            }

            return false;
        }

        private bool CheckInputURLList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (input.Contains(";"))
            {
                string[] arrListURL = input.Split(';');

                int countCheck = 0;

                foreach (var item in arrListURL)
                {
                    var regex = @"^(http[s]?:\/\/|[a-z]*\.[a-z]{3}\.[a-z]{2})([a-z]*\.[a-z]{3})|([a-z]*\.[a-z]*\.[a-z]{3}\.[a-z]{2})|([a-z]+\.[a-z]{3})";

                    Match match = Regex.Match(item, regex);

                    if (match.Success)
                    {
                        countCheck += 1;
                    }
                }

                if (countCheck == arrListURL.Length)
                {
                    return true;
                }
            }
            else
            {
                var regex = @"^(http[s]?:\/\/|[a-z]*\.[a-z]{3}\.[a-z]{2})([a-z]*\.[a-z]{3})|([a-z]*\.[a-z]*\.[a-z]{3}\.[a-z]{2})|([a-z]+\.[a-z]{3})";

                Match match = Regex.Match(input, regex);

                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
