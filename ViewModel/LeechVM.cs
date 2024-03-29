﻿using NAudio.Wave;
using NAudio.WaveFormRenderer;
using Radio_Leech.Model;
using Radio_Leech.Model.Database;
using Radio_Leech.Model.Settings;
using Radio_Leech.ViewModel.Commands;
using Radio_Leech.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Radio_Leech.ViewModel
{
    public partial class LeechVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private readonly string ROOT = @"http://www.rpgamers.net/radio/data/"; //  + data_# + / +  4 digit number .dat

        private readonly WaveFormRenderer waveFormRenderer = new();
        public ImageSource? WaveFormSource { get => waveFormSource; set { waveFormSource = value; OnPropertyChanged(); } }
        public double FillWidth { get => fillWidth; set { fillWidth = value; OnPropertyChanged(); } }

        private string status = string.Empty;
        public string Status
        {
            get => status;
            set
            {
                if (value == string.Empty)
                    status = "Ready";
                else
                    status = value;
                OnPropertyChanged();
            }
        }

        private string songCount = string.Empty;
        public string SongCount
        {
            get => songCount;
            set
            {
                if (value == string.Empty)
                    songCount = "No Songs";
                else
                    songCount = value;
                OnPropertyChanged();
            }
        }

        private string duration = string.Empty;
        public string Duration
        {
            get => duration;
            set
            {
                duration = value;
                OnPropertyChanged();
            }
        }

        private double volume = 0.5;
        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                SetVolume();
                OnPropertyChanged();
            }
        }

        private string search = string.Empty;
        public string Search
        {
            get => search;
            set { search = value; Query(); OnPropertyChanged(); }
        }

        private Song? selectedSong = null;
        public Song? SelectedSong
        {
            get => selectedSong;
            set { selectedSong = value; if (value != null) PlaySong(value); }
        }

        private Playlist? selectedPlaylist = null;
        public Playlist? SelectedPlaylist
        {
            get => selectedPlaylist;
            set { selectedPlaylist = value; }
        }

        private bool isPlaying = false;
        public bool IsPlaying
        {
            get => isPlaying;
            set { isPlaying = value; OnPropertyChanged(); }
        }
        private bool isRequesting = true;
        public bool IsRequesting
        {
            get => isRequesting;
            set { isRequesting = value; OnPropertyChanged(); }
        }

        public SaveVolumeCommand SaveVolumeCommand { get; set; }
        public SearchLinksCommand SearchLinksCommand { get; set; }
        public DownloadCommand DownloadCommand { get; set; }
        public DownloadAllCommand DownloadAllCommand { get; set; }
        public PreviousCommand PreviousCommand { get; set; }
        public NextCommand NextCommand { get; set; }
        public PauseCommand PauseCommand { get; set; }
        public CreatePlaylistCommand CreatePlaylistCommand { get; set; }
        public FixTitleCommand FixTitleCommand { get; set; }
        public ClearRequestsCommand ClearRequestsCommand { get; set; }
        public ObservableCollection<Song> FoundLinks { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }


        private List<Song> filteredSongs = new();
        public List<Song> FilteredSongs
        {
            get => filteredSongs;
            set { filteredSongs = value; OnPropertyChanged(); }
        }
        //private List<Song> requestedSongs = new();
        //public List<Song> RequestedSongs
        //{
        //    get => requestedSongs;
        //    set { requestedSongs = value; OnPropertyChanged(); }
        //}
        public ObservableCollection<Song> RequestedSongs { get; set; }
        public Stack<Song> previousSongs = new();


        public LeechVM()
        {
            Status = string.Empty;

            FoundLinks = new();
            RequestedSongs = new();
            Playlists = new();

            SaveVolumeCommand = new(this);
            SearchLinksCommand = new(this);
            DownloadCommand = new(this);
            PreviousCommand = new(this);
            NextCommand = new(this);
            PauseCommand = new(this);
            CreatePlaylistCommand = new(this);
            DownloadAllCommand = new(this);
            FixTitleCommand = new(this);
            ClearRequestsCommand = new(this);

            DatabaseHelper.InitializeFolder();

            ReadSongs();
            ReadPlaylists();
            StartTimer();
            ReadPreferences();

            //Connect to twitch
            BotManager.Init(TwitchInfo.Token, TwitchInfo.Name, this, TwitchInfo.Channel);
        }
        

        public void SongRequest(string request)
        {
            if(!IsRequesting) return;
            var possibleSongs = FoundLinks.Where(x => x.Title != null && x.Title.ToLower().Contains(request.ToLower()));
            if (possibleSongs.Any())
                AddSongRequest(possibleSongs.First());
        }
        private void AddSongRequest(Song song)
        {
            popups.Push("Song added to requests");
            Application.Current.Dispatcher.Invoke(() =>
            {
                RequestedSongs.Add(song);
            });
        }
        public void ClearRequests() => RequestedSongs.Clear();

        public void FixTitle()
        {
            new Thread ( async ()  => 
            {
                var songs = DatabaseHelper.Read<Song>(DatabaseHelper.Target.Database);
                foreach(var song in songs)
                    await FixSongInfo(song);
                ReadSongs();
            }).Start();
        }

        public static async Task FixSongInfo(Song? song)
        {
            if (song == null) return;

            string filePath = @"D:\temp.mp3";
            File.Delete(filePath);
            HttpClient client = new();
            using var stream = await client.GetStreamAsync(song.Url);
            using var fileStream = new FileStream(filePath, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);
            fileStream.Close();
            var tfile = TagLib.File.Create(filePath);
            string title = tfile.Tag.Title;
            string game = tfile.Tag.Album;
            //MessageBox.Show($"{title}\n" +
            //                $"{game}");
            song.Title = title;
            song.Game = game;
            DatabaseHelper.Update(song, DatabaseHelper.Target.Database);

            //using HttpClient client = new();
            //HttpResponseMessage response = await client.GetAsync(song.Url);
            //Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
            //using StreamReader sr = new(streamToReadFrom, Encoding.UTF8);
            //string? info = sr.ReadLine();
            //info += sr.ReadLine();
            //info = Decode(info);
            ////MessageBox.Show(info);

            //var gameSplit = info.Split("TALB", StringSplitOptions.None);
            //string game = string.Empty;
            //if (gameSplit.Length < 2)
            //    game = gameSplit[0];
            //else
            //    game = gameSplit[1].Split("TPE1", StringSplitOptions.None)[0];

            //var titleSplit = info.Split("TIT2", StringSplitOptions.None);
            //string title = string.Empty;
            //if (titleSplit.Length < 2)
            //    title = titleSplit[0];
            //else
            //    title = titleSplit[1].Split("TRCK", StringSplitOptions.None)[0];

            //string removeAtStart = "ID3vTALB";
            //if (title.Contains(removeAtStart))
            //    title = title.Replace(removeAtStart, "");
            //if (game.Contains(removeAtStart))
            //    game = game.Replace(removeAtStart, "");

            //string[] cut = { "TALB", "TXXX", "TRCK", "TPE1", "TYER", "OS", "Xing", "WXXX", "TCON", "TDRC", "COMM" };
            //foreach(var item in cut)
            //{
            //    if(title.Contains(item))
            //        title = title.Split(item, StringSplitOptions.None)[0];
            //    if (game.Contains(item))
            //        game = game.Split(item, StringSplitOptions.None)[0];
            //}

            ////var message = $"Game = {game}\n" +
            ////              $"Title = {title}";
            ////if (MessageBox.Show(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            ////{
            //    song.Title = title;
            //    song.Game = game;
            //    DatabaseHelper.Update(song, DatabaseHelper.Target.Database);
            //    //ReadSongs();
            ////}
        }

        private void ReadPreferences()
        {
            var preferences = DatabaseHelper.Read<UserPreference>(DatabaseHelper.Target.UserPrefs);
            double? volume = preferences.FirstOrDefault(x => x?.Name == SettingNames.Volume, null)?.Percent;
            Volume = volume != null ? (double)volume : 0.25;
        }

        private void ReadPlaylists()
        {
            Playlists.Clear();
            var allPlaylists = DatabaseHelper.Read<Playlist>(DatabaseHelper.Target.UserPrefs);
            if (allPlaylists.Count == 0)
                return;
            foreach (var item in allPlaylists)
                Playlists.Add(item);
        }

        private void ReadSongs()
        {
            FoundLinks.Clear();
            var allSongs = DatabaseHelper.Read<Song>(DatabaseHelper.Target.Database);
            if (allSongs.Count == 0)
            {
                DatabaseHelper.ImportFromOnlineAsync();
                allSongs = DatabaseHelper.Read<Song>(DatabaseHelper.Target.Database);
            }
            if (allSongs.Count == 0)
                return;
            foreach (var item in allSongs)
                FoundLinks.Add(item);
            Query();
        }

        public void Query()
        {
            FilteredSongs = FoundLinks.OrderBy(s => s.Url).
                    Where(x => x.Game != null && x.Game.ToLower().Contains(Search.ToLower())).ToList();
            SongCount = $"{FilteredSongs.Count} songs";
        }

        public Task LookForLinksAsync()
        {
            string dataUrl = Path.Combine(ROOT, "data_");
            for (int i = 0; i < 4000; i++)
            {
                string digit = i.ToString();
                char dataNumber = digit.First();
                while (digit.Length < 4)
                    digit = "0" + digit;
                string url = string.Concat(dataUrl, dataNumber, "/", digit, ".dat");
                _ = ReadSongInfoAsync(url, i);
            }
            return Task.CompletedTask;
        }
        private async Task ReadSongInfoAsync(string url, int id)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);
            Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
            using StreamReader sr = new(streamToReadFrom, Encoding.UTF8);
            string line = sr.ReadLine()?? "";

            line = Decode(line);

            if (line.Contains("DOCTYPE"))
                return;

            var gameSplit = line.Split("TALB", StringSplitOptions.None);
            string game = string.Empty;
            if (gameSplit.Length < 2)
                game = gameSplit[0];
            else
                game = gameSplit[1].Split("TPE1", StringSplitOptions.None)[0];

            var titleSplit = line.Split("TIT2", StringSplitOptions.None);
            string title = string.Empty;
            if (titleSplit.Length < 2)
                title = titleSplit[0];
            else
                title = titleSplit[1].Split("TRCK", StringSplitOptions.None)[0];

            Song song = new()
            {
                Id = id,
                Url = url,
                Title = title,
                Game = game
            };

            var allSongs = DatabaseHelper.Read<Song>(DatabaseHelper.Target.Database);
            if (!allSongs.Select(x => x.Url).Contains(url))
                DatabaseHelper.Insert(song, DatabaseHelper.Target.Database);
            ReadSongs();
        }

        private static string Decode(string input) => MyRegex().Replace(input, string.Empty);

        private bool subscribed = false;
        private void PlaySong(Song? song) => PlaySong(song, false);
        private void PlaySong(Song? song, bool isPrevious)
        {
            //new Thread(async () =>
            //{
            //    await LogSongInfoAsync(song);
            //}).Start();

            if (song == null || song.Url == null) return;
            if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
            {
                element.Source = new Uri(song.Url);
                element.Play();
                SetVolume();
                if (!isPrevious)
                    previousSongs.Push(song);
                CheckHistory();
                Status = $"{song.Game} | {song.Title}";

                IsPlaying = true;
                if (!subscribed)
                {
                    element.MediaEnded += Element_MediaEnded;
                    subscribed = true;
                }
                WaveFormSource = new BitmapImage();
                new Thread(() =>
                {
                    using var waveStream = new AudioFileReader(song.Url);
                    var image = waveFormRenderer.Render(waveStream, new MaxPeakProvider(), new StandardWaveFormRendererSettings() { Width = 1650 });
                    if (image == null) return;
                    using var ms = new MemoryStream();
                    image.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        WaveFormSource = bitmapImage;
                    });
                    
                }).Start();
            }
        }

        

        //private static async Task LogSongInfoAsync(Song? song)
        //{
        //    if(song == null) return;
        //    using HttpClient client = new();
        //    HttpResponseMessage response = await client.GetAsync(song.Url);
        //    Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
        //    using StreamReader sr = new(streamToReadFrom, Encoding.UTF8);
        //    string info = sr.ReadLine();
        //    info += sr.ReadLine();
        //    //info += sr.ReadLine();
        //    info = Decode(info);
        //    using StreamWriter sw = new (@"D:\\log.txt", true);
        //    sw.WriteLine("NEW");
        //    sw.WriteLine(info);
        //}

        private void CheckHistory()
        {
            if (previousSongs.Count < 50) return;
            Stack<Song> temp = new();
            for (int i = 0; i < 10; i++)
                temp.Push(previousSongs.Pop());
            previousSongs.Clear();
            for (int i = 0; i < 10; i++)
                previousSongs.Push(temp.Pop());
        }

        private void SetVolume()
        {
            if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
                element.Volume = Volume;
        }
        public void SetVolumePreference()
        {
            SetPreference(SettingNames.Volume, Volume);
            popups.Push("Volume Level Saved");
        }

        private void StartTimer()
        {
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            timer.Tick += TimerTick;
            timer.Start();
            DispatcherTimer updater = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            updater.Tick += Update;
            updater.Start();
        }

        void TimerTick(object? sender, EventArgs e)
        {
            if (Application.Current.MainWindow != null)
                if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
                    if (element.Source != null)
                        if (element.NaturalDuration.HasTimeSpan)
                        {
                            Duration = string.Format("{0} / {1}", element.Position.ToString(@"mm\:ss"), element.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                            FillWidth = element.Position.TotalMilliseconds / element.NaturalDuration.TimeSpan.TotalMilliseconds * 800;
                        }
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e) => PlayRandomSong();

        public void PlayRandomSong()
        {
            if (RequestedSongs.Count > 0)
            {
                var song = RequestedSongs.First();

                if (FilteredSongs.Any(s => s.Id == song.Id))
                {
                    SelectedSong = FilteredSongs.Where(s => s.Id == song.Id).First(); ;
                    RequestedSongs.Remove(RequestedSongs.First());
                }
            }
            else
            {
                Random rand = new();
                SelectedSong = FilteredSongs[rand.Next(FilteredSongs.Count)];
            }
            PlaySong(SelectedSong);
        }
        public void PlayPrevious()
        {
            if (previousSongs.Count == 0) return;
            PlaySong(previousSongs.Pop(), true);
        }

        public static async Task SaveSong(Song? song)
        {
            if (song == null) return;
            string userRoot = Environment.GetEnvironmentVariable("USERPROFILE") ?? "C:\\";
            string downloadFolder = Path.Combine(userRoot, "Downloads", $"{song.Title}.mp3");

            MessageBox.Show($"Downloading to:\n{downloadFolder}");
            HttpClient client = new();
            using var stream = await client.GetStreamAsync(song.Url);
            using var fileStream = new FileStream(downloadFolder, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);
        }

        public void Pause()
        {
            if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
            {
                if (IsPlaying)
                {
                    element.Pause();
                    IsPlaying = false;
                }
                else
                {
                    if(SelectedSong == null) PlayRandomSong();
                    element.Play();
                    IsPlaying = true;
                }

            }
        }

        public void CreatePlaylist()
        {
            DatabaseHelper.Insert(new Playlist { Name = "New playlist" }, DatabaseHelper.Target.UserPrefs);
            ReadPlaylists();
        }

        #region SetPreferences
        private static void SetPreference(string preferenceName, bool value)
        {
            var target = DatabaseHelper.Read<UserPreference>(DatabaseHelper.Target.UserPrefs).FirstOrDefault(x => x.Name == preferenceName, CreatePreference(preferenceName));
            if (target == null) return;
            target.IsTrue = value;
            DatabaseHelper.Update(target, DatabaseHelper.Target.UserPrefs);
        }
        private static void SetPreference(string preferenceName, double value)
        {
            var target = DatabaseHelper.Read<UserPreference>(DatabaseHelper.Target.UserPrefs).FirstOrDefault(x => x.Name == preferenceName, CreatePreference(preferenceName));
            if (target == null) return;
            target.Percent = value;
            DatabaseHelper.Update(target, DatabaseHelper.Target.UserPrefs);
        }
        private static void SetPreference(string preferenceName, int value)
        {
            var target = DatabaseHelper.Read<UserPreference>(DatabaseHelper.Target.UserPrefs).FirstOrDefault(x => x.Name == preferenceName, CreatePreference(preferenceName));
            if (target == null) return;
            target.Value = value;
            DatabaseHelper.Update(target, DatabaseHelper.Target.UserPrefs);
        }
        private static UserPreference CreatePreference(string name)
        {
            var pref = new UserPreference { Name = name };
            DatabaseHelper.Insert(pref, DatabaseHelper.Target.UserPrefs);
            return pref;
        }
        #endregion



        public double PopupHeight { get => popupHeight; set { popupHeight = value; OnPropertyChanged(); } }
        public double PopupFontSize { get => popupFontSize; set { popupFontSize = value; OnPropertyChanged(); } }
        public string PopupText { get => popupText; set { popupText = value; OnPropertyChanged(); } }
        private readonly Stack<string> popups = new();
        private enum popupState
        {
            Waiting,
            Expanding,
            Collapsing,
            Display
        }
        private popupState PopupState = popupState.Waiting;
        private double popupHeight = -40;
        private double popupFontSize = 1;
        private string popupText = string.Empty;
        private int popupDisplayTick = 0;
        private double fillWidth;
        private ImageSource? waveFormSource;

        private void Update(object? sender, EventArgs e)
        {
            if (PopupState == popupState.Waiting && popups.Any())
            {
                PopupState = popupState.Expanding;
                PopupText = popups.Pop();
            }
            else if (PopupState == popupState.Expanding)
            {
                PopupHeight += 1.2;
                PopupFontSize += 0.5;
                if (PopupHeight >= 0)
                    PopupState = popupState.Display;
            }
            else if (PopupState == popupState.Collapsing)
            {
                PopupHeight -= 1.2;
                PopupFontSize -= 0.5;
                if (PopupHeight <= -30)
                    PopupState = popupState.Waiting;

            }
            else if (PopupState == popupState.Display)
            {
                if (popupDisplayTick++ == 20)
                {
                    PopupState = popupState.Collapsing;
                    popupDisplayTick = 0;
                }
            }
        }


        public void DownloadAll()
        {
            new Thread(async () =>
            {
                await DatabaseHelper.Refresh();
                ReadSongs();
            }).Start();
        }

        //public void DownloadAll()
        //{
        //    Status = "Download all songs....";
        //    new Thread(async () =>
        //    {
        //        HttpClient client = new();
        //        string userRoot = @"D:\\";
        //        int count = 0;
        //        foreach (var song in FoundLinks)
        //        {

        //            if (song == null) return;

        //            string downloadFolder = Path.Combine(userRoot, "Radio", $"song_{count++}.mpeg");

        //            //MessageBox.Show($"Downloading to:\n{downloadFolder}");

        //            using var stream = await client.GetStreamAsync(song.Url);
        //            using var fileStream = new FileStream(downloadFolder, FileMode.CreateNew);
        //            await stream.CopyToAsync(fileStream);
        //        }
        //    }).Start();
        //    Status = string.Empty;
        //}

        [GeneratedRegex("[^\\u0020-\\u007E]")]
        private static partial Regex MyRegex();
    }
}
