using Blackjack.Presentation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace Blackjack.WinUI.Controls;

/// <summary>
/// The status line and the five action buttons. Enabled states and the status text come from the
/// <see cref="GameController"/>; the keyboard accelerators (B H S D Q, no modifiers) live on the
/// buttons, so they work wherever focus is, a disabled button's key does nothing, and a focused
/// button never fires twice. Hit, Stand and Double Down call the controller directly; Place Bet and
/// Quit are raised as events because the window decides how to bet and how to close.
/// </summary>
public sealed partial class ActionBar : UserControl
{
    public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register(
        nameof(Controller), typeof(GameController), typeof(ActionBar),
        new PropertyMetadata(null, OnControllerChanged));

    private ActionBarStatus? _status;

    public ActionBar()
    {
        InitializeComponent();
        Attach(PlaceBetButton, GameActions.PlaceBet);
        Attach(HitButton, GameActions.Hit);
        Attach(StandButton, GameActions.Stand);
        Attach(DoubleDownButton, GameActions.DoubleDown);
        Attach(QuitButton, GameActions.Quit);
    }

    /// <summary>Raised when the player chooses Place Bet (click or B). The window handles it.</summary>
    public event EventHandler? PlaceBetRequested;

    /// <summary>Raised when the player chooses Quit (click or Q). The window closes itself.</summary>
    public event EventHandler? QuitRequested;

    /// <summary>The session this bar shows and drives.</summary>
    public GameController Controller
    {
        get => (GameController)GetValue(ControllerProperty);
        set => SetValue(ControllerProperty, value);
    }

    /// <summary>
    /// Shows <paramref name="notice"/> in the status line until the controller next changes state;
    /// the controller's own messages are never hidden for longer than that.
    /// </summary>
    public void ShowNotice(string notice) => _status?.ShowNotice(notice);

    /// <summary>
    /// Moves keyboard focus to the first enabled action button, in display order. After a bet that
    /// starts a round that is Hit; after a rejected bet it is Place Bet again.
    /// </summary>
    public void FocusFirstEnabledAction()
    {
        foreach (var button in new[] { PlaceBetButton, HitButton, StandButton, DoubleDownButton, QuitButton })
        {
            if (button.IsEnabled && button.Focus(FocusState.Programmatic))
                return;
        }
    }

    private static void OnControllerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        ((ActionBar)sender).BindStatus((GameController)e.NewValue);

    private void BindStatus(GameController controller)
    {
        _status = new ActionBarStatus(controller);
        _status.PropertyChanged += (_, _) => StatusText.Text = _status.Text;
        StatusText.Text = _status.Text;
    }

    private static void Attach(Button button, GameAction action)
    {
        // Letter keys share their code with the upper-case character. Shift is accepted as well so a
        // capital H works like h (case-insensitive); Ctrl and Alt combinations stay free for the system.
        foreach (var modifiers in new[] { VirtualKeyModifiers.None, VirtualKeyModifiers.Shift })
        {
            var accelerator = new KeyboardAccelerator { Key = (VirtualKey)action.Key, Modifiers = modifiers };
            accelerator.Invoked += OnAcceleratorInvoked;
            button.KeyboardAccelerators.Add(accelerator);
        }
    }

    // Marking the invocation handled stops the button from being clicked. Typing in a text box and
    // answering a dialog (which sits in a popup) must never play the game behind them.
    private static void OnAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        if ((args.Element as UIElement)?.XamlRoot is { } xamlRoot && IsTextEntryOrModalActive(xamlRoot))
            args.Handled = true;
    }

    private static bool IsTextEntryOrModalActive(XamlRoot xamlRoot) =>
        FocusManager.GetFocusedElement(xamlRoot) is TextBox or PasswordBox or RichEditBox
        || VisualTreeHelper.GetOpenPopupsForXamlRoot(xamlRoot).Count > 0;

    private void OnPlaceBetClick(object sender, RoutedEventArgs e) => PlaceBetRequested?.Invoke(this, EventArgs.Empty);

    private void OnHitClick(object sender, RoutedEventArgs e) => Controller.Hit();

    private void OnStandClick(object sender, RoutedEventArgs e) => Controller.Stand();

    private void OnDoubleDownClick(object sender, RoutedEventArgs e) => Controller.DoubleDown();

    private void OnQuitClick(object sender, RoutedEventArgs e) => QuitRequested?.Invoke(this, EventArgs.Empty);
}
