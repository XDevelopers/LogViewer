using log4net;
using Sitatex.Logging.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Xml;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace Sitatex.Logging.Views
{
    /// <summary>
    /// Interaction logic for MasterPage.xaml
    /// </summary>
    public partial class MasterPage : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _FileName = string.Empty;

        private string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                RecentFileList.InsertFile(value);
            }
        }

        private List<LogEntry> _Entries = new List<LogEntry>();

        public List<LogEntry> Entries
        {
            get { return _Entries; }
            set { _Entries = value; }
        }

        public MasterPage()
        {
            InitializeComponent();

            listViewLogs.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(HeaderClicked));
            RecentFileList.UseXmlPersister();
            RecentFileList.MenuClick += (s, e) => OpenFile(e.Filepath);

            imageError.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, null);
            imageInfo.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, null);
            imageWarn.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, null);
            imageDebug.Source = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null);

            Title = string.Format("Sitatex.Logging  v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        protected override void OnClosed(EventArgs e)
        {
            App.Current.Shutdown();
            base.OnClosed(e);
        }

        private void Clear()
        {
            textBoxLevel.Text = string.Empty;
            textBoxTimeStamp.Text = string.Empty;
            textBoxMachineName.Text = string.Empty;
            textBoxThread.Text = string.Empty;
            textBoxItem.Text = string.Empty;
            textBoxHostName.Text = string.Empty;
            textBoxUserName.Text = string.Empty;
            textBoxApp.Text = string.Empty;
            textBoxClass.Text = string.Empty;
            textBoxMethod.Text = string.Empty;
            textBoxLine.Text = string.Empty;
            textBoxfile.Text = string.Empty;
            textBoxMessage.Text = string.Empty;
            textBoxThrowable.Text = string.Empty;
        }

        private void OpenFile(string fileName)
        {
            FileName = fileName;
            LoadFile();
        }

        private void LoadFile()
        {
            textboxFileName.Text = FileName;
            _Entries.Clear();
            listViewLogs.ItemsSource = null;

            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string sXml = string.Empty;
            string sBuffer = string.Empty;
            int iIndex = 1;

            Clear();

            try
            {
                if (string.IsNullOrEmpty(FileName)) return;

                FileStream oFileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                StreamReader oStreamReader = new StreamReader(oFileStream);
                sBuffer = string.Format("<root>{0}</root>", oStreamReader.ReadToEnd());
                oStreamReader.Close();
                oFileStream.Close();

                #region Read File Buffer
                ////////////////////////////////////////////////////////////////////////////////
                StringReader oStringReader = new StringReader(sBuffer);
                XmlTextReader oXmlTextReader = new XmlTextReader(oStringReader);
                oXmlTextReader.Namespaces = false;
                while (oXmlTextReader.Read())
                {
                    if ((oXmlTextReader.NodeType == XmlNodeType.Element) && (oXmlTextReader.Name == "log4j:event"))
                    {
                        LogEntry logentry = new LogEntry();

                        logentry.Item = iIndex;

                        double dSeconds = Convert.ToDouble(oXmlTextReader.GetAttribute("timestamp"));
                        logentry.TimeStamp = dt.AddMilliseconds(dSeconds).ToLocalTime();
                        logentry.Thread = oXmlTextReader.GetAttribute("thread");

                        #region get level
                        ////////////////////////////////////////////////////////////////////////////////
                        logentry.Level = oXmlTextReader.GetAttribute("level");
                        switch (logentry.Level)
                        {
                            case "ERROR":
                                {
                                    logentry.Image = LogEntry.Images(LogEntry.IMAGE_TYPE.ERROR);
                                    break;
                                }
                            case "INFO":
                                {
                                    logentry.Image = LogEntry.Images(LogEntry.IMAGE_TYPE.INFO);
                                    break;
                                }
                            case "DEBUG":
                                {
                                    logentry.Image = LogEntry.Images(LogEntry.IMAGE_TYPE.DEBUG);
                                    break;
                                }
                            case "WARN":
                                {
                                    logentry.Image = LogEntry.Images(LogEntry.IMAGE_TYPE.WARN);
                                    break;
                                }
                            case "FATAL":
                                {
                                    logentry.Image = LogEntry.Images(LogEntry.IMAGE_TYPE.FATAL);
                                    break;
                                }
                            default:
                                {
                                    logentry.Image = LogEntry.Images(LogEntry.IMAGE_TYPE.CUSTOM);
                                    break;
                                }
                        }
                        ////////////////////////////////////////////////////////////////////////////////
                        #endregion

                        #region read xml
                        ////////////////////////////////////////////////////////////////////////////////
                        while (oXmlTextReader.Read())
                        {
                            if (oXmlTextReader.Name == "log4j:event")   // end element
                                break;
                            else
                            {
                                switch (oXmlTextReader.Name)
                                {
                                    case ("log4j:message"):
                                        {
                                            logentry.Message = oXmlTextReader.ReadString();
                                            break;
                                        }
                                    case ("log4j:data"):
                                        {
                                            switch (oXmlTextReader.GetAttribute("name"))
                                            {
                                                case ("log4jmachinename"):
                                                    {
                                                        logentry.MachineName = oXmlTextReader.GetAttribute("value");
                                                        break;
                                                    }
                                                case ("log4net:HostName"):
                                                    {
                                                        logentry.HostName = oXmlTextReader.GetAttribute("value");
                                                        break;
                                                    }
                                                case ("log4net:UserName"):
                                                    {
                                                        logentry.UserName = oXmlTextReader.GetAttribute("value");
                                                        break;
                                                    }
                                                case ("log4japp"):
                                                    {
                                                        logentry.App = oXmlTextReader.GetAttribute("value");
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case ("log4j:throwable"):
                                        {
                                            logentry.Throwable = oXmlTextReader.ReadString();
                                            break;
                                        }
                                    case ("log4j:locationInfo"):
                                        {
                                            logentry.Class = oXmlTextReader.GetAttribute("class");
                                            logentry.Method = oXmlTextReader.GetAttribute("method");
                                            logentry.File = oXmlTextReader.GetAttribute("file");
                                            logentry.Line = oXmlTextReader.GetAttribute("line");
                                            break;
                                        }
                                }
                            }
                        }
                        ////////////////////////////////////////////////////////////////////////////////
                        #endregion

                        _Entries.Add(logentry);
                        iIndex++;

                        #region Show Counts
                        ////////////////////////////////////////////////////////////////////////////////
                        int ErrorCount =
                        (
                            from entry in Entries
                            where entry.Level == "ERROR"
                            select entry
                        ).Count();

                        if (ErrorCount > 0)
                        {
                            labelErrorCount.Content = string.Format("{0:#,#}  ", ErrorCount);
                            labelErrorCount.Visibility = Visibility.Visible;
                            imageError.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            labelErrorCount.Visibility = Visibility.Hidden;
                            imageError.Visibility = Visibility.Hidden;
                        }

                        int InfoCount =
                        (
                            from entry in Entries
                            where entry.Level == "INFO"
                            select entry
                        ).Count();

                        if (InfoCount > 0)
                        {
                            labelInfoCount.Content = string.Format("{0:#,#}  ", InfoCount);
                            labelInfoCount.Visibility = Visibility.Visible;
                            imageInfo.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            labelInfoCount.Visibility = Visibility.Hidden;
                            imageInfo.Visibility = Visibility.Hidden;
                        }

                        int WarnCount =
                        (
                            from entry in Entries
                            where entry.Level == "WARN"
                            select entry
                        ).Count();

                        if (WarnCount > 0)
                        {
                            labelWarnCount.Content = string.Format("{0:#,#}  ", WarnCount);
                            labelWarnCount.Visibility = Visibility.Visible;
                            imageWarn.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            labelWarnCount.Visibility = Visibility.Hidden;
                            imageWarn.Visibility = Visibility.Hidden;
                        }

                        int DebugCount =
                        (
                            from entry in Entries
                            where entry.Level == "DEBUG"
                            select entry
                        ).Count();

                        if (DebugCount > 0)
                        {
                            labelDebugCount.Content = string.Format("{0:#,#}  ", DebugCount);
                            labelDebugCount.Visibility = Visibility.Visible;
                            imageDebug.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            labelDebugCount.Visibility = Visibility.Hidden;
                            labelDebugCount.Visibility = Visibility.Hidden;
                        }
                        ////////////////////////////////////////////////////////////////////////////////
                        #endregion
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.listViewLogs.ItemsSource = _Entries;
            this.listViewLogs.UpdateLayout();
        }

        #region [ Menu ]

        private void MenuFileOpenClick(object sender, RoutedEventArgs e)
        {
            var oOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (oOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileName = oOpenFileDialog.FileName;
                LoadFile();
            }
        }

        private void MenuFileExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuRefreshClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadFile();
                listViewLogs.SelectedIndex = listViewLogs.Items.Count - 1;
                if (listViewLogs.Items.Count > 4)
                {
                    listViewLogs.SelectedIndex -= 3;
                }
                if (listViewLogs.SelectedItem == null) return;
                listViewLogs.ScrollIntoView(listViewLogs.SelectedItem);
                ListViewItem lvi = listViewLogs.ItemContainerGenerator.ContainerFromIndex(listViewLogs.SelectedIndex) as ListViewItem;
                lvi.BringIntoView();
                lvi.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuFilterClick(object sender, RoutedEventArgs e)
        {
            var filter = new Filter(Entries);
            filter.ShowDialog();
            if (filter.DialogResult == true)
            {
                string level = filter.Level;
                string message = filter.Message;
                string username = filter.UserName;

                List<LogEntry> query = new List<LogEntry>();

                if (level.Length > 0)
                {
                    var q =
                    (
                        from entry in Entries
                        where entry.Level == level
                        select entry
                    ).ToList<LogEntry>();

                    query.AddRange(q);
                }

                if (message.Length > 0)
                {
                    var q =
                    (
                        from entry in Entries
                        where entry.Message.Contains(message)
                        select entry
                    ).ToList<LogEntry>();
                    query.AddRange(q);
                }

                if (username.Length > 0)
                {
                    var q =
                    (
                        from entry in Entries
                        where entry.UserName.Contains(username)
                        select entry
                    ).ToList<LogEntry>();
                    query.AddRange(q);
                }

                this.listViewLogs.ItemsSource = query.Count > 0 ? query : Entries;
            }
        }

        private void MenuAboutClick(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void MenuExportClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileName)) return;

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Title = "Choose file to save to",
                FileName = $"Sitatex.Logging-Export-{DateTime.Now.ToString("yyyy-MM-dd-HHmmss")}.csv",
                Filter = "CSV (*.csv)|*.csv",
                FilterIndex = 0,
                InitialDirectory = @"C:\Temp"//Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if(saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var table = new DataTable();

                // First get the columns
                if (listViewLogs.View != null)
                {
                    foreach(var column in ((GridView)listViewLogs.View).Columns)
                    {
                        if(!string.IsNullOrEmpty(column.Header.ToString()))
                            table.Columns.Add(column.Header.ToString());
                    }
                    table.Columns.Add("Method");
                    table.Columns.Add("Throwable");
                }

                foreach (LogEntry item in listViewLogs.Items)
                {
                    table.Rows.Add
                        (
                            item.Item,
                            item.TimeStamp,
                            item.Level,
                            item.Thread,
                            item.Message,
                            item.MachineName,
                            item.HostName,
                            item.UserName,
                            item.App,
                            item.Class,
                            item.Method,
                            item.Throwable
                        );
                }

                CreateCSVFile(table, saveFileDialog.FileName);

                MessageBox.Show($"The file: {saveFileDialog.FileName} was exported!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddLogClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            switch (menuItem.Header)
            {
                case "Info":
                    {
                        Log.Info($"MenuItem: {menuItem.Header} was clicked!");
                    }
                    break;
                case "Warning":
                    {
                        Log.Warn($"MenuItem: {menuItem.Header} was clicked!");
                    }
                    break;
                case "Error":
                    {
                        Log.Error($"MenuItem: {menuItem.Header} was clicked!", new Exception("Exception created dynamically!"));
                    }
                    break;
                case "Debug":
                    {
                        Log.Debug($"MenuItem: {menuItem.Header} was clicked!");
                    }
                    break;
                case "Fatal":
                    {
                        Log.Fatal($"MenuItem: {menuItem.Header} was clicked!", new ApplicationException("ApplicationException created dynamically"));
                    }
                    break;
            }

            MenuRefreshClick(null, null);
        }

        #endregion [ Menu ]

        private void FindKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBoxFind.Text.Length > 0)
                {
                    Find(0);
                }
            }
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            Find(0);
        }

        private void FindPreviousClick(object sender, RoutedEventArgs e)
        {
            Find(1);
        }

        private void HeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            ListView source = e.Source as ListView;
            try
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(source.ItemsSource);
                dataView.SortDescriptions.Clear();
                _Direction = _Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                SortDescription description = new SortDescription(header.Content.ToString(), _Direction);
                dataView.SortDescriptions.Add(description);
                dataView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Clear();
                LogEntry logentry = this.listViewLogs.SelectedItem as LogEntry;

                this.image1.Source = logentry.Image;
                this.textBoxLevel.Text = logentry.Level;
                this.textBoxTimeStamp.Text = string.Format("{0} {1}", logentry.TimeStamp.ToShortDateString(), logentry.TimeStamp.ToShortTimeString());
                this.textBoxMachineName.Text = logentry.MachineName;
                this.textBoxThread.Text = logentry.Thread;
                this.textBoxItem.Text = logentry.Item.ToString();
                this.textBoxHostName.Text = logentry.HostName;
                this.textBoxUserName.Text = logentry.UserName;
                this.textBoxApp.Text = logentry.App;
                this.textBoxClass.Text = logentry.Class;
                this.textBoxMethod.Text = logentry.Method;
                this.textBoxLine.Text = logentry.Line;
                this.textBoxMessage.Text = logentry.Message;
                this.textBoxThrowable.Text = logentry.Throwable;
                this.textBoxfile.Text = logentry.File;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ListSortDirection _Direction = ListSortDirection.Descending;

        private delegate void VoidDelegate();

        private void DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                    if (a != null)
                    {
                        FileName = a.GetValue(0).ToString();
                        Dispatcher.BeginInvoke
                            (
                                DispatcherPriority.Background,
                                new VoidDelegate(delegate { LoadFile(); })
                            );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int CurrentIndex = 0;

        private void Find(int Direction)
        {
            if (textBoxFind.Text.Length > 0)
            {
                if (Direction == 0)
                {
                    for (int i = CurrentIndex + 1; i < listViewLogs.Items.Count; i++)
                    {
                        LogEntry item = (LogEntry)listViewLogs.Items[i];
                        if (item.Message.Contains(textBoxFind.Text))
                        {
                            listViewLogs.SelectedIndex = i;
                            listViewLogs.ScrollIntoView(listViewLogs.SelectedItem);
                            ListViewItem lvi = listViewLogs.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            lvi.BringIntoView();
                            lvi.Focus();
                            CurrentIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = CurrentIndex - 1; i > 0 && i < listViewLogs.Items.Count; i--)
                    {
                        LogEntry item = (LogEntry)listViewLogs.Items[i];
                        if (item.Message.Contains(textBoxFind.Text))
                        {
                            listViewLogs.SelectedIndex = i;
                            listViewLogs.ScrollIntoView(listViewLogs.SelectedItem);
                            ListViewItem lvi = listViewLogs.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            lvi.BringIntoView();
                            lvi.Focus();
                            CurrentIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        public void CreateCSVFile(DataTable dt, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);

            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < iColCount - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);

            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < iColCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        sw.Write(dr[i].ToString());
                    }
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
