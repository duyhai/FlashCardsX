using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using FlashCardsX.Model;

namespace FlashCardsX.View
{
    /// <summary>
    /// Interaction logic for NewDeckForm.xaml
    /// </summary>
    public partial class NewDeckWindow
    {
        public NewDeckWindow()
        {
            InitializeComponent();
        }

        // Creating a new XML file for the deck.
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var deck = new Deck
            {
                Name = NameTextBox.Text,
                TopicA = TopicATextBox.Text,
                TopicB = TopicBTextBox.Text
            };
            deck.Add(new Card
            {
                Difficulty = Card.Difficulties.Easy,
                TopicA = "",
                TopicB = ""
            });
            var serializer = new DataContractSerializer(typeof(Deck));
            using (var sw = XmlWriter.Create(
                Settings.Instance[Settings.LocalPath] + "\\" +
                deck.Name + "_" +
                deck.TopicA + "_" +
                deck.TopicB + ".xml"))
            {
                serializer.WriteObject(sw, deck);
                sw.Close();
                Close();
            }
            Settings.Instance[Settings.LastSync] = (new DateTime()).ToString("s");
        }
    }
}
