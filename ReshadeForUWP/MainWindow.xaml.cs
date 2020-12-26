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
        string uwpPackagesList;
        string uwpFullPackagesList;
        string uwpPackagesFormatted;
        string uwpPackagesFullFormatted;
        string uwpAppID;

        string homeDir = Environment.ExpandEnvironmentVariables(@"%userprofile%"); // Home Directory of the user
        string reshadeDir = Environment.ExpandEnvironmentVariables(@"%userprofile%\ReshadeInjectUWP"); // Reshade Directory

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

            // 
            var psProcess = Process.Start("powershell", "-windowstyle hidden -ExecutionPolicy Unrestricted -Command $uwpList = get-appxpackage | findstr ^PackageFamilyName; $formatted=foreach ($uwp in $uwpList) { $uwp.Remove(0, 20) }; $formatted | Out-File C:/Users/$env:UserName/uwpPackageList.txt");
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
                        listbox1.Items.Add(line);
                    }

                } while (line != null);
            }

            // Now redo the same stuff except for the PackageFullName
            psProcess = Process.Start("powershell", "-windowstyle hidden -ExecutionPolicy Unrestricted -Command $uwpList = get-appxpackage | findstr ^PackageFullName; $formatted=foreach ($uwp in $uwpList) { $uwp.Remove(0, 20) }; $formatted | Out-File C:/Users/$env:UserName/uwpFullPackageList.txt");
            psProcess.WaitForExit();

            // Retrive the packages we filtered from above
            uwpFullPackagesList = homeDir + @"\uwpFullPackageList.txt";

            // Now read it
            IEnumerable<string> uwpFullPackagesLines = File.ReadLines(uwpFullPackagesList);
            uwpPackagesFullFormatted = (String.Join(Environment.NewLine, uwpFullPackagesLines));
            //Console.WriteLine(lines);

            using (StringReader reader = new StringReader(uwpPackagesFullFormatted))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        listbox2.Items.Add(line);
                    }

                } while (line != null);
            }

        }

        // Interaction Section
        // All of the frontend helper code will go here

        // Setup mouse dragging anywhere in the window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

            string filePath = @"C:\Program Files\WindowsApps\" + listbox2.SelectedItem.ToString() + @"\appxmanifest.xml";
            if (!File.Exists(filePath))
            {
                filePath = @"C:\Program Files\WindowsApps\" + listbox2.SelectedItem.ToString() + @"\AppxManifest.xml";
            }

            // Regex search query that's pretty accurate:
            // /(Application Id=")+\w+"/gi

            Console.WriteLine(filePath);
            using (StreamReader r = new StreamReader(filePath))
            {
                string lineXml;
                while ((lineXml = r.ReadLine()) != null)
                {
                    string regexPattern = @"(Executable=.*?)+\w+";

                    Regex rg = new Regex(regexPattern);
                    MatchCollection matchedString = rg.Matches(lineXml);

                    if (matchedString.Count != 0)
                    {

                        processName = matchedString[0].ToString().Remove(0, 12) + ".exe";
                        Console.WriteLine(processName);

                    }

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
                                if (!ProcessNameTextbox.Text.Contains("Auto")) { processName = ProcessNameTextbox.Text; }
                                sw.WriteLine(@"cd %UserProfile%\ReshadeInjectUWP");
                                // Very hacky solution that finally works :D
                                // I'm thinking of extracting info from the Xbox Game Pass desktop shortcuts
                                sw.WriteLine(@"cmd /c powershell -windowstyle hidden -Command Start-Process -filepath inject.exe {0}; Start-Process -filepath explorer.exe shell:appsFolder\{1}!{2}", processName, listbox1.SelectedItem.ToString(), uwpAppID);
                            }
                        }
                    }
                }
            }
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
            }
        }
    }
}
