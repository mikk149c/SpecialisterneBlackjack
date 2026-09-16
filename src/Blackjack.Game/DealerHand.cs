namespace Blackjack.Game;

internal class DealerHand : Hand
{
    public override void AddCard(Card card)
    {
        if (Cards.Count == 0)
        {
            card.IsFaceUp = false; // hole card
        }
        else
        {
            card.IsFaceUp = true;
        }

        base.AddCard(card);
    }
    public override void RevealHoleCard()
    {
        foreach (Card card in Cards)
            card.IsFaceUp = true;
    }
}
