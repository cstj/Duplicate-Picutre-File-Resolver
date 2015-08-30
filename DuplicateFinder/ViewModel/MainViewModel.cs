using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
//using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
//using ZetaLongPaths;
using System.Diagnostics;
using Delimon.Win32.IO;

namespace DuplicateFinder.ViewModel
{

    public class DuplicateFile
    {
        public string displayName { get; set; }
        public ObservableCollection<String> filesList { get; set; }
    }

    public enum ExifOrientations
    {
        None = 0,
        Normal = 1,
        HorizontalFlip = 2,
        Rotate180 = 3,
        VerticalFlip = 4,
        Transpose = 5,
        Rotate270 = 6,
        Transverse = 7,
        Rotate90 = 8
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public string DefaultImage = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"res\picture.png");
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
        private System.Windows.Media.ImageSource _ImageSource = null;
        public System.Windows.Media.ImageSource ImageSource
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
        #region GetSourceLocationEnabled
        public const string GetSourceLocationEnabledName = "GetSourceLocationEnabled";
        private bool _GetSourceLocationEnabled;
        public bool GetSourceLocationEnabled
        {
            get { return _GetSourceLocationEnabled; }
            set
            {
                if (_GetSourceLocationEnabled == value) return;
                _GetSourceLocationEnabled = value;
                RaisePropertyChanged(GetSourceLocationEnabledName);
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
        #region FilterMask
        public const string FilterMaskName = "FilterMask";
        private string _FilterMask;
        public string FilterMask
        {
            get { return _FilterMask; }
            set
            {
                if (_FilterMask == value) return;
                _FilterMask = value;
                RaisePropertyChanged(FilterMaskName);
            }
        }
        #endregion
        #region RegexChecked
        public const string RegexCheckedName = "RegexChecked";
        private bool _RegexChecked;
        public bool RegexChecked
        {
            get { return _RegexChecked; }
            set
            {
                if (_RegexChecked == value) return;
                _RegexChecked = value;
                RaisePropertyChanged(RegexCheckedName);
            }
        }
        #endregion
        #region StopEnabled
        public const string StopEnabledName = "StopEnabled";
        private bool _StopEnabled = false;
        public bool StopEnabled
        {
            get { return _StopEnabled; }
            set
            {
                if (_StopEnabled == value) return;
                _StopEnabled = value;
                RaisePropertyChanged(StopEnabledName);
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
        System.IO.MemoryStream imageMemoryStream;
        #endregion
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            GetSourceLocationEnabled = true;
            //Init Versions, titles and such
            dispatch = App.Current.Dispatcher;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TitleString = "Duplicate Picutre File Resolver " + version;
            DupList = new ObservableCollection<DuplicateFile>();
            DupFilesList = new ObservableCollection<string>();
            
            //Commands
            GetSourceLocationCommand = new RelayCommand(GetSourceExecute, () => true);
            KeepSelectedCommand = new RelayCommand(KeepSelectedExecute, () => true);
            DeleteSelectedCommand = new RelayCommand(DeleteSelectedExecute, () => true);
            StopCommand = new RelayCommand(StopExectute, () => true);
            ScanCommand = new RelayCommand(ScanExecute, () => true);

            //Init the worker thread information
            scanWorker = new BackgroundWorker();
            scanWorker.WorkerSupportsCancellation = true;
            scanWorker.DoWork += new DoWorkEventHandler(scanPath_DoWork);
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanPath_Completed);
            scanWorker.WorkerReportsProgress = true;
            
            //Catch property changes
            this.PropertyChanged += MainViewModel_PropertyChanged;
            
            //Get the previous directory location specified
            if (Properties.Settings.Default.SourceLocation != string.Empty) SourceLocation = Properties.Settings.Default.SourceLocation;
            
            //If it is the standard runtime then load the default image, if not dont.  It was causing the xaml debugger to crash.
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Runtime) LoadImage(DefaultImage);
        }

        //Closes the memory stream on the previous image.
        private void ImageSource_Changed(object sender, EventArgs e)
        {
            //Try to dispose of the image memory stream on image change
            if (imageMemoryStream != null)
            {
                try
                {
                    imageMemoryStream.Close();
                    imageMemoryStream.Dispose();
                }
                catch (Exception ex)
                {
                    DebugBreak();
                }
            }
            ImageSource.Changed -= ImageSource_Changed;
            GC.Collect();
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
                    DirectoryInfo sourceDir = new DirectoryInfo(SourceLocation);
                    if (sourceDir.Exists) ScanEnabled = true;
                    else ScanEnabled = false;
                    break;

                case DupSelectedName:
                    if (DupSelected != null)
                    { 
                        FileInfo f = new FileInfo(DupSelected.filesList[0]);
                        if (f.Exists)
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
                    GC.Collect();
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
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                System.IO.MemoryStream ms;
                //Cache File
                using (System.IO.FileStream f = file.OpenRead())
                {
                    byte[] bytes;
                    using (System.IO.BinaryReader br = new System.IO.BinaryReader(f))
                    {
                        bytes = br.ReadBytes((int)f.Length);
                    }
                    ms = new System.IO.MemoryStream(bytes);
                }
                int angle = 0;
                TransformedBitmap timage = new TransformedBitmap();
                
                //Get Metadata (EXIF)
                BitmapFrame frame = BitmapFrame.Create(ms);
                if (frame.Metadata != null)
                {
                    BitmapMetadata meta = (BitmapMetadata)frame.Metadata;
                    if (meta.GetQuery("/app1/ifd/{ushort=274}") != null)
                    {
                        var orientation = (ExifOrientations)Enum.Parse(typeof(ExifOrientations), meta.GetQuery("/app1/ifd/{ushort=274}").ToString());
                        switch (orientation)
                        {
                            case ExifOrientations.Rotate90:
                                angle = -90;
                                break;
                            case ExifOrientations.Rotate180:
                                angle = -180;
                                break;
                            case ExifOrientations.Rotate270:
                                angle = -270;
                                break;
                        }
                    }
                }

                //Create Image
                timage.BeginInit();
                timage.Source = frame;
                System.Windows.Media.RotateTransform xform = new System.Windows.Media.RotateTransform((double)angle);
                timage.Transform = xform;
                timage.EndInit();
                ImageSource = timage;
                imageMemoryStream = ms;
                ImageSource.Changed += ImageSource_Changed;
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
            if (!scanWorker.IsBusy)
            {
                ScanEnabled = false;
                scanWorker.RunWorkerAsync();
                StopEnabled = true;
                GetSourceLocationEnabled = false;
            }
        }

        private ConcurrentBag<FileInfo> GetFiles(DirectoryInfo path)
        {
            ConcurrentBag<FileInfo> list = new ConcurrentBag<FileInfo>();
            //get files
            //get dirs
            Parallel.ForEach(path.GetDirectories(), (d) =>
            {
                GetFiles(d).ToList().ForEach((f) => list.Add(f));
            });
            path.GetFiles().ToList().ForEach((f) => list.Add(f));
            return list;
        }

        private void scanPath_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PgsVal = 0;
                dispatch.Invoke(() => {
                    DupList.Clear();
                    DupFilesList.Clear();
                });

                //Get Listing of all files and files in sub directories
                InfoProgress = "Getting File List";
                DirectoryInfo dir = new DirectoryInfo(SourceLocation);
                //dir.GetFiles()
                var fileList = GetFiles(dir);
                //var fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                int skipChrs = SourceLocation.Length;
                System.Text.RegularExpressions.Regex regex;
                //if we have no filter then match everything
                if (FilterMask == string.Empty || FilterMask == null)
                {
                    regex = new System.Text.RegularExpressions.Regex("^$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
                else
                {
                    //If we have a wildcard, create that, otherwise use the regex
                    if (!RegexChecked)
                    {
                        Helpers.Wildcard wildcard = new Helpers.Wildcard(FilterMask, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        regex = wildcard;
                    }
                    else
                    {
                        try
                        {
                            regex = new System.Text.RegularExpressions.Regex(FilterMask, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                        catch
                        {
                            regex = new System.Text.RegularExpressions.Regex("^$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                    }
                }

                //Group Files into groups based on exact lengths (Cant Be the Same if their now the same length).  I didnt use linq asparallel as it was causing issues wiht the long file name lib
                int i = 0;
                int percent = 0;
                double imax = fileList.Count();
                ConcurrentDictionary<Int64, ConcurrentBag<FileInfo>> fileGroupAll = new ConcurrentDictionary<long, ConcurrentBag<FileInfo>>();
                Parallel.ForEach(fileList, (f) =>
                {
                    InfoProgress = "Detecting Potential Duplicates " + i + "/" + imax;
                    if (!regex.IsMatch(f.FullName))
                    {
                        if (!fileGroupAll.ContainsKey(f.Length))
                        {
                            fileGroupAll.TryAdd(f.Length, new ConcurrentBag<FileInfo>());
                        }
                        fileGroupAll[f.Length].Add(f);
                    }
                    Interlocked.Increment(ref i);
                });
                InfoProgress = "Detecting Potential Duplicates - Completing List";
                var fileGroup = from f in fileGroupAll
                                 where f.Value.Count > 1
                                 select f.Value;
                percent = 0;
                PgsVal = 0;
                imax = 0;
                i = 0;
                var queryLengthDupsList = fileGroup.ToList();
                queryLengthDupsList.ForEach((b) => { imax = imax + b.Count(); });

                //Start Processing
                InfoProgress = "Processing Files";
                Xam.Plugins.ManageSleep.SleepMode sleep = new Xam.Plugins.ManageSleep.SleepMode();
                sleep.DoWithoutSleep(() =>
                {
                    foreach (var fg in queryLengthDupsList)
                    {
                        if (scanWorker.CancellationPending) break;
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
                            if (!scanWorker.CancellationPending)
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
                                using (System.IO.FileStream fi = f.OpenRead())
                                using (System.Security.Cryptography.SHA1Managed hashAlgorithm = new System.Security.Cryptography.SHA1Managed())
                                {
                                    hash = hashAlgorithm.ComputeHash(fi);
                                    fi.Close();
                                }
                                lock (hashLock)
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
                            }
                        });
                        //Do we have dups?
                        if (tmpFiles.Count() > 1)
                        {
                            //Add our files list
                            d.filesList = new ObservableCollection<string>(tmpFiles.ToList());
                            d.filesList.CollectionChanged += FilesList_CollectionChanged;
                            //Add the duplicate to the dup collection
                            lock (DupList)
                            {
                                dispatch.Invoke(() => DupList.Add(d));
                            }
                        }
                    }
                });
                PgsVal = 100;
                if (scanWorker.CancellationPending) InfoProgress = "Canceled";
                else InfoProgress = "Done";
            }
            catch
            {
                InfoProgress = "Error";
            }

        }

        private void scanPath_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            ScanEnabled = true;
            StopEnabled = false;
            GetSourceLocationEnabled = true;
        }

        public RelayCommand StopCommand { get; internal set; }
        private void StopExectute()
        {
            scanWorker.CancelAsync();
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
            if (startPath == "") startPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DirectoryInfo dir = new DirectoryInfo(startPath);
            if (startPath != null && dir.Exists) fd.SelectedPath = startPath;
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
                    FileInfo file = new FileInfo(f);
                    if (file.Exists) file.Delete();
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
                FileInfo file = new FileInfo(DupFileSelected);
                if (file.Exists) file.Delete();
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

        [Conditional("DEBUG")]
        void DebugBreak()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }
}