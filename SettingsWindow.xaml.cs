using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.Globalization; //needs to get a user's windows country
using System.Security.Policy;

namespace Glass_Audio
{
    /// <summary>
    /// Логика взаимодействия для settingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        public SettingsWindow()
        {
            InitializeComponent();

            string countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpper();
            string user = Environment.UserName; // for future settings window

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri($"/resources/flags/{countryCode}.png", UriKind.Relative);
            bitmap.EndInit();

            this.CountryImg.Source = bitmap;
        }


        private void ButtonClick_PinUnpin(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Made by @harvkl");

            /* IMPORTANT NOTE
                we are getting THE MAIN WINDOW here through type of window, 
                cause this line -> Application.Current.MainWindow doessn't give you MainWindow, it will return the last window you clicked on.
            */

            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); 

            if (main.Topmost == false)
            {
                this.PinButton.Content = "Unpin OnTop";
                main.Topmost = true;
            }
            else 
            {
                this.PinButton.Content = "Pin OnTop";
                main.Topmost = false;
            }

        }
        private void ButtonClick_CloseApp(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Made by @harvkl");
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            main.Close();
        }
    }
}
