using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FlashCardsX.Model;

namespace FlashCardsX.ViewModel
{
    public class TestingViewModel:INotifyPropertyChanged
    {
        private Deck _deck;
        private Deck _wrongDeck;

        private int _cardNum;
        public int CardNum
        {
            get { return _cardNum; }
            set
            {
                _cardNum = value;
                NotifyPropertyChanged("CardNum");
            }
        }

        private int _currentCard;
        public int CurrentCard
        {
            get { return _currentCard; }
            set
            {
                _currentCard = value;
                CardNum = value + 1;
                NotifyPropertyChanged("CurrentCard");
            }
        }

        private int _correct;
        public int Correct
        {
            get { return _correct; }
            set
            {
                _correct = value;
                NotifyPropertyChanged("Correct");
            }
        }

        private int _allCard;
        public int AllCard
        {
            get { return _allCard; }
            set
            {
                _allCard = value;
                NotifyPropertyChanged("AllCard");
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

        private Visibility _aVisibility;
        public Visibility AVisibility
        {
            get { return _aVisibility; }
            set
            {
                _aVisibility = value;
                NotifyPropertyChanged("AVisibility");
            }
        }

        private Visibility _bVisibility;
        public Visibility BVisibility
        {
            get { return _bVisibility; }
            set
            {
                _bVisibility = value;
                NotifyPropertyChanged("BVisibility");
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

        private string _difficulty;
        public string Difficulty
        {
            get { return _difficulty; }
            set
            {
                _difficulty = value;
                NotifyPropertyChanged("Difficulty");
            }
        }

        public TestingViewModel(Deck deck)
        {
            BVisibility = Visibility.Collapsed;
            AVisibility = Visibility.Visible;
            _wrongDeck = new Deck();
            _deck = deck;
            AllCard = _deck.Count;
            DeckName = _deck.Name;
            Correct = 0;
            CurrentCard = 0;
            LoadCard();

            CorrectAnswerCommand = new DelegateCommand(
                x =>
                {
                    Correct++;
                    NextCard();
                },
                x => CurrentCard < _deck.Count);

            WrongAnswerCommand = new DelegateCommand(
                x =>
                {
                    _wrongDeck.Add(_deck[CurrentCard]);
                    NextCard();
                },
                x => CurrentCard < _deck.Count);

            FlipCommand = new DelegateCommand(x =>
            {
                var temp = AVisibility;
                AVisibility = BVisibility;
                BVisibility = temp;
            },
            x => true);
        }

        // Switch to next card.
        private void NextCard()
        {
            BVisibility = Visibility.Collapsed;
            AVisibility = Visibility.Visible;
            var current = CurrentCard + 1;
            if (current >= _deck.Count)
            {
                if (_wrongDeck.Count == 0)
                {
                    if (MessageBox.Show("Deck finished. Want to Restart?", "Finished", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        Close();
                        return;
                    }
                    CurrentCard = 0;
                    Correct = 0;
                    LoadCard();
                }
                else
                {
                    if (MessageBox.Show("Deck finished. Want to redo the wrong cards?", "Finished",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        Close();
                        return;
                    }
                    _deck = _wrongDeck;
                    AllCard = _deck.Count;
                    CurrentCard = 0;
                    Correct = 0;
                    LoadCard();
                    _wrongDeck = new Deck();
                }
            }
            else
            {
                CurrentCard++;
                LoadCard();
            }
        }

        // Load a card image
        private void LoadCard()
        {
            TopicAImage = LoadImage(_deck[CurrentCard].TopicA);
            TopicBImage = LoadImage(_deck[CurrentCard].TopicB);
            Difficulty = _deck[CurrentCard].Difficulty.ToString();
        }

        // Render a card image from text
        private static StackPanel LoadImage(string code)
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
                        ms.Write(imageBytes, 0, imageBytes.Length);
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        ms.Position = 0;
                        image.EndInit();
                        var imageCtrl = new Image
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public DelegateCommand CorrectAnswerCommand { get; set; }
        public DelegateCommand WrongAnswerCommand { get; set; }
        public DelegateCommand FlipCommand { get; set; }

        public delegate void CloseWindow();

        public CloseWindow Close;
    }
}
