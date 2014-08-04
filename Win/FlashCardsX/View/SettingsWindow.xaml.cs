using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FlashCardsX.ViewModel;

namespace FlashCardsX.View
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {

        // Constructor
        // Binding fields
        public SettingsWindow()
        {
            InitializeComponent();
            var swm = new SettingsWindowModel();
            SkyDriveWebBrowser.LoadCompleted += swm.SkyDriveWebBrowser_OnLoadCompleted;
            DropboxWebBrowser.LoadCompleted += swm.DropboxWebBrowser_OnLoadCompleted;
            DataContext = swm;
        }

        // Allowing only numbers in a TextBox.
        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text.Last()) && !Char.IsControl(e.Text.Last())) e.Handled = true;
        }
        
        // Exit button

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
