namespace Blackjack.Presentation.Tests;

public class BetPromptTests
{
    [Theory]
    [InlineData(1000, "Enter bet amount (balance 1000):")]
    [InlineData(0, "Enter bet amount (balance 0):")]
    [InlineData(37, "Enter bet amount (balance 37):")]
    public void Message_ContainsTheBalance(int balance, string expected)
    {
        Assert.Equal(expected, BetPrompt.Message(balance));
    }

    [Fact]
    public void Labels_MatchTheDialogSpecification()
    {
        Assert.Equal("Place Bet", BetPrompt.Title);
        Assert.Equal("Deal", BetPrompt.ConfirmLabel);
        Assert.Equal("Cancel", BetPrompt.CancelLabel);
    }
}
