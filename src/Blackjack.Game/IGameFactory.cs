namespace Blackjack.Game;

/// <summary>
/// Creates the game engine's building blocks. This is the only seam through
/// which concrete implementations of <see cref="IDeck"/>, <see cref="IHand"/>,
/// <see cref="IWallet"/>, <see cref="IRoundResolver"/>
/// and <see cref="IBlackjackGame"/> come into existence.
/// </summary>
public interface IGameFactory
{
    /// <summary>A standard, unshuffled 52-card deck (every Rank x Suit combination once).</summary>
    IDeck CreateDeck();

    /// <summary>
    /// A deck containing exactly the given cards, in the given order.
    /// <see cref="IDeck.DrawCard"/> returns them front-to-back. Intended for
    /// tests that need a predictable sequence of cards.
    /// </summary>
    IDeck CreateDeck(IEnumerable<Card> cards);

    IHand CreateHand();

    IWallet CreateWallet(int startingBalance);

    IRoundResolver CreateRoundResolver();

    /// <summary>A new session that deals from its own internal deck, freshly shuffled before every round.</summary>
    IBlackjackGame CreateGame(int startingBalance);

    /// <summary>
    /// A new session that deals from the given deck as-is (no implicit
    /// shuffle), so callers can control the exact cards dealt and drawn.
    /// </summary>
    IBlackjackGame CreateGame(int startingBalance, IDeck deck);
}
