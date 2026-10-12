namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the engine members used for deck management and the remaining
/// wallet, card and hand edge cases not exercised by the scenario tests.
/// </summary>
public class CoverageGapTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void Deck_RefreshDeck_RestoresFullDeckAfterDrawing()
    {
        var deck = _factory.CreateDeck();
        deck.DrawCard();
        deck.DrawCard();
        deck.DrawCard();

        deck.RefreshDeck();

        Assert.Equal(52, deck.CardsRemaining);
        Assert.True(deck.HasCards());
    }

    [Fact]
    public void Deck_RefreshDeck_ReplacesAStackedDeckWithAFreshFaceDownDeck()
    {
        var deck = _factory.CreateDeck(new[] { new Card(Rank.Two, Suit.Clubs) });

        deck.RefreshDeck();

        Assert.Equal(52, deck.CardsRemaining);
        Assert.False(deck.DrawCard().IsFaceUp);
    }

    [Fact]
    public void Game_NewDeck_RefillsCardsRemaining()
    {
        var game = _factory.CreateGame(1000);
        game.PlaceBet(100);
        Assert.Equal(48, game.GetCardsRemaining());

        game.NewDeck();

        Assert.Equal(52, game.GetCardsRemaining());
    }

    [Fact]
    public void Game_ShuffleDeck_KeepsCardsRemaining()
    {
        var game = _factory.CreateGame(1000);

        game.ShuffleDeck();

        Assert.Equal(52, game.GetCardsRemaining());
    }

    [Fact]
    public void Game_GetCardsRemaining_DecreasesByFourWhenInitialCardsAreDealt()
    {
        var game = _factory.CreateGame(1000);

        game.PlaceBet(100);

        Assert.Equal(48, game.GetCardsRemaining());
    }

    [Fact]
    public void Game_PlaceBet_WhileRoundInProgress_ThrowsAndKeepsBalance()
    {
        var game = _factory.CreateGame(1000);
        game.PlaceBet(100);

        var ex = Assert.Throws<InvalidOperationException>(() => game.PlaceBet(100));

        Assert.StartsWith("A round is already in progress", ex.Message);
        Assert.Equal(900, game.Balance);
    }

    [Fact]
    public void Game_PlayRound_IsANoOpAndLeavesStateUntouched()
    {
        var game = new Game(
            _factory.CreateDeck(),
            _factory.CreateWallet(1000),
            _factory.CreateRoundResolver(),
            _factory.CreateHand(),
            _factory.CreateHand());

        game.PlayRound();

        Assert.Equal(1000, game.Balance);
        Assert.False(game.IsRoundInProgress);
        Assert.Equal(52, game.GetCardsRemaining());
    }

    [Fact]
    public void Wallet_Deposit_IncreasesBalance()
    {
        var wallet = new Wallet(1000);

        wallet.Deposit(250);

        Assert.Equal(1250, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Wallet_Deposit_NonPositiveAmount_ThrowsAndLeavesBalanceUnchanged(int amount)
    {
        var wallet = new Wallet(1000);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Deposit(amount));

        Assert.Contains("Deposit amount must be positive.", ex.Message);
        Assert.Equal(1000, wallet.Balance);
    }

    [Fact]
    public void Wallet_Settle_UnknownOutcome_ThrowsAndLeavesStateUnchanged()
    {
        var wallet = new Wallet(1000);
        wallet.PlaceBet(200);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Settle((RoundOutcome)999));

        Assert.Contains("Invalid round outcome.", ex.Message);
        Assert.Equal(800, wallet.Balance);
        Assert.Equal(200, wallet.Bet);
    }

    [Fact]
    public void Wallet_Settle_ClearsBetAndAllowsNextBet()
    {
        var wallet = new Wallet(1000);
        wallet.PlaceBet(200);

        wallet.Settle(RoundOutcome.DealerWin);

        Assert.False(wallet.BetPlaced);
        Assert.Equal(0, wallet.Bet);
    }

    [Fact]
    public void Card_Equals_Null_ReturnsFalse()
    {
        var card = new Card(Rank.Ace, Suit.Spades);

        Assert.False(card.Equals(null));
    }

    [Fact]
    public void Card_Equals_SameRankAndSuit_ReturnsTrue()
    {
        var a = new Card(Rank.Ten, Suit.Hearts);
        var b = new Card(Rank.Ten, Suit.Hearts);

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(new Card(Rank.Ten, Suit.Clubs)));
    }

    [Fact]
    public void PlayerHand_RevealHoleCard_ThrowsInvalidOperationException()
    {
        var hand = _factory.CreateHand();

        var ex = Assert.Throws<InvalidOperationException>(() => hand.RevealHoleCard());

        Assert.Equal("RevealHoleCard should only be called on a DealerHand.", ex.Message);
    }
}
