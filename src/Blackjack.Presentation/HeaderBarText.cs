namespace Blackjack.Presentation;

/// <summary>
/// The wording of the info bar across the top of the screen: the same labels in
/// every front end, so the console header line and the WinUI header cannot drift
/// apart.
/// </summary>
public static class HeaderBarText
{
    // Column widths of the fixed-width console line (label plus value column);
    // a value wider than its column is never cut.
    private const int BalanceColumnWidth = 9 + 8;
    private const int BetColumnWidth = 5 + 6;
    private const int LastRoundColumnWidth = 12 + 12;

    private const string NoLastRound = "-";

    /// <summary>The balance entry, e.g. <c>Balance: 1000</c>.</summary>
    public static string Balance(int balance) => $"Balance: {balance}";

    /// <summary>The current-bet entry, e.g. <c>Bet: 50</c>.</summary>
    public static string Bet(int currentBet) => $"Bet: {currentBet}";

    /// <summary>The last-round entry, e.g. <c>Last Round: PlayerWin</c> or <c>Last Round: -</c>.</summary>
    public static string LastRound(string lastRoundLabel) => $"Last Round: {lastRoundLabel}";

    /// <summary>
    /// The three entries as one console line: a leading space, then each entry padded to
    /// a fixed column and separated by a space. It is not truncated to the screen width.
    /// </summary>
    public static string ConsoleLine(int balance, int currentBet, string lastRoundLabel) =>
        $" {Balance(balance),-BalanceColumnWidth} {Bet(currentBet),-BetColumnWidth} {LastRound(lastRoundLabel),-LastRoundColumnWidth}";

    /// <summary>
    /// A screen-reader description of the whole bar, e.g.
    /// <c>Balance 1000, bet 0, last round none</c>. The "-" placeholder is read as "none".
    /// </summary>
    public static string AccessibleName(int balance, int currentBet, string lastRoundLabel)
    {
        string lastRound = lastRoundLabel == NoLastRound ? "none" : lastRoundLabel;
        return $"Balance {balance}, bet {currentBet}, last round {lastRound}";
    }
}
