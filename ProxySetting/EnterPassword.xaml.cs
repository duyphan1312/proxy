using IniParser;
using IniParser.Model;
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

namespace ProxySetting
{
    /// <summary>
    /// Interaction logic for EnterPassword.xaml
    /// </summary>
    public partial class EnterPassword : Window
    {
        public EnterPassword()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var file = StrongProxy.Constant.CONFIG_PATH;

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(file);

            string password = data["Password"]["pass"];

            if (string.Compare(txtPassword.Text, EncryptPassword.Decrypt(password)) == 0)
            {
                ProxyServerSettingDialog setting = new ProxyServerSettingDialog();

                this.Hide();

                setting.Owner = this;

                setting.Show();
            }
            else
            {
                MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_PASSWORD, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
