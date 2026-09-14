namespace Blackjack.Game;

/// <summary>
/// The dealer's fixed drawing strategy: keep hitting from the deck until the
/// hand's value is at least 17 (or busts).
/// </summary>
public interface IDealerPlay
{
    void Play(IHand dealerHand, IDeck deck);
}
