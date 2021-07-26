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
    /// Interaction logic for PasswordSetting.xaml
    /// </summary>
    public partial class PasswordSetting : Window
    {
        public PasswordSetting()
        {
            InitializeComponent();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPassword())
            {
                if (CheckRePassword())
                {
                    var file = StrongProxy.Constant.CONFIG_PATH;

                    var parser = new FileIniDataParser();
                    IniData data = parser.ReadFile(file);

                    string encrypt = EncryptPassword.Encrypt(txtPassword.Text);

                    data["Password"]["pass"] = encrypt;
                    parser.WriteFile(file, data);

                    MessageBox.Show("" + StrongProxy.Constant.DISPLAY_SUCCESS_SETTINGPASSWORD, "" + StrongProxy.Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);

                    ProxyServerSettingDialog password = new ProxyServerSettingDialog();

                    this.Hide();

                    password.Owner = this;
                    password.Show();
                }
                else
                {
                    MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_REPASSWORD, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("" + StrongProxy.Constant.DISPLAY_ERROR_EMPTYPASSWORD, "" + StrongProxy.Constant.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool CheckRePassword()
        {
            if (string.Compare(txtPassword.Text, txtRePassword.Text) != 0)
            {
                return false;
            }

            return true;
        }

        private bool CheckPassword()
        {
            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                return true;
            }

            return false;
        }
    }
}
