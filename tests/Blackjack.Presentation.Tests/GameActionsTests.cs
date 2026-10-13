using Blackjack.Game;

namespace Blackjack.Presentation.Tests;

public class GameActionsTests
{
    private const int RefillThreshold = 15;

    private readonly IGameFactory _factory = new GameFactory();

    private GameController NewController(int balance = 1000)
    {
        var filler = Enumerable.Range(0, RefillThreshold + 10).Select(_ => new Card(Rank.Two, Suit.Clubs));
        return new GameController(_factory.CreateGame(balance, _factory.CreateDeck(filler)));
    }

    [Fact]
    public void All_ListsTheFiveActionsInConsoleOrder()
    {
        Assert.Equal(
            new[] { "[B] Place Bet", "[H] Hit", "[S] Stand", "[D] Double Down", "[Q] Quit" },
            GameActions.All.Select(action => action.DisplayText));
    }

    [Fact]
    public void DisplayText_MatchesTheEntryTheConsoleBuiltBeforeTheTableExisted()
    {
        // The console used to format each entry inline as $"[{key}] {label}".
        foreach (var action in GameActions.All)
            Assert.Equal($"[{action.Key}] {action.Label}", action.DisplayText);
    }

    [Theory]
    [InlineData("B", "Place Bet")]
    [InlineData("H", "Hit")]
    [InlineData("S", "Stand")]
    [InlineData("D", "Double Down")]
    [InlineData("Q", "Quit")]
    public void AcceleratorKeyAndLabel_AreExposedSeparately(string key, string label)
    {
        var action = GameActions.All.Single(candidate => candidate.AcceleratorKey == key);

        Assert.Equal(label, action.Label);
    }

    [Fact]
    public void IsEnabled_WithNoRound_AllowsOnlyPlaceBetAndQuit()
    {
        var controller = NewController();

        Assert.Equal(
            new[] { true, false, false, false, true },
            GameActions.All.Select(action => action.IsEnabled(controller)));
    }

    [Fact]
    public void IsEnabled_DuringRound_AllowsHitStandDoubleDownAndQuit()
    {
        var controller = NewController();
        controller.PlaceBet(100);

        Assert.Equal(
            new[] { false, true, true, true, true },
            GameActions.All.Select(action => action.IsEnabled(controller)));
    }

    [Fact]
    public void IsEnabled_DoubleDownNeedsBalanceForASecondBet()
    {
        var controller = NewController(balance: 100);
        controller.PlaceBet(100);

        Assert.False(GameActions.DoubleDown.IsEnabled(controller));
        Assert.True(GameActions.Hit.IsEnabled(controller));
    }

}
