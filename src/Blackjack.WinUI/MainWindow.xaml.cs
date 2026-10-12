using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace Blackjack.WinUI;

public sealed partial class MainWindow : Window
{
    private const int DefaultWidth = 1000;
    private const int DefaultHeight = 760;

    public MainWindow()
    {
        InitializeComponent();
        AppWindow.Resize(new SizeInt32(DefaultWidth, DefaultHeight));
    }
}
