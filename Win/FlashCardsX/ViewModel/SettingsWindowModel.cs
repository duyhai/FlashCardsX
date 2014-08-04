using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using DropNet;
using Microsoft.Live;

namespace FlashCardsX.ViewModel
{
    // ViewModel for Settings window.
    public class SettingsWindowModel: INotifyPropertyChanged
    {
        #region Properties

        public int PdfWidth { get; set; }
        public int PdfHeight { get; set; }

        private bool _skyDrive;
        public bool SkyDrive
        {
            get { return _skyDrive; }
            set
            {
                if (value)
                {
                    Dropbox = false;
                }
                else if(_skyDrive)
                {
                    SkyDriveUser = "Not signed in.";
                    Settings.Instance.DeleteSkyDriveData();
                    DeleteCookies();
                }
                _skyDrive = value;
                NotifyPropertyChanged("SkyDrive");
            }
        }

        private bool _dropbox;
        public bool Dropbox
        {
            get { return _dropbox; }
            set
            {
                if (value)
                {
                    SkyDrive = false;
                }
                else if (_dropbox)
                {
                    DropboxUser = "Not signed in.";
                    Settings.Instance.DeleteDropboxData();
                    DeleteCookies();
                }
                _dropbox = value;
                NotifyPropertyChanged("Dropbox");
            }
        }

        private string _skyDriveUser;
        public string SkyDriveUser
        {
            get { return _skyDriveUser; }
            set
            {
                _skyDriveUser = value;
                NotifyPropertyChanged("SkyDriveUser");
            }
        }

        private string _dropboxUser;
        public string DropboxUser
        {
            get { return _dropboxUser; }
            set
            {
                _dropboxUser = value;
                NotifyPropertyChanged("DropboxUser");
            }
        }

        private string _skyDriveSignInText;
        public string SkyDriveSignInText
        {
            get { return _skyDriveSignInText; }
            set
            {
                _skyDriveSignInText = value;
                NotifyPropertyChanged("SkyDriveSignInText");
            }
        }

        private string _dropboxSignInText;
        public string DropboxSignInText
        {
            get { return _dropboxSignInText; }
            set
            {
                _dropboxSignInText = value;
                NotifyPropertyChanged("DropboxSignInText");
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        private string _localPath;
        public string LocalPath
        {
            get { return _localPath; }
            set
            {
                _localPath = value;
                NotifyPropertyChanged("LocalPath");
            }
        }

        #endregion

        private bool _skyDriveLoggingIn;
        private bool _dropboxLoggingIn;

        public SettingsWindowModel()
        {
            LocalPath = Settings.Instance[Settings.LocalPath];
            PdfWidth = Int32.Parse(Settings.Instance[Settings.PdfWidth]);
            PdfHeight = Int32.Parse(Settings.Instance[Settings.PdfHeight]);
            SkyDrive = Settings.Instance[Settings.SkyDrive] == "1";
            Dropbox = Settings.Instance[Settings.Dropbox] == "1";
            Status = "";
            SkyDriveUser = "Not signed in.";
            DropboxUser = "Not signed in.";
            SkyDriveSignInText = "Sign in";
            DropboxSignInText = "Sign in";

            // Set Save command.
            SaveCommand = new DelegateCommand(x => ExecuteSave(), x => true);
            SkyDriveSignInCommand = new DelegateCommand(
                x => SkyDriveSignIn((WebBrowser)x),
                x => SkyDrive);
            DropboxSignInCommand = new DelegateCommand(
                x => DropboxSignIn((WebBrowser)x),
                x => Dropbox);

            if (Settings.Instance[Settings.SkyDrive] == "1") GetSkyDriveUserData();
            if (Settings.Instance[Settings.Dropbox] == "1") GetDropboxUserData();
        }

        #region SkyDrive

        // Get SkyDrive user data
        public async void GetSkyDriveUserData()
        {
            try
            {
                Settings.Instance.SkyDriveAuthClient = new LiveAuthClient(Settings.SkyDriveClientId, Settings.Instance);
                var loginResult = await Settings.Instance.SkyDriveAuthClient.InitializeAsync(Settings.SkyDriveScope);
                if (loginResult == null || LiveConnectSessionStatus.Connected != loginResult.Status) return;

                Settings.Instance.SkyDriveClient = new LiveConnectClient(loginResult.Session);
                var operationResult = await Settings.Instance.SkyDriveClient.GetAsync("me");
                dynamic result = operationResult.Result;
                SkyDriveUser = result.last_name + ", " + result.first_name;
            }
            catch (LiveAuthException)
            {
                Status = "Could not log in to SkyDrive";
            }
        }

        // Sign in button. Opens authentication page in a mini browser. If already open, cancels.
        private void SkyDriveSignIn(WebBrowser browser)
        {
            if (!_skyDriveLoggingIn)
            {
                if (Settings.Instance.SkyDriveAuthClient != null)
                {
                    var logoutUrl = Settings.Instance.SkyDriveAuthClient.GetLogoutUrl();
                    browser.Navigate(logoutUrl);
                }
                DeleteCookies();
                Settings.Instance.SkyDriveAuthClient = new LiveAuthClient(Settings.SkyDriveClientId, Settings.Instance);
                var signInUrl = Settings.Instance.SkyDriveAuthClient.GetLoginUrl(Settings.SkyDriveScope);
                browser.Navigate(signInUrl);
                browser.Visibility = Visibility.Visible;
                SkyDriveSignInText = "Cancel";
                _skyDriveLoggingIn = true;
            }
            else
            {
                browser.Visibility = Visibility.Collapsed;
                SkyDriveSignInText = "Sign in";
                _skyDriveLoggingIn = false;
            }
        }

        // If on correct page of browser. Sign in complete.
        public async void SkyDriveWebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = (WebBrowser) sender;
            SetZoom(browser, 0.75);
            if ((!e.Uri.AbsoluteUri.Contains("code=") && !e.Uri.AbsoluteUri.Contains("access_denied")) ||
                !_skyDriveLoggingIn) return;
            if (e.Uri.AbsoluteUri.Contains("code="))
            {
                var authCode = "";
                var queryParams = e.Uri.Query.TrimStart('?').Split('&');
                foreach (var kvp in queryParams.Select(param => param.Split('=')).Where(kvp => "code" == kvp[0]))
                {
                    authCode = kvp[1];
                }
                if (Settings.Instance.SkyDriveAuthClient != null)
                {
                    var session = await Settings.Instance.SkyDriveAuthClient.ExchangeAuthCodeAsync(authCode);
                    Settings.Instance.SkyDriveClient = new LiveConnectClient(session);
                    Settings.Instance[Settings.LastSync] = (new DateTime()).ToString("s");
                    GetSkyDriveUserData();
                }
            }
            browser.Visibility = Visibility.Collapsed;
            SkyDriveSignInText = "Sign in";
            _skyDriveLoggingIn = false;
        }

        #endregion

        #region Dropbox

        // Get Dropbox user data
        public void GetDropboxUserData()
        {
            var client = new DropNetClient(Settings.DropboxKey, Settings.DropboxSecret) { UserLogin = Settings.Instance.DropboxLogin };
            Settings.Instance.DropboxClient = client;
            client.UseSandbox = true;
            client.AccountInfoAsync(
                accountInfo => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    DropboxUser = accountInfo.display_name;
                })),
                error => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Status = "Could not log in to Dropbox";
                })));
        }

        // Sign in button. Opens authentication page in a mini browser. If already open, cancels.
        private void DropboxSignIn(WebBrowser browser)
        {
            var client = new DropNetClient(Settings.DropboxKey, Settings.DropboxSecret);
            Settings.Instance.DropboxClient = client;
            if (!_dropboxLoggingIn)
            {
                client.GetTokenAsync(
                    userLogin => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        DeleteCookies();
                        var loginUrl = client.BuildAuthorizeUrl(userLogin);
                        browser.Navigate(loginUrl);
                        browser.Visibility = Visibility.Visible;
                        DropboxSignInText = "Cancel";
                        _dropboxLoggingIn = true;
                    })),
                    error =>
                    {

                    });
            }
            else
            {
                browser.Visibility = Visibility.Collapsed;
                DropboxSignInText = "Sign in";
                _dropboxLoggingIn = false;
            }
        }

        // If on correct page of browser. Sign in complete.
        public void DropboxWebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = (WebBrowser)sender;
            SetZoom(browser, 0.7);
            if ((!e.Uri.AbsoluteUri.Equals("https://www.dropbox.com/home") &&
                 !e.Uri.AbsoluteUri.Equals("https://www.dropbox.com/1/oauth/authorize_submit")) || !_dropboxLoggingIn)
                return;
            var client = Settings.Instance.DropboxClient;
            if (client != null)
            {
                client.GetAccessTokenAsync(
                    accessToken =>
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            Settings.Instance.DropboxLogin = accessToken;
                            Settings.Instance[Settings.LastSync] = (new DateTime()).ToString("s");
                            GetDropboxUserData();
                        })),
                    error =>
                    {

                    });
            }
            browser.Visibility = Visibility.Collapsed;
            DropboxSignInText = "Sign in";
            _dropboxLoggingIn = false;
        }

        #endregion
        

        // An external function used to delete cookies.
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        // Delete all IE cookies.
        private static void DeleteCookies()
        {
            unsafe
            {
                var option = 3 /* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
                var optionPtr = &option;

                var success = InternetSetOption(IntPtr.Zero, 81 /*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/,
                    new IntPtr(optionPtr), sizeof(int));
                if (!success)
                {
                    MessageBox.Show("Failed to delete cookies. Logout failed.");
                }
            }
        }

        // www.tonysistemas.com.br
        public static void SetZoom(WebBrowser browser, double zoom)
        {
            var doc = browser.Document as mshtml.IHTMLDocument2;
            if (doc == null) return;
            doc.parentWindow.execScript("document.body.style.zoom=" + zoom.ToString("F").Replace(",", ".") + ";");
        }

        // Saving settings into the Settings object.
        private void ExecuteSave()
        {
            if (!Directory.Exists(LocalPath))
            {
                if (MessageBox.Show(LocalPath + " does not exist. Do you wish to create it?", "",
                        MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                {
                    LocalPath = Settings.Instance[Settings.LocalPath];
                }
                else
                {
                    Directory.CreateDirectory(LocalPath);
                }
            }
            Settings.Instance[Settings.LocalPath] = LocalPath;
            Settings.Instance[Settings.PdfWidth] = PdfWidth.ToString("D");
            Settings.Instance[Settings.PdfHeight] = PdfHeight.ToString("D");
            Settings.Instance[Settings.SkyDrive] = SkyDrive ? "1" : "0";
            Settings.Instance[Settings.Dropbox] = Dropbox ? "1" : "0";
            Status = Settings.Instance.SaveSettings();
        }

        public DelegateCommand SkyDriveSignInCommand { get; set; }
        public DelegateCommand DropboxSignInCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        // Handling property change
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
