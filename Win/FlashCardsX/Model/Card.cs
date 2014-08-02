

namespace FlashCardsX.Model
{
    // A class representing a flash card
    public class Card
    {
        // A card has 3 difficulties. This is helpful for filtering cards in a deck.
        public enum Difficulties
        {
            Easy = 0,
            Medium = 1,
            Hard = 2
        };
        public Difficulties Difficulty { get; set; }

        // A card has two sides. Each has a text on it.
        private readonly string[] _text = new string[2];
        public string TopicA
        {
            get
            {
                return _text[0];
            }
            set
            {
                _text[0] = value;
            }
        }
        public string TopicB
        {
            get
            {
                return _text[1];
            }
            set
            {
                _text[1] = value;
            }
        }
    }
}
