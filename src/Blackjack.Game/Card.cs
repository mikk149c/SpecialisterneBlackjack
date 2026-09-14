namespace Blackjack.Game;

internal class Card
{
    public int Index {get; private set;}
    public bool IsFaceUp {get; set;}
    public Card(int index)
    {
        Index = index;
        IsFaceUp = false;
    }

    public Suit Suit => (Suit)(Index / 13);
    public int Value => Index % 13;
    public Rank Rank => (Rank)(Value + 2);
}
