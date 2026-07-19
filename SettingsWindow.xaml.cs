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
            string user = Environment.UserName; // for greetings
            this.GreetingsLabel.Text = $"Hi, {user}!";

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
                this.PinButtonText.Text = "Unpin OnTop";
                main.Topmost = true;
            }
            else 
            {
                this.PinButtonText.Text = "Pin OnTop";
                main.Topmost = false;
            }

        }
        private void ButtonClick_CloseApp(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Made by @harvkl");
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            main.Close();
        }

        // ------------------------------------------------------------------------------------
        // themes switch functions

        private void ButtonClick_ThemeWhite(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            var converter = new BrushConverter();

            main.Background = (SolidColorBrush)converter.ConvertFrom("#FFD9D9D9");
            this.Background = (SolidColorBrush)converter.ConvertFrom("#FFD9D9D9");
        }
        private void ButtonClick_ThemeG1(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            LinearGradientBrush gradientBrush = new LinearGradientBrush();

            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(0, 1);

            Color startColor = (Color)ColorConverter.ConvertFromString("#D0B5B5");
            Color endColor = (Color)ColorConverter.ConvertFromString("#442A6B");

            gradientBrush.GradientStops.Add(new GradientStop(startColor, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(endColor, 1.0));

            main.Background = gradientBrush;
            this.Background = gradientBrush;
        }
        private void ButtonClick_ThemeG2(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            LinearGradientBrush gradientBrush = new LinearGradientBrush();

            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(0, 1);

            Color startColor = (Color)ColorConverter.ConvertFromString("#D0B5B5");
            Color endColor = (Color)ColorConverter.ConvertFromString("#536B2A");

            gradientBrush.GradientStops.Add(new GradientStop(startColor, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(endColor, 1.0));

            main.Background = gradientBrush;
            this.Background = gradientBrush;
        }
        private void ButtonClick_ThemeG3(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            LinearGradientBrush gradientBrush = new LinearGradientBrush();

            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(0, 1);

            Color startColor = (Color)ColorConverter.ConvertFromString("#D0B5B5");
            Color endColor = (Color)ColorConverter.ConvertFromString("#2A526B");

            gradientBrush.GradientStops.Add(new GradientStop(startColor, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(endColor, 1.0));

            main.Background = gradientBrush;
            this.Background = gradientBrush;
        }

        // ----------------------------------------------------------------------------------
        // window positioning functions
        private void ButtonClick_SetAppPos_TL(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/resources/media/top left.png", UriKind.Relative);
            bitmap.EndInit();

            PositionBindingImage.Source = bitmap;

            main.SetMainWindowPosition();
            main.SetSettingsWindowPosition();
        }

        private void ButtonClick_SetAppPos_TR(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/resources/media/top right.png", UriKind.Relative);
            bitmap.EndInit();

            PositionBindingImage.Source = bitmap;

            main.SetMainWindowPosition();
            main.SetSettingsWindowPosition();
        }

        private void ButtonClick_SetAppPos_BL(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/resources/media/bottom left.png", UriKind.Relative);
            bitmap.EndInit();

            PositionBindingImage.Source = bitmap;

            main.SetMainWindowPosition();
            main.SetSettingsWindowPosition();
        }

        private void ButtonClick_SetAppPos_BR(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/resources/media/bottom right.png", UriKind.Relative);
            bitmap.EndInit();

            PositionBindingImage.Source = bitmap;

            main.SetMainWindowPosition();
            main.SetSettingsWindowPosition();
        }

    }
}
