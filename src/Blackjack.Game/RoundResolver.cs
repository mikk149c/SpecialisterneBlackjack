namespace Blackjack.Game;

internal class RoundResolver : IRoundResolver
{
    public RoundOutcome Resolve(IHand playerHand, IHand dealerHand)
    {
        if (playerHand.IsBust)
            return RoundOutcome.DealerWin;

        if (dealerHand.IsBust)
            return RoundOutcome.PlayerWin;

        if (playerHand.IsBlackjack && !dealerHand.IsBlackjack)
            return RoundOutcome.Blackjack;

        if (!playerHand.IsBlackjack && dealerHand.IsBlackjack)
            return RoundOutcome.DealerWin;

        if (playerHand.Value > dealerHand.Value)
            return RoundOutcome.PlayerWin;

        if (playerHand.Value < dealerHand.Value)
            return RoundOutcome.DealerWin;

        return RoundOutcome.Push;
    }
}