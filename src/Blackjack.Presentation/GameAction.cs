namespace Blackjack.Presentation;

/// <summary>
/// One entry of the action bar: the key that triggers it, its name, and the rule
/// that says whether it is currently allowed. Every front end shows the same
/// entries in the same order (see <see cref="GameActions"/>).
/// </summary>
/// <param name="Key">The upper-case letter that triggers the action; matching is case-insensitive.</param>
/// <param name="Label">The action's name without the key, also its accessible name, e.g. <c>Place Bet</c>.</param>
/// <param name="IsEnabled">Reads the enable rule from the controller; Quit is always enabled.</param>
public sealed record GameAction(char Key, string Label, Func<GameController, bool> IsEnabled)
{
    /// <summary>The label as shown on screen, e.g. <c>[B] Place Bet</c>.</summary>
    public string DisplayText => $"[{Key}] {Label}";

    /// <summary>The key as text for UI automation, e.g. <c>B</c>.</summary>
    public string AcceleratorKey => Key.ToString();
}
