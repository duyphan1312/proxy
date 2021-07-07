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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proxy_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var setting = new ProxyServerSettingDialog();

            setting.Owner = this;

            setting.ShowDialog();
        }

        private void SchoolButton_Click(object sender, RoutedEventArgs e)
        {
            SchoolSuccessIcon.Visibility = Visibility.Visible;
            HomeSuccessIcon.Visibility = Visibility.Collapsed;
            
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            HomeSuccessIcon.Visibility = Visibility.Visible;
            SchoolSuccessIcon.Visibility = Visibility.Collapsed;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
