using Microsoft.Win32;
using NAudio.WaveFormRenderer;
using Radio_Leech.Model.Database;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Radio_Leech.ViewModel.Helpers
{
    public static class AudioHelper
    {
        public static readonly string SamplePath = Path.Combine(Environment.CurrentDirectory, @"View\Sample\sample-3s.mp3");

        public static ImageSource UpdateGraphic()
        {
            //selectedFile = song.Url;
            RenderWaveform();
            return source;
        }

        private static ImageSource source;
        private static string selectedFile = SamplePath;
        private static string imageFile;
        private static readonly WaveFormRenderer waveFormRenderer = new();
        private static readonly WaveFormRendererSettings standardSettings = new StandardWaveFormRendererSettings();

        //public MainForm()
        //{
        //    InitializeComponent();
        //    waveFormRenderer = new WaveFormRenderer();

        //    standardSettings = new StandardWaveFormRendererSettings() { Name = "Standard" };
        //    var soundcloudOriginalSettings = new SoundCloudOriginalSettings() { Name = "SoundCloud Original" };

        //    var soundCloudLightBlocks = new SoundCloudBlockWaveFormSettings(Color.FromArgb(102, 102, 102), Color.FromArgb(103, 103, 103), Color.FromArgb(179, 179, 179),
        //        Color.FromArgb(218, 218, 218))
        //    { Name = "SoundCloud Light Blocks" };

        //    var soundCloudDarkBlocks = new SoundCloudBlockWaveFormSettings(Color.FromArgb(52, 52, 52), Color.FromArgb(55, 55, 55), Color.FromArgb(154, 154, 154),
        //        Color.FromArgb(204, 204, 204))
        //    { Name = "SoundCloud Darker Blocks" };

        //    var soundCloudOrangeBlocks = new SoundCloudBlockWaveFormSettings(Color.FromArgb(255, 76, 0), Color.FromArgb(255, 52, 2), Color.FromArgb(255, 171, 141),
        //        Color.FromArgb(255, 213, 199))
        //    { Name = "SoundCloud Orange Blocks" };

        //    var topSpacerColor = Color.FromArgb(64, 83, 22, 3);
        //    var soundCloudOrangeTransparentBlocks = new SoundCloudBlockWaveFormSettings(Color.FromArgb(196, 197, 53, 0), topSpacerColor, Color.FromArgb(196, 79, 26, 0),
        //        Color.FromArgb(64, 79, 79, 79))
        //    {
        //        Name = "SoundCloud Orange Transparent Blocks",
        //        PixelsPerPeak = 2,
        //        SpacerPixels = 1,
        //        TopSpacerGradientStartColor = topSpacerColor,
        //        BackgroundColor = Color.Transparent
        //    };

        //    var topSpacerColor2 = Color.FromArgb(64, 224, 224, 224);
        //    var soundCloudGrayTransparentBlocks = new SoundCloudBlockWaveFormSettings(Color.FromArgb(196, 224, 225, 224), topSpacerColor2, Color.FromArgb(196, 128, 128, 128),
        //        Color.FromArgb(64, 128, 128, 128))
        //    {
        //        Name = "SoundCloud Gray Transparent Blocks",
        //        PixelsPerPeak = 2,
        //        SpacerPixels = 1,
        //        TopSpacerGradientStartColor = topSpacerColor2,
        //        BackgroundColor = Color.Transparent
        //    };


        //    buttonBottomColour.BackColor = standardSettings.BottomPeakPen.Color;
        //    buttonTopColour.BackColor = standardSettings.TopPeakPen.Color;
        //    comboBoxPeakCalculationStrategy.Items.Add("Max Absolute Value");
        //    comboBoxPeakCalculationStrategy.Items.Add("Max Rms Value");
        //    comboBoxPeakCalculationStrategy.Items.Add("Sampled Peaks");
        //    comboBoxPeakCalculationStrategy.Items.Add("Scaled Average");
        //    comboBoxPeakCalculationStrategy.SelectedIndex = 0;
        //    comboBoxPeakCalculationStrategy.SelectedIndexChanged += (sender, args) => RenderWaveform();

        //    comboBoxRenderSettings.DisplayMember = "Name";

        //    comboBoxRenderSettings.DataSource = new[]
        //    {
        //        standardSettings,
        //        soundcloudOriginalSettings,
        //        soundCloudLightBlocks,
        //        soundCloudDarkBlocks,
        //        soundCloudOrangeBlocks,
        //        soundCloudOrangeTransparentBlocks,
        //        soundCloudGrayTransparentBlocks
        //    };

        //    comboBoxRenderSettings.SelectedIndex = 5;
        //    comboBoxRenderSettings.SelectedIndexChanged += (sender, args) => RenderWaveform();

        //    labelRendering.Visible = false;
        //}

        private static IPeakProvider GetPeakProvider()
        {
            //switch (comboBoxPeakCalculationStrategy.SelectedIndex)
            //{
            //    case 0:
            //        return new MaxPeakProvider();
            //    case 1:
            //        return new RmsPeakProvider((int)upDownBlockSize.Value);
            //    case 2:
            //        return new SamplingPeakProvider((int)upDownBlockSize.Value);
            //    case 3:
            //        return new AveragePeakProvider(4);
            //    default:
            //        throw new InvalidOperationException("Unknown calculation strategy");
            //}
            return new MaxPeakProvider();
        }

        private static WaveFormRendererSettings GetRendererSettings()
        {
            //var settings = (WaveFormRendererSettings)comboBoxRenderSettings.SelectedItem;
            //settings.TopHeight = (int)upDownTopHeight.Value;
            //settings.BottomHeight = (int)upDownBottomHeight.Value;
            //settings.Width = (int)upDownWidth.Value;
            //settings.DecibelScale = checkBoxDecibels.Checked;
            return standardSettings;
        }

        private static void RenderWaveform()
        {
            if (selectedFile == null) return;
            var settings = GetRendererSettings();
            if (imageFile != null)
            {
                settings.BackgroundImage = new Bitmap(imageFile);
            }
            //pictureBox1.Image = null;
            //labelRendering.Visible = true;
            //Enabled = false;
            var peakProvider = GetPeakProvider();
            Task.Factory.StartNew(() => RenderThreadFunc(peakProvider, settings));
        }

        private static void RenderThreadFunc(IPeakProvider peakProvider, WaveFormRendererSettings settings)
        {
            Image? image = null;
            try
            {
                using var waveStream = new NAudio.Wave.AudioFileReader(selectedFile);
                image = waveFormRenderer.Render(waveStream, new MaxPeakProvider(), new StandardWaveFormRendererSettings());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            new Thread(() => FinishedRender(image)).Start();
        }

        private static void FinishedRender(Image? image)
        {
            if (image == null) return;
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            source = bitmapImage;
            //labelRendering.Visible = false;
            //source = image;
            //Enabled = true;
        }

        //private void OnLoadSoundFileClick(object sender, EventArgs e)
        //{
        //    var ofd = new OpenFileDialog
        //    {
        //        Filter = "MP3 Files|*.mp3|WAV files|*.wav"
        //    };
        //    if (ofd.ShowDialog() == true)
        //    {
        //        selectedFile = ofd.FileName;
        //        RenderWaveform();
        //    }
        //}

        //private void OnButtonSaveClick(object sender, EventArgs e)
        //{
        //    var sfd = new SaveFileDialog
        //    {
        //        Filter = "PNG files|*.png"
        //    };
        //    if (sfd.ShowDialog() == true)
        //    {
        //        //pictureBox1.Image.Save(sfd.FileName);
        //    }
        //}

        //private void OnRefreshImageClick(object sender, EventArgs e)
        //{
        //    RenderWaveform();
        //}

        //private void OnColorButtonClick(object sender, EventArgs e)
        //{
        //    //var button = (Button)sender;
        //    //var cd = new ColorDialog
        //    //{
        //    //    Color = button.BackColor
        //    //};
        //    //if (cd.ShowDialog() == true)
        //    //{
        //    //    button.BackColor = cd.Color;

        //    //    //standardSettings.TopPeakPen = new Pen(buttonTopColour.BackColor);
        //    //    //standardSettings.BottomPeakPen = new Pen(buttonBottomColour.BackColor);
        //    //    RenderWaveform();
        //    //}
        //}

        //private void OnDecibelsCheckedChanged(object sender, EventArgs e)
        //{
        //    RenderWaveform();
        //}

        //private void OnButtonLoadImageClick(object sender, EventArgs e)
        //{
        //    var ofd = new OpenFileDialog
        //    {
        //        Filter = "Image Files|*.bmp;*.png;*.jpg"
        //    };
        //    if (ofd.ShowDialog() == true)
        //    {
        //        imageFile = ofd.FileName;
        //        RenderWaveform();
        //    }
        //}

    }
}
