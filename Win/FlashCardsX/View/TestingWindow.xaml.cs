using FlashCardsX.Model;
using FlashCardsX.ViewModel;

namespace FlashCardsX.View
{
    /// <summary>
    /// Interaction logic for TestingWindow.xaml
    /// </summary>
    public partial class TestingWindow
    {
        public TestingWindow(Deck deck)
        {
            InitializeComponent();
            var twm = new TestingViewModel(deck) {Close = Close};
            DataContext = twm;
        }
    }
}
