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

         private bool CheckExitsPassword()
        {
            INIFILE inif = new INIFILE("config.ini");

            if (!string.IsNullOrEmpty(inif.GetValue("Password", "pass", "null")))
            {
                return true;
            }

            return false;
        }

        private void Password()
        {
            if (CheckExitsPassword())
            {
                EnterPassword password = new EnterPassword();

                password.Owner = this;
                password.ShowDialog();
            }
            else
            {
                PasswordSetting password = new PasswordSetting();

                password.Owner = this;
                password.ShowDialog();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPassword())
            {
                if (CheckRePassword())
                {

                    INIFILE inif = new INIFILE("config.ini");

                    string encrypt = EncryptPassword.Encrypt(txtPassword.Text);

                    inif.SetValue("Password", "pass", encrypt);

                    MessageBox.Show("" + StrongProxy.Constant.DISPLAY_SUCCESS_SETTINGPASSWORD, "" + StrongProxy.Constant.NOTIFICATION, MessageBoxButton.OK, MessageBoxImage.Information);

                    EnterPassword password = new EnterPassword();

                    password.Owner = this;
                    password.ShowDialog();
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
