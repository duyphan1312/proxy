using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProxySetting
{
    /// <summary>
    /// Interaction logic for ProxyServerSettingDialog.xaml
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
            var file = StrongProxy.Constant.SETTING_PATH;

            if (File.Exists(file) == false)
            {
                StreamWriter stream = File.CreateText(file);
                stream.Close();
            }

            using (var reader = new StreamReader(file))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<StrongProxy.ProxySetting>();

                StrongProxy.ProxySetting setting = records.FirstOrDefault();

                if (setting != null)
                {

                    string[] proxyServer = setting.ProxyServer.Split('.', ':');
                    if (proxyServer.Length >= 5)
                    {
                        txtProxyServer.firstBox.Text = proxyServer[0];
                        txtProxyServer.secondBox.Text = proxyServer[1];
                        txtProxyServer.thirdBox.Text = proxyServer[2];
                        txtProxyServer.fourthBox.Text = proxyServer[3];
                        txtProxyServer.fivethBox.Text = proxyServer[4];
                        txtURLList.Text = setting.URLList;
                    }
                }

                reader.Close();
            }

            txtProxyServer.firstBox.Focusable = true;
            txtProxyServer.firstBox.Focus();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputProxyServerValue() == true)
            {
                string strProxyServer = txtProxyServer.firstBox.Text + "." + txtProxyServer.secondBox.Text + "." + txtProxyServer.thirdBox.Text + "." + txtProxyServer.fourthBox.Text + ":" + txtProxyServer.fivethBox.Text;

                if (CheckInputProxyServer(strProxyServer) == false)
                {
                    System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PROXYSERVER, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (CheckInputURLList(txtURLList.Text) == false)
                    {
                        System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_URLLIST, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                            SaveFileDialog save = new SaveFileDialog();

                            string filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";

                            save.Filter = filter;
                            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            save.FileName = StrongProxy.Constant.FILENAME_EXPORT;


                            if (save.ShowDialog() == true)
                            {
                                filter = save.FileName;

                                var writer = new StreamWriter(save.FileName);

                                var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                                {
                                    csv.WriteHeader<StrongProxy.ProxySetting>();
                                    csv.NextRecord();

                                    string proxyServer = txtProxyServer.firstBox.Text + "." + txtProxyServer.secondBox.Text + "." + txtProxyServer.thirdBox.Text + "." + txtProxyServer.fourthBox.Text + ":" + txtProxyServer.fivethBox.Text;

                                    StrongProxy.ProxySetting newSetting = new StrongProxy.ProxySetting()
                                    {
                                        URLList = txtURLList.Text,
                                        ProxyServer = proxyServer,
                                        HomeLabel = txtHomeLabel.Text,
                                        SchoolLabel = txtSchoolLabel.Text
                                    };

                                    csv.WriteRecord(newSetting);
                                    csv.NextRecord();
                                }

                                writer.Close();
                            }
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PROXYSERVER, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputProxyServerValue() == true)
            {
                string strProxyServer = txtProxyServer.firstBox.Text + "." + txtProxyServer.secondBox.Text + "." + txtProxyServer.thirdBox.Text + "." + txtProxyServer.fourthBox.Text + ":" + txtProxyServer.fivethBox.Text;

                if (CheckInputProxyServer(strProxyServer) == false)
                {
                    System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PROXYSERVER, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (CheckInputURLList(txtURLList.Text) == false)
                    {
                        System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_URLLIST, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        var result = System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_QUESTION_SAVE, "" + StrongProxy.Constant.QUESTION, MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            var file = StrongProxy.Constant.SETTING_PATH;

                            var writer = new StreamWriter(file);

                            var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                            {
                                csv.WriteHeader<StrongProxy.ProxySetting>();
                                csv.NextRecord();

                                string proxyServer = txtProxyServer.firstBox.Text + "." + txtProxyServer.secondBox.Text + "." + txtProxyServer.thirdBox.Text + "." + txtProxyServer.fourthBox.Text + ":" + txtProxyServer.fivethBox.Text;

                                StrongProxy.ProxySetting newSetting = new StrongProxy.ProxySetting()
                                {
                                    URLList = txtURLList.Text,
                                    ProxyServer = proxyServer,
                                    HomeLabel = txtHomeLabel.Text,
                                    SchoolLabel = txtSchoolLabel.Text
                                };

                                csv.WriteRecord(newSetting);
                                csv.NextRecord();
                            }

                            writer.Close();

                            System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_SUCCESS_SAVE, "" + StrongProxy.Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PROXYSERVER, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == true)
            {
                string file = open.FileName;

                string fileExt = System.IO.Path.GetExtension(file);

                if (fileExt != ".csv")
                {
                    System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_FILE, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    using (var reader = new StreamReader(file))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        try
                        {
                            var records = csv.GetRecords<StrongProxy.ProxySetting>();

                            StrongProxy.ProxySetting setting = records.FirstOrDefault();

                            if (setting != null)
                            {
                                if (CheckInputProxyServer(setting.ProxyServer) == false)
                                {
                                    System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PROXYSERVER, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                {
                                    string[] strPServer = setting.ProxyServer.Split('.', ':');

                                    if (Int32.Parse(strPServer[0]) >= 266 || Int32.Parse(strPServer[1]) >= 266 || Int32.Parse(strPServer[2]) >= 266 || Int32.Parse(strPServer[3]) >= 266 || Int32.Parse(strPServer[4]) >= 65536)
                                    {
                                        System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PROXYSERVER, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    else
                                    {
                                        if (CheckInputURLList(setting.URLList) == false)
                                        {
                                            System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_URLLIST, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                        else
                                        {
                                            if (CheckLabel(txtSchoolLabel.Text) && CheckLabel(txtHomeLabel.Text))
                                            {
                                                txtSchoolLabel.Text = setting.SchoolLabel;
                                                txtHomeLabel.Text = setting.HomeLabel;
                                                string[] proxyServer = setting.ProxyServer.Split('.', ':');
                                                txtProxyServer.firstBox.Text = proxyServer[0];
                                                txtProxyServer.secondBox.Text = proxyServer[1];
                                                txtProxyServer.thirdBox.Text = proxyServer[2];
                                                txtProxyServer.fourthBox.Text = proxyServer[3];
                                                txtProxyServer.fivethBox.Text = proxyServer[4];
                                                txtURLList.Text = setting.URLList; ;

                                                reader.Close();

                                                var fileOld = StrongProxy.Constant.SETTING_PATH;

                                                var writer = new StreamWriter(fileOld);

                                                var csvOld = new CsvWriter(writer, CultureInfo.CurrentCulture);
                                                {
                                                    csvOld.WriteHeader<StrongProxy.ProxySetting>();
                                                    csvOld.NextRecord();

                                                    string prxServer = txtProxyServer.firstBox.Text + "." + txtProxyServer.secondBox.Text + "." + txtProxyServer.thirdBox.Text + "." + txtProxyServer.fourthBox.Text + ":" + txtProxyServer.fivethBox.Text;

                                                    StrongProxy.ProxySetting newSetting = new StrongProxy.ProxySetting()
                                                    {
                                                        URLList = txtURLList.Text,
                                                        ProxyServer = prxServer,
                                                        HomeLabel = txtHomeLabel.Text,
                                                        SchoolLabel = txtSchoolLabel.Text
                                                    };

                                                    csvOld.WriteRecord(newSetting);
                                                    csvOld.NextRecord();
                                                }

                                                writer.Close();
                                            }
                                            else
                                            {
                                                System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_LABEL, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                                            }

                                        }
                                    }

                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_VALUE, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                                txtProxyServer.firstBox.Text = "";
                                txtProxyServer.secondBox.Text = "";
                                txtProxyServer.thirdBox.Text = "";
                                txtProxyServer.fourthBox.Text = "";
                                txtProxyServer.fivethBox.Text = "";
                                txtURLList.Text = "";
                                txtHomeLabel.Text = "";
                                txtSchoolLabel.Text = "";
                            }
                        }
                        catch
                        {

                            System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_FILE, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private bool CheckInputProxyServer(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var regex = @"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}";

            Match match = Regex.Match(input, regex);

            if (match.Success)
            {
                return true;
            }

            return false;
        }

        private bool CheckInputProxyServerValue()
        {
            if ((string.IsNullOrEmpty(txtProxyServer.firstBox.Text) || string.IsNullOrEmpty(txtProxyServer.secondBox.Text) || string.IsNullOrEmpty(txtProxyServer.thirdBox.Text) || string.IsNullOrEmpty(txtProxyServer.fourthBox.Text) || string.IsNullOrEmpty(txtProxyServer.fivethBox.Text)) || Int32.Parse(txtProxyServer.fivethBox.Text) >= 65536)
            {
                return false;
            }

            return true;
        }

        private bool CheckInputURLList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            if (input.Contains(";"))
            {
                string[] arrListURL = input.Split(';');

                int countCheck = 0;

                foreach (var item in arrListURL)
                {
                    var regex = @"^(http[s]?:\/\/|[a-z]*\.[a-z]{3}\.[a-z]{2})([a-z]*\.[a-z]{3})|([a-z]*\.[a-z]*\.[a-z]{3}\.[a-z]{2})|([a-z]+\.[a-z]{2})";

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
                var regex = @"^(http[s]?:\/\/|[a-z]*\.[a-z]{3}\.[a-z]{2})([a-z]*\.[a-z]{3})|([a-z]*\.[a-z]*\.[a-z]{3}\.[a-z]{2})|([a-z]+\.[a-z]{2})";

                Match match = Regex.Match(input, regex);

                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckLabel(string input)
        {
            if (input.Length >= 7)
            {
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
