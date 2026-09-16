namespace Blackjack.Game;

/// <summary>
/// GUI-independent single-player-vs-dealer blackjack engine. One instance
/// represents a session: the balance carries over between rounds, and a new
/// round is started by calling <see cref="PlaceBet"/> again once the
/// previous one has resolved.
/// </summary>
public interface IBlackjackGame
{
    int Balance { get; }

    /// <summary>The amount currently at stake for the round in progress.</summary>
    int CurrentBet { get; }

    bool IsRoundInProgress { get; }

    IHand PlayerHand { get; }

    IHand DealerHand { get; }

    /// <summary>
    /// False while the dealer's second card is still hidden (i.e. until the
    /// player ends their turn via <see cref="Stand"/> or <see cref="DoubleDown"/>,
    /// or busts via <see cref="Hit"/>).
    /// </summary>
    bool IsDealerHoleCardRevealed { get; }

    /// <summary>Result of the most recently resolved round, if any.</summary>
    RoundOutcome? LastOutcome { get; }

    /// <summary>
    /// Places the bet for a new round and deals two cards each to player and
    /// dealer. Throws <see cref="InvalidOperationException"/> if a round is
    /// already in progress or the bet exceeds <see cref="Balance"/>, and
    /// <see cref="ArgumentOutOfRangeException"/> if the bet isn't positive.
    /// </summary>
    void PlaceBet(int amount);

    /// <summary>
    /// Draws one card into <see cref="PlayerHand"/>. If it busts, the round
    /// ends immediately as a dealer win.
    /// </summary>
    void Hit();

    /// <summary>
    /// Ends the player's turn: reveals the dealer's hole card, plays out the
    /// dealer's hand, and resolves the round.
    /// </summary>
    void Stand();

    /// <summary>
    /// Doubles <see cref="CurrentBet"/>, draws exactly one more card into
    /// <see cref="PlayerHand"/>, and then stands. Only valid while
    /// <see cref="PlayerHand"/> holds exactly two cards; otherwise throws
    /// <see cref="InvalidOperationException"/>.
    /// </summary>
    void DoubleDown();
    int GetCardsRemaining();
}
