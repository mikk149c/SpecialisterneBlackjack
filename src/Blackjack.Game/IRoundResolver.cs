namespace Blackjack.Game;

/// <summary>
/// Pure rules for turning two finished hands into a <see cref="RoundOutcome"/>:
/// a bust loses immediately, blackjack beats a normal 21, and equal winning
/// values push.
/// </summary>
public interface IRoundResolver
{
    RoundOutcome Resolve(IHand playerHand, IHand dealerHand);
}
