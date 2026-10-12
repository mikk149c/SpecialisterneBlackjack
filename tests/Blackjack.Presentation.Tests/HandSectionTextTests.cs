namespace Blackjack.Presentation.Tests;

public class HandSectionTextTests
{
    [Theory]
    [InlineData("PLAYER", "21", "-- PLAYER (21) --")]
    [InlineData("DEALER", "-", "-- DEALER (-) --")]
    [InlineData("DEALER", "10 + ?", "-- DEALER (10 + ?) --")]
    public void Header_UsesTheConsoleWording(string title, string valueLabel, string expected)
    {
        Assert.Equal(expected, HandSectionText.Header(title, valueLabel));
    }

    [Theory]
    [InlineData("PLAYER", "21", 2, "Player hand, 2 cards, value 21")]
    [InlineData("DEALER", "10 + ?", 2, "Dealer hand, 2 cards, value 10 + ?")]
    [InlineData("PLAYER", "11", 1, "Player hand, 1 card, value 11")]
    [InlineData("PLAYER", "-", 0, "Player hand, empty")]
    [InlineData("DEALER", "-", 0, "Dealer hand, empty")]
    [InlineData("", "-", 0, "Hand, empty")]
    public void AccessibleName_DescribesTheHand(string title, string valueLabel, int cardCount, string expected)
    {
        Assert.Equal(expected, HandSectionText.AccessibleName(title, valueLabel, cardCount));
    }
}
