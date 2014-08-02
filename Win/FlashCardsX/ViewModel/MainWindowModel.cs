

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using DropNet;
using DropNet.Exceptions;
using FlashCardsX.Model;
using FlashCardsX.View;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Live;

namespace FlashCardsX.ViewModel
{
    // ViewModel for the Mainwindow's datagrid
    public class MainWindowModel : INotifyPropertyChanged
    {
        #region Properties

        public bool Flipped { get; set; }

        public bool Shuffled { get; set; }

        public int Comparator { get; set; }

        public int Difficulty { get; set; }

        private int _selectedDeck;
        public int SelectedDeck
        {
            get
            {
                return _selectedDeck;
            }
            set
            {
                _selectedDeck = value;
                NotifyPropertyChanged("SelectedDeck");
            }
        }

        private Brush _skyDriveStatus;
        public Brush SkyDriveStatus
        {
            get
            {
                return _skyDriveStatus;
            }
            set
            {
                _skyDriveStatus = value;
                NotifyPropertyChanged("SkyDriveStatus");
            }
        }

        private Brush _dropboxStatus;
        public Brush DropboxStatus
        {
            get
            {
                return _dropboxStatus;
            }
            set
            {
                _dropboxStatus = value;
                NotifyPropertyChanged("DropboxStatus");
            }
        }

        private string _helpText;
        public string HelpText
        {
            get
            {
                return _helpText;
            }
            set
            {
                _helpText = value;
                NotifyPropertyChanged("HelpText");
            }
        }

        private string _countText;
        public string CountText
        {
            get
            {
                return _countText;
            }
            set
            {
                _countText = value;
                NotifyPropertyChanged("CountText");
            }
        }

        private List<MainWindowGridVm> _deckList;

        public List<MainWindowGridVm> DeckList
        {
            get
            {
                return _deckList;
            }
            set
            {
                _deckList = value;
                NotifyPropertyChanged("DeckList");
            }
        }
        #endregion

        public MainWindowModel()
        {
            LoadDataGrid();
            ConnectToSkyDrive();
            ConnectToDropbox();
            CountText = "0 items |";
            SkyDriveStatus = Brushes.Red;
            DropboxStatus = Brushes.Red;

            DeleteDeckCommand = new DelegateCommand(
                x => DeleteDeck(),
                x => SelectedDeck >= 0);

            OptionsCommand = new DelegateCommand(
                x =>
                {
                    var sw = new SettingsWindow();
                    sw.Show();
                },
                x => true);

            NewDeckCommand = new DelegateCommand(
                x =>
                {
                    var nd = new NewDeckWindow();
                    nd.Show();
                },
                x => true);

            ViewDeckCommand = new DelegateCommand(
                x => ViewDeck(),
                x => SelectedDeck >= 0);

            StartTestCommand = new DelegateCommand(
                x => StartTest(),
                x => SelectedDeck >= 0);

            ExportPdfCommand = new DelegateCommand(
                x => ExportPdf(),
                x => SelectedDeck >= 0);
        }

        #region SkyDrive

        private async void SyncDeletionWithSkyDrive(string delfile)
        {
            if (Settings.Instance.SkyDriveClient != null && Settings.Instance[Settings.SkyDrive] == "1")
            {
                var rootId = await CreateDirectoryAsync("ProgramData", "me/skydrive");
                var fCardsXId = await CreateDirectoryAsync("FlashCardsX", rootId);

                // Get filelist from SkyDrive
                var filesResult = await Settings.Instance.SkyDriveClient.GetAsync(fCardsXId + "/files");
                dynamic files = filesResult.Result;

                // Downloading deletion log file
                foreach (var file in files.data)
                {
                    if (!((string)file.name).EndsWith(".log")) continue;
                    try
                    {
                        var dlResult = await Settings.Instance.SkyDriveClient.DownloadAsync(file.id + "//content");
                        var path = Settings.Instance[Settings.LocalPath] + "\\" + file.name;
                        using (Stream fs = File.Create(path))
                        {
                            CopyStream(dlResult.Stream, fs);
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        HelpText = ex.Message;
                        SkyDriveStatus = Brushes.Red;
                    }
                }
                // Deserialise log and add record then serialise
                var logFile = DeserialiseLog("Deletion.log");
                logFile[delfile] = DateTime.UtcNow;
                logFile = new SerializableDictionary<string, DateTime>(MaintainLog(logFile));
                SerialiseLog(logFile, "Deletion.log");

                // Uploading log.
                using (var infile = new FileStream(Settings.Instance[Settings.LocalPath] + "/Deletion.log", FileMode.Open))
                {
                    await Settings.Instance.SkyDriveClient.UploadAsync(fCardsXId, "Deletion.log", infile,
                        OverwriteOption.Overwrite);
                }
                foreach (var file in files.data)
                {
                    if (!((string)file.name).Equals(Path.GetFileName(delfile))) continue;
                    await Settings.Instance.SkyDriveClient.DeleteAsync(file.id);
                }
            }
            else
            {
                SkyDriveStatus = Brushes.Red;
            }
        }

        private async void ConnectToSkyDrive()
        {
            if (Settings.Instance[Settings.SkyDrive] == "0")
            {
                SkyDriveStatus = Brushes.Red;
                return;
            }
            try
            {
                Settings.Instance.SkyDriveAuthClient = new LiveAuthClient(Settings.SkyDriveClientId, Settings.Instance);
                var loginResult = await Settings.Instance.SkyDriveAuthClient.InitializeAsync(Settings.SkyDriveScope);
                if (loginResult == null || LiveConnectSessionStatus.Connected != loginResult.Status)
                {
                    HelpText = "Could not connect to SkyDrive. Please sign in again.";
                    SkyDriveStatus = Brushes.Red;
                    return;
                }
                Settings.Instance.SkyDriveClient = new LiveConnectClient(loginResult.Session);
                SkyDriveStatus = Brushes.LawnGreen;
            }
            catch (LiveAuthException)
            {
                HelpText = "Could not connect to SkyDrive. Please sign in again.";
                SkyDriveStatus = Brushes.Red;
            }
            SyncSkyDrive();
        }

        // Create Directory on SkyDrive
        public async Task<string> CreateDirectoryAsync(string folderName, string parentFolder)
        {
            if (Settings.Instance.SkyDriveClient == null) return null;
            string folderId = null;

            // Retrieves all the directories.
            var queryFolder = parentFolder + "/files?filter=folders,albums";
            var opResult = await Settings.Instance.SkyDriveClient.GetAsync(queryFolder);
            dynamic result = opResult.Result;

            foreach (var folder in result.data)
            {
                // Checks if current folder has the passed name.
                if (folder.name.ToLowerInvariant() != folderName.ToLowerInvariant()) continue;
                folderId = folder.id;
                break;
            }

            if (folderId != null) return folderId;

            // Directory hasn't been found, so creates it using the PostAsync method.
            var folderData = new Dictionary<string, object> { { "name", folderName } };
            opResult = await Settings.Instance.SkyDriveClient.PostAsync(parentFolder, folderData);
            result = opResult.Result;

            // Retrieves the id of the created folder.
            folderId = result.id;

            return folderId;
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        private void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        // Syncing files on SkyDrive
        public async void SyncSkyDrive()
        {
            if (Settings.Instance.SkyDriveClient == null || Settings.Instance[Settings.SkyDrive] == "0")
            {
                SkyDriveStatus = Brushes.Red;
                return;
            }
            SkyDriveStatus = Brushes.Purple;
            var rootId = await CreateDirectoryAsync("ProgramData", "me/skydrive");
            var fCardsXId = await CreateDirectoryAsync("FlashCardsX", rootId);

            // Get filelist from SkyDrive
            var filesResult = await Settings.Instance.SkyDriveClient.GetAsync(fCardsXId + "/files");
            dynamic files = filesResult.Result;

            // Get all local XML files
            var localFiles = (from file in Directory.EnumerateFiles(Settings.Instance[Settings.LocalPath])
                              where file.EndsWith(".xml")
                              select file).ToList();

            // Downloading deletion log file
            foreach (var file in files.data)
            {
                if (!((string)file.name).EndsWith(".log")) continue;
                try
                {
                    var dlResult = await Settings.Instance.SkyDriveClient.DownloadAsync(file.id + "//content");
                    var path = Settings.Instance[Settings.LocalPath] + "\\" + file.name;
                    using (Stream fs = File.Create(path))
                    {
                        CopyStream(dlResult.Stream, fs);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    HelpText = ex.Message;
                    SkyDriveStatus = Brushes.Red;
                }
            }
            var deletedFiles = new List<string>();
            var logfile = DeserialiseLog("Deletion.log");
            foreach (
                var deletable in
                    logfile.Where(deletable => deletable.Value >= Directory.GetLastAccessTimeUtc(deletable.Key))
                )
            {
                deletedFiles.Add(deletable.Key);
                if (File.Exists(deletable.Key))
                    File.Delete(deletable.Key);
            }
            localFiles.RemoveAll(deletedFiles.Contains);

            // Uploading new files
            foreach (var localFile in localFiles)
            {
                if ((localFile).EndsWith(".log")) continue;
                var toUpload = true;
                foreach (var file in files.data)
                {
                    if (((string)file.name).EndsWith(".log")) continue;
                    var servertime = DateTime.Parse(file.description);
                    var localtime = Directory.GetLastWriteTime(localFile);
                    if (file.name != Path.GetFileName(localFile) || servertime < localtime) continue;
                    toUpload = false;
                    break;
                }
                if (!toUpload) continue;
                try
                {
                    using (var fs = new FileStream(localFile, FileMode.Open))
                    {
                        var uploadResult = await Settings.Instance.SkyDriveClient.UploadAsync(fCardsXId,
                            Path.GetFileName(localFile), fs, OverwriteOption.Overwrite);
                        dynamic uploadData = uploadResult.Result;
                        uploadData.description = Directory.GetLastWriteTime(localFile).ToString("s");
                        await Settings.Instance.SkyDriveClient.PutAsync(uploadData.id, uploadData);
                    }
                }
                catch (Exception ex)
                {
                    HelpText = ex.Message;
                    SkyDriveStatus = Brushes.Red;
                }
            }

            // Downloading files
            foreach (var file in files.data)
            {
                if (((string)file.name).EndsWith(".log")) continue;
                var toDl =
                    localFiles.All(
                        localFile =>
                            file.name != Path.GetFileName(localFile) ||
                            DateTime.Parse(file.description) > Directory.GetLastWriteTime(localFile));
                if (!toDl) continue;
                try
                {
                    var dlResult = await Settings.Instance.SkyDriveClient.DownloadAsync(file.id + "//content");
                    var path = Settings.Instance[Settings.LocalPath] + "\\" + file.name;
                    using (Stream fs = File.Create(path))
                    {
                        CopyStream(dlResult.Stream, fs);
                    }
                    File.SetLastWriteTime(path, DateTime.Parse(file.description));
                }
                catch (Exception ex)
                {
                    HelpText = ex.Message;
                    SkyDriveStatus = Brushes.Red;
                }
            }
            SkyDriveStatus = Brushes.LawnGreen;
            Settings.Instance[Settings.LastSync] = DateTime.Now.ToString("s");
            LoadDataGrid();
        }

        #endregion

        #region Dropbox

        private void SyncDeletionWithDropbox(string delfile)
        {
            var client = Settings.Instance.DropboxClient;
            if (client != null && Settings.Instance[Settings.Dropbox] == "1")
            {
                // Get filelist from Dropbox
                client.GetFileAsync("/Deletion.log",
                    file =>
                    {
                        var log = client.GetFile("/Deletion.log");
                        File.WriteAllBytes(Settings.Instance[Settings.LocalPath] + "/Deletion.log", log);

                        var logFile = DeserialiseLog("Deletion.log");
                        logFile[delfile] = DateTime.UtcNow;
                        logFile = new SerializableDictionary<string, DateTime>(MaintainLog(logFile));
                        SerialiseLog(logFile, "Deletion.log");

                        // Uploading log.
                        using (var infile = new FileStream(Settings.Instance[Settings.LocalPath] + "/Deletion.log", FileMode.Open))
                        {
                            client.UploadFile("/", "Deletion.log", infile);
                        }
                        client.Delete("/" + Path.GetFileName(delfile));
                    },
                    error => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        if (error.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            var logFile = DeserialiseLog("Deletion.log");
                            logFile[delfile] = DateTime.UtcNow;
                            logFile = new SerializableDictionary<string, DateTime>(MaintainLog(logFile));
                            SerialiseLog(logFile, "Deletion.log");

                            // Uploading log.
                            using (
                                var infile = new FileStream(Settings.Instance[Settings.LocalPath] + "/Deletion.log",
                                    FileMode.Open))
                            {
                                client.UploadFile("/", "Deletion.log", infile);
                            }
                            client.Delete("/" + Path.GetFileName(delfile));
                        }
                        DropboxStatus = Brushes.Red;
                    })));
            }
        }

        private void ConnectToDropbox()
        {
            if (Settings.Instance[Settings.Dropbox] == "0")
            {
                DropboxStatus = Brushes.Red;
                return;
            }
            var client = new DropNetClient(Settings.DropboxKey, Settings.DropboxSecret) { UserLogin = Settings.Instance.DropboxLogin };
            Settings.Instance.DropboxClient = client;
            client.UseSandbox = true;
            client.AccountInfoAsync(
                accountInfo => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    DropboxStatus = Brushes.LawnGreen;
                    SyncDropbox();
                })),
                error => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    HelpText = "Could not connect to Dropbox. Please sign in again.";
                    DropboxStatus = Brushes.Red;
                })));
        }

        // Syncing files on SkyDrive
        public void SyncDropbox()
        {
            var client = Settings.Instance.DropboxClient;
            if (client == null || Settings.Instance[Settings.Dropbox] == "0")
            {
                DropboxStatus = Brushes.Red;
                return;
            }
            DropboxStatus = Brushes.Purple;

            // Get all local XML files
            var localFiles = (from file in Directory.EnumerateFiles(Settings.Instance[Settings.LocalPath])
                              where file.EndsWith(".xml")
                              select file).ToList();

            // Get filelist from Dropbox
            client.GetMetaDataAsync("/",
                metaData =>
                {
                    var files = metaData.Contents;

                    var deletedFiles = new List<string>();
                    try
                    {
                        // Downloading deletion log file
                        var log = client.GetFile("/Deletion.log");
                        File.WriteAllBytes(Settings.Instance[Settings.LocalPath] + "/Deletion.log", log);

                        var logfile = DeserialiseLog("Deletion.log");
                        foreach (
                            var deletable in
                                logfile.Where(
                                    deletable => deletable.Value >= Directory.GetLastAccessTimeUtc(deletable.Key))
                            )
                        {
                            deletedFiles.Add(deletable.Key);
                            if (File.Exists(deletable.Key))
                                File.Delete(deletable.Key);
                        }
                    }
                    catch (DropboxException)
                    {

                    }
                    finally
                    {
                        localFiles.RemoveAll(deletedFiles.Contains);
                    }

                    var changelog = new SerializableDictionary<string, DateTime>();
                    try
                    {
                        // Downloading change log file
                        var log = client.GetFile("/Change.log");
                        File.WriteAllBytes(Settings.Instance[Settings.LocalPath] + "/Change.log", log);

                        changelog = DeserialiseLog("Change.log");
                    }
                    catch (IOException)
                    {
                        HelpText = "Can not load change logs.";
                        return;
                    }
                    catch (DropboxException deException)
                    {
                        if (deException.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            foreach (var localFile in localFiles)
                            {
                                var name = Path.GetFileName(localFile) ?? "";
                                changelog[name] = Directory.GetLastWriteTimeUtc(localFile);
                            }
                        }
                    }

                    // Uploading new files
                    foreach (var localFile in localFiles)
                    {
                        if ((localFile).EndsWith(".log")) continue;
                        var toUpload = !(from file in files
                                         where !file.Name.EndsWith(".log")
                                         let localtime = Directory.GetLastWriteTimeUtc(localFile)
                                         where file.Name == Path.GetFileName(localFile) && changelog[file.Name] >= localtime
                                         select file).Any();
                        if (!toUpload) continue;
                        try
                        {
                            using (var fs = new FileStream(localFile, FileMode.Open))
                            {
                                client.UploadFile("/", Path.GetFileName(localFile), fs);
                                changelog[Path.GetFileName(localFile)] = Directory.GetLastWriteTimeUtc(localFile);
                            }
                        }
                        catch (Exception ex)
                        {
                            HelpText = ex.Message;
                            DropboxStatus = Brushes.Red;
                        }
                    }
                    SerialiseLog(changelog, "Change.log");
                    using (var fs = new FileStream(Settings.Instance[Settings.LocalPath] + "/Change.log", FileMode.Open))
                    {
                        client.UploadFile("/", "Change.log", fs);
                    }

                    // Downloading files
                    foreach (var file in files)
                    {
                        if ((file.Name).EndsWith(".log")) continue;
                        var toDl =
                            localFiles.All(
                                localFile =>
                                    file.Name != Path.GetFileName(localFile) ||
                                    changelog[file.Name] > Directory.GetLastWriteTimeUtc(localFile));
                        if (!toDl) continue;
                        try
                        {
                            var dlResult = client.GetFile(file.Path);
                            var path = Settings.Instance[Settings.LocalPath] + "\\" + file.Name;
                            File.WriteAllBytes(path, dlResult);
                            File.SetLastWriteTime(path, changelog[file.Name]);
                        }
                        catch (Exception ex)
                        {
                            HelpText = ex.Message;
                            DropboxStatus = Brushes.Red;
                        }
                    }
                    DropboxStatus = Brushes.LawnGreen;
                    Settings.Instance[Settings.LastSync] = DateTime.Now.ToString("s");
                    LoadDataGrid();
                },
                error =>
                {
                    HelpText = "Dropbox error: " + error.Response.StatusCode.ToString();
                    DropboxStatus = Brushes.Red;
                });
        }

        #endregion
        
        private Deck LoadDeck()
        {
            if (SelectedDeck < 0) return null;
            var path = GetDeckPath(SelectedDeck);
            try
            {
                using (var file = XmlReader.Create(path))
                {
                    var ser = new DataContractSerializer(typeof(Deck));
                    return (Deck)ser.ReadObject(file);
                }

            }
            catch (IOException ex)
            {
                MessageBox.Show("Could not open deck: " + ex.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return null;
            }
        }

        private string GetDeckPath(int selectedDeck)
        {
            var item = DeckList[selectedDeck];
            return Settings.Instance[Settings.LocalPath] +
                        item.Name + "_" +
                       item.TopicA + "_" +
                       item.TopicB + ".xml";
        }

        private void ViewDeck()
        {
            var deck = LoadDeck();
            if (deck == null) return;
            var vd = new CardEditWindow(deck);
            vd.Show();
        }

        private void StartTest()
        {
            var deck = LoadDeck();
            deck = PrepareDeckForTest(deck);
            if (deck == null) return;
            var td = new TestingWindow(deck);
            td.Show();
        }

        private Deck PrepareDeckForTest(Deck deck)
        {
            if (deck == null) return null;
            Deck.Comparator comparator;
            if (Comparator == 0)
            {
                comparator = (i, i1) => i >= i1;
            }
            else
            {
                comparator = (i, i1) => i <= i1;
            }
            deck.MassRemove(comparator, Difficulty);
            if (Flipped) deck.FlipDeck();
            if (Shuffled) deck.Shuffle();
            if (deck.Count != 0) return deck;
            MessageBox.Show("Deck is empty with these settings. Please change them.");
            return null;
        }

        private void DeleteDeck()
        {
            var delfile = GetDeckPath(SelectedDeck);
            File.Delete(delfile);
            SyncDeletionWithSkyDrive(delfile);
            SyncDeletionWithDropbox(delfile);
            LoadDataGrid();
        }

        private SerializableDictionary<string, DateTime> DeserialiseLog(string logName)
        {
            try
            {
                using (var infile = new StreamReader(Settings.Instance[Settings.LocalPath] + "/" + logName))
                {
                    var reader = new XmlSerializer(typeof(SerializableDictionary<string, DateTime>));
                    return (SerializableDictionary<string, DateTime>)reader.Deserialize(infile);
                }
            }
            catch (IOException)
            {
                return new SerializableDictionary<string, DateTime>();
            }
        }

        private void SerialiseLog(SerializableDictionary<string, DateTime> log, string logName)
        {
            using (var outfile = new StreamWriter(Settings.Instance[Settings.LocalPath] + "/" + logName))
            {
                var writer = new XmlSerializer(typeof(SerializableDictionary<string, DateTime>));
                writer.Serialize(outfile, log);
            }
        }

        private Dictionary<string, DateTime> MaintainLog(SerializableDictionary<string, DateTime> log)
        {
            return log.Where(x => (DateTime.UtcNow - x.Value).Days < 30).ToDictionary(x => x.Key, x => x.Value);
        }

        private void LoadDataGrid()
        {
            try
            {
                if(!Directory.Exists(Settings.Instance[Settings.LocalPath])) Directory.CreateDirectory(Settings.Instance[Settings.LocalPath]);
                // Get all XML files
                var files = (from file in Directory.EnumerateFiles(Settings.Instance[Settings.LocalPath], "*.xml")
                             select file).ToList();
                var deckList = new List<MainWindowGridVm>();

                // Parse filename according to syntax and filling Deck list
                foreach (var f in files)
                {
                    char[] separators = { '\\', '_', '.' };
                    var fileName = f.Split(separators);
                    var count = fileName.Count();
                    if (count >= 5)
                    {
                        deckList.Add(new MainWindowGridVm
                        {
                            Size = ((((float)(new FileInfo(f)).Length)) / 1048576).ToString("#0.###") + " MB",
                            LastMod = File.GetLastWriteTime(f),
                            Name = fileName[count - 4],
                            TopicA = fileName[count - 3],
                            TopicB = fileName[count - 2]
                        });
                    }
                    else
                    {
                        HelpText = f + "'s file name is not valid.";
                    }
                }

                // Update statusbar
                CountText = deckList.Count + " items |";
                DeckList = deckList;
            }
            // Dump exception messages to statusbar
            catch (DirectoryNotFoundException dnfException)
            {
                HelpText = dnfException.Message;
            }
            catch (ArgumentException aException)
            {
                HelpText = aException.Message;
            }
            catch (UnauthorizedAccessException uaException)
            {
                HelpText = uaException.Message;
            }
            catch (PathTooLongException ptlException)
            {
                HelpText = ptlException.Message;
            }
        }

        private void ExportPdf()
        {
            if (SelectedDeck < 0) return;
            var path = GetDeckPath(SelectedDeck);
            var deck = LoadDeck();
            deck = PrepareDeckForTest(deck);
            if (deck == null) return;
            var doc1 =
                new Document(new Rectangle(Int32.Parse(Settings.Instance[Settings.PdfWidth]),
                    Int32.Parse(Settings.Instance[Settings.PdfHeight])));
            PdfWriter.GetInstance(doc1, new FileStream(path, FileMode.Create));
            var arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "ARIALUNI.TTF");
            var arialBaseFont = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(arialBaseFont);
            doc1.Open();
            for (var i = 0; i < deck.Count; i++)
            {
                var card = new[] { deck[i].TopicA, deck[i].TopicB };
                for (var j = 0; j < 2; j++)
                {
                    doc1.NewPage();
                    var code = card[j];
                    foreach (var line in code.Split('\n'))
                    {
                        if (line.StartsWith("[Image]"))
                        {
                            var base64String = line.Substring(7);
                            var imageBytes = Convert.FromBase64String(base64String);
                            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                            {
                                // Convert byte[] to Image
                                ms.Write(imageBytes, 0, imageBytes.Length);
                                ms.Position = 0;
                                var image = Image.GetInstance(ms);
                                image.Alignment = Element.ALIGN_CENTER;
                                doc1.Add(image);
                            }
                        }
                        else
                        {
                            var paragraph = new Paragraph(line, font) { Alignment = Element.ALIGN_CENTER };
                            doc1.Add(paragraph);
                        }
                    }
                }
            }
            doc1.Close();
            MessageBox.Show("Pdf created at: "+ path);
        }

        // Reload decklist and check cloud service statuses on window activation.
        public void OnActivated(object sender, EventArgs e)
        {
            LoadDataGrid();
            var time = DateTime.Parse(Settings.Instance[Settings.LastSync]);
            if ((DateTime.Now - time).TotalMinutes < 5)
            {
                return;
            }
            SyncSkyDrive();
            SyncDropbox();
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Instance.SaveSettings();
        }

        public DelegateCommand ViewDeckCommand { get; set; }
        public DelegateCommand DeleteDeckCommand { get; set; }
        public DelegateCommand OptionsCommand { get; set; }
        public DelegateCommand NewDeckCommand { get; set; }
        public DelegateCommand StartTestCommand { get; set; }
        public DelegateCommand ExportPdfCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
