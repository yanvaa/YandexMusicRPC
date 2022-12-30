using DiscordRPC;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;


namespace YandexMusicRPC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        DiscordRpcClient client = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client = new DiscordRpcClient("1011365853645787227");
            client.Initialize();
            var timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            Change();
        }
        private async void Change()
        {
            try
            {
                MusicData data = await new MusicData().GetData();
                OutputText.Text = data.Title + " " + data.Artist;
                OutputImage.Source = data.Thumbnail;
                client.SetPresence(new RichPresence()
                {
                    Details = data.Title,
                    State = data.Artist,
                    Assets = new Assets()
                    {
                        LargeImageKey = "image_large"
                    },
                    Buttons = new Button[]
                    {
                    new Button() { Label = "Послушать", Url = data.url }
                    }
                });
            }
            catch
            {
                return;
            }
        }

        private void StopRPC_Click(object sender, RoutedEventArgs e)
        {
            client.Deinitialize();
            client.Invoke();
            StopRPC.IsEnabled = false;
            StartRPC.IsEnabled = true;
        }

        private void StartRPC_Click(object sender, RoutedEventArgs e)
        {
            client = null;
            client = new DiscordRpcClient("1011365853645787227");
            client.Initialize();
            client.Invoke();
            StopRPC.IsEnabled = true;
            StartRPC.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            string link = e.Uri.AbsoluteUri;
            Process.Start(new ProcessStartInfo("cmd", $"/c start " + link));
            e.Handled = true;
        }

        private void Minimaze_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void TrayIcon_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Visibility = Visibility.Visible;
        }
    }
}
