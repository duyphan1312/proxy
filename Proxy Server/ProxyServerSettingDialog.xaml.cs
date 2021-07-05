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
            if (!CheckExistFile())
            {
                //File không tồn tại
                MessageBox.Show("File không tồn tại");
            }
            else
            {
                if (IsTextFileEmpty())
                {

                    //File csv có dữ liệu
                    var streamReader = File.OpenText("ProxySetting.csv");

                    var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

                    while (csvReader.Read())
                    {
                        var proxyServer = csvReader.GetField(0);
                        txtProxyServer.Text = proxyServer;

                        var urlList = csvReader.GetField(1);
                        txtURLList.Text = urlList;

                    }
                }
                else
                {
                    //File csv không có dữ liệu
                }
            }
        }

        private bool CheckExistFile()
        {
            if (File.Exists("ProxySetting.csv"))
            {
                return true;
            }

            return false;
        }

        private bool IsTextFileEmpty()
        {
            var info = new FileInfo("ProxySetting.csv");

            if (info.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String path = @"ProxySetting.csv";
            List<String> lines = new List<String>();

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!line.Contains("ProxyServer"))
                        {
                            String[] split = line.Split(',');
                            split[0] = txtProxyServer.Text;
                            split[1] = txtURLList.Text;
                            line = String.Join(",", split);
                        }

                        lines.Add(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (String line in lines)
                        writer.WriteLine(line);
                }
            }
        }
    }
}
