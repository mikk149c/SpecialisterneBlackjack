namespace Blackjack.Game
{
    public class Game : IBlackjackGame
    {
        private readonly IDeck _deck;
        private readonly IWallet _wallet;
        private readonly IRoundResolver _roundResolver;
        private readonly IDealerPlay _dealerPlay;

        public int Balance => _wallet.Balance;

        public int CurrentBet => throw new NotImplementedException();

        public bool IsRoundInProgress {get; set;}

        public IHand PlayerHand => throw new NotImplementedException();

        public IHand DealerHand => throw new NotImplementedException();

        public bool IsDealerHoleCardRevealed => throw new NotImplementedException();

        public RoundOutcome? LastOutcome => throw new NotImplementedException();

        public Game(IDeck deck, IWallet wallet, IRoundResolver roundResolver, IDealerPlay dealerPlay)
        {
            _deck = deck;
            _wallet = wallet;
            _roundResolver = roundResolver;
            _dealerPlay = dealerPlay;
            IsRoundInProgress = false;
        }

        public void PlayRound()
        {
            // Implementation of the game round logic goes here.
            // This would include dealing cards, allowing player actions,
            // invoking the dealer's play, and resolving the round.
        }

        public void PlaceBet(int amount)
        {
            throw new NotImplementedException();
        }

        public void Hit()
        {
            throw new NotImplementedException();
        }

        public void Stand()
        {
            throw new NotImplementedException();
        }

        public void DoubleDown()
        {
            throw new NotImplementedException();
        }

    }
}