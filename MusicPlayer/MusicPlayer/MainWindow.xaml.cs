using System.Windows;

namespace MusicPlayer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new SongListView(this);
        }

        public void NavigateToNowPlaying(List<Song> playlist, int currentIndex)
        {
            MainContent.Content = new NowPlayingView(playlist, currentIndex, this);
        }

        public void NavigateToList()
        {
            MainContent.Content = new SongListView(this);
        }
    }
}
