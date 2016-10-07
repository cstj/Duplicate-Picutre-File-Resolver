using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Forms;

using WixSharp;
using WixSharp.CommonTasks;
using Microsoft.Deployment.WindowsInstaller;

class Script
{
    //Set Information for build
    const string VersionFileName = "Duplicate_Picutre_File_Resolver.exe";
    const string DotNetFileName = "dotnet.exe";
    const string wixProjectName = "DuplicateFinder";
    const string wixCompany = "St John";
    //The location of .net.  This is 4.5.2
    const string DotNetSource = "http://download.microsoft.com/download/B/4/1/B4119C11-0423-477B-80EE-7A474314B347/NDP452-KB2901954-Web.exe";
    
    static string ExecutePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    static Project project;

    static public void Main(string[] args)
    {
        //Add list of projects to be included
        List<string> projects = new List<string>();
        projects.Add(wixProjectName);

        //Construct Project
        project = new Project(wixProjectName);
        project.GUID = new Guid("9E58F671-65CB-4971-BCE5-19B81A11138A");
        //project.UpgradeCode = new Guid("BE0C7208-6E28-48AD-89AF-82DDB2150CE9");
        project.Dirs = GetFiles(projects);
        project.Binaries = GetBinaries(project);
        project.UI = WUI.WixUI_InstallDir;        

        //Get Licence if it exists.
        string LicenceFile = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(ExecutePath, ".."), ".."), ".."), "LICENSE.rtf");
        if (System.IO.File.Exists(LicenceFile)) project.LicenceFile = LicenceFile;

        //Some Defaults for the builder
        Compiler.PreserveTempFiles = false;
        Compiler.AllowNonRtfLicense = false;
         
        //The Builder Locations (Assumes you have included wix as a package.
        Compiler.WixLocation = System.IO.Path.Combine(Application.StartupPath, @"..\..\..\packages\WiX.3.9.2\tools");
        Compiler.WixSdkLocation = System.IO.Path.Combine(Compiler.WixLocation, "sdk");

        //Standard Upgrade Strategy
        project.WixSourceGenerated += doc => doc.Root.Select("Product").AddElement("MajorUpgrade")
            .AddAttributes(new Dictionary<string, string>
            {
                { "Schedule","afterInstallInitialize" },
                { "DowngradeErrorMessage","A later version of [ProductName] is already installed. Setup will now exit." }
            });

        //Name project with version if we have one
        if (project.Version != null) project.Name = project.Name + " " + project.Version.ToString();
        project.ControlPanelInfo.UrlUpdateInfo = "https://github.com/cstj/Duplicate-Picutre-File-Resolver/releases";
        project.ControlPanelInfo.UrlInfoAbout = "https://github.com/cstj/Duplicate-Picutre-File-Resolver/";

        //Build the MSI
        Compiler.BuildMsi(project, wixProjectName + " " + project.Version + ".msi");
    }


    private static Dir[] GetFiles(List<string> projects)
    {
        //Get if in debug or release mode and set path to append
        string pathToFiles = System.IO.Path.Combine("bin", "release");
        #if DEBUG
        pathToFiles = System.IO.Path.Combine("bin", "debug");
        #endif

        string curPath = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(ExecutePath, ".."), ".."), "..");
        List<Dir> dirs = new List<Dir>();
        foreach (string p in projects)
        {
            string finalPath = System.IO.Path.Combine(System.IO.Path.Combine(curPath, p), pathToFiles);
            if (System.IO.Directory.Exists(finalPath))
            {
                //Search Finalpath for ProductVersion
                string VersionPath = System.IO.Path.Combine(finalPath, VersionFileName);
                if (System.IO.File.Exists(VersionPath)) project.Version = System.Reflection.AssemblyName.GetAssemblyName(VersionPath).Version;
                //Add base directory of VS project
                string dirPath = @"%ProgramFiles%\" + wixCompany + "\\" + wixProjectName;
                Dir d = new Dir(dirPath);
                AddFilesToDir(ref d.Dirs[0].Dirs[0], System.IO.Path.Combine(p, pathToFiles), finalPath, true, true);
                dirs.Add(d);
            }
        }
        return dirs.ToArray();
    }
    private static void AddFilesToDir(ref Dir Dir, string RelPath, string SourcePath, bool Recursive, bool shortcuts)
    {
        //Add list of files
        string currentWorkingDir = System.IO.Path.GetFileName(SourcePath);
        List<File> fileList = new List<File>();
        foreach (string f in System.IO.Directory.GetFiles(SourcePath))
        {
            string fileName = System.IO.Path.GetFileName(f);
            if (shortcuts && System.IO.Path.GetExtension(f).ToLower() == ".exe" && !System.IO.Path.GetFileName(f).Contains("vshost"))
            {
                //Add Spaces to CamelCase string - Camel Case Strings
                string shortName = Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(f), @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
                shortName = shortName.Replace("_", " ").Replace("-", " ");  //Replace - and _ with spaces
                //Add the file and create the shortcut using the icon form the exe.
                fileList.Add(new File(f, new FileShortcut(shortName, @"%ProgramMenu%\" + wixCompany + "\\" + wixProjectName) { IconFile = f, WorkingDirectory = @"INSTALLDIR" }));
                project.ControlPanelInfo.ProductIcon = f;
            }
            else
            {
                fileList.Add(new File(f));
            }
        }
        Dir.Files = fileList.ToArray();

        //If recursive then cycle through direcories
        if (Recursive)
        {
            List<Dir> dirList = new List<Dir>();
            foreach (string d in System.IO.Directory.GetDirectories(SourcePath))
            {
                //Create new DIR and fill it with files
                string newDirName = System.IO.Path.GetFileName(d);
                Dir subDir = new Dir(newDirName);
                AddFilesToDir(ref subDir, System.IO.Path.Combine(RelPath, newDirName), d, Recursive, shortcuts);
                dirList.Add(subDir);   //Add the filled dir to the parent
            }
            Dir.Dirs = dirList.ToArray();
        }
    }

    private static Binary[] GetBinaries(Project project)
    {
        //Get Actions to add to
        List<WixSharp.Action> actions = new List<WixSharp.Action>();
        foreach (WixSharp.Action a in project.Actions) actions.Add(a);

        List<Binary> bins = new List<Binary>();
        //Add .net framework
        bins.Add(new Binary(GetFramework()));
        //Add action to install .net
        actions.Add(new ManagedAction(@"InstallDotNetAction",
            Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence));

        project.Actions = actions.ToArray();
        return bins.ToArray();
    }

    private static string GetFramework()
    {
        //Download .net web setup
        string tmpFramework = System.IO.Path.Combine(ExecutePath, DotNetFileName);
        if (!System.IO.File.Exists(tmpFramework))
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                //.net 4.5.2 web setup
                wc.DownloadFile(DotNetSource, tmpFramework);
            }
        }
        return tmpFramework;
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult InstallDotNetAction(Session session)
    {
        //If .net 4.5 or later is not installed get it out and run it
        if (!GetDotNetGt45())
        {
            var netExeFile = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".exe");
            SaveBinaryToFile(session, "dotnet.exe".Expand(), netExeFile);
            //Install
            System.Diagnostics.Process.Start(netExeFile).WaitForExit();
            if (GetDotNetGt45())
            {
                var result = MessageBox.Show(".NET 4.5 was not Installed" + Environment.NewLine + "This is required for this program.  Do you want to continue wihtout .NET 4.5?", "Prerequisite is not found", MessageBoxButtons.YesNo);
                if (result == DialogResult.No) return ActionResult.UserExit;
            }
        }
        return ActionResult.Success;
    }
    private static bool GetDotNetGt45()
    {
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
        {
            if (key == null) return false;
            int releaseKey = Convert.ToInt32(key.GetValue("Release"));
            return releaseKey >= 378389;
        }
    }

    static void SaveBinaryToFile(Session session, string binary, string file)
    {
        using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
        {
            sql.Execute();
            System.IO.Stream stream = sql.Fetch().GetStream(1);

            using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Create))
            {
                int Length = 256;
                var buffer = new Byte[Length];
                int bytesRead = stream.Read(buffer, 0, Length);
                while (bytesRead > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    bytesRead = stream.Read(buffer, 0, Length);
                }
            }
        }
    }
}