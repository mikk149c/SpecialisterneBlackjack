namespace Blackjack.Game;

/// <summary>
/// A player's or dealer's set of cards, and the blackjack rules for scoring them.
/// </summary>
public interface IHand
{
    /// <summary>Cards held, in the order they were added.</summary>
    IReadOnlyList<Card> Cards { get; }

    void AddCard(Card card);

    /// <summary>
    /// Best possible total for this hand: Aces count as 11 unless that would
    /// bust the hand, in which case they count as 1. Face cards count as 10.
    /// </summary>
    int Value { get; }

    /// <summary>True when <see cref="Value"/> exceeds 21.</summary>
    bool IsBust { get; }

    /// <summary>True when this hand is exactly two cards totalling 21.</summary>
    bool IsBlackjack { get; }

    public void RevealHoleCard();
}
