namespace Blackjack.Game;

public class Card
{
    public int Index {get; private set;}
    public bool IsFaceUp {get; set;}
    public Card(int index)
    {
        Index = index;
        IsFaceUp = false;
    }

    public Card(Rank rank, Suit suit)
    {
        setIndex(rank, suit);
    }

    private void setIndex(Rank rank, Suit suit)
    {
        Index = ((int)rank - 2) + ((int)suit * 13);
    }


    public Suit Suit => (Suit)(Index / 13);
    public int Value => (Index % 13) + 2;
    public Rank Rank => (Rank)(Value);
}
