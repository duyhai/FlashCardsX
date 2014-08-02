using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using FlashCardsX.Model;
using Microsoft.Win32;
using Image = System.Drawing.Image;

namespace FlashCardsX.ViewModel
{
    public class CardEditViewModel : INotifyPropertyChanged
    {
        private readonly Deck _deck;
        private int _currentCard;
        public int CurrentCard
        {
            get { return _currentCard; }
            set
            {
                _currentCard = value;
                CountText = "Card No.: " + (_currentCard + 1) + "/" + _deck.Count + " | ";
                NotifyPropertyChanged("CurrentCard");
            }
        }

        private string _countText;
        public string CountText
        {
            get { return _countText; }
            set
            {
                _countText = value;
                NotifyPropertyChanged("CountText");
            }
        }

        private string _statusMsg;
        public string StatusMsg
        {
            get { return _statusMsg; }
            set
            {
                _statusMsg = value;
                NotifyPropertyChanged("StatusMsg");
            }
        }

        private string _deckName;
        public string DeckName
        {
            get { return _deckName; }
            set
            {
                _deckName = value;
                NotifyPropertyChanged("DeckName");
            }
        }

        private Visibility _editing;
        public Visibility Editing
        {
            get { return _editing; }
            set
            {
                _editing = value;
                NotifyPropertyChanged("Editing");
            }
        }

        private Visibility _previewing;
        public Visibility Previewing
        {
            get { return _previewing; }
            set
            {
                _previewing = value;
                NotifyPropertyChanged("Previewing");
            }
        }

        private string _topicA;
        public string TopicA
        {
            get { return _topicA; }
            set
            {
                _topicA = value;
                NotifyPropertyChanged("TopicA");
            }
        }

        private string _topicB;
        public string TopicB
        {
            get { return _topicB; }
            set
            {
                _topicB = value;
                NotifyPropertyChanged("TopicB");
            }
        }

        private StackPanel _topicAImage;
        public StackPanel TopicAImage
        {
            get { return _topicAImage; }
            set
            {
                _topicAImage = value;
                NotifyPropertyChanged("TopicAImage");
            }
        }

        private StackPanel _topicBImage;
        public StackPanel TopicBImage
        {
            get { return _topicBImage; }
            set
            {
                _topicBImage = value;
                NotifyPropertyChanged("TopicBImage");
            }
        }

        private int _difficulty;
        public int Difficulty
        {
            get { return _difficulty; }
            set
            {
                _difficulty = value;
                NotifyPropertyChanged("Difficulty");
            }
        }

        public CardEditViewModel(Deck deck)
        {
            Previewing = Visibility.Collapsed;
            Editing = Visibility.Visible;
            _deck = deck;
            DeckName = _deck.Name;
            CurrentCard = 0;
            LoadCard();

            SaveCommand = new DelegateCommand(x =>
            {
                SaveCard();
                SaveDeck();
            },
            x => true);

            NextCardCommand = new DelegateCommand(x =>
            {
                CurrentCard++;
                LoadCard();
            },
            x => CurrentCard < _deck.Count - 1);

            PrevCardCommand = new DelegateCommand(x =>
            {
                CurrentCard--;
                LoadCard();
            },
            x => CurrentCard > 0);

            DeleteCardCommand = new DelegateCommand(x =>
            {
                if (MessageBoxResult.No ==
                    MessageBox.Show("Do you really want to delete this card?", "Confirmation", MessageBoxButton.YesNo,
                        MessageBoxImage.Question)) return;
                _deck.Remove(_deck[CurrentCard]);
                CurrentCard = Math.Max(0, --CurrentCard);
                LoadCard();
                SaveDeck();
            },
            x => _deck.Count > 1);

            NewCardCommand = new DelegateCommand(x =>
            {
                _deck.Add(new Card
                {
                    Difficulty = (Card.Difficulties)Difficulty,
                    TopicA = "",
                    TopicB = ""
                });
                CurrentCard = _deck.Count - 1;
                LoadCard();

                SaveDeck();
            },
            x => true);

            InsertImgCommand = new DelegateCommand(x =>
            {
                var dialog = new OpenFileDialog();
                var imageExtensions = string.Join(";", ImageCodecInfo.GetImageDecoders().Select(ici => ici.FilenameExtension));
                dialog.Filter = string.Format("Images|{0}", imageExtensions);
                dialog.FileOk += (sender, args) =>
                {
                    using (var ms = new MemoryStream())
                    {
                        var image = Image.FromFile(dialog.FileName);
                        // Convert Image to byte[]
                        image.Save(ms, image.RawFormat);
                        var imageBytes = ms.ToArray();

                        // Convert byte[] to Base64 String
                        var base64String = "\n[Image]" + Convert.ToBase64String(imageBytes);
                        Clipboard.SetText(base64String);
                        StatusMsg = "Code copied to clipboard. Paste it into your card.";
                    }
                };
                dialog.ShowDialog();
            },
            x => true);

            PreviewCommand = new DelegateCommand(x =>
            {
                Editing = Visibility.Collapsed;
                Previewing = Visibility.Visible;
                LoadCard();
            },
            x => Editing == Visibility.Visible);

            EditCommand = new DelegateCommand(x =>
            {
                Previewing = Visibility.Collapsed;
                Editing = Visibility.Visible;
            },
            x => Previewing == Visibility.Visible);
        }

        private void SaveCard()
        {
            _deck[CurrentCard].TopicA = TopicA;
            _deck[CurrentCard].TopicB = TopicB;
            _deck[CurrentCard].Difficulty = (Card.Difficulties)Difficulty;
            StatusMsg = "Saved!";
        }

        private void LoadCard()
        {
            Difficulty = (int)_deck[CurrentCard].Difficulty;

            if (Previewing == Visibility.Visible)
            {
                TopicAImage = LoadImage(TopicA);
                TopicBImage = LoadImage(TopicB);
            }
            else
            {
                TopicA = _deck[CurrentCard].TopicA;
                TopicB = _deck[CurrentCard].TopicB;
            }
        }

        private StackPanel LoadImage(string code)
        {
            var resultImage = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
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
                        //var image = Image.FromStream(ms);
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        ms.Position = 0;
                        image.EndInit();
                        var imageCtrl = new System.Windows.Controls.Image
                        {
                            Source = image
                        };
                        resultImage.Children.Add(imageCtrl);
                    }
                }
                else
                {
                    var text = new TextBlock { Text = line };
                    text.BeginInit();
                    text.TextAlignment = TextAlignment.Center;
                    text.EndInit();
                    resultImage.Children.Add(text);
                }
            }
            return resultImage;
        }

        private void SaveDeck()
        {
            var serializer = new DataContractSerializer(typeof(Deck));
            try
            {
                using (var sw = XmlWriter.Create(
                    Settings.Instance[Settings.LocalPath] + "\\" +
                    _deck.Name + "_" +
                    _deck.TopicA + "_" +
                    _deck.TopicB + ".xml"))
                {
                    serializer.WriteObject(sw, _deck);
                    sw.Close();
                }
            }
            catch (IOException ex)
            {
                StatusMsg = "Could not save deck: " + ex.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public DelegateCommand PrevCardCommand { get; set; }
        public DelegateCommand NextCardCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand NewCardCommand { get; set; }
        public DelegateCommand DeleteCardCommand { get; set; }
        public DelegateCommand InsertImgCommand { get; set; }
        public DelegateCommand PreviewCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
    }
}
