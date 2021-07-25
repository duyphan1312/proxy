using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProxySetting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //add some bootstrap or startup logic 
            if (CheckExitsPassword())
            {
                EnterPassword password = new EnterPassword();

                password.Show();
            }
            else
            {
                PasswordSetting password = new PasswordSetting();

                password.Show();
            }
        }

        private bool CheckExitsPassword()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            if (!string.IsNullOrEmpty(data["Password"]["pass"]))
            {
                return true;
            }

            return false;
        }
    }
}
