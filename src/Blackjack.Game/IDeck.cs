namespace Blackjack.Game;

/// <summary>
/// A supply of cards that can be shuffled and drawn from one at a time.
/// </summary>
public interface IDeck
{
    /// <summary>Number of cards left to draw.</summary>
    int CardsRemaining { get; }

    /// <summary>Randomizes the order of the remaining cards in place.</summary>
    void Shuffle();

    /// <summary>
    /// Removes and returns the top card. Throws <see cref="InvalidOperationException"/>
    /// when the deck is empty.
    /// </summary>
    Card DrawCard();
}
