using Blackjack.Presentation;
using Microsoft.UI.Xaml;

namespace Blackjack.WinUI;

public sealed partial class App : Application
{
    private Window? _mainWindow;

    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            // Everything that can fail at startup (the art file, the engine) happens here, in one place.
            var cardBack = CardArtService.Gallery.GetCardBack();
            var controller = GameController.CreateDefault();

            _mainWindow = new MainWindow(controller, cardBack);
            _mainWindow.Activate();
        }
        catch (Exception exception)
        {
            // There is no game to show an error in, so a small window with the message takes over;
            // closing it ends the app because it is then the only window.
            _mainWindow = new StartupErrorWindow(ErrorMessages.Startup(exception));
            _mainWindow.Activate();
        }
    }

    // A fault that reaches here (a handler's exception, a failed async void) must not take the game
    // down: while the game window exists it is shown in its status line. Before that there is nothing
    // to recover, so the default handling applies.
    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        if (_mainWindow is not MainWindow window)
            return;

        e.Handled = true;
        window.ShowError(e.Exception);
    }
}
