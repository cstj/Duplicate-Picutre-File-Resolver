using System.Windows;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Linq;
using System.Diagnostics;

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
        }

        private void UpgradeURI_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void UpgradeURIText_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            var s = (System.Windows.Controls.TextBlock)e.Source;
            if (s.Text == "" || s.Text == null)
            {
                updateURIRow.Height = new GridLength(0);
            }
            else
            {
                updateURIRow.Height = new GridLength(26, GridUnitType.Pixel);
            }
        }
    }
}