namespace Blackjack.Game;

internal class Deck : IDeck
{
    private List<Card> _cards = new List<Card>();
    public int CardsRemaining => throw new NotImplementedException();
    public Deck()
    {
        for (int i = 0; i < 51; i++)
        {
            _cards.Add(new Card(i));
        }
    }

    public Card DrawCard()
    {
        throw new NotImplementedException();
    }

    public void Shuffle()
    {
        throw new NotImplementedException();
    }
}