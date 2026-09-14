namespace Blackjack.Game.Tests;

/// <summary>
/// Covers the outcome rules as a pure function of two finished hands: a bust
/// loses immediately, Blackjack beats a normal 21, and equal winning values
/// push.
/// </summary>
public class RoundResolverTests
{
    private readonly IGameFactory _factory = new GameFactory();

    private IHand HandWorth(params Rank[] ranks)
    {
        var hand = _factory.CreateHand();
        foreach (var rank in ranks)
        {
            hand.AddCard(new Card(rank, Suit.Spades));
        }

        return hand;
    }

    [Fact]
    public void Resolve_PlayerBust_DealerWins()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ten, Rank.Nine, Rank.Five); // 24, bust
        var dealer = HandWorth(Rank.Ten, Rank.Six); // 16

        Assert.Equal(RoundOutcome.DealerWin, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_DealerBustPlayerNotBust_PlayerWins()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ten, Rank.Eight); // 18
        var dealer = HandWorth(Rank.Ten, Rank.Nine, Rank.Five); // 24, bust

        Assert.Equal(RoundOutcome.PlayerWin, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_PlayerBlackjackDealerNormalHand_ReturnsBlackjack()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ace, Rank.King); // Blackjack
        var dealer = HandWorth(Rank.Ten, Rank.Nine); // 19

        Assert.Equal(RoundOutcome.Blackjack, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_BothBlackjack_IsPush()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ace, Rank.King);
        var dealer = HandWorth(Rank.Ace, Rank.Queen);

        Assert.Equal(RoundOutcome.Push, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_DealerBlackjackPlayerNormalTwentyOne_DealerWins()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Seven, Rank.Seven, Rank.Seven); // normal 21
        var dealer = HandWorth(Rank.Ace, Rank.King); // Blackjack

        Assert.Equal(RoundOutcome.DealerWin, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_PlayerHigherValueNeitherBlackjack_PlayerWins()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ten, Rank.Nine); // 19
        var dealer = HandWorth(Rank.Ten, Rank.Seven); // 17

        Assert.Equal(RoundOutcome.PlayerWin, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_DealerHigherValueNeitherBlackjack_DealerWins()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ten, Rank.Seven); // 17
        var dealer = HandWorth(Rank.Ten, Rank.Nine); // 19

        Assert.Equal(RoundOutcome.DealerWin, resolver.Resolve(player, dealer));
    }

    [Fact]
    public void Resolve_EqualNormalValues_IsPush()
    {
        var resolver = _factory.CreateRoundResolver();
        var player = HandWorth(Rank.Ten, Rank.Nine); // 19
        var dealer = HandWorth(Rank.King, Rank.Nine); // 19

        Assert.Equal(RoundOutcome.Push, resolver.Resolve(player, dealer));
    }
}
