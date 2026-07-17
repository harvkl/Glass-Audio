using System.Runtime.CompilerServices;
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
using System;

namespace Glass_Audio;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    //new object of our audio manager.cs
    private readonly AudioManager _audioManager = new();

    //new object of our volume manager.cs
    private readonly VolumeManager _volumeManager = new();

    // flag to see the diff between user/code changing the slider
    private bool _isUpdatingSlider = false;

    public MainWindow()
    {
        InitializeComponent();

        SetMainWindowPosition(); //sets main window at right bottom corner of your screen

        Loaded += MainWindow_Loaded;
    }

    //method is calling when window is fully ready
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // defining and configuring all the Bitmaps we need
        BitmapImage previewBM = new BitmapImage();
        BitmapImage playBM = new BitmapImage();
        BitmapImage pauseBM = new BitmapImage();

        previewBM.BeginInit();
        previewBM.UriSource = new Uri(@"/resources/media/PreviewImage.png", UriKind.Relative);
        previewBM.EndInit();

        playBM.BeginInit();
        playBM.UriSource = new Uri(@"/resources/media/Play circle.png", UriKind.Relative);
        playBM.EndInit();

        pauseBM.BeginInit();
        pauseBM.UriSource = new Uri(@"/resources/media/Pause.png", UriKind.Relative);
        pauseBM.EndInit();

        // processing the text
        _audioManager.TrackInfoChanged += info =>
        {
            if (info.Contains(" ` "))
            {
                string[] temp = info.Split('`'); //temp[1] - title, 0 - artist
                Dispatcher.Invoke(() => FirstLabel.Content = temp[1]);
                Dispatcher.Invoke(() => SecondLabel.Content = temp[0]);
            }
            else
            {
                Dispatcher.Invoke(() => FirstLabel.Content = info); //we will get only title if there is no "-" then there is no artist
                Dispatcher.Invoke(() => SecondLabel.Content = info);
            }
        };

        // processing the image
        _audioManager.ImageChanged += image =>
        {

            Dispatcher.Invoke(() =>
            {
                if (image != null)
                {
                    Preview.Source = image;
                }
                else
                {
                    Preview.Source = previewBM; //setting our default image
                }
            });
        };

        // processing play/pause status
        _audioManager.IsPlayingChanged += isPlaying =>
        {


            Dispatcher.Invoke(() => 
            {
                if (isPlaying)
                {
                    this.PlayPauseImg.Source = pauseBM;
                }

                else
                {
                    this.PlayPauseImg.Source = playBM;
                }

            });
        };

        await _audioManager.InitializeAsync();

        ////////////////////
        // VOLUME MANAGER //
        ////////////////////

        VolumeSlider.Value = _volumeManager.GetVolume();
        UpdateMuteIcon(_volumeManager.IsMuted());

        // following the change of volume in Windows
        _volumeManager.VolumeChanged += volume =>
        {
            Dispatcher.Invoke(() =>
            {
                _isUpdatingSlider = true;
                VolumeSlider.Value = volume;
                _isUpdatingSlider = false;
            });
        };

        // following the mute status in Windows
        _volumeManager.MuteChanged += isMuted =>
        {
            Dispatcher.Invoke(() => UpdateMuteIcon(isMuted));
        };
    }


    // volume events' processors
    private void VolumeSlider_ValueChanged(object sender, EventArgs e)
    {
        if (!_isUpdatingSlider && IsLoaded)
        {
            _volumeManager.SetVolume(VolumeSlider.Value);
        }
    }

    private void MuteClick(object sender, RoutedEventArgs e)
    {
        _volumeManager.ToogleMute();
    }

    private void UpdateMuteIcon(bool isMuted)
    {
        // defining and configuring all the Bitmaps we need
        BitmapImage volume1 = new BitmapImage();
        BitmapImage volume2 = new BitmapImage();
        BitmapImage volumeX = new BitmapImage();

        volume1.BeginInit();
        volume1.UriSource = new Uri(@"/resources/media/Volume 1.png", UriKind.Relative);
        volume1.EndInit();

        volume2.BeginInit();
        volume2.UriSource = new Uri(@"/resources/media/Volume 2.png", UriKind.Relative);
        volume2.EndInit();

        volumeX.BeginInit();
        volumeX.UriSource = new Uri(@"/resources/media/Volume x.png", UriKind.Relative);
        volumeX.EndInit();

        if (isMuted || VolumeSlider.Value == 0)
        {
            VolumeIcon.Source = volumeX;
        }
        else if (!isMuted && VolumeSlider.Value < 60)
        {
            VolumeIcon.Source = volume1;
        }
        else if (!isMuted && VolumeSlider.Value > 60)
        {
            VolumeIcon.Source = volume2;
        }
    }

    private async void PlayPauseClick(object sender, RoutedEventArgs e)
    {
        await _audioManager.TogglePlayPauseAsync();
    }

    private async void NextClick(object sender, RoutedEventArgs e)
    {
        await _audioManager.NextAsync();
    }

    private async void PrevClick(object sender, RoutedEventArgs e)
    {
        await _audioManager.PrevAsync();
    }

    //new object of our settings window
    SettingsWindow _settingsWindow = new SettingsWindow();

    private void ButtonClick_Settings(object sender, RoutedEventArgs e)
    {
        //MessageBox.Show("Made by @harvkl");

        SetSettingsWindowPosition(); //sets settings window pos based on main window of your screen

        if (_settingsWindow.IsVisible)
        {
            _settingsWindow.Hide();
        }

        else
        {
            _settingsWindow.Show();
        }
    }

    private void ButtonClick_CollapseExpand(object sender, RoutedEventArgs e)
    {
        //MessageBox.Show("Made by @harvkl");

        if (this.CollapseImgButton.Source.ToString() == "pack://application:,,,/resources/media/collapse.png") // if we have expanded main window
        {
            ExpandOrCollapseWindow("collapse");
        }
        else // if we have collapsed main window
        {
            ExpandOrCollapseWindow("expand");
        }
    }

    private void SetMainWindowPosition()
    {
        this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 25;
        this.Top = SystemParameters.PrimaryScreenHeight - this.Height - 50;
    }

    private void SetSettingsWindowPosition()
    {
        _settingsWindow.Owner = this; // this is necessary to close all sub-windows in case if main window was closed

        _settingsWindow.WindowStartupLocation = WindowStartupLocation.Manual;

        if (SystemParameters.PrimaryScreenWidth > this.Left + this.ActualWidth + 25)
        {
            _settingsWindow.Left = this.Left + this.ActualWidth; //sets the location on right of the main window
            _settingsWindow.Top = this.Top;
        }
        else if(this.Width == 210) //checking if we have an collapsed main window
        {
            _settingsWindow.Left = this.Left - this.ActualWidth - 155; //sets the location on left of the main window
            _settingsWindow.Top = this.Top;
        }
        else
        {
            _settingsWindow.Left = this.Left - this.ActualWidth + 145; //sets the location on left of the main window
            _settingsWindow.Top = this.Top;
        }
    }

    private void ExpandOrCollapseWindow(string option)
    {
        BitmapImage bitmap = new BitmapImage();

        if (option == "collapse")
        {
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"/resources/media/expand.png", UriKind.Relative);
            bitmap.EndInit();
            this.CollapseImgButton.Source = bitmap;

            this.Width = 210;

            this.FirstLabel.Visibility = Visibility.Collapsed;
            this.SettingsButton.Visibility = Visibility.Collapsed;
            this.VolumePanel.Visibility = Visibility.Collapsed;
            this.VolumeButton.Visibility = Visibility.Collapsed;
            this.VolumeIcon.Visibility = Visibility.Collapsed;
            this.VolumeSlider.Visibility = Visibility.Collapsed;

            Grid.SetColumn(this.InteractionPanel, 0);
            Grid.SetRow(this.InteractionPanel, 6);
            Grid.SetColumnSpan(this.InteractionPanel, 7);

            Grid.SetRow(this.CollapseButton, 0);
            Grid.SetColumn(this.CollapseButton, 6);

            Grid.SetColumn(this.Preview, 2);

            Grid.SetColumn(this.SecondLabel, 0);
            Grid.SetRow(this.SecondLabel, 4);
            Grid.SetColumnSpan(this.SecondLabel, 7);

            Grid.SetColumn(this.TextPanel, 0);
            Grid.SetRow(this.TextPanel, 4);
            Grid.SetColumnSpan(this.TextPanel, 7);

            this.CollapseImgButton.Width = 20;
            this.CollapseImgButton.Height = 20;
            this.TextPanel.Width = 210;
            this.SecondLabel.Width = 210;
            this.SecondLabel.Foreground = Brushes.Black;
            this.SecondLabel.FontWeight = FontWeights.DemiBold;

            this.GridMainWindow.ColumnDefinitions.Clear();
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
        }
        else if (option == "expand")
        {
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"/resources/media/collapse.png", UriKind.Relative);
            bitmap.EndInit();
            this.CollapseImgButton.Source = bitmap;

            this.Width = 510;

            this.FirstLabel.Visibility = Visibility.Visible;
            this.SettingsButton.Visibility = Visibility.Visible;
            this.VolumePanel.Visibility = Visibility.Visible;
            this.VolumeButton.Visibility = Visibility.Visible;
            this.VolumeIcon.Visibility = Visibility.Visible;
            this.VolumeSlider.Visibility = Visibility.Visible;

            Grid.SetColumn(this.InteractionPanel, 7);
            Grid.SetRow(this.InteractionPanel, 5);
            Grid.SetColumnSpan(this.InteractionPanel, 6);

            Grid.SetRow(this.CollapseButton, 1);
            Grid.SetColumn(this.CollapseButton, 15);

            Grid.SetColumn(this.Preview, 1);

            Grid.SetColumn(this.SecondLabel, 5);
            Grid.SetRow(this.SecondLabel, 3);
            Grid.SetColumnSpan(this.SecondLabel, 10);

            Grid.SetColumn(this.TextPanel, 5);
            Grid.SetRow(this.TextPanel, 1);
            Grid.SetColumnSpan(this.TextPanel, 10);

            this.CollapseImgButton.Width = 30;
            this.CollapseImgButton.Height = 30;
            this.TextPanel.Width = 300;
            this.SecondLabel.Width = 300;

            var converter = new BrushConverter();
            this.SecondLabel.Foreground = (SolidColorBrush)converter.ConvertFrom("#FF636363");
            this.SecondLabel.FontWeight = FontWeights.Regular;

            this.GridMainWindow.ColumnDefinitions.Clear();
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            this.GridMainWindow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
        }

        SetMainWindowPosition(); //sets main window at right bottom corner of your screen
        SetSettingsWindowPosition(); //sets settings window pos based on main window of your screen
    }
}