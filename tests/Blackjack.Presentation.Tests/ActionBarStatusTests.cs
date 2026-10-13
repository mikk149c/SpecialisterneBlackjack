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
