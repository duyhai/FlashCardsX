using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DropNet;
using DropNet.Models;
using Microsoft.Live;

namespace FlashCardsX
{
    // Singleton class for storing settings
    public sealed class Settings: IRefreshTokenHandler
    {
        private static readonly object Lock = new object();
        private static Settings _instance;
        private RefreshTokenInfo _refreshTokenInfo;

        // Clients for accessing cloud storage
        public DropNetClient DropboxClient;
        public UserLogin DropboxLogin;
        public LiveConnectClient SkyDriveClient;
        public LiveAuthClient SkyDriveAuthClient;

        // Constants for setting up cloud connections
        public static string SkyDriveClientId = <Enter your own SkyDrive clientID>;
        public static string[] SkyDriveScope = { "wl.signin", "wl.basic", "wl.skydrive_update", "wl.offline_access" };
        public static string DropboxKey = <Enter your own Dropbox key>;
        public static string DropboxSecret = <Enter your own Dropbox secret>;

        // Constants for settings keys
        public static readonly string
            LocalPath = "LocalPath",
            PdfWidth = "PdfWidth",
            PdfHeight = "PdfHeight",
            LastSync = "LastSync",
            SkyDrive = "SkyDrive",
            Dropbox = "Dropbox";
        private static SerializableDictionary<string, string> _settingsValues;

        // Deserialise or initialise on construction.
        private Settings()
        {
            // Try retrieving persisted settings.
            try
            {
                using (var file = new StreamReader(@"Settings.xml"))
                {
                    var reader = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    _settingsValues = (SerializableDictionary<string, string>)reader.Deserialize(file);

                    // SkyDrive Token
                    _refreshTokenInfo = _settingsValues["SkyDriveRefreshToken"] == "" ? null : new RefreshTokenInfo(_settingsValues["SkyDriveRefreshToken"]);

                    // Dropbox Token
                    DropboxLogin = new UserLogin
                    {
                        Token = _settingsValues["DropboxUserToken"],
                        Secret = _settingsValues["DropboxUserSecret"]
                    };
                    file.Close();
                }
            }
            // If we cannot retrieve, set default values.
            catch (IOException)
            {
                _settingsValues = new SerializableDictionary<string, string>
                {
                    {LocalPath, "Decks\\"},
                    {PdfWidth, "300"},
                    {PdfHeight, "300"},
                    {LastSync, (new DateTime()).ToString("s")},
                    {SkyDrive, "0"},
                    {Dropbox, "0"}
                };
                DropboxLogin = new UserLogin();
            }
        }

        // Singleton getter
        public static Settings Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        return _instance = new Settings();
                    }
                }
                return _instance;
            }
        }

        // Deleting all persisted SkyDrive data
        public void DeleteSkyDriveData()
        {
            SkyDriveClient = null;
            SkyDriveAuthClient = null;
            _refreshTokenInfo = null;
        }

        // Deleting all persisted Dropbox data
        public void DeleteDropboxData()
        {
            DropboxLogin = new UserLogin();
            DropboxClient = null;
        }

        // Indexed property for getting values.
        public string this[string id]
        {
            get { return _settingsValues[id]; }
            set
            {
                _settingsValues[id] = value;
            }
        }

        // Serialise settings.
        public string SaveSettings()
        {
            // Save cloud service tokens
            _settingsValues["SkyDriveUserId"] = _refreshTokenInfo == null ? "" : _refreshTokenInfo.UserId;
            _settingsValues["SkyDriveRefreshToken"] = _refreshTokenInfo == null ? "" : _refreshTokenInfo.RefreshToken;
            _settingsValues["DropboxUserToken"] = DropboxLogin == null ? "" : DropboxLogin.Token;
            _settingsValues["DropboxUserSecret"] = DropboxLogin == null ? "" : DropboxLogin.Secret;

            // Serialise
            try
            {
                using (var file = new StreamWriter(@"Settings.xml"))
                {
                    var writer = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    writer.Serialize(file, _settingsValues);
                    file.Close();
                    return "Saved!";
                }
            }
            catch (Exception)
            {
                return "Could not save settings!";
            }
        }

        // These are needed for SkyDrive session persistance
        Task IRefreshTokenHandler.SaveRefreshTokenAsync(RefreshTokenInfo tokenInfo)
        {
            // Note: 
            // 1) In order to receive refresh token, wl.offline_access scope is needed.
            // 2) Alternatively, we can persist the refresh token.
            return Task.Factory.StartNew(() =>
            {
                _refreshTokenInfo = tokenInfo;
            });
        }

        Task<RefreshTokenInfo> IRefreshTokenHandler.RetrieveRefreshTokenAsync()
        {
            return Task.Factory.StartNew(() => _refreshTokenInfo);
        }
    }
}
