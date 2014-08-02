using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FlashCardsX.Model
{
    // A flashcard deck. Contains flash cards.
    [DataContract]
    public class Deck
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string TopicA { get; set; }
        [DataMember]
        public string TopicB { get; set; }

        [DataMember]
        // Flash cards are stored in a List.
        private List<Card> _cards = new List<Card>();

        // Cards can be accessed by the Deck's indexed property.
        public Card this[int i]
        {
            get
            {
                return _cards[i];
            }
        }

        // Get card count
        public int Count
        {
            get { return _cards.Count; }
        }

        // Add cards
        public void Add(Card c)
        {
            _cards.Add(c);
        }

        // Remove cards
        public void Remove(Card c)
        {
            _cards.Remove(c);
        }

        // Shuffle card order. Randomly swap two cards for each card in the deck.
        public void Shuffle()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                var rdm = new Random();
                var n = rdm.Next(_cards.Count);
                var temp = _cards[i];
                _cards[i] = _cards[n];
                _cards[n] = temp;
            }
        }

        public delegate bool Comparator(int x, int y);

        // Filter by difficulty.
        public void MassRemove(Comparator comp, int difficulty)
        {
            _cards = _cards.Where(x => comp((int)x.Difficulty, difficulty)).ToList();
        }

        // Swap topics
        public void FlipDeck()
        {
            var tempTopic = TopicA;
            TopicA = TopicB;
            TopicB = tempTopic;
            foreach (var card in _cards)
            {
                var tempCard = card.TopicA;
                card.TopicA = card.TopicB;
                card.TopicB = tempCard;
            }
        }
    }
}
