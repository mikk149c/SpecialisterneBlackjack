namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the standard 52-card deck, drawing, and that shuffling reorders
/// without losing or duplicating cards.
/// </summary>
public class DeckTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void CreateDeck_Default_ContainsFiftyTwoCards()
    {
        var deck = _factory.CreateDeck();

        Assert.Equal(52, deck.CardsRemaining);
    }

    [Fact]
    public void DrawCard_ReducesCardsRemainingByOne()
    {
        var deck = _factory.CreateDeck();

        deck.DrawCard();

        Assert.Equal(51, deck.CardsRemaining);
    }

    [Fact]
    public void DrawCard_DeckExhausted_ThrowsInvalidOperationException()
    {
        var deck = _factory.CreateDeck();
        for (var i = 0; i < 52; i++)
        {
            deck.DrawCard();
        }

        Assert.Throws<InvalidOperationException>(() => deck.DrawCard());
    }

    [Fact]
    public void CreateDeck_WithCards_DrawsThemInGivenOrder()
    {
        var first = new Card(Rank.Two, Suit.Clubs);
        var second = new Card(Rank.King, Suit.Hearts);
        var deck = _factory.CreateDeck(new[] { first, second });

        Assert.Equal(first, deck.DrawCard());
        Assert.Equal(second, deck.DrawCard());
    }

    [Fact]
    public void CreateDeck_WithCards_CardsRemainingMatchesCount()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Clubs),
            new Card(Rank.Three, Suit.Clubs),
            new Card(Rank.Four, Suit.Clubs)
        });

        Assert.Equal(3, deck.CardsRemaining);
    }

    [Fact]
    public void Shuffle_ReordersTheCards()
    {
        var originalOrder = new List<int>();
        var reference = _factory.CreateDeck(true);
        while (reference.CardsRemaining > 0)
        {
            originalOrder.Add(reference.DrawCard().Index);
        }

        var deck = _factory.CreateDeck();
        deck.Shuffle();

        var shuffledOrder = new List<int>();
        while (deck.CardsRemaining > 0)
        {
            shuffledOrder.Add(deck.DrawCard().Index);
        }

        // A true shuffle landing back in the original order is a 1-in-52! chance.
        Assert.NotEqual(originalOrder, shuffledOrder);
    }

    [Fact]
    public void Shuffle_DoesNotChangeCardsRemaining()
    {
        var deck = _factory.CreateDeck();

        deck.Shuffle();

        Assert.Equal(52, deck.CardsRemaining);
    }

    [Fact]
    public void Shuffle_PreservesTheSameSetOfCards()
    {
        var cards = Enum.GetValues<Suit>()
            .SelectMany(suit => Enum.GetValues<Rank>().Select(rank => new Card(rank, suit)))
            .ToList();

        foreach (Card card in cards)
            card.IsFaceUp = true;
        var shuffled = _factory.CreateDeck(cards);
        shuffled.Shuffle();

        var drawnAfterShuffle = new List<Card>();
        while (shuffled.CardsRemaining > 0)
        {
            drawnAfterShuffle.Add(shuffled.DrawCard());
        }

        Assert.Equal(cards.OrderBy(c => c.Suit).ThenBy(c => c.Rank),
            drawnAfterShuffle.OrderBy(c => c.Suit).ThenBy(c => c.Rank));
    }
}
