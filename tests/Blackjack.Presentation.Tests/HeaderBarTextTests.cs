namespace Blackjack.Presentation.Tests;

public class HeaderBarTextTests
{
    [Fact]
    public void Entries_UseTheConsoleWording()
    {
        Assert.Equal("Balance: 1000", HeaderBarText.Balance(1000));
        Assert.Equal("Bet: 0", HeaderBarText.Bet(0));
        Assert.Equal("Last Round: -", HeaderBarText.LastRound("-"));
        Assert.Equal("Last Round: PlayerWin", HeaderBarText.LastRound("PlayerWin"));
    }

    // The console printed this interpolation before the text moved to Presentation;
    // the output must stay character for character the same.
    [Theory]
    [InlineData(1000, 0, "-")]
    [InlineData(950, 50, "PlayerWin")]
    [InlineData(0, 100, "DealerWin")]
    [InlineData(123456789, 1234567, "Blackjack")]
    [InlineData(5, 5, "Push")]
    public void ConsoleLine_MatchesTheOriginalConsoleHeader(int balance, int bet, string lastRound)
    {
        string original = $" Balance: {balance,-8} Bet: {bet,-6} Last Round: {lastRound,-12}";

        Assert.Equal(original, HeaderBarText.ConsoleLine(balance, bet, lastRound));
    }

    [Theory]
    [InlineData(1000, 0, "-", "Balance 1000, bet 0, last round none")]
    [InlineData(950, 50, "PlayerWin", "Balance 950, bet 50, last round PlayerWin")]
    public void AccessibleName_ReadsTheWholeBar(int balance, int bet, string lastRound, string expected)
    {
        Assert.Equal(expected, HeaderBarText.AccessibleName(balance, bet, lastRound));
    }
}
