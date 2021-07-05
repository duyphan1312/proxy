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
                txtProxyServer.Text = setting.ProxyServer;
                txtURLList.Text = setting.DomainList; ;
            }
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
            //File csv không tồn tại, tạo một File csv chứa header
            var file = Constant.SETTING_PATH;
            var writer = new StreamWriter(file);
            var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
            {
                csv.WriteHeader<ProxySetting>();
                csv.NextRecord();
                ProxySetting newSetting = new ProxySetting()
                {
                    DomainList = txtURLList.Text,
                    ProxyServer = txtProxyServer.Text
                };
                csv.WriteRecord(newSetting);
                csv.NextRecord();
            }
            writer.Close();
        }
    }
}
