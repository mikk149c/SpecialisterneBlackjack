namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the dealer's fixed strategy: draw until the hand is worth at least
/// 17. This logic lives inline in <c>Game.endRound</c>, so these scenarios
/// are exercised through <see cref="IBlackjackGame.Stand"/>, which triggers
/// <c>endRound</c>. Player cards are an arbitrary low, non-blackjack hand in
/// every case since only the dealer's draw behavior is under test.
/// </summary>
public class DealerDrawTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void Stand_DealerHandBelowSeventeen_DrawsUntilAtLeastSeventeen()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Diamonds), // player 1
            new Card(Rank.Six, Suit.Hearts), // dealer up card
            new Card(Rank.Three, Suit.Diamonds), // player 2 -> player has 5
            new Card(Rank.Five, Suit.Spades), // dealer hole card -> dealer has 11
            new Card(Rank.Two, Suit.Clubs), // dealer draw -> 13
            new Card(Rank.Four, Suit.Diamonds), // dealer draw -> 17, should stop here
            new Card(Rank.King, Suit.Clubs) // must not be drawn
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Stand();

        Assert.Equal(17, game.DealerHand.Value);
        Assert.Equal(4, game.DealerHand.Cards.Count);
        Assert.Equal(1, deck.CardsRemaining);
    }

    [Fact]
    public void Stand_DealerHandAlreadySeventeenOrMore_DrawsNothing()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Diamonds), // player 1
            new Card(Rank.Ten, Suit.Hearts), // dealer up card
            new Card(Rank.Three, Suit.Diamonds), // player 2 -> player has 5
            new Card(Rank.Eight, Suit.Spades), // dealer hole card -> dealer has 18
            new Card(Rank.Two, Suit.Clubs) // must not be drawn
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Stand();

        Assert.Equal(2, game.DealerHand.Cards.Count);
        Assert.Equal(1, deck.CardsRemaining);
    }

    [Fact]
    public void Stand_DealerHandAtExactlySeventeen_DrawsNothing()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Diamonds), // player 1
            new Card(Rank.Ace, Suit.Hearts), // dealer up card
            new Card(Rank.Three, Suit.Diamonds), // player 2 -> player has 5
            new Card(Rank.Six, Suit.Spades), // dealer hole card -> soft 17
            new Card(Rank.Two, Suit.Clubs) // must not be drawn
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Stand();

        Assert.Equal(2, game.DealerHand.Cards.Count);
        Assert.Equal(17, game.DealerHand.Value);
        Assert.Equal(1, deck.CardsRemaining);
    }

    [Fact]
    public void Stand_DealerDrawingPastTwentyOne_StopsAndLeavesHandBust()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Diamonds), // player 1
            new Card(Rank.Ten, Suit.Hearts), // dealer up card
            new Card(Rank.Three, Suit.Diamonds), // player 2 -> player has 5
            new Card(Rank.Six, Suit.Spades), // dealer hole card -> dealer has 16
            new Card(Rank.King, Suit.Clubs), // dealer draw -> 26, bust
            new Card(Rank.Two, Suit.Hearts) // must not be drawn
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Stand();

        Assert.True(game.DealerHand.IsBust);
        Assert.Equal(3, game.DealerHand.Cards.Count);
        Assert.Equal(1, deck.CardsRemaining);
    }
}
