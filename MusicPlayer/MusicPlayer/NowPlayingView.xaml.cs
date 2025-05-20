using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MusicPlayer
{
    public partial class NowPlayingView : UserControl
    {
        private List<Song> _playlist;
        private int _currentIndex;
        private Song? _song;
        private bool _isPlaying = true;
        private bool _isRepeatEnabled = false;
        private DispatcherTimer _timer = new DispatcherTimer();
        private MainWindow _main;

        public NowPlayingView(List<Song> playlist, int currentIndex, MainWindow main)
        {
            InitializeComponent();
            _main = main;
            _playlist = playlist;
            _currentIndex = currentIndex;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            LoadSong(_playlist[_currentIndex]);
        }

        private void LoadSong(Song song)
        {
            _song = song;

            TitleText.Text = song.Title;
            ArtistText.Text = song.Artist;

            if (!string.IsNullOrWhiteSpace(song.ImagePath))
            {
                string imageFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, song.ImagePath.TrimStart('/', '\\'));
                if (File.Exists(imageFullPath))
                {
                    AlbumArt.Source = new BitmapImage(new Uri(imageFullPath, UriKind.Absolute));
                }
                else
                {
                    TitleText.Text += " (slika nije pronađena)";
                }
            }

            if (!string.IsNullOrWhiteSpace(song.Path) && File.Exists(song.Path))
            {
                Player.Source = new Uri(song.Path, UriKind.Absolute);
                Player.Volume = VolumeSlider.Value;
                Player.Play();
                _isPlaying = true;
                _timer.Start();
                PlayPauseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            else
            {
                TitleText.Text = "Fajl nije pronađen";
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                Player.Pause();
                PlayPauseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            }
            else
            {
                Player.Play();
                PlayPauseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }

            _isPlaying = !_isPlaying;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Player != null)
            {
                Player.Volume = VolumeSlider.Value;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressSlider.Value = Player.Position.TotalSeconds;

                CurrentTimeText.Text = Player.Position.ToString(@"m\:ss");
                TotalTimeText.Text = Player.NaturalDuration.TimeSpan.ToString(@"m\:ss");

                if (Player.Position >= Player.NaturalDuration.TimeSpan)
                {
                    if (_isRepeatEnabled)
                    {
                        Player.Position = TimeSpan.Zero;
                        Player.Play();
                    }
                    else
                    {
                        PlayPauseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                        _isPlaying = false;
                        _timer.Stop();
                    }
                }
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(Player.Position.TotalSeconds - e.NewValue) > 1)
            {
                Player.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
            }
            else
            {
                _currentIndex = _playlist.Count - 1;
            }

            _timer.Stop();
            Player.Stop();
            LoadSong(_playlist[_currentIndex]);
        }


        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _playlist.Count - 1)
            {
                _currentIndex++;
            }
            else
            {
                _currentIndex = 0; 
            }

            _timer.Stop();
            Player.Stop();
            LoadSong(_playlist[_currentIndex]);
        }


        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Player.Stop();
            _main.NavigateToList();
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            _isRepeatEnabled = !_isRepeatEnabled;
            RepeatIcon.Kind = _isRepeatEnabled
                ? MaterialDesignThemes.Wpf.PackIconKind.RepeatOne
                : MaterialDesignThemes.Wpf.PackIconKind.Repeat;
        }
    }
}
