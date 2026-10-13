using System.ComponentModel;

namespace Blackjack.Presentation;

/// <summary>
/// What the WinUI action bar adds on top of the <see cref="GameController"/>: the text of
/// its status line and whether Place Bet is available. The text is the controller's
/// status, except that a front end can show a one-shot notice (for example an error) on
/// top of it. The notice is cleared as soon as the controller changes anything, so it
/// can never hide a later game message.
/// </summary>
public sealed class ActionBarStatus : INotifyPropertyChanged
{
    private readonly GameController _controller;
    private string _notice = string.Empty;

    public ActionBarStatus(GameController controller)
    {
        _controller = controller;
        _controller.PropertyChanged += OnControllerChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// WinUI-only addition (the console has no such handling): no round is running and there is
    /// nothing left to bet, so no further round can ever start.
    /// </summary>
    public bool IsOutOfCredits => _controller.Balance == 0 && !_controller.IsRoundInProgress;

    /// <summary>The controller's rule, except that out of credits no bet is possible.</summary>
    public bool CanPlaceBet => _controller.CanPlaceBet && !IsOutOfCredits;

    /// <summary>
    /// The notice while one is showing, then "out of credits" when the game cannot continue,
    /// otherwise <see cref="GameController.Status"/>.
    /// </summary>
    public string Text
    {
        get
        {
            if (_notice.Length > 0)
                return _notice;

            return IsOutOfCredits ? StatusMessages.OutOfCredits : _controller.Status;
        }
    }

    /// <summary>Shows <paramref name="notice"/> until the controller next changes state.</summary>
    public void ShowNotice(string notice)
    {
        _notice = notice;
        Raise(nameof(Text));
    }

    private void OnControllerChanged(object? sender, PropertyChangedEventArgs e)
    {
        _notice = string.Empty;
        Raise(nameof(Text));
        Raise(nameof(CanPlaceBet));
    }

    private void Raise(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
