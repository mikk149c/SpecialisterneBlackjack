namespace Blackjack.Presentation;

/// <summary>Wording rules for the action bar's status line.</summary>
public static class ActionBarText
{
    /// <summary>
    /// The text of the status line: a front-end notice (for example "this action is not
    /// available yet") takes precedence over the controller's status while it is set.
    /// </summary>
    public static string StatusLine(string? status, string? notice) =>
        string.IsNullOrEmpty(notice) ? status ?? string.Empty : notice;
}
