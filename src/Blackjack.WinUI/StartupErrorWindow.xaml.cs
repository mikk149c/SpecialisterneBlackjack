using Blackjack.Presentation;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace Blackjack.WinUI;

/// <summary>
/// Tells the player why the game could not start, then ends the app when closed. It shows text
/// only; the wording comes from <see cref="ErrorMessages"/>.
/// </summary>
public sealed partial class StartupErrorWindow : Window
{
    // Device-independent pixels; large enough for a long file path without a scroll bar.
    private const int WidthInDips = 560;
    private const int HeightInDips = 260;

    public StartupErrorWindow(string message)
    {
        InitializeComponent();
        HeadingText.Text = ErrorMessages.StartupTitle;
        MessageText.Text = message;

        Root.Loaded += (_, _) =>
        {
            double scale = Root.XamlRoot.RasterizationScale;
            AppWindow.Resize(new SizeInt32((int)Math.Ceiling(WidthInDips * scale), (int)Math.Ceiling(HeightInDips * scale)));
            CloseButton.Focus(FocusState.Programmatic);
        };
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
