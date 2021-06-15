using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;

using Pokedex.ViewModels;


namespace Pokedex
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainPageVM();

            this.SizeChanged += MainPage_SizeChanged;

            SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;
        }

        private void MainPage_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size(1080, 720));
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.IsSuggestionListOpen = false;
        }
    }
}
