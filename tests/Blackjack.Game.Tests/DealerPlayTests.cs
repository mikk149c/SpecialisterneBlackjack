namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the dealer's fixed strategy: draw until the hand is worth at least 17.
/// </summary>
public class DealerPlayTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void Play_HandBelowSeventeen_DrawsUntilAtLeastSeventeen()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Six, Suit.Hearts));
        hand.AddCard(new Card(Rank.Five, Suit.Spades)); // 11
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Clubs), // 13
            new Card(Rank.Four, Suit.Diamonds), // 17, should stop here
            new Card(Rank.King, Suit.Clubs) // must not be drawn
        });

        _factory.CreateDealerPlay().Play(hand, deck);

        Assert.Equal(17, hand.Value);
        Assert.Equal(4, hand.Cards.Count);
        Assert.Equal(1, deck.CardsRemaining);
    }

    [Fact]
    public void Play_HandAlreadySeventeenOrMore_DrawsNothing()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ten, Suit.Hearts));
        hand.AddCard(new Card(Rank.Eight, Suit.Spades)); // 18
        var deck = _factory.CreateDeck(new[] { new Card(Rank.Two, Suit.Clubs) });

        _factory.CreateDealerPlay().Play(hand, deck);

        Assert.Equal(2, hand.Cards.Count);
        Assert.Equal(1, deck.CardsRemaining);
    }

    [Fact]
    public void Play_HandAtExactlySeventeen_DrawsNothing()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ace, Suit.Hearts));
        hand.AddCard(new Card(Rank.Six, Suit.Spades)); // soft 17
        var deck = _factory.CreateDeck(new[] { new Card(Rank.Two, Suit.Clubs) });

        _factory.CreateDealerPlay().Play(hand, deck);

        Assert.Equal(2, hand.Cards.Count);
        Assert.Equal(17, hand.Value);
    }

    [Fact]
    public void Play_DrawingPastTwentyOne_StopsAndLeavesHandBust()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ten, Suit.Hearts));
        hand.AddCard(new Card(Rank.Six, Suit.Spades)); // 16
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.King, Suit.Clubs), // 26, bust
            new Card(Rank.Two, Suit.Diamonds) // must not be drawn
        });

        _factory.CreateDealerPlay().Play(hand, deck);

        Assert.True(hand.IsBust);
        Assert.Equal(3, hand.Cards.Count);
        Assert.Equal(1, deck.CardsRemaining);
    }
}
