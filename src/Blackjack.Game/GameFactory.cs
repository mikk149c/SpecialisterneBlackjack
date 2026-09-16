namespace Blackjack.Game;

/// <summary>
/// Default <see cref="IGameFactory"/>. Every method is a stub that throws
/// <see cref="NotImplementedException"/> until it is implemented to
/// construct and return a real <see cref="IDeck"/>, <see cref="IHand"/>,
/// <see cref="IWallet"/>, <see cref="IRoundResolver"/>
/// or <see cref="IBlackjackGame"/>.
/// </summary>
public sealed class GameFactory : IGameFactory
{
    public const int DefaultStartingBalance = 1000;

    public IDeck CreateDeck(bool reveal = false) => new Deck(reveal);

    public IDeck CreateDeck(IEnumerable<Card> cards) => new Deck(cards);

    public IHand CreateHand() => new PlayerHand();

    public IWallet CreateWallet(int startingBalance) => new Wallet(startingBalance);

    public IRoundResolver CreateRoundResolver() => new RoundResolver();

    public IBlackjackGame CreateGame(int startingBalance) => CreateGame(startingBalance, CreateDeck());

    private IHand CreatePlayerHand() => new PlayerHand();

    private IHand CreateDealerHand() => new DealerHand();


    public IBlackjackGame CreateGame(int startingBalance, IDeck deck) => new Game(deck, CreateWallet(startingBalance), CreateRoundResolver(), CreateDealerHand(), CreatePlayerHand());
}
