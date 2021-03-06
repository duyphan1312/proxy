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
                    var result = System.Windows.MessageBox.Show("" + StrongProxy.Constant.DISPLAY_QUESTION_SAVE, "" + StrongProxy.Constant.QUESTION, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var file = StrongProxy.Constant.CONFIG_PATH;

                        var parser = new FileIniDataParser();
                        IniData data = parser.ReadFile(file);

                        string encrypt = EncryptPassword.Encrypt(txtPassword.Password.ToString());

                        data["Password"]["pass"] = encrypt;
                        parser.WriteFile(file, data);

                        MessageBox.Show("" + StrongProxy.Constant.DISPLAY_SUCCESS_SETTINGPASSWORD, "" + StrongProxy.Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);

                        ProxyServerSettingDialog password = new ProxyServerSettingDialog();

                        this.Hide();

                        password.Owner = this;
                        password.Show();
                    }
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
            this.Close();
        }

        private bool CheckRePassword()
        {
            if (string.Compare(txtPassword.Password.ToString(), txtRePassword.Password.ToString()) != 0)
            {
                return false;
            }

            return true;
        }

        private bool CheckPassword()
        {
            if (!string.IsNullOrEmpty(txtPassword.Password.ToString()))
            {
                return true;
            }

            return false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtPassword.Focusable = true;
            txtPassword.Focus();
        }
    }
}
