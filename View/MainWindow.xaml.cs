using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace Radio_Leech
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string message = $"{Application.Current.MainWindow.Title}\n" +
                             $"Created by: Derek Gooding\n" +
                             $"©2023\n" +
                             $"Version 0.9.0 beta";
            MessageBox.Show(message);
        }

        private void Github_Click(object sender, RoutedEventArgs e) => OpenBrowser("https://github.com/DerekGooding/RPGGamers-Radio-Premium");

        private void RadioSite_Click(object sender, RoutedEventArgs e) => OpenBrowser("http://www.rpgamers.net/radio/");

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        
    }
}
