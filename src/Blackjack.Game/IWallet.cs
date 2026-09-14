namespace Blackjack.Game;

/// <summary>
/// Tracks a player's credit balance and the payouts for a round's outcome:
/// 1:1 for a normal win, 3:2 for blackjack, stake returned on a push, nothing
/// staked back on a loss.
/// </summary>
public interface IWallet
{
    int Balance { get; set; }
    int Bet { get; }
    bool BetPlaced { get; }


    /// <summary>
    /// Deducts <paramref name="amount"/> from <see cref="Balance"/>.
    /// Throws <see cref="ArgumentOutOfRangeException"/> when the amount isn't positive,
    /// and <see cref="InvalidOperationException"/> when it exceeds the current balance.
    /// </summary>
    void PlaceBet(int amount);

    /// <summary>
    /// Credits the payout for <paramref name="outcome"/> on the given
    /// <paramref name="bet"/> back to <see cref="Balance"/>, and returns the
    /// amount credited.
    /// </summary>
    int Settle(RoundOutcome outcome, int bet);
}
