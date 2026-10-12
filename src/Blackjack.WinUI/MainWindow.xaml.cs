using Blackjack.Presentation;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace Blackjack.WinUI;

/// <summary>
/// The game screen. It owns no game logic: the controller and the card-back art
/// come in from <see cref="App"/>, and the XAML binds everything to them.
/// </summary>
public sealed partial class MainWindow : Window
{
    // Sizes are in device-independent pixels. The minimum fits a deck column plus 8 cards
    // (8 x 96 px + gaps) side by side, and the default leaves room for the dealer's hand.
    private const int DefaultWidth = 1000;
    private const int DefaultHeight = 760;
    private const int MinimumWidth = 1000;
    private const int MinimumHeight = 560;

    public MainWindow(GameController controller, IReadOnlyList<string> cardBackLines)
    {
        Controller = controller;
        CardBackLines = cardBackLines;

        InitializeComponent();
        Root.Loaded += (_, _) => ApplyWindowSizes();
    }

    /// <summary>The session shown by this window; the XAML bindings read it.</summary>
    public GameController Controller { get; }

    /// <summary>Card-back art for the deck stack.</summary>
    public IReadOnlyList<string> CardBackLines { get; }

    // AppWindow works in physical pixels, so scale the DIP sizes by the monitor's factor.
    private void ApplyWindowSizes()
    {
        double scale = Root.XamlRoot.RasterizationScale;
        AppWindow.Resize(new SizeInt32(Scaled(DefaultWidth, scale), Scaled(DefaultHeight, scale)));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = Scaled(MinimumWidth, scale);
            presenter.PreferredMinimumHeight = Scaled(MinimumHeight, scale);
        }
    }

    private static int Scaled(int dips, double scale) => (int)Math.Ceiling(dips * scale);
}
