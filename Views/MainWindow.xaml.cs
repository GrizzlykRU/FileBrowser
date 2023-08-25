using FileBrowser.Utility;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FileBrowser.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TimeSpan _mediaTotalTime;

        private DispatcherTimer mediaTimer;

        public MainWindow()
        {
            InitializeComponent();
            mediaTimer = new DispatcherTimer();
            mediaTimer.Interval = TimeSpan.FromMilliseconds(1);
            mediaTimer.Tick += new EventHandler(TimerTick);
            TextContent.IsReadOnly = true;
            MediaProgress.Visibility = Visibility.Collapsed;
            MediaTimer.Visibility = Visibility.Collapsed;
            mediaTimer.Start();
        }

        private void CurrentFilePath_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            var path = CurrentFilePath.Text;
            var extension = Path.GetExtension(path);
            var fileType = FileExtensions.GetFileTypeByExtension(extension);
            if(fileType == FileExtensions.FileType.Text)
            {
                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    TextContent.Text += reader.ReadToEnd();
                    TextContent.Visibility = string.IsNullOrEmpty(TextContent.Text) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            else
            {
                TextContent.Text = string.Empty;
                TextContent.Visibility = Visibility.Collapsed;
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaElement.NaturalDuration.HasTimeSpan)
            { 
                _mediaTotalTime = MediaElement.NaturalDuration.TimeSpan;
                MediaProgress.Maximum = _mediaTotalTime.TotalSeconds;
                MediaProgress.Visibility = Visibility.Visible;
                MediaTimer.Visibility = Visibility.Visible;
            }
            else
            {
                MediaProgress.Visibility = Visibility.Collapsed;
                MediaTimer.Visibility = Visibility.Collapsed;
            }
        }

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MediaProgress.Visibility = Visibility.Collapsed;
            MediaTimer.Visibility = Visibility.Collapsed;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var totalTime = _mediaTotalTime.ToString("mm\\:ss");
            var currentTime = MediaElement.Position.ToString("mm\\:ss");
            MediaProgress.Value = MediaElement.Position.TotalSeconds;
            MediaTimer.Text = $"{currentTime}/{totalTime}";
        }
    }
}
