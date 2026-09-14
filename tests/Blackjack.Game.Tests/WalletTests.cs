namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the starting balance and the payout rules: 1:1 for a normal win,
/// 3:2 for Blackjack, the stake back on a push, and nothing on a loss.
/// </summary>
public class WalletTests
{
    private readonly IGameFactory _factory = new GameFactory();

    [Fact]
    public void CreateWallet_SetsStartingBalance()
    {
        var wallet = _factory.CreateWallet(1000);

        Assert.Equal(1000, wallet.Balance);
    }

    [Fact]
    public void PlaceBet_DeductsAmountFromBalance()
    {
        var wallet = _factory.CreateWallet(1000);

        wallet.PlaceBet(200);

        Assert.Equal(800, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void PlaceBet_NonPositiveAmount_ThrowsArgumentOutOfRangeException(int amount)
    {
        var wallet = _factory.CreateWallet(1000);

        Assert.Throws<ArgumentOutOfRangeException>(() => wallet.PlaceBet(amount));
    }

    [Fact]
    public void PlaceBet_MoreThanBalance_ThrowsInvalidOperationExceptionAndLeavesBalanceUnchanged()
    {
        var wallet = _factory.CreateWallet(1000);

        Assert.Throws<InvalidOperationException>(() => wallet.PlaceBet(1001));
        Assert.Equal(1000, wallet.Balance);
    }

    [Fact]
    public void PlaceBet_EqualToBalance_IsAllowed()
    {
        var wallet = _factory.CreateWallet(1000);

        wallet.PlaceBet(1000);

        Assert.Equal(0, wallet.Balance);
    }

    [Fact]
    public void Settle_PlayerWin_PaysOneToOne()
    {
        var wallet = _factory.CreateWallet(1000);
        wallet.PlaceBet(200);

        var payout = wallet.Settle(RoundOutcome.PlayerWin, 200);

        Assert.Equal(400, payout);
        Assert.Equal(1200, wallet.Balance);
    }

    [Fact]
    public void Settle_Blackjack_PaysThreeToTwo()
    {
        var wallet = _factory.CreateWallet(1000);
        wallet.PlaceBet(200);

        var payout = wallet.Settle(RoundOutcome.Blackjack, 200);

        Assert.Equal(500, payout); // 200 stake back + 300 winnings
        Assert.Equal(1300, wallet.Balance);
    }

    [Fact]
    public void Settle_Push_ReturnsOriginalBet()
    {
        var wallet = _factory.CreateWallet(1000);
        wallet.PlaceBet(200);

        var payout = wallet.Settle(RoundOutcome.Push, 200);

        Assert.Equal(200, payout);
        Assert.Equal(1000, wallet.Balance);
    }

    [Fact]
    public void Settle_DealerWin_PaysNothing()
    {
        var wallet = _factory.CreateWallet(1000);
        wallet.PlaceBet(200);

        var payout = wallet.Settle(RoundOutcome.DealerWin, 200);

        Assert.Equal(0, payout);
        Assert.Equal(800, wallet.Balance);
    }
}
