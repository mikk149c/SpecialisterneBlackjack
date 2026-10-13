using System.Diagnostics;
using Blackjack.Presentation;
using Blackjack.WinUI.Dialogs;
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

    private bool _isBetDialogOpen;

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

    // WinUI throws if a second ContentDialog is shown while one is open, and the B key auto-repeats
    // while held, so a request that arrives while the dialog is up is dropped. The flag is reset in
    // finally so a failed ShowAsync cannot lock betting for the rest of the session. This is an
    // async void event handler, so an exception would escape to Application.UnhandledException;
    // it is caught here instead and shown in the status line.
    private async void OnPlaceBetRequested(object? sender, EventArgs e)
    {
        if (_isBetDialogOpen)
            return;

        _isBetDialogOpen = true;
        try
        {
            var dialog = new BetDialog(Root.XamlRoot, Controller.Balance);
            string? input = await dialog.AskAsync();
            if (input is not null)
                Controller.PlaceBet(input);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            _isBetDialogOpen = false;
        }

        // The dialog hands focus back to Place Bet, which is disabled once a round has started.
        Actions.FocusFirstEnabledAction();
    }

    /// <summary>Logs an unexpected exception and tells the player in the status line that something went wrong.</summary>
    public void ShowError(Exception exception)
    {
        Debug.WriteLine($"Unexpected error: {exception}");
        Actions.ShowNotice(ErrorMessages.Unexpected(exception));
    }

    // Window.Close, not Application.Exit: it runs the window's Closed handlers (so the lifecycle
    // work in issue #12 can hook there) and ends the app because this is its only window.
    private void OnQuitRequested(object? sender, EventArgs e) => Close();

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
