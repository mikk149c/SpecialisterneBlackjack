namespace Blackjack.Game;

public class Card : IEquatable<Card>
{
    private int _index;
    public int Index {get => IsFaceUp ? _index : -1; private set => _index = value; }
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
        _index = ((int)rank - 2) + ((int)suit * 13);
    }

    public bool Equals(Card? other)
    {
        if (other is null)
            return false;

        return _index == other._index;
    }

    public Suit Suit => IsFaceUp ? (Suit)(Index / 13) : Suit.Blank;
    public int Value => IsFaceUp ? (Index % 13) + 2 : -1;
    public Rank Rank => IsFaceUp ? (Rank)(Value) : Rank.Blank;
}
