using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;
using Microsoft.Win32;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace ReshadeForUWP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        // Variables
        string processName;
        string uwpPackagesFormatted;
        string uwpPackagesList;
        string uwpAppID;
        string packageFamilyName;
        string packageFullName;

        string homeDir = Environment.ExpandEnvironmentVariables(@"%userprofile%"); // Home Directory of the user
        string reshadeDir = Environment.ExpandEnvironmentVariables(@"%userprofile%\ReshadeInjectUWP"); // Reshade Directory

        Process[] allProcesses;

        public MainWindow() {

            // 👇 Why do you exist?
            InitializeComponent();

            //PowerShell ps = PowerShell.Create();
            //ps.AddCommand("$uwpList = get-appxpackage | findstr ^Name; foreach ($uwp in $uwpList) { $uwp.Remove(0, 20) }");
            //uwpPackageList = ps.Invoke();

            // Extract our inject.exe file to C:\Users\*user*\ReshadeInjectUWP\inject.exe

            if (!File.Exists(reshadeDir + @"\inject.exe"))
            {
                Directory.CreateDirectory(reshadeDir);

                using (var client = new WebClient())
                {
                    client.DownloadFile("https://reshade.me/downloads/inject64.exe", reshadeDir + @"\inject.exe");
                    client.DownloadFile("https://github.com/MilkyDeveloper/dump/releases/download/%E2%99%BE/ReShade64.dll", reshadeDir + @"\Reshade64.dll");
                }

            }

            if (!File.Exists(reshadeDir + @"\getShortcutTarget.bat"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://gist.githubusercontent.com/MilkyDeveloper/050c6657760b08ffc651c1192768fe3f/raw/4476db13cb79a4b6dad872eb751f64925181d6d4/findShortcutTarget.bat", reshadeDir + @"\getShortcutTarget.bat");
                }

            }

            var psProcess = Process.Start("powershell", "-windowstyle hidden -ExecutionPolicy Unrestricted -Command $uwpList = get-appxpackage -PackageTypeFilter Main | findstr ^Name; $formatted=foreach ($uwp in $uwpList) { $uwp.Remove(0, 20) + ' : ' + (Get-AppxPackage -Name $uwp.Remove(0, 20) | Get-AppxPackageManifest).package.applications.application.VisualElements.DisplayName.Replace('ms-resource:', '').Replace(' : AppName', '') }; $formatted | Out-File C:/Users/$env:UserName/uwpPackageList.txt");
            psProcess.WaitForExit();

            // Retrive the packages we filtered from above
            uwpPackagesList = homeDir + @"\uwpPackageList.txt";

            // Now read it
            IEnumerable<string> uwpPackagesLines = File.ReadLines(uwpPackagesList);
            uwpPackagesFormatted = (String.Join(Environment.NewLine, uwpPackagesLines));
            //Console.WriteLine(lines);

            using (StringReader reader = new StringReader(uwpPackagesFormatted))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        if (line.Contains("Microsoft.Xbox") || line.Contains("Microsoft.Windows") || line.Contains("Windows"))
                        {} else
                        {
                            listbox1.Items.Add(line.Replace(".", " "));
                        }
                    }

                } while (line != null);
            }

            otherDataButton.IsEnabled = false;
            otherDataButton.ToolTip = "You have to select the package and regenerate the .bat file to access this.";

        }

        // Interaction Section
        // All of the frontend helper code will go here

        // Setup mouse dragging anywhere in the window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void chooseprocessNameButton_Click(object sender, RoutedEventArgs e)
        {
            listbox3.Items.Clear();
            allProcesses = Process.GetProcesses();

            foreach (Process x in allProcesses)
            {
                if (x.MainWindowTitle != "") {
                    listbox3.Items.Add(x.MainWindowTitle + " : " + x.ProcessName + ".exe");
                }
            }
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Process x in allProcesses)
            {
                string mainWindowTitle = listbox3.SelectedItem.ToString();
                if (x.MainWindowTitle == mainWindowTitle)
                {
                    processName = x.ProcessName;
                }
            }

            string listbox1SelectedItem = listbox1.SelectedItem.ToString().Substring(0, listbox1.SelectedItem.ToString().LastIndexOf(":") + 1).Replace(" :", "").Replace(" ", ".").Trim();
            Console.WriteLine(listbox1SelectedItem);

            string psProcessArgs = $@"-windowstyle hidden -ExecutionPolicy Unrestricted -Command $uwpList = get-appxpackage -PackageTypeFilter Main -Name {listbox1SelectedItem} | findstr ^PackageFamilyName; $formatted=foreach ($uwp in $uwpList) {{ $uwp.Remove(0, 20) }}; $formatted | Out-File C:/Users/$env:UserName/uwpPackageFamilyName.txt";
            var psProcess = Process.Start("powershell", psProcessArgs);
            psProcess.WaitForExit();

            System.IO.StreamReader file = new System.IO.StreamReader(homeDir + @"\uwpPackageFamilyName.txt");
            string line;
            while ((line = file.ReadLine()) != null) { packageFamilyName = line; }

            psProcessArgs = $@"-windowstyle hidden -ExecutionPolicy Unrestricted -Command $uwpList = get-appxpackage -PackageTypeFilter Main -Name {listbox1SelectedItem} | findstr ^PackageFullName; $formatted=foreach ($uwp in $uwpList) {{ $uwp.Remove(0, 20) }}; $formatted | Out-File C:/Users/$env:UserName/uwpPackageFullName.txt";
            psProcess = Process.Start("powershell", psProcessArgs);
            psProcess.WaitForExit();

            file = new System.IO.StreamReader(homeDir + @"\uwpPackageFullName.txt");
            while ((line = file.ReadLine()) != null) { packageFullName = line; }

            string filePath = @"C:\Program Files\WindowsApps\" + packageFullName + @"\appxmanifest.xml";
            if (!File.Exists(filePath))
            {
                filePath = @"C:\Program Files\WindowsApps\" + packageFullName + @"\AppxManifest.xml";
            }

            // Regex search query that's pretty accurate:
            // /(Application Id=")+\w+"/gi

            Console.WriteLine(filePath);
            using (StreamReader r = new StreamReader(filePath))
            {
                processName = listbox3.SelectedItem.ToString().Substring(listbox3.SelectedItem.ToString().IndexOf(":")).Replace(": ", "");

                string lineXml;
                while ((lineXml = r.ReadLine()) != null)
                {
                    String regexPatternx = @"(Application Id=.*?)+\w+";

                    Regex rgx = new Regex(regexPatternx);
                    MatchCollection matchedStringx = rgx.Matches(lineXml);

                    if (matchedStringx.Count != 0)
                    {
                        uwpAppID = matchedStringx[0].ToString().Remove(0, 16);
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "Batch Script (*.bat)|*.bat";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            // Check if file already exists. If yes, delete it.     
                            if (File.Exists(saveFileDialog.FileName))
                            {
                                File.Delete(saveFileDialog.FileName);
                            }

                            // Create a new file     
                            using (StreamWriter sw = File.CreateText(saveFileDialog.FileName))
                            {
                                sw.WriteLine(@"cd %UserProfile%\ReshadeInjectUWP");
                                // Very hacky solution that finally works :D
                                // I'm thinking of extracting info from the Xbox Game Pass desktop shortcuts
                                // ^^ Actually impossible because Microsoft made it not even readable by scripts...? like tf
                                sw.WriteLine(@"cmd /c powershell -windowstyle hidden -Command Start-Process -filepath inject.exe {0}; Start-Process -filepath explorer.exe shell:appsFolder\{1}!{2}", processName, packageFamilyName, uwpAppID);
                            }
                        }
                    }
                }
            }

            otherDataButton.IsEnabled = true;
            otherDataButton.ToolTip = null;
        }

        void moveFiles(string dirName, string outDir) // Note to self: for outDir always put a backslash at the back
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);

            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in files)
            {
                // you can delete file here if you want (destination file)
                if (File.Exists(outDir + file.Name))
                {
                    File.Delete(outDir + file.Name);
                }

                // then copy the file here
                file.MoveTo(outDir + file.Name);
            }

            foreach (DirectoryInfo dirx in dirs)
            {
                // you can delete file here if you want (destination file)
                if (File.Exists(outDir + dirx.Name))
                {
                    File.Delete(outDir + dirx.Name);
                }


                // then copy the file here
                dir.MoveTo(outDir + dirx.Name);
            }
        }

        void removeFiles(string dirName) // Note to self: for outDir always put a backslash at the back
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);

            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in files)
            {
                file.Delete();
            }

            foreach (DirectoryInfo dirx in dirs)
            {
                Directory.Delete(reshadeDir + @"\Shaders\" + dirx.ToString(), true);
            }
        }

        private void generateReshadeINI(object sender, RoutedEventArgs e)
        {
            // Create mandatory directories
            Directory.CreateDirectory(reshadeDir + @"\Shaders");
            Directory.CreateDirectory(reshadeDir + @"\Textures");

            // Download all the github repos from the Reshade Setup
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/CeeJayDK/SweetFX/archive/master.zip", reshadeDir + @"\sweetfx.zip");
                client.DownloadFile("https://github.com/martymcmodding/qUINT/archive/master.zip", reshadeDir + @"\quint.zip");
                client.DownloadFile("https://github.com/prod80/prod80-ReShade-Repository/archive/master.zip", reshadeDir + @"\prod80.zip");
                client.DownloadFile("https://github.com/BlueSkyDefender/Depth3D/archive/master.zip", reshadeDir + @"\depth3d.zip");
                client.DownloadFile("https://github.com/BlueSkyDefender/AstrayFX/archive/master.zip", reshadeDir + @"\astrayfx.zip");
                client.DownloadFile("https://github.com/FransBouma/OtisFX/archive/master.zip", reshadeDir + @"\otisfx.zip");
                client.DownloadFile("https://github.com/brussell1/Shaders/archive/master.zip", reshadeDir + @"\brussell1.zip");
                client.DownloadFile("https://github.com/Daodan317081/reshade-shaders/archive/master.zip", reshadeDir + @"\daodan.zip");
                client.DownloadFile("https://github.com/Fubaxiusz/fubax-shaders/archive/master.zip", reshadeDir + @"\fubax.zip");
                client.DownloadFile("https://github.com/LordOfLunacy/Insane-Shaders/archive/master.zip", reshadeDir + @"\insane.zip");
                client.DownloadFile("https://github.com/luluco250/FXShaders/archive/master.zip", reshadeDir + @"\fxshaders.zip");
                client.DownloadFile("https://github.com/originalnicodr/CorgiFX/archive/master.zip", reshadeDir + @"\corgifx.zip");
                client.DownloadFile("https://github.com/Radegast-FFXIV/reshade-shaders/archive/master.zip", reshadeDir + @"\radegast.zip");
            }

            removeFiles(reshadeDir + @"\Shaders");
            removeFiles(reshadeDir + @"\Textures");

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(reshadeDir + @"\sweetfx.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\quint.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\prod80.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\depth3d.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\astrayfx.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\otisfx.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\brussell1.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\daodan.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\fubax.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\insane.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\fxshaders.zip", reshadeDir, null);
            fastZip.ExtractZip(reshadeDir + @"\corgifx.zip", reshadeDir, null);

            moveFiles(reshadeDir + @"\SweetFX-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\SweetFX-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\qUINT-master\Shaders", reshadeDir + @"\Shaders\");

            moveFiles(reshadeDir + @"\prod80-ReShade-Repository-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\prod80-ReShade-Repository-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\Depth3d-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\AstrayFX-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\AstrayFX-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\OtisFX-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\OtisFX-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\Shaders-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\Shaders-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\reshade-shaders-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\reshade-shaders-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\fubax-shaders-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\fubax-shaders-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\Insane-Shaders-master\Shaders", reshadeDir + @"\Shaders\");

            moveFiles(reshadeDir + @"\FXShaders-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\FXShaders-master\Textures", reshadeDir + @"\Textures\");

            moveFiles(reshadeDir + @"\CorgiFX-master\Shaders", reshadeDir + @"\Shaders\");
            moveFiles(reshadeDir + @"\CorgiFX-master\Textures", reshadeDir + @"\Textures\");

            using (StreamWriter sw = new StreamWriter(reshadeDir + @"\ReShade.ini"))
            {
                sw.WriteLine(@"[GENERAL]");
                sw.WriteLine(@"EffectSearchPaths = .\," + reshadeDir + @"\Shaders");
                sw.WriteLine(@"TextureSearchPaths = .\," + reshadeDir + @"\Textures");
                sw.WriteLine(@"CurrentPresetPath = " + reshadeDir + @"\ReshadePreset.ini");
            }
        }

        private void otherDataButton_Click(object sender, RoutedEventArgs e)
        {

            // Now just update the extra info stuff
            uwpPackageFamilyTextbox.Text = $"{uwpPackageFullTextbox.Text} {packageFamilyName}"; ;
            uwpPackageFullTextbox.Text = $"{uwpPackageFullTextbox.Text} {packageFullName}";
        }
    }
}
