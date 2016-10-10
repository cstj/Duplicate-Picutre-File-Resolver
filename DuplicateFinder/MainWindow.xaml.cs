using System.Windows;

namespace DuplicateFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //AutoUpdater.OpenDownloadPage = true;
            //AutoUpdater.Start("https://raw.githubusercontent.com/cstj/Duplicate-Picutre-File-Resolver/master/DuplicateFinder/DuplicateFinderVersion.xml");
        }
    }
}