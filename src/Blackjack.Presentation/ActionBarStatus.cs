using System.ComponentModel;

namespace Blackjack.Presentation;

/// <summary>
/// The text of the action bar's status line. It is the controller's status, except
/// that a front end can show a one-shot notice (for example "this action is not
/// available yet") on top of it. The notice is cleared as soon as the controller
/// changes anything, so it can never hide a later game message.
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

    /// <summary>The notice while one is showing, otherwise <see cref="GameController.Status"/>.</summary>
    public string Text => _notice.Length > 0 ? _notice : _controller.Status;

    /// <summary>Shows <paramref name="notice"/> until the controller next changes state.</summary>
    public void ShowNotice(string notice)
    {
        _notice = notice;
        RaiseTextChanged();
    }

    private void OnControllerChanged(object? sender, PropertyChangedEventArgs e)
    {
        _notice = string.Empty;
        RaiseTextChanged();
    }

    private void RaiseTextChanged() =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
}
