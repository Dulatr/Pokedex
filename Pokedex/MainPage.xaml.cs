using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using Pokedex.ViewModels;

namespace Pokedex
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainPageVM();

            SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.IsSuggestionListOpen = false;
        }
    }
}
