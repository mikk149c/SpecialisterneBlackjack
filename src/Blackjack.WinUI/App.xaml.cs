using Blackjack.Presentation;
using Microsoft.UI.Xaml;

namespace Blackjack.WinUI;

public sealed partial class App : Application
{
    private Window? _mainWindow;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Everything that can fail at startup (the art file, the engine) happens here, in one place.
        var cardBack = CardArtService.Gallery.GetCardBack();
        var controller = GameController.CreateDefault();

        _mainWindow = new MainWindow(controller, cardBack);
        _mainWindow.Activate();
    }
}
