using Radio_Leech.Model;
using Radio_Leech.ViewModel.Commands;
using Radio_Leech.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Radio_Leech.ViewModel
{
	public class LeechVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private readonly string ROOT = @"http://www.rpgamers.net/radio/data/"; //  + data_# + / +  4 digit number .dat

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
			set { selectedSong = value; if(value != null)PlaySong(value); }
		}

		public bool IsPlaying = false;

		public SearchLinksCommand SearchLinksCommand { get; set; }
        public DownloadCommand DownloadCommand { get; set; }
        public NextCommand NextCommand { get; set; }
        public PauseCommand PauseCommand { get; set; }
        public ObservableCollection<Song> FoundLinks { get; set; }


		private List<Song> filteredSongs = new();

		public List<Song> FilteredSongs
        {
			get => filteredSongs;
			set { filteredSongs = value; OnPropertyChanged(); }
		}


		public LeechVM()
		{
			Status = string.Empty;

			FoundLinks = new();
			

            SearchLinksCommand = new(this);
			DownloadCommand = new(this);
            NextCommand = new(this);
			PauseCommand = new(this);

            ReadSongs();
			StartTimer();
        }

		private void ReadSongs()
		{
			FoundLinks.Clear();
			var allSongs = DatabaseHelper.Read<Song>();
			foreach (var item in allSongs)
				FoundLinks.Add(item);
			Query();
		}

		public void Query()
		{
            FilteredSongs = FoundLinks.OrderBy(s => s.Url).
				Where(x=> x.Game.ToLower().Contains(Search.ToLower())).ToList();
			SongCount = $"{FilteredSongs.Count} songs";
        }

		public Task LookForLinksAsync()
		{
            string dataUrl = Path.Combine(ROOT, "data_");
			for (int i = 0; i < 4000; i++)
			{
				string digit = i.ToString();
				char dataNumber = digit.First();
				while(digit.Length < 4)
					digit = "0" + digit;
				string url = dataUrl + dataNumber + "/" + digit + ".dat";
				_ = ReadSongInfoAsync(url, i);
            }
			//string url = "http://www.rpgamers.net/radio/data/data_2/2819.dat";
            //var task = ReadSongInfoAsync(url, 2819);
            return Task.CompletedTask;
		}

		private async Task ReadSongInfoAsync(string url, int id)
		{
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);
            Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
			using StreamReader sr = new(streamToReadFrom, Encoding.UTF8);
			string line = sr.ReadLine() ?? "";
			line = Decode(line);

			//GetSpacers(line, out string x, out string y, out string w, out string z);
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

			Song song = new(){
                Id = id,
				Url = url,
				Title = title,
				Game = game
            };
            
			var allSongs = DatabaseHelper.Read<Song>();
			if(!allSongs.Select(x => x.Url).Contains(url))
                DatabaseHelper.Insert(song);
			ReadSongs();
        }

		//private static void GetSpacers(string input, out string gameSpacerFirst, out string gameSpacerSecond,
		//						out string titleSpaceFirst, out string titleSpacerSecond)
		//{
  //          gameSpacerFirst = string.Empty;
  //          gameSpacerSecond = string.Empty;
  //          titleSpaceFirst = string.Empty;
  //          titleSpacerSecond = string.Empty;

  //      }

		private static string Decode(string input) => Regex.Replace(input, @"[^\u0020-\u007E]", string.Empty);

		private bool subscribed = false;
        private void PlaySong(Song song)
		{

            if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
            {
                element.Source = new Uri(song.Url);
				element.Play();
				Status = $"{song.Title}";
                
				IsPlaying = true;
				if(!subscribed)
				{
                    element.MediaEnded += Element_MediaEnded;
					subscribed = true;
                }
            }
        }

		private void SetVolume()
		{
            if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
				element.Volume = Volume / 2;

        }

		private void StartTimer()
		{
			DispatcherTimer timer = new()
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			timer.Tick += Timer_Tick;
            timer.Start();
        }

        void Timer_Tick(object? sender, EventArgs e)
        {
            if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
                if (element.Source != null)
					if (element.NaturalDuration.HasTimeSpan)
						Duration = string.Format("{0} / {1}", element.Position.ToString(@"mm\:ss"), element.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

        }

		private void Element_MediaEnded(object sender, RoutedEventArgs e) => PlayRandomSong();

		public void PlayRandomSong()
		{
            Random rand = new();
            PlaySong(FilteredSongs[rand.Next(FilteredSongs.Count)]);
        }

		public static async Task SaveSong(Song song)
		{
            HttpClient client = new();
            using var stream = await client.GetStreamAsync(song.Url);
			using var fileStream = new FileStream($"C:\\Users\\djgoo\\Desktop\\Songs\\{song.Title}01.mpeg", FileMode.CreateNew);
			await stream.CopyToAsync(fileStream);
		}

		public void Pause()
		{
			if (((MainWindow)Application.Current.MainWindow).MyPlayer is MediaElement element)
			{
				if(IsPlaying)
				{
                    element.Pause();
					IsPlaying = false;
                }
				else
				{
                    element.Play();
                    IsPlaying = true;
                }
				
			}
        }
	}
}
