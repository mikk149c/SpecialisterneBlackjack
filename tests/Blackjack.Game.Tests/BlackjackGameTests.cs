namespace Blackjack.Game.Tests;

/// <summary>
/// End-to-end coverage of the game engine against the requirements: dealing,
/// the hidden dealer hole card, Hit/Stand/Double Down, immediate loss on
/// bust, betting limits, payouts, and starting another round.
///
/// Deterministic scenarios use <c>IGameFactory.CreateGame(int, IDeck)</c>
/// with a stacked, unshuffled deck (from <c>CreateDeck(IEnumerable&lt;Card&gt;)</c>).
/// Cards are dealt in the standard order: player, dealer (up card), player,
/// dealer (hole card).
/// </summary>
public class BlackjackGameTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void CreateGame_StartsWithGivenBalance()
    {
        var game = _factory.CreateGame(1000);

        Assert.Equal(1000, game.Balance);
    }

    [Fact]
    public void NoRoundStarted_IsRoundInProgressIsFalse()
    {
        var game = _factory.CreateGame(1000);

        Assert.False(game.IsRoundInProgress);
    }

    [Fact]
    public void Hit_BeforePlacingBet_ThrowsInvalidOperationException()
    {
        var game = _factory.CreateGame(1000);

        Assert.Throws<InvalidOperationException>(() => game.Hit());
    }

    [Fact]
    public void Stand_BeforePlacingBet_ThrowsInvalidOperationException()
    {
        var game = _factory.CreateGame(1000);

        Assert.Throws<InvalidOperationException>(() => game.Stand());
    }

    [Fact]
    public void DoubleDown_BeforePlacingBet_ThrowsInvalidOperationException()
    {
        var game = _factory.CreateGame(1000);

        Assert.Throws<InvalidOperationException>(() => game.DoubleDown());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void PlaceBet_NonPositiveAmount_ThrowsArgumentOutOfRangeException(int amount)
    {
        var game = _factory.CreateGame(1000);

        Assert.Throws<ArgumentOutOfRangeException>(() => game.PlaceBet(amount));
    }

    [Fact]
    public void PlaceBet_MoreThanBalance_ThrowsInvalidOperationException()
    {
        var game = _factory.CreateGame(1000);

        Assert.Throws<InvalidOperationException>(() => game.PlaceBet(1001));
    }

    [Fact]
    public void PlaceBet_DealsTwoCardsEachAndHidesDealerHoleCard()
    {
        var game = _factory.CreateGame(1000);

        game.PlaceBet(100);

        Assert.True(game.IsRoundInProgress);
        Assert.Equal(100, game.CurrentBet);
        Assert.Equal(900, game.Balance);
        Assert.Equal(2, game.PlayerHand.Cards.Count);
        Assert.Equal(2, game.DealerHand.Cards.Count);
        Assert.False(game.IsDealerHoleCardRevealed);
    }

    [Fact]
    public void PlaceBet_WhileRoundInProgress_ThrowsInvalidOperationException()
    {
        var game = _factory.CreateGame(1000);
        game.PlaceBet(100);

        Assert.Throws<InvalidOperationException>(() => game.PlaceBet(50));
    }

    [Fact]
    public void Hit_BustingCard_EndsRoundImmediatelyAsDealerWin()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Ten, Suit.Clubs), // player 1
            new Card(Rank.Six, Suit.Hearts), // dealer up card
            new Card(Rank.Eight, Suit.Diamonds), // player 2 -> player has 18
            new Card(Rank.Five, Suit.Spades), // dealer hole card -> dealer has 11
            new Card(Rank.Ten, Suit.Spades) // player hit -> 28, bust
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Hit();

        Assert.True(game.PlayerHand.IsBust);
        Assert.False(game.IsRoundInProgress);
        Assert.Equal(RoundOutcome.DealerWin, game.LastOutcome);
        Assert.Equal(900, game.Balance); // bet already lost, nothing more deducted
    }

    [Fact]
    public void Hit_NonBustingCard_KeepsRoundInProgress()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Clubs), // player 1
            new Card(Rank.Ten, Suit.Hearts), // dealer up card
            new Card(Rank.Three, Suit.Diamonds), // player 2 -> player has 5
            new Card(Rank.Nine, Suit.Spades), // dealer hole card -> dealer has 19
            new Card(Rank.Four, Suit.Spades) // player hit -> 9
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Hit();

        Assert.True(game.IsRoundInProgress);
        Assert.Equal(9, game.PlayerHand.Value);
        Assert.Equal(3, game.PlayerHand.Cards.Count);
    }

    [Fact]
    public void Stand_RevealsHoleCardPlaysDealerAndResolvesRound()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Ten, Suit.Clubs), // player 1
            new Card(Rank.Six, Suit.Hearts), // dealer up card
            new Card(Rank.Nine, Suit.Diamonds), // player 2 -> player has 19
            new Card(Rank.Five, Suit.Spades), // dealer hole card -> dealer has 11
            new Card(Rank.Two, Suit.Clubs), // dealer draw -> 13
            new Card(Rank.Four, Suit.Diamonds) // dealer draw -> 17, stop
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Stand();

        Assert.True(game.IsDealerHoleCardRevealed);
        Assert.False(game.IsRoundInProgress);
        Assert.Equal(17, game.DealerHand.Value);
        Assert.Equal(RoundOutcome.PlayerWin, game.LastOutcome);
        Assert.Equal(1100, game.Balance); // 900 after bet + 200 payout (1:1)
    }

    [Fact]
    public void Stand_BothNaturalBlackjack_IsPushAndReturnsStake()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Ace, Suit.Clubs), // player 1
            new Card(Rank.Ace, Suit.Hearts), // dealer up card
            new Card(Rank.King, Suit.Diamonds), // player 2 -> Blackjack
            new Card(Rank.King, Suit.Spades) // dealer hole card -> Blackjack
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.Stand();

        Assert.Equal(RoundOutcome.Push, game.LastOutcome);
        Assert.Equal(1000, game.Balance);
    }

    [Fact]
    public void Stand_PlayerBlackjackDealerNormalHand_PaysThreeToTwo()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Ace, Suit.Clubs), // player 1
            new Card(Rank.Ten, Suit.Hearts), // dealer up card
            new Card(Rank.Queen, Suit.Diamonds), // player 2 -> Blackjack
            new Card(Rank.Eight, Suit.Spades) // dealer hole card -> dealer has 18
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(200);

        game.Stand();

        Assert.Equal(RoundOutcome.Blackjack, game.LastOutcome);
        Assert.Equal(1300, game.Balance); // 800 after bet + 500 payout (3:2 on 200)
    }

    [Fact]
    public void DoubleDown_WithTwoCards_DoublesBetDrawsOneCardAndStands()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Five, Suit.Clubs), // player 1
            new Card(Rank.Ten, Suit.Hearts), // dealer up card
            new Card(Rank.Six, Suit.Diamonds), // player 2 -> player has 11
            new Card(Rank.Nine, Suit.Spades), // dealer hole card -> dealer has 19
            new Card(Rank.Six, Suit.Clubs) // double-down draw -> player has 17
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);

        game.DoubleDown();

        Assert.Equal(3, game.PlayerHand.Cards.Count);
        Assert.Equal(17, game.PlayerHand.Value);
        Assert.False(game.IsRoundInProgress);
        Assert.True(game.IsDealerHoleCardRevealed);
        Assert.Equal(RoundOutcome.DealerWin, game.LastOutcome);
        Assert.Equal(800, game.Balance); // 900 - extra 100 doubled, lost the round
    }

    [Fact]
    public void DoubleDown_AfterAlreadyHitting_ThrowsInvalidOperationException()
    {
        var deck = _factory.CreateDeck(new[]
        {
            new Card(Rank.Two, Suit.Clubs), // player 1
            new Card(Rank.Ten, Suit.Hearts), // dealer up card
            new Card(Rank.Three, Suit.Diamonds), // player 2 -> player has 5
            new Card(Rank.Nine, Suit.Spades), // dealer hole card -> dealer has 19
            new Card(Rank.Four, Suit.Spades) // player hit -> 9, now 3 cards
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);
        game.Hit();

        Assert.Throws<InvalidOperationException>(() => game.DoubleDown());
    }

    [Fact]
    public void NewRound_AfterPreviousRoundResolved_DealsFreshHands()
    {
        var deck = _factory.CreateDeck(new[]
        {
            // Round 1
            new Card(Rank.Ten, Suit.Clubs),
            new Card(Rank.Ten, Suit.Hearts),
            new Card(Rank.Nine, Suit.Diamonds), // player 19
            new Card(Rank.Nine, Suit.Spades), // dealer 19, no draw needed
            // Round 2
            new Card(Rank.Two, Suit.Clubs),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Four, Suit.Diamonds),
            new Card(Rank.Five, Suit.Spades)
        });
        var game = _factory.CreateGame(1000, deck);
        game.PlaceBet(100);
        game.Stand();

        game.PlaceBet(50);

        Assert.True(game.IsRoundInProgress);
        Assert.Equal(50, game.CurrentBet);
        Assert.Equal(2, game.PlayerHand.Cards.Count);
        Assert.Equal(2, game.DealerHand.Cards.Count);
        Assert.False(game.IsDealerHoleCardRevealed);
    }
}
