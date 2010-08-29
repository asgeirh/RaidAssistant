using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;

namespace RaidAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> logFolders = new List<string>();
        List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

        Thread readerThread = null;

        Dictionary<string, LogFile> logFiles = new Dictionary<string, LogFile>();

        List<AOETimer> timers = new List<AOETimer>();

        Timer updatetimer;

        public MainWindow()
        {
            InitializeComponent();

            logFolders.Add(@"C:\Logs");

            timers.Add(new AOETimer("Test", 10, ".*test.*"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFileMonitor();

            StartThread();

            updatetimer = new Timer(new TimerCallback(UpdateUI), null, 1000, 1000);
        }

        int secs = 120;
        

        private void UpdateUI(object state)
        {
            secs--;
            System.Diagnostics.Debug.WriteLine("UPDATE");

            Dispatcher.BeginInvoke((Action)UpdateUIMain);
        }

        private void UpdateUIMain()
        {
        //    canvas1.BeginInit();

            
            canvas1.Children.Clear();

            var ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(Colors.Blue);
            ellipse.Height = 75;
            ellipse.Width = 75;

            canvas1.Children.Add(ellipse);

        //    canvas1.EndInit();
        }

        private void StartThread()
        {
            if (readerThread == null)
            {
                readerThread = new Thread(new ThreadStart(LogReader));
                readerThread.Start();
            }
        }

        private void LogReader()
        {
            Dictionary<string, string> metadata;
            int timer;

            while (true)
            {
                lock (logFiles)
                {
                    foreach (var item in logFiles)
                    {
                        string line;

                        while ((line = item.Value.ReadLine()) != null)
                        {
                            System.Diagnostics.Debug.WriteLine("GOT: " + line);

                            for (var i = 0; i < timers.Count; i++)
                            {
                                timers[i].Test(ref line, out metadata, out timer);
                            }
                        }
                    }
                }

                Thread.Sleep(50);
            }
        }

        private void LoadFileMonitor()
        {
            foreach (var log in logFolders)
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

                fileSystemWatcher.Path = log;
                fileSystemWatcher.IncludeSubdirectories = true;
                fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;

                fileSystemWatcher.Filter = "*_*.txt";

                fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);

                fileSystemWatcher.EnableRaisingEvents = true;

                fileSystemWatchers.Add(fileSystemWatcher);
            }
        }

        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Name + " - " + e.ChangeType);

            lock (logFiles)
            {
                if (!logFiles.ContainsKey(e.FullPath))
                {
                    logFiles.Add(e.FullPath, new LogFile(e.FullPath));
                }
            }
        }
    }
}
