using NAudio.WaveFormRenderer;
using Radio_Leech.Model.Database;
using Radio_Leech.ViewModel;
using Radio_Leech.ViewModel.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Radio_Leech
{
    public partial class MainWindow : Window
    {
        public LeechVM LeechVM
        {
            set { DataContext = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            PrepareViewMenu();
            LeechVM = new();
        }

        private void PrepareViewMenu()
        {
            foreach (TabItem item in MainTabControl.Items)
            {
                MenuItem menuItem = new()
                {
                    Header = item.Header
                };
                menuItem.Click += SelectView_Click;
                menuItem.IsEnabled = item.IsEnabled;
                ViewMenu.Items.Add(menuItem);
                
            }
        }

        private void SelectView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
                foreach (TabItem item in MainTabControl.Items)
                    if (menuItem.Header == item.Header)
                        MainTabControl.SelectedItem = item;
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string message = $"{Application.Current.MainWindow.Title}\n" +
                             $"Created by: Derek Gooding\n" +
                             $"©2023\n" +
                             $"Libertas Infinitum\n" +
                             $"Version 0.9.5";
            
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

        private void ShowDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            string userRoot = Environment.GetEnvironmentVariable("USERPROFILE")?? "C:\\";
            string downloadFolder = Path.Combine(userRoot, "Downloads");
            MessageBox.Show(downloadFolder);
        }

        //readonly WaveFormRenderer waveFormRenderer = new ();
        //private void Sample_Click(object sender, RoutedEventArgs e)
        //{
        //    if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
        //    {
        //        element.Source = new Uri(AudioHelper.SamplePath);
        //        element.Play();
                
        //        using var waveStream = new AudioFileReader(AudioHelper.SamplePath);
        //        var image = waveFormRenderer.Render(waveStream, new MaxPeakProvider(), new StandardWaveFormRendererSettings() { Width = 1600 });
        //        if (image == null) return;
        //        using var ms = new MemoryStream();
        //        image.Save(ms, ImageFormat.Bmp);
        //        ms.Seek(0, SeekOrigin.Begin);

        //        var bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.StreamSource = ms;
        //        bitmapImage.EndInit();

        //        MyWaveImage.Source = bitmapImage;
        //    }
        //}

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(sender is UIElement rectangle)
            {
                var mousePosition = e.MouseDevice.GetPosition(rectangle);
                if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
                    element.Position = element.NaturalDuration.TimeSpan / (800 / mousePosition.X);
            }
            
        }

        private void Fix_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var item in Resources.Values)
                if (item is LeechVM vm)
                    new Thread(async () =>
                    {
                        await LeechVM.FixSongInfo(vm.SelectedSong);
                    }).Start();
        }
    }
}
