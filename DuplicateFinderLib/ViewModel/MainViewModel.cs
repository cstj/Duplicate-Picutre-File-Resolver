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
using Helpers.MVVMHelpers;
using System.Diagnostics;
using Path = Pri.LongPath.Path;
using Directory = Pri.LongPath.Directory;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;
using File = Pri.LongPath.File;
using FileInfo = Pri.LongPath.FileInfo;
using Helpers;
using System.Xml;
using System.ServiceModel.Syndication;

namespace DuplicateFinderLib.ViewModel
{

    public class DuplicateFile
    {
        public string displayName { get; set; }
        public ObservableCollection<ListItem<string>> filesList { get; set; }
    }

    public class FilterItem
    {
        public string filter { get; set; }
        public System.Text.RegularExpressions.Regex regex { get; set; }

        public override string ToString()
        {
            return filter;
        }
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
        #region Upgrade Information
        public const string UpgradeURIName = "UpgradeURI";
        private string _UpgradeURI;
        public string UpgradeURI
        {
            get { return _UpgradeURI; }
            set
            {
                if (_UpgradeURI == value) return;
                _UpgradeURI = value;
                RaisePropertyChanged(UpgradeURIName);
            }
        }

        public const string UpgradeURITextName = "UpgradeURIText";
        private string _UpgradeURIText;
        public string UpgradeURIText
        {
            get { return _UpgradeURIText; }
            set
            {
                if (_UpgradeURIText == value) return;
                _UpgradeURIText = value;
                RaisePropertyChanged(UpgradeURITextName);
            }
        }
        #endregion

        #region SkipDeleteConfirmations
        public const string SkipDeleteConfirmationsName = "SkipDeleteConfirmations";
        private bool _SkipDeleteConfirmations = false;
        public bool SkipDeleteConfirmations
        {
            get { return _SkipDeleteConfirmations; }
            set
            {
                if (_SkipDeleteConfirmations == value) return;
                _SkipDeleteConfirmations = value;
                RaisePropertyChanged(SkipDeleteConfirmationsName);
            }
        }

        #endregion

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
        public const string DupFilesListName = "DupFilesList";
        private ObservableCollection<ListItem<string>> _DupFilesList = new ObservableCollection<ListItem<string>>();
        public ObservableCollection<ListItem<string>> DupFilesList
        {
            get { return _DupFilesList; }
            set
            {
                if (_DupFilesList == value) return;
                if (_DupFilesList != null)
                {
                    foreach (var i in _DupFilesList) i.PropertyChanged -= DupFileListItem_PropertyChanged;
                    _DupFilesList.CollectionChanged -= DupFilesList_CollectionChanged;
                }
                _DupFilesList = value;
                if (_DupFilesList != null)
                {
                    _DupFilesList.CollectionChanged += DupFilesList_CollectionChanged;
                    foreach (var i in _DupFilesList) i.PropertyChanged += DupFileListItem_PropertyChanged;
                }
                RaisePropertyChanged(DupFilesListName);
            }
        }

        private void DupFileListItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case ListItem<object>.IsSelectedName:
                    var DupFileSelected = (ListItem<string>)sender;
                    if (DupFileSelected != null)
                    {
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
                    }
                    break;
            }
            RaisePropertyChanged(DupFilesListName);
        }

        private void DupFilesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null) foreach (var i in e.OldItems) ((ListItem<string>)i).PropertyChanged -= DupFileListItem_PropertyChanged;
            if (e.NewItems != null) foreach (var i in e.NewItems) ((ListItem<string>)i).PropertyChanged += DupFileListItem_PropertyChanged;
            RaisePropertyChanged(DupFilesListName);
        }
        #endregion
        #region Dup Items
        public const string DupListName = "DupList";
        private ObservableCollection<ListItem<DuplicateFile>> _DupList = new ObservableCollection<ListItem<DuplicateFile>>();
        public ObservableCollection<ListItem<DuplicateFile>> DupList
        {
            get { return _DupList; }
            set
            {
                if (_DupList == value) return;
                if (_DupList != null)
                {
                    foreach (var i in _DupList) i.PropertyChanged -= DupListItem_PropertyChanged;
                    _DupList.CollectionChanged -= DupList_CollectionChanged;
                }
                _DupList = value;
                if (_DupList != null)
                {
                    _DupList.CollectionChanged += DupList_CollectionChanged;
                    foreach (var i in _DupList) i.PropertyChanged += DupListItem_PropertyChanged;
                }
                RaisePropertyChanged(DupListName);
            }
        }

        private void DupList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null) foreach (var i in e.OldItems) ((ListItem<DuplicateFile>)i).PropertyChanged -= DupListItem_PropertyChanged;
            if (e.NewItems != null) foreach (var i in e.NewItems) ((ListItem<DuplicateFile>)i).PropertyChanged += DupListItem_PropertyChanged;
            RaisePropertyChanged(DupListName);
        }

        private void DupListItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //List Item Property Changed
            switch (e.PropertyName)
            {
                case ListItem<object>.IsSelectedName:
                    var DupSelected = (ListItem<DuplicateFile>)sender;
                    if (DupSelected != null)
                    {
                        FileInfo f = new FileInfo(DupSelected.Value.filesList[0].Value);
                        if (f.Exists)
                        {
                            //Set the files list
                            DupFilesList = DupSelected.Value.filesList;
                            DupFilesList[0].IsSelected = true;
                            //Set the picture
                            LoadImage(DupSelected.Value.filesList[0].Value);
                        }
                    }
                    else
                    {
                        LoadImage(DefaultImage);
                    }
                    GC.Collect();
                    RaisePropertyChanged(DupListName);
                    break;
            }
        }
        #endregion
        #region FilterList
        public const string FilterListName = "FilterList";
        private ObservableCollection<ListItem<FilterItem>> _FilterList;
        public ObservableCollection<ListItem<FilterItem>> FilterList
        {
            get { return _FilterList; }
            set
            {
                if (_FilterList == value) return;
                _FilterList = value;
                if (_FilterList != null) _FilterList.CollectionChanged -= FilterList_CollectionChanged;
                _FilterList.CollectionChanged += FilterList_CollectionChanged;
                RaisePropertyChanged(FilterListName);
            }
        }

        private void FilterList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(FilterListName);
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
        public MainViewModel(System.Windows.Threading.Dispatcher dispatcher)
        {
            GetSourceLocationEnabled = true;
            //Init Versions, titles and such
            dispatch = dispatcher;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TitleString = "Duplicate Picutre File Resolver " + version;
            DupList = new ObservableCollection<ListItem<DuplicateFile>>();
            DupFilesList = new ObservableCollection<ListItem<string>>();
            FilterList = new ObservableCollection<ListItem<FilterItem>>();
            
            //Commands
            GetSourceLocationCommand = new RelayCommand(GetSourceExecute);
            KeepSelectedCommand = new RelayCommand(KeepSelectedExecute);
            DeleteSelectedCommand = new RelayCommand(DeleteSelectedExecute);
            StopCommand = new RelayCommand(StopExectute);
            ScanCommand = new RelayCommand(ScanExecute);
            AddFilterCommnad = new RelayCommand(AddFilterExecute);
            RemoveFilterCommand = new RelayCommand(RemoveFilterExecute);

            //Init the worker thread information
            scanWorker = new BackgroundWorker();
            scanWorker.WorkerSupportsCancellation = true;
            scanWorker.DoWork += new DoWorkEventHandler(scanPath_DoWork);
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanPath_Completed);
            scanWorker.WorkerReportsProgress = true;
            
            //Catch property changes
            this.PropertyChanged += MainViewModel_PropertyChanged;

            //Merge Settings from old version if required
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            //Get the previous directory location specified
            if (Properties.Settings.Default.SourceLocation != string.Empty) SourceLocation = Properties.Settings.Default.SourceLocation;
            SkipDeleteConfirmations = Properties.Settings.Default.SkipDeleteDialogs;

            if (Properties.Settings.Default.FilterList != null)
            {
                if (Properties.Settings.Default.FilterList.Count > 0)
                {
                    foreach (var a in Properties.Settings.Default.FilterList)
                    {
                        var split = a.Split('\n');
                        if (split.GetUpperBound(0) == 1)
                        {
                            //Create the filters list item
                            FilterItem fi = CreateFilterItem(true, split[1]);
                            fi.filter = split[0];

                            FilterList.Add(new ListItem<FilterItem> { Value = fi, IsSelected = false, Title = fi.filter });
                        }
                    }
                }
            }

            //Start background to check for update
            Task.Run(() => CheckNewVersion());
                        
            //If it is the standard runtime then load the default image, if not dont.  It was causing the xaml debugger to crash.
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Runtime) LoadImage(DefaultImage);

            
        }

        private void CheckNewVersion()
        {
            try
            {
                string baseAddy = "https://github.com";
                string url = $"{baseAddy}/cstj/Duplicate-Picutre-File-Resolver/releases.atom";
                XmlTextReader reader = new XmlTextReader(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                var item = feed.Items.FirstOrDefault();
                if (item != null)
                {
                    var uri = item.Links.FirstOrDefault();
                    if (uri != null)
                    {
                        var tag = System.IO.Path.GetFileName(uri.Uri.ToString());
                        System.Version uriVersion;
                        if (System.Version.TryParse(tag, out uriVersion))
                        {
                            //Test the current version and check if the uri has a newer version
                            System.Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                            if (currentVersion < uriVersion)
                            {
                                dispatch.Invoke(() =>
                                {
                                    UpgradeURI = baseAddy + uri.Uri.ToString();
                                    UpgradeURIText = $"New version avilable, Click here to download!";
                                });
                            }
                        }
                    }
                }
            }
            catch { }
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

                case SkipDeleteConfirmationsName:
                    Properties.Settings.Default.SkipDeleteDialogs = SkipDeleteConfirmations;
                    Properties.Settings.Default.Save();
                    break;
            }
        }
        
        private void FilesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

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
            if (ScanEnabled)
            {
                if (!scanWorker.IsBusy)
                {
                    ScanEnabled = false;
                    scanWorker.RunWorkerAsync();
                    StopEnabled = true;
                    GetSourceLocationEnabled = false;
                }
            }
        }

        private ConcurrentBag<FileInfo> GetFiles(DirectoryInfo path)
        {
            ConcurrentBag<FileInfo> list = new ConcurrentBag<FileInfo>();
            //get files
            //get dirs
            Parallel.ForEach(path.GetDirectories(), (d) =>
            {
                try
                {
                    GetFiles(d).ToList().ForEach((f) => list.Add(f));
                }
                catch { }
            });
            try
            {
                path.GetFiles().ToList().ForEach((f) => list.Add(f));
            }
            catch { }
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
                //GetFilters
                var RegexList = FilterList.Select(d => d.Value.regex).ToList();

                //Group Files into groups based on exact lengths (Cant Be the Same if their now the same length).  I didnt use linq asparallel as it was causing issues wiht the long file name lib
                int i = 0;
                int percent = 0;
                double imax = fileList.Count();
                ConcurrentDictionary<Int64, ConcurrentBag<Pri.LongPath.FileInfo>> fileGroupAll = new ConcurrentDictionary<long, ConcurrentBag<FileInfo>>();
                Parallel.ForEach(fileList, (f) =>
                {
                    InfoProgress = "Detecting Potential Duplicates " + i + "/" + imax;
                    bool exclude = false;
                    //Does it match any of our filters.
                    foreach (var r in RegexList)
                    {
                        if (r.IsMatch(f.FullName))
                        {
                            exclude = true;
                            break;
                        }
                    }
                    //If its not excluded, add it to the list (if its not already)
                    if (!exclude)
                    {
                        if (!fileGroupAll.ContainsKey(f.Length))
                        {
                            fileGroupAll.TryAdd(f.Length, new ConcurrentBag<FileInfo>());
                        }
                        fileGroupAll[f.Length].Add(f);
                    }
                    Interlocked.Increment(ref i);
                });

                double totalFiles = imax;
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
                int foundDups = 1;
                object foundLock = new object();

                sleep.DoWithoutSleep(() =>
                {
                    Parallel.ForEach(queryLengthDupsList, new ParallelOptions { MaxDegreeOfParallelism = 2 } , (fg) =>
                    //foreach (var fg in queryLengthDupsList)
                    {
                        if (!scanWorker.CancellationPending)
                        {
                            //Create a new set of dups
                            DuplicateFile d = new DuplicateFile();
                            d.displayName = string.Empty;

                            //Store for first hash data
                            byte[] data1 = null;
                            string fileName1 = null;

                            //Get First File Data
                            ConcurrentBag<string> tmpFiles = new ConcurrentBag<string>();

                            FileInfo firstFile = fg.First();
                            fileName1 = firstFile.FullName;
                            data1 = File.ReadAllBytes(fileName1);
                            tmpFiles.Add(fileName1);

                            lock (foundLock)
                            {
                                d.displayName = "Group " + foundDups + "(" + System.IO.Path.GetExtension(fileName1) + " - " + firstFile.LengthHumanReadable() + ")";
                                foundDups++;
                            }
                            //For every file in the list of files iwth the same length
                            Parallel.ForEach(fg, new ParallelOptions { MaxDegreeOfParallelism = 2 }, (f) =>
                            {
                                if (f.FullName != fileName1)
                                {
                                    if (!scanWorker.CancellationPending)
                                    {
                                        //Set some inits and set the percentage for the progress bar
                                        Interlocked.Increment(ref i);
                                        InfoProgress = "Processing Files " + i + "/" + imax;
                                        percent = Convert.ToInt32((i / imax) * 100);
                                        if (percent > PgsVal) PgsVal = percent;

                                        //Open the file and calculate the hash.  If its the same, add it to the list of files.
                                        using (var fs = f.OpenRead())
                                        {
                                            //byte[] data = null;
                                            //data = File.ReadAllBytes(f.FullName);

                                            //Compare and add it ot the list it is the same.
                                            if (data1.SequenceEqual(fs))
                                            {
                                                tmpFiles.Add(f.FullName);
                                            }
                                        }
                                    }
                                }
                            });
                            //Do we have dups?
                            if (tmpFiles.Count() > 1)
                            {
                                //Add our files list
                                ObservableCollection<ListItem<string>> tmpObList = new ObservableCollection<ListItem<string>>();
                                foreach(var f in tmpFiles)
                                {
                                    tmpObList.Add(new ListItem<string> { Value = f, Title = f });
                                }
                                d.filesList = tmpObList;
                                d.filesList.CollectionChanged += FilesList_CollectionChanged;
                                //Add the duplicate to the dup collection
                                lock (DupList)
                                {
                                    dispatch.Invoke(() => DupList.Add(new ListItem<DuplicateFile> { Value = d, Title = d.displayName, IsSelected = false }));
                                }
                            }
                            data1 = null;
                        }
                    });
                });
                GC.Collect();
                PgsVal = 100;
                if (scanWorker.CancellationPending) InfoProgress = "Canceled";
                else InfoProgress = "Done - Processed " + totalFiles + " Files (" + DupList.Count().ToString() + " Duplicates in " + i + " Files";
            }
            catch (Exception ex)
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
            if (KeepFileEnabled)
            {
                var DupFileSelected = DupFilesList.Where(d => d.IsSelected).FirstOrDefault();
                if (DupFileSelected != null)
                {
                    Ookii.Dialogs.Wpf.TaskDialogButton confirm = new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Cancel);
                    if (!SkipDeleteConfirmations)
                    {
                        using (Ookii.Dialogs.Wpf.TaskDialog dialog = new Ookii.Dialogs.Wpf.TaskDialog())
                        {
                            dialog.Buttons.Add(new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Ok));
                            dialog.Buttons.Add(new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Cancel));
                            dialog.Content = $"Are you sure you want to delete ALL other duplicates apart from \n{DupFileSelected.Value}?";
                            dialog.WindowTitle = "Confirm Keep Only This File";
                            dialog.MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Warning;
                            confirm = dialog.ShowDialog();
                        }
                    }

                    if (confirm.ButtonType == Ookii.Dialogs.Wpf.ButtonType.Ok || SkipDeleteConfirmations)
                    {
                        //Find all other dups and delte them
                        var filesToDel = (from f in DupFilesList
                                          where f.Value != DupFileSelected.Value
                                          select f).ToList();
                        //Delete all of the others
                        foreach (var f in filesToDel)
                        {
                            try
                            {
                                FileInfo file = new FileInfo(f.Value);
                                if (file.Exists) file.Delete();
                                DupFilesList.Remove(f);
                            }
                            catch (Exception e)
                            {
                                System.Windows.MessageBox.Show("Error deleting file:" + Environment.NewLine + e.Message, "Error Copying Files", System.Windows.MessageBoxButton.OK);
                            }
                        }
                        //Get Currently Selected
                        var DupSelected = DupList.Where(s => s.IsSelected).FirstOrDefault();
                        if (DupSelected != null)
                        {
                            DupSelected.Value.filesList = DupFilesList;
                            TestDupItem();
                        }
                    }
                }
            }
        }

        public RelayCommand DeleteSelectedCommand { get; private set; }
        private void DeleteSelectedExecute()
        {
            if (DeleteFileEnabled)
            {
                var DupFileSelected = DupFilesList.Where(d => d.IsSelected).FirstOrDefault();
                if (DupFileSelected != null)
                {
                    Ookii.Dialogs.Wpf.TaskDialogButton confirm = new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Cancel);
                    if (!SkipDeleteConfirmations)
                    {
                        using (Ookii.Dialogs.Wpf.TaskDialog dialog = new Ookii.Dialogs.Wpf.TaskDialog())
                        {
                            dialog.Buttons.Add(new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Ok));
                            dialog.Buttons.Add(new Ookii.Dialogs.Wpf.TaskDialogButton(Ookii.Dialogs.Wpf.ButtonType.Cancel));
                            dialog.Content = $"Are you sure you want to delete \n{DupFileSelected.Value}?";
                            dialog.WindowTitle = "Confirm File Delete";
                            dialog.MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Warning;
                            confirm = dialog.ShowDialog();
                        }
                    }

                    if (confirm.ButtonType == Ookii.Dialogs.Wpf.ButtonType.Ok || SkipDeleteConfirmations)
                    {
                        try
                        {
                            FileInfo file = new FileInfo(DupFileSelected.Value);
                            if (file.Exists) file.Delete();
                            RemoveDupFileItem(ref DupFileSelected);
                            var DupSelected = DupList.Where(s => s.IsSelected).FirstOrDefault();
                            if (DupSelected != null)
                            {
                                DupSelected.Value.filesList = DupFilesList;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Windows.MessageBox.Show("Error deleting file:" + Environment.NewLine + e.Message, "Error Copying Files", System.Windows.MessageBoxButton.OK);
                        }
                        TestDupItem();
                    }
                }
            }
        }

        private void RemoveDupFileItem(ref ListItem<string> item)
        {
            //Get Current selection
            int currentSelection = -1;
            for (int i = 0; i < DupFilesList.Count; i++)
            {
                if (DupFilesList[i] == item)
                {
                    currentSelection = i;
                }
            }
            DupFilesList.Remove(item);
            if (currentSelection != -1 && currentSelection < DupFilesList.Count - 1)
                DupFilesList[currentSelection].IsSelected = true;
            else
                DupFilesList[DupFilesList.Count - 1].IsSelected = true;
        }

        private void TestDupItem()
        {
            int currentSelection = -1;
            ListItem<DuplicateFile> DupSelected = null;
            for (int i = 0; i < DupList.Count; i++)
            {
                if (DupList[i].IsSelected)
                {
                    DupSelected = DupList[i];
                    currentSelection = i;
                    break;
                }
            }

            if (DupSelected != null)
            {
                if (DupSelected.Value.filesList.Count == 1)
                {
                    //Remove Image 
                    LoadImage(DefaultImage);
                    DupList.Remove(DupSelected);
                    DupFilesList.Clear();
                    if (currentSelection != -1 && DupList.Count - 1 > currentSelection)
                        DupList[currentSelection].IsSelected = true;
                    else
                        DupList[DupList.Count - 1].IsSelected = true;
                }
            }
        }

        public RelayCommand AddFilterCommnad { get; private set; }
        public void AddFilterExecute()
        {
            FilterItem fi;
            //if we have no filter then match everything
            if (FilterMask == string.Empty || FilterMask == null)
            {
                fi = null;
            }
            else
            {
                fi = CreateFilterItem(RegexChecked, FilterMask);
            }
            if (fi != null)
            {
                FilterList.Add(new ListItem<FilterItem> { Value = fi, IsSelected = false, Title = fi.filter });
                FilterMask = "";
            }

            //Reset User settings
            System.Collections.Specialized.StringCollection saveFilters = new System.Collections.Specialized.StringCollection();
            foreach(var a in FilterList)
            {
                string saveItem = $"{a.Value.filter}\n{a.Value.regex.ToString()}";
                saveFilters.Add(saveItem);
            }
            Properties.Settings.Default.FilterList = saveFilters;
            Properties.Settings.Default.Save();
        }

        public RelayCommand RemoveFilterCommand { get; private set; }
        public void RemoveFilterExecute()
        {
            var remList = FilterList.Where(s => s.IsSelected).ToList();
            foreach(var a in remList)
            {
                FilterList.Remove(a);
            }
        }
        #endregion

        [Conditional("DEBUG")]
        void DebugBreak()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        private FilterItem CreateFilterItem(bool RegExFilter, string filter)
        {
            System.Text.RegularExpressions.Regex regex;
            //If we have a wildcard, create that, otherwise use the regex
            if (!RegExFilter)
            {
                Helpers.Wildcard wildcard = new Helpers.Wildcard(filter, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                regex = wildcard;
            }
            else
            {
                try
                {
                    regex = new System.Text.RegularExpressions.Regex(filter, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
                catch
                {
                    regex = null;
                }
            }
            if (regex != null)
            {
                FilterItem fi = new FilterItem();
                fi.regex = regex;
                fi.filter = filter;
                return fi;
            }
            else
            {
                return null;
            }
        }
    }
}