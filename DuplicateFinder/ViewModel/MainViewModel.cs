using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace DuplicateFinder.ViewModel
{

    public class DuplicateFile
    {
        public string displayName { get; set; }
        public ObservableCollection<String> filesList { get; set; }
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public const string DefaultImage = "./res/picture.png";

        #region Public Vars

        #region InfoProgress
        public const string InfoProgressName = "InfoProgress";
        private string _InfoProgress;
        public string InfoProgress
        {
            get { return _InfoProgress; }
            set
            {
                if (_InfoProgress == value) return;
                _InfoProgress = value;
                RaisePropertyChanged(InfoProgressName);
            }
        }
        #endregion   
        #region Title String
        public const string TitleStringName = "TitleString";
        private string _TitleString = null;
        public string TitleString
        {
            get { return _TitleString; }
            set
            {
                if (_TitleString == value) return;
                _TitleString = value;
                RaisePropertyChanged(TitleStringName);
            }
        }
        #endregion
        #region ImageSource
        public const string ImageSourceName = "ImageSource";
        private System.Windows.Media.Imaging.BitmapImage _ImageSource = null;
        public System.Windows.Media.Imaging.BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set
            {
                if (_ImageSource == value) return;
                _ImageSource = value;
                RaisePropertyChanged(ImageSourceName);
            }
        }
        #endregion
        #region PgsVal
        public const string PgsValName = "PgsVal";
        private int _PgsVal = 0;
        public int PgsVal
        {
            get { return _PgsVal; }
            set
            {
                if (_PgsVal == value) return;
                _PgsVal = value;
                RaisePropertyChanged(PgsValName);
            }
        }
        #endregion
        #region SourceLocation
        public const string SourceLocationName = "SourceLocation";
        private string _SourceLocation = string.Empty;
        public string SourceLocation
        {
            get { return _SourceLocation; }
            set
            {
                if (_SourceLocation == value) return;
                _SourceLocation = value;
                RaisePropertyChanged(SourceLocationName);
            }
        }
        #endregion
        #region ScanEnabled
        public const string ScanEnabledName = "ScanEnabled";
        private bool _ScanEnabled = false;
        public bool ScanEnabled
        {
            get { return _ScanEnabled; }
            set
            {
                if (_ScanEnabled == value) return;
                _ScanEnabled = value;
                RaisePropertyChanged(ScanEnabledName);
            }
        }
        #endregion
        #region KeepFileEnabled
        public const string KeepFileEnabledName = "KeepFileEnabled";
        private bool _KeepFileEnabled = false;
        public bool KeepFileEnabled
        {
            get { return _KeepFileEnabled; }
            set
            {
                if (_KeepFileEnabled == value) return;
                _KeepFileEnabled = value;
                RaisePropertyChanged(KeepFileEnabledName);
            }
        }
        #endregion
        #region DeleteFileEnabled
        public const string DeleteFileEnabledName = "DeleteFileEnabled";
        private bool _DeleteFileEnabled = false;
        public bool DeleteFileEnabled
        {
            get { return _DeleteFileEnabled; }
            set
            {
                if (_DeleteFileEnabled == value) return;
                _DeleteFileEnabled = value;
                RaisePropertyChanged(DeleteFileEnabledName);
            }
        }
        #endregion
        #region DupFiles
        public const string DupFileSelectedName = "DupFileSelected";
        private string _DupFileSelected = string.Empty;
        public string DupFileSelected
        {
            get { return _DupFileSelected; }
            set
            {
                if (_DupFileSelected == value) return;
                _DupFileSelected = value;
                RaisePropertyChanged(DupFileSelectedName);
            }
        }

        public const string DupFilesListName = "DupFilesList";
        private ObservableCollection<string> _DupFilesList = new ObservableCollection<string>();
        public ObservableCollection<string> DupFilesList
        {
            get { return _DupFilesList; }
            set
            {
                if (_DupFilesList == value) return;
                _DupFilesList = value;
                _DupFilesList.CollectionChanged += DupFilesList_CollectionChanged;
                RaisePropertyChanged(DupFilesListName);
            }
        }
        #endregion
        #region Dup Items
        public const string DupSelectedName = "DupSelected";
        private DuplicateFile _DupSelected = new DuplicateFile();
        public DuplicateFile DupSelected
        {
            get { return _DupSelected; }
            set
            {
                if (_DupSelected == value) return;
                _DupSelected = value;
                RaisePropertyChanged(DupSelectedName);
            }
        }

        public const string DupListName = "DupList";
        private ObservableCollection<DuplicateFile> _DupList = new ObservableCollection<DuplicateFile>();
        public ObservableCollection<DuplicateFile> DupList
        {
            get { return _DupList; }
            set
            {
                if (_DupList == value) return;
                _DupList = value;
                _DupList.CollectionChanged += DupList_CollectionChanged;
                RaisePropertyChanged(DupListName);
            }
        }
        #endregion
        
        #endregion
        
        #region Private Vars
        private Dispatcher dispatch;
        #endregion
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
            dispatch = App.Current.Dispatcher;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            TitleString = "Duplicate Picutre File Resolver " + version;
            GetSourceLocationCommand = new RelayCommand(GetSourceExecute, () => true);
            KeepSelectedCommand = new RelayCommand(KeepSelectedExecute, () => true);
            DeleteSelectedCommand = new RelayCommand(DeleteSelectedExecute, () => true);
            
            DupList = new ObservableCollection<DuplicateFile>();
            DupFilesList = new ObservableCollection<string>();

            
            ScanCommand = new RelayCommand(ScanExecute, () => true);
            scanWorker = new BackgroundWorker();
            scanWorker.DoWork += new DoWorkEventHandler(scanPath_DoWork);
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanPath_Completed);
            scanWorker.WorkerReportsProgress = true;
            
            this.PropertyChanged += MainViewModel_PropertyChanged;
            
            if (Properties.Settings.Default.SourceLocation != string.Empty) SourceLocation = Properties.Settings.Default.SourceLocation;
            
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Runtime) LoadImage(DefaultImage);
        }

        #region Property Change Events
        public void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case SourceLocationName:
                    //If the source changes save it for next time
                    Properties.Settings.Default.SourceLocation = SourceLocation;
                    Properties.Settings.Default.Save();
                    if (Directory.Exists(SourceLocation)) ScanEnabled = true;
                    else ScanEnabled = false;
                    break;

                case DupSelectedName:
                    if (DupSelected != null)
                    {
                        if (File.Exists(DupSelected.filesList[0]))
                        {
                            //Set the files list
                            DupFilesList = DupSelected.filesList;
                            //Set the picture

                            LoadImage(DupSelected.filesList[0]);
                            //ImageSource = DupSelected.filesList[0];
                        }
                    }
                    else
                    {
                        LoadImage(DefaultImage);
                    }
                    break;
                case DupFileSelectedName:
                    if (DupFileSelected != null)
                    {
                        KeepFileEnabled = true;
                        DeleteFileEnabled = true;
                    }
                    else
                    {
                        KeepFileEnabled = false;
                        DeleteFileEnabled = false;
                    }
                    break;
            }
        }
        
        private void FilesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void DupFilesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(DupFilesListName);
        }

        private void DupList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(DupListName);
        }
        #endregion
        
        private void LoadImage(string path)
        {
            if (File.Exists(path))
            {
                System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();
                using (FileStream f = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(f))
                    {
                        bytes = br.ReadBytes((int)f.Length);
                    }
                    MemoryStream ms = new MemoryStream(bytes);
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.EndInit();
                    ImageSource = image;
                }
            }
            else
            {
                LoadImage(DefaultImage);
            }
        }
        
        #region commands
        #region scan
        /// <summary>
        /// Scans the Selected Directory for dups
        /// </summary>
        public RelayCommand ScanCommand { get; private set; }
        private readonly BackgroundWorker scanWorker;
        private void ScanExecute()
        {
            ScanEnabled = false;
            Thread.Sleep(0);
            if (!scanWorker.IsBusy)
            {
                scanWorker.RunWorkerAsync();
            }
        }

        private void scanPath_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            ScanEnabled = true;
        }

        private void scanPath_DoWork(object sender, DoWorkEventArgs e)
        {
            PgsVal = 0;
            dispatch.Invoke(() => DupList.Clear());

            //Get Listing of all files and files in sub directories
            InfoProgress = "Getting File List";
            DirectoryInfo dir = new DirectoryInfo(SourceLocation);
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);

            int skipChrs = SourceLocation.Length;

            //Group Files into groups based on exact lengths (Cant Be the Same if their now the same length)
            var queryLengthDups =
                from file in fileList.AsParallel()
                group file by file.Length into fileGroup
                where fileGroup.Count() != 1
                select fileGroup;
            //Set Max and number of dups to process.
            int i = 0;
            double imax = 0;
            var queryLengthDupsList = queryLengthDups.ToList();
            queryLengthDupsList.ForEach((b) => { imax = imax + b.Count(); });
            int percent = 0;

            //Start Processing
            foreach (var fg in queryLengthDups)
            {
                //Create a new set of dups
                DuplicateFile d = new DuplicateFile();
                d.displayName = string.Empty;

                //Calculate hashs  
                object hashLock = new object();
                byte[] hash = null;
                byte[] hash1 = null;
                ConcurrentBag<string> tmpFiles = new ConcurrentBag<string>();
                //For every file in the list of files iwth the same length
                Parallel.ForEach(fg, (f) =>
                {
                    //For every file in the list of files iwth the same length
                    Parallel.ForEach(fg, (f) =>
                    {
                        //Set some inits and set the percentage for the progress bar
                        Interlocked.Increment(ref i);
                        InfoProgress = "Processing Files " + i + "/" + imax;
                        percent = Convert.ToInt32((i / imax) * 100);
                        if (percent > PgsVal) PgsVal = percent;

                        //Set the display name to the first file in the list
                        lock (d.displayName)
                        {
                            if (d.displayName == string.Empty) d.displayName = f.Name;
                        }
                        //Open the file and calculate the hash.  If its the same, add it to the list of files.
                        using (FileStream fi = f.OpenRead())
                        using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
                        {
                            hash = sha1.ComputeHash(fi);
                            fi.Close();
                        }
                        lock(hashLock)
                        {
                            //If the stored hash (of the first scanned item) is not null
                            if (hash1 != null)
                            {
                                //Compare and add it ot the list it is the same.
                                if (hash1.SequenceEqual(hash)) tmpFiles.Add(f.FullName);
                            }
                            else
                            {
                                //Otherwise store this one as the first hash and add it to the list
                                hash1 = hash;
                                tmpFiles.Add(f.FullName);
                            }
                        }
                    });
                }
                //Do we have dups?
                if (tmpFiles.Count() > 1)
                {
                    //Add our files list
                    d.filesList = new ObservableCollection<string>(tmpFiles.ToList());
                    d.filesList.CollectionChanged += FilesList_CollectionChanged;
                    //Add the duplicate to the dup collection
                    lock(DupList)
                    {
                        dispatch.Invoke(() => DupList.Add(d));
                    }
                }
            }
            if (scanWorker.CancellationPending) InfoProgress = "Canceled";
        }
        #endregion

        #region get source location
        public RelayCommand GetSourceLocationCommand { get; private set; }
        private void GetSourceExecute()
        {
            SourceLocation = GetDirectory(SourceLocation);
        }

        private string GetDirectory(string startPath)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog fd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            fd.Description = "Select Folder with Links";
            if (startPath != null && Directory.Exists(startPath)) fd.SelectedPath = startPath;
            if (fd.ShowDialog() == true)
            {
                return fd.SelectedPath;
            }
            else return startPath;
        }

        #endregion

        public RelayCommand KeepSelectedCommand { get; private set; }
        private void KeepSelectedExecute()
        {
            //Find all other dups and delte them
            var filesToDel = (from f in DupFilesList
                              where f != DupFileSelected
                              select f).ToList();
            //Delete all of the others
            foreach (var f in filesToDel)
            {
                try
                {
                    if (File.Exists(f)) File.Delete(f);
                    DupFilesList.Remove(f);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Error deleting file:" + Environment.NewLine + e.Message, "Error Copying Files", System.Windows.MessageBoxButton.OK);
                }
            }
            DupSelected.filesList = DupFilesList;
            TestDupItem();

        }

        public RelayCommand DeleteSelectedCommand { get; private set; }
        private void DeleteSelectedExecute()
        {
            try
            {
                if (File.Exists(DupFileSelected)) File.Delete(DupFileSelected);
                DupFilesList.Remove(DupFileSelected);
                DupSelected.filesList = DupFilesList;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Error deleting file:" + Environment.NewLine + e.Message, "Error Copying Files", System.Windows.MessageBoxButton.OK);
            }
            TestDupItem();
        }

        private void TestDupItem()
        {
            if (DupSelected.filesList.Count == 1)
            {
                //Remove Image 
                LoadImage(DefaultImage);
                DupList.Remove(DupSelected);
                DupFilesList.Clear();
            }
        }
        #endregion
    }
}