using Blackjack.Game;

namespace Blackjack.Presentation.Tests;

public class ActionBarStatusTests
{
    private const int DeckSize = 25;

    private static GameController NewController()
    {
        var factory = new GameFactory();
        var cards = Enumerable.Range(0, DeckSize).Select(_ => new Card(Rank.Two, Suit.Clubs));
        return new GameController(factory.CreateGame(1000, factory.CreateDeck(cards)));
    }

    [Fact]
    public void Text_WithoutNotice_IsTheControllerStatus()
    {
        var controller = NewController();
        var status = new ActionBarStatus(controller);

        Assert.Equal(StatusMessages.Initial, status.Text);
    }

    [Fact]
    public void ShowNotice_ReplacesTheStatusAndRaisesChanged()
    {
        var status = new ActionBarStatus(NewController());
        var raised = new List<string?>();
        status.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        status.ShowNotice("Not yet.");

        Assert.Equal("Not yet.", status.Text);
        Assert.Equal(new[] { nameof(ActionBarStatus.Text) }, raised);
    }

    [Fact]
    public void Notice_IsClearedWhenTheControllerChanges()
    {
        var controller = NewController();
        var status = new ActionBarStatus(controller);
        status.ShowNotice("Not yet.");

        controller.PlaceBet(100);

        Assert.Equal(StatusMessages.PlayerTurn, status.Text);
    }

    // Balance 1000, bet 1000, player Ten + Six hits a King and busts: balance 0, round over.
    private static GameController OutOfCreditsController()
    {
        var factory = new GameFactory();
        var scripted = new[]
        {
            new Card(Rank.Ten, Suit.Hearts), new Card(Rank.Eight, Suit.Spades),
            new Card(Rank.Six, Suit.Hearts), new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Hearts),
        };
        var filler = Enumerable.Range(0, DeckSize).Select(_ => new Card(Rank.Two, Suit.Clubs));
        var controller = new GameController(factory.CreateGame(1000, factory.CreateDeck(scripted.Concat(filler))));
        controller.PlaceBet(1000);
        controller.Hit();
        return controller;
    }

    [Fact]
    public void OutOfCredits_BalanceZeroAndNoRound_ShowsMessageAndDisablesPlaceBet()
    {
        var controller = OutOfCreditsController();
        var status = new ActionBarStatus(controller);

        Assert.Equal(0, controller.Balance);
        Assert.True(status.IsOutOfCredits);
        Assert.False(status.CanPlaceBet);
        Assert.Equal("You are out of credits.", status.Text);
        Assert.True(controller.CanPlaceBet);
    }

    [Fact]
    public void OutOfCredits_WithBalanceLeft_IsNotOutOfCredits()
    {
        var controller = NewController();
        controller.PlaceBet(100);
        controller.Stand();
        var status = new ActionBarStatus(controller);

        Assert.False(status.IsOutOfCredits);
        Assert.True(status.CanPlaceBet);
    }

    [Fact]
    public void OutOfCredits_BalanceZeroWhileRoundRunning_IsNotOutOfCredits()
    {
        var controller = NewController();
        controller.PlaceBet(1000);
        var status = new ActionBarStatus(controller);

        Assert.Equal(0, controller.Balance);
        Assert.True(controller.IsRoundInProgress);
        Assert.False(status.IsOutOfCredits);
        Assert.Equal(StatusMessages.PlayerTurn, status.Text);
    }

    [Fact]
    public void OutOfCredits_ErrorNoticeStillTakesPriority()
    {
        var status = new ActionBarStatus(OutOfCreditsController());

        status.ShowNotice("Something went wrong: x");

        Assert.Equal("Something went wrong: x", status.Text);
    }

    [Fact]
    public void ControllerChange_RaisesTextAndCanPlaceBetChanged()
    {
        var controller = NewController();
        var status = new ActionBarStatus(controller);
        var raised = new List<string?>();
        status.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        controller.PlaceBet(100);

        Assert.Equal(new[] { nameof(ActionBarStatus.Text), nameof(ActionBarStatus.CanPlaceBet) }.Distinct(), raised.Distinct());
    }

    [Fact]
    public void Text_FollowsTheControllerStatusAfterTheNoticeIsCleared()
    {
        var controller = NewController();
        var status = new ActionBarStatus(controller);
        controller.PlaceBet(100);
        status.ShowNotice("Not yet.");

        controller.Stand();

        Assert.Equal(controller.Status, status.Text);
        Assert.NotEqual("Not yet.", status.Text);
    }
}
