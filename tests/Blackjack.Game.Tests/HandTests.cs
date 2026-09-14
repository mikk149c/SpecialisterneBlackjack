namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the scoring rules from the requirements: face cards are worth 10,
/// Aces are 1 or 11 (whichever keeps the hand valid), and a two-card 21 is
/// Blackjack while a longer 21 is not.
/// </summary>
public class HandTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void Value_TwoNumberCards_SumsFaceValues()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Seven, Suit.Clubs));
        hand.AddCard(new Card(Rank.Eight, Suit.Diamonds));

        Assert.Equal(15, hand.Value);
    }

    [Theory]
    [InlineData(Rank.Jack)]
    [InlineData(Rank.Queen)]
    [InlineData(Rank.King)]
    public void Value_FaceCardPlusNumberCard_FaceCardCountsAsTen(Rank faceRank)
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(faceRank, Suit.Hearts));
        hand.AddCard(new Card(Rank.Seven, Suit.Spades));

        Assert.Equal(17, hand.Value);
    }

    [Fact]
    public void Value_AceWithFaceCard_AceCountsAsEleven()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ace, Suit.Clubs));
        hand.AddCard(new Card(Rank.King, Suit.Diamonds));

        Assert.Equal(21, hand.Value);
    }

    [Fact]
    public void Value_TwoAces_OnlyOneCountsAsEleven()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ace, Suit.Clubs));
        hand.AddCard(new Card(Rank.Ace, Suit.Diamonds));

        Assert.Equal(12, hand.Value);
    }

    [Fact]
    public void Value_AceWouldBustAsEleven_CountsAsOneInstead()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ace, Suit.Clubs));
        hand.AddCard(new Card(Rank.Nine, Suit.Diamonds));
        hand.AddCard(new Card(Rank.Five, Suit.Spades));

        Assert.Equal(15, hand.Value);
        Assert.False(hand.IsBust);
    }

    [Fact]
    public void IsBust_ValueOverTwentyOne_IsTrue()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ten, Suit.Clubs));
        hand.AddCard(new Card(Rank.Nine, Suit.Diamonds));
        hand.AddCard(new Card(Rank.Five, Suit.Spades));

        Assert.True(hand.IsBust);
        Assert.Equal(24, hand.Value);
    }

    [Fact]
    public void IsBust_ValueAtTwentyOne_IsFalse()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ten, Suit.Clubs));
        hand.AddCard(new Card(Rank.Ace, Suit.Diamonds));

        Assert.False(hand.IsBust);
    }

    [Fact]
    public void IsBlackjack_TwoCardsTotallingTwentyOne_IsTrue()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ace, Suit.Clubs));
        hand.AddCard(new Card(Rank.Queen, Suit.Diamonds));

        Assert.True(hand.IsBlackjack);
    }

    [Fact]
    public void IsBlackjack_ThreeCardsTotallingTwentyOne_IsFalse()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Seven, Suit.Clubs));
        hand.AddCard(new Card(Rank.Seven, Suit.Diamonds));
        hand.AddCard(new Card(Rank.Seven, Suit.Spades));

        Assert.Equal(21, hand.Value);
        Assert.False(hand.IsBlackjack);
    }

    [Fact]
    public void IsBlackjack_TwoCardsNotTotallingTwentyOne_IsFalse()
    {
        var hand = _factory.CreateHand();
        hand.AddCard(new Card(Rank.Ten, Suit.Clubs));
        hand.AddCard(new Card(Rank.Nine, Suit.Diamonds));

        Assert.False(hand.IsBlackjack);
    }

    [Fact]
    public void AddCard_AppendsToCardsInOrder()
    {
        var hand = _factory.CreateHand();
        var first = new Card(Rank.Two, Suit.Clubs);
        var second = new Card(Rank.Three, Suit.Diamonds);

        hand.AddCard(first);
        hand.AddCard(second);

        Assert.Equal(new[] { first, second }, hand.Cards);
    }
}
