using System.Linq;
using FlashCardsX.Model;
using FlashCardsX.ViewModel;

namespace FlashCardsX.View
{
    /// <summary>
    /// Interaction logic for CardEditWindow.xaml
    /// </summary>
    public partial class CardEditWindow
    {
        public CardEditWindow(Deck deck)
        {
            InitializeComponent();
            DataContext = new CardEditViewModel(deck);
        }
    }
}
