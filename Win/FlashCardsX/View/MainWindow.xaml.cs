using FlashCardsX.ViewModel;

namespace FlashCardsX.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var mwm = new MainWindowModel();
            Closing += mwm.OnClosing;
            Activated += mwm.OnActivated;
            DataContext = mwm;
        }
    }
}
