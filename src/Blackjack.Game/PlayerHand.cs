namespace Blackjack.Game;

internal class PlayerHand : Hand
{
    public override void AddCard(Card card)
    {
        card.IsFaceUp = true;
        base.AddCard(card);
    }
}
