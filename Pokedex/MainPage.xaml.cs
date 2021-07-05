using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.GamepadDPadUp || args.VirtualKey == VirtualKey.Up || args.VirtualKey == VirtualKey.GamepadLeftThumbstickUp)
                DPadUp.Command.Execute("Up");
            else if (args.VirtualKey == VirtualKey.GamepadDPadLeft || args.VirtualKey == VirtualKey.Left || args.VirtualKey == VirtualKey.GamepadLeftThumbstickLeft)
                DPadLeft.Command.Execute("Left");
            else if (args.VirtualKey == VirtualKey.GamepadDPadRight || args.VirtualKey == VirtualKey.Right || args.VirtualKey == VirtualKey.GamepadLeftThumbstickRight)
                DPadRight.Command.Execute("Right");
            else if (args.VirtualKey == VirtualKey.GamepadDPadDown || args.VirtualKey == VirtualKey.Down || args.VirtualKey == VirtualKey.GamepadLeftThumbstickDown)
                DPadDown.Command.Execute("Down");
            else if (args.VirtualKey == VirtualKey.GamepadA || args.VirtualKey == VirtualKey.A)
                A_Button.Command.Execute("A");
            else if (args.VirtualKey == VirtualKey.GamepadB || args.VirtualKey == VirtualKey.B)
                B_Button.Command.Execute("B");

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
