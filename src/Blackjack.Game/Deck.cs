namespace Blackjack.Game;

internal class Deck : IDeck
{
    private List<Card> _cards = new List<Card>();
    public int CardsRemaining => _cards.Count;
    public Deck(bool reveal = false)
    {
        _cards = generateDeck(reveal);
    }

    private List<Card> generateDeck(bool reveal = false)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < 52; i++)
        {
            Card card = new Card(i);
            card.IsFaceUp = reveal;
            cards.Add(card);
        }
        return cards;
    }

    public void RefreshDeck()
    {
        _cards = generateDeck();
    }

    public Deck(IEnumerable<Card> cards)
    {
        _cards = new List<Card>(cards);
    }

    public Card DrawCard()
    {
        if (CardsRemaining == 0)
        {
            throw new InvalidOperationException("Cannot draw a card from an empty deck.");
        }
        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void Shuffle()
    {
        Random random = new Random();
        List<Card> shuffled = new List<Card>();
        while (_cards.Count > 0)
        {
            int workingIndex = random.Next(_cards.Count);
            Card card = _cards[workingIndex];
            _cards.RemoveAt(workingIndex);
            shuffled.Add(card);
        }
        _cards = shuffled;
    }

    public bool HasCards()
    {
        return CardsRemaining > 0;
    }
}