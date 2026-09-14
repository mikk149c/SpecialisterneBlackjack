namespace Blackjack.Game;

internal class PlayerHand : IHand
{
    public IReadOnlyList<Card> Cards => throw new NotImplementedException();

    public int Value => throw new NotImplementedException();

    public bool IsBust => throw new NotImplementedException();

    public bool IsBlackjack => throw new NotImplementedException();

    public void AddCard(Card card)
    {
        throw new NotImplementedException();
    }
}