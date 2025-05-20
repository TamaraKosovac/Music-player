using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MusicPlayer
{
    public partial class SongListView : UserControl
    {
        private MainWindow _main;
        private List<Song> _songs;

        public SongListView(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            _songs = Directory.GetFiles("Songs", "*.mp3")
                .Select(path => new Song
                {
                    Title = Path.GetFileNameWithoutExtension(path),
                    Artist = "Anyma",
                    Path = Path.GetFullPath(path),
                    ImagePath = "Images/AnymaPicture.png"
                })
                .ToList();

            SongList.ItemsSource = _songs;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Song song)
            {
                int index = _songs.IndexOf(song);
                _main.NavigateToNowPlaying(_songs, index);
            }
        }
    }
}
