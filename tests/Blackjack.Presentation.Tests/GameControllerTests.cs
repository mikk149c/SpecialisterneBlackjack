using Blackjack.Game;

namespace Blackjack.Presentation.Tests;

/// <summary>
/// Drives <see cref="GameController"/> against real engine games built on
/// stacked decks. Cards are dealt player, dealer (hole card), player, dealer
/// (up card), then drawn in order.
/// </summary>
public class GameControllerTests
{
    private const int StartingBalance = 1000;
    private const int RefillThreshold = 15;

    private readonly IGameFactory _factory = new GameFactory();

    // Padding keeps the deck above the refill threshold so stacked cards are
    // not replaced by a fresh deck before the scenario has played out.
    private GameController ControllerWith(int balance, params Card[] scenarioCards)
    {
        var filler = Enumerable.Range(0, RefillThreshold + 5).Select(_ => Card(Rank.Two, Suit.Clubs));
        var deck = _factory.CreateDeck(scenarioCards.Concat(filler));
        return new GameController(_factory.CreateGame(balance, deck));
    }

    private GameController ControllerWithDeckSize(int deckSize)
    {
        var cards = Enumerable.Range(0, deckSize).Select(_ => Card(Rank.Two, Suit.Clubs));
        var deck = _factory.CreateDeck(cards);
        return new GameController(_factory.CreateGame(StartingBalance, deck));
    }

    private static Card Card(Rank rank, Suit suit) => new(rank, suit);

    // Player 19 vs dealer 18 (face-up King, hidden Eight).
    private GameController PlayerWinningScenario() => ControllerWith(StartingBalance,
        Card(Rank.Ten, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
        Card(Rank.Nine, Suit.Hearts), Card(Rank.King, Suit.Spades));

    [Fact]
    public void NewController_HasInitialState()
    {
        var controller = PlayerWinningScenario();

        Assert.Equal("Place your bet to start the first round.", controller.Status);
        Assert.Equal(StartingBalance, controller.Balance);
        Assert.Equal(0, controller.CurrentBet);
        Assert.Equal("-", controller.LastRoundLabel);
        Assert.False(controller.IsRoundInProgress);
        Assert.Empty(controller.PlayerCards);
        Assert.Empty(controller.DealerCards);
        Assert.Equal("-", controller.PlayerValueLabel);
        Assert.Equal("-", controller.DealerValueLabel);
        Assert.Equal(RefillThreshold + 5 + 4, controller.CardsRemaining);
        Assert.False(controller.JustRefilled);
        Assert.True(controller.CanPlaceBet);
        Assert.False(controller.CanHit);
        Assert.False(controller.CanStand);
        Assert.False(controller.CanDoubleDown);
    }

    [Fact]
    public void CreateDefault_StartsWithDefaultBalanceAndFullDeck()
    {
        var controller = GameController.CreateDefault();

        Assert.Equal(GameFactory.DefaultStartingBalance, controller.Balance);
        Assert.Equal(52, controller.CardsRemaining);
        Assert.Equal(StatusMessages.Initial, controller.Status);
    }

    [Fact]
    public void PlaceBet_ValidAmount_StartsRoundAndShowsTurnPrompt()
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet(100);

        Assert.Equal("Your turn: Hit, Stand, or Double Down.", controller.Status);
        Assert.True(controller.IsRoundInProgress);
        Assert.Equal(100, controller.CurrentBet);
        Assert.Equal(StartingBalance - 100, controller.Balance);
        Assert.Equal(2, controller.PlayerCards.Count);
        Assert.Equal(2, controller.DealerCards.Count);
        Assert.False(controller.CanPlaceBet);
        Assert.True(controller.CanHit);
        Assert.True(controller.CanStand);
        Assert.True(controller.CanDoubleDown);
    }

    [Fact]
    public void PlaceBet_NumericText_PlacesBet()
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet("250");

        Assert.Equal(250, controller.CurrentBet);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("12.5")]
    [InlineData(null)]
    public void PlaceBet_NonNumericText_ShowsInvalidAmountAndStartsNoRound(string? input)
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet(input);

        Assert.Equal("Invalid bet amount - enter a whole number.", controller.Status);
        Assert.False(controller.IsRoundInProgress);
        Assert.Equal(StartingBalance, controller.Balance);
    }

    [Fact]
    public void PlaceBet_MoreThanBalance_ShowsEngineMessage()
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet(StartingBalance + 1);

        Assert.Equal("Insufficient balance to place the bet.", controller.Status);
        Assert.False(controller.IsRoundInProgress);
        Assert.Equal(StartingBalance, controller.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void PlaceBet_NonPositiveAmount_ShowsEngineMessage(int amount)
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet(amount);

        Assert.StartsWith("Bet amount must be positive.", controller.Status);
        Assert.False(controller.IsRoundInProgress);
    }

    [Fact]
    public void PlaceBet_WhileRoundInProgress_IsIgnored()
    {
        var controller = PlayerWinningScenario();
        controller.PlaceBet(100);

        controller.PlaceBet(50);
        controller.PlaceBet("abc");

        Assert.Equal(100, controller.CurrentBet);
        Assert.Equal(StatusMessages.PlayerTurn, controller.Status);
    }

    [Fact]
    public void Hit_RoundContinues_ShowsTurnPrompt()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Two, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Three, Suit.Hearts), Card(Rank.King, Suit.Spades),
            Card(Rank.Four, Suit.Hearts));
        controller.PlaceBet(100);

        controller.Hit();

        Assert.Equal(StatusMessages.PlayerTurn, controller.Status);
        Assert.True(controller.IsRoundInProgress);
        Assert.Equal(3, controller.PlayerCards.Count);
        Assert.Equal("9", controller.PlayerValueLabel);
    }

    [Fact]
    public void Hit_Bust_ShowsDealerWins()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Ten, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Six, Suit.Hearts), Card(Rank.King, Suit.Spades),
            Card(Rank.King, Suit.Hearts));
        controller.PlaceBet(100);

        controller.Hit();

        Assert.Equal("Dealer wins. Press [B] to play again.", controller.Status);
        Assert.Equal("DealerWin", controller.LastRoundLabel);
        Assert.False(controller.IsRoundInProgress);
        Assert.Equal(StartingBalance - 100, controller.Balance);
    }

    [Fact]
    public void Stand_PlayerHigher_ShowsYouWin()
    {
        var controller = PlayerWinningScenario();
        controller.PlaceBet(100);

        controller.Stand();

        Assert.Equal("You win! Press [B] to play again.", controller.Status);
        Assert.Equal("PlayerWin", controller.LastRoundLabel);
        Assert.Equal(StartingBalance + 100, controller.Balance);
        Assert.Equal(0, controller.CurrentBet);
        Assert.True(controller.CanPlaceBet);
    }

    [Fact]
    public void Stand_DealerHigher_ShowsDealerWins()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Ten, Suit.Hearts), Card(Rank.Ten, Suit.Spades),
            Card(Rank.Seven, Suit.Hearts), Card(Rank.Nine, Suit.Spades));
        controller.PlaceBet(100);

        controller.Stand();

        Assert.Equal("Dealer wins. Press [B] to play again.", controller.Status);
    }

    [Fact]
    public void Stand_PlayerBlackjack_ShowsBlackjackMessage()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Ace, Suit.Hearts), Card(Rank.Ten, Suit.Spades),
            Card(Rank.King, Suit.Hearts), Card(Rank.Seven, Suit.Spades));
        controller.PlaceBet(100);

        controller.Stand();

        Assert.Equal("Blackjack! You win 3:2. Press [B] to play again.", controller.Status);
        Assert.Equal("Blackjack", controller.LastRoundLabel);
    }

    [Fact]
    public void Stand_EqualHands_ShowsPush()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Ten, Suit.Hearts), Card(Rank.Ten, Suit.Spades),
            Card(Rank.Eight, Suit.Hearts), Card(Rank.Eight, Suit.Spades));
        controller.PlaceBet(100);

        controller.Stand();

        Assert.Equal("Push - bet returned. Press [B] to play again.", controller.Status);
        Assert.Equal(StartingBalance, controller.Balance);
    }

    [Fact]
    public void DoubleDown_WithEnoughBalance_ResolvesRoundWithOutcomeMessage()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Five, Suit.Hearts), Card(Rank.Ten, Suit.Spades),
            Card(Rank.Six, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Ten, Suit.Hearts));
        controller.PlaceBet(100);

        controller.DoubleDown();

        Assert.Equal("You win! Press [B] to play again.", controller.Status);
        Assert.Equal(StartingBalance + 200, controller.Balance);
        Assert.Equal(3, controller.PlayerCards.Count);
    }

    [Fact]
    public void CanDoubleDown_ThreeCardHand_IsFalse()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Two, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Three, Suit.Hearts), Card(Rank.King, Suit.Spades),
            Card(Rank.Four, Suit.Hearts));
        controller.PlaceBet(100);

        controller.Hit();

        Assert.False(controller.CanDoubleDown);
        Assert.True(controller.CanHit);
    }

    [Fact]
    public void DoubleDown_ThreeCardHand_IsIgnored()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Two, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Three, Suit.Hearts), Card(Rank.King, Suit.Spades),
            Card(Rank.Four, Suit.Hearts));
        controller.PlaceBet(100);
        controller.Hit();

        controller.DoubleDown();

        Assert.True(controller.IsRoundInProgress);
        Assert.Equal(StatusMessages.PlayerTurn, controller.Status);
        Assert.Equal(3, controller.PlayerCards.Count);
    }

    [Fact]
    public void CanDoubleDown_BalanceBelowBet_IsFalse()
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet(600);

        Assert.Equal(400, controller.Balance);
        Assert.False(controller.CanDoubleDown);
    }

    [Fact]
    public void DoubleDown_BalanceBelowBet_ShowsEngineMessageAndKeepsRoundOpen()
    {
        var controller = PlayerWinningScenario();
        controller.PlaceBet(600);

        controller.DoubleDown();

        Assert.Equal("Insufficient balance to place the bet.", controller.Status);
        Assert.True(controller.IsRoundInProgress);
    }

    [Fact]
    public void Actions_WithoutRoundInProgress_AreIgnored()
    {
        var controller = PlayerWinningScenario();

        controller.Hit();
        controller.Stand();
        controller.DoubleDown();

        Assert.Equal(StatusMessages.Initial, controller.Status);
        Assert.False(controller.IsRoundInProgress);
    }

    [Fact]
    public void DealerValueLabel_HoleCardHidden_ShowsVisibleTotalPlusQuestionMark()
    {
        var controller = PlayerWinningScenario();

        controller.PlaceBet(100);

        Assert.Equal("10 + ?", controller.DealerValueLabel);
        Assert.Equal("19", controller.PlayerValueLabel);
    }

    [Fact]
    public void DealerValueLabel_AceUpCard_CountsElevenInVisibleTotal()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Ten, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Nine, Suit.Hearts), Card(Rank.Ace, Suit.Spades));

        controller.PlaceBet(100);

        Assert.Equal("11 + ?", controller.DealerValueLabel);
    }

    [Fact]
    public void DealerValueLabel_NumericUpCard_CountsPipValue()
    {
        var controller = ControllerWith(StartingBalance,
            Card(Rank.Ten, Suit.Hearts), Card(Rank.Eight, Suit.Spades),
            Card(Rank.Nine, Suit.Hearts), Card(Rank.Seven, Suit.Spades));

        controller.PlaceBet(100);

        Assert.Equal("7 + ?", controller.DealerValueLabel);
    }

    [Fact]
    public void DealerValueLabel_AfterReveal_ShowsFullHandValue()
    {
        var controller = PlayerWinningScenario();
        controller.PlaceBet(100);

        controller.Stand();

        Assert.Equal("18", controller.DealerValueLabel);
    }

    [Fact]
    public void DealerCards_HoleCardIsFaceDownUntilStand()
    {
        var controller = PlayerWinningScenario();
        controller.PlaceBet(100);
        Assert.Contains(controller.DealerCards, card => !card.IsFaceUp);

        controller.Stand();

        Assert.All(controller.DealerCards, card => Assert.True(card.IsFaceUp));
    }

    [Fact]
    public void CardsRemaining_MidRoundBelowThreshold_DoesNotRefill()
    {
        // 4 dealt cards leave 12, under the threshold, but a round is running.
        var controller = ControllerWithDeckSize(RefillThreshold + 1);
        controller.PlaceBet(100);

        Assert.True(controller.IsRoundInProgress);
        Assert.Equal(RefillThreshold + 1 - 4, controller.CardsRemaining);
        Assert.False(controller.JustRefilled);

        controller.Hit();

        Assert.Equal(RefillThreshold + 1 - 5, controller.CardsRemaining);
        Assert.False(controller.JustRefilled);
    }

    [Fact]
    public void JustRefilled_RoundEndsBelowThreshold_RefillsDeckAndFlagsItOnce()
    {
        var controller = ControllerWithDeckSize(RefillThreshold + 1);
        controller.PlaceBet(100);

        controller.Stand();

        Assert.Equal(52, controller.CardsRemaining);
        Assert.True(controller.JustRefilled);

        controller.PlaceBet(10);

        Assert.False(controller.JustRefilled);
    }

    [Fact]
    public void PlaceBet_DeckBelowThreshold_RefillsBeforeDealing()
    {
        var controller = ControllerWithDeckSize(RefillThreshold - 1);

        controller.PlaceBet(100);

        Assert.True(controller.IsRoundInProgress);
        Assert.Equal(52 - 4, controller.CardsRemaining);
        Assert.True(controller.JustRefilled);
    }

    [Fact]
    public void PlaceBet_RejectedBetWithLowDeck_StillShowsRefill()
    {
        var controller = ControllerWithDeckSize(RefillThreshold - 1);

        controller.PlaceBet(StartingBalance + 1);

        Assert.Equal(52, controller.CardsRemaining);
        Assert.True(controller.JustRefilled);
    }

    [Fact]
    public void JustRefilled_IgnoredActionAfterRefill_ClearsFlag()
    {
        var controller = ControllerWithDeckSize(RefillThreshold - 1);
        controller.PlaceBet(StartingBalance + 1);

        controller.Hit();

        Assert.False(controller.JustRefilled);
    }

    [Fact]
    public void PlaceBet_RaisesPropertyChangedForChangedPropertiesOnly()
    {
        var controller = PlayerWinningScenario();
        var raised = Record(controller);

        controller.PlaceBet(100);

        string[] expected =
        {
            nameof(GameController.Status), nameof(GameController.Balance),
            nameof(GameController.CurrentBet), nameof(GameController.IsRoundInProgress),
            nameof(GameController.PlayerCards), nameof(GameController.DealerCards),
            nameof(GameController.PlayerValueLabel), nameof(GameController.DealerValueLabel),
            nameof(GameController.CardsRemaining), nameof(GameController.CanPlaceBet),
            nameof(GameController.CanHit), nameof(GameController.CanStand),
            nameof(GameController.CanDoubleDown),
        };
        Assert.Equal(expected.OrderBy(name => name), raised.OrderBy(name => name));
    }

    [Fact]
    public void Stand_RaisesPropertyChangedForRevealedHoleCardAndOutcome()
    {
        var controller = PlayerWinningScenario();
        controller.PlaceBet(100);
        var raised = Record(controller);

        controller.Stand();

        Assert.Contains(nameof(GameController.DealerCards), raised);
        Assert.Contains(nameof(GameController.DealerValueLabel), raised);
        Assert.Contains(nameof(GameController.LastRoundLabel), raised);
        Assert.Contains(nameof(GameController.Status), raised);
        Assert.DoesNotContain(nameof(GameController.PlayerCards), raised);
    }

    [Fact]
    public void IgnoredAction_RaisesNoPropertyChanged()
    {
        var controller = PlayerWinningScenario();
        var raised = Record(controller);

        controller.Hit();

        Assert.Empty(raised);
    }

    [Fact]
    public void PropertyChanged_HandlerSeesFullyUpdatedState()
    {
        var controller = PlayerWinningScenario();
        int balanceSeenOnFirstEvent = -1;
        controller.PropertyChanged += (_, _) =>
        {
            if (balanceSeenOnFirstEvent == -1)
                balanceSeenOnFirstEvent = controller.Balance;
        };

        controller.PlaceBet(100);

        Assert.Equal(StartingBalance - 100, balanceSeenOnFirstEvent);
    }

    private static List<string> Record(GameController controller)
    {
        var raised = new List<string>();
        controller.PropertyChanged += (_, args) => raised.Add(args.PropertyName!);
        return raised;
    }
}
