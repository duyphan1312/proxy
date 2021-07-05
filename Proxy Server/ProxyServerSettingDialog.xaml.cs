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
                //File csv không tồn tại, tạo một File csv chứa header
                var file = @"ProxySetting.csv";

                using (var stream = File.CreateText(file))
                {
                    string first = "ProxyServer";
                    string second = "ListURL";
                    string csvRow = string.Format("{0},{1}", first, second);

                    stream.WriteLine(csvRow);
                }
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
            var streamReader = File.OpenText("ProxySetting.csv");

            var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            while (csvReader.Read())
            {
                var proxyServer = csvReader.GetField(0);
                if (proxyServer != "" || !(String.IsNullOrEmpty(proxyServer)))
                {
                    return true;
                }              
            }

            return false;
        }


        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            string filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";

            save.Filter = filter;

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
