using Blackjack.Game;

namespace Blackjack.Presentation.Tests;

public class StatusMessagesTests
{
    [Theory]
    [InlineData(RoundOutcome.PlayerWin, "You win! Press [B] to play again.")]
    [InlineData(RoundOutcome.DealerWin, "Dealer wins. Press [B] to play again.")]
    [InlineData(RoundOutcome.Blackjack, "Blackjack! You win 3:2. Press [B] to play again.")]
    [InlineData(RoundOutcome.Push, "Push - bet returned. Press [B] to play again.")]
    [InlineData(null, "Round resolved. Press [B] to play again.")]
    public void DescribeOutcome_ReturnsMessageForOutcome(RoundOutcome? outcome, string expected)
    {
        Assert.Equal(expected, StatusMessages.DescribeOutcome(outcome));
    }
}
