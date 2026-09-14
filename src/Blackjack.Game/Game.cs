namespace Blackjack.Game
{
    public class Game : IBlackjackGame
    {
        private readonly IDeck _deck;
        private readonly IWallet _wallet;
        private readonly IRoundResolver _roundResolver;
        private readonly IDealerPlay _dealerPlay;

        public int Balance => _wallet.Balance;

        public int CurrentBet => _wallet.Bet;

        public bool IsRoundInProgress {get; set;}

        public IHand PlayerHand {get; private set;}

        public IHand DealerHand {get; private set;}

        public bool IsDealerHoleCardRevealed => DealerHand.Cards.All(card => card.IsFaceUp);

        public RoundOutcome? LastOutcome => throw new NotImplementedException();


        public Game(IDeck deck, IWallet wallet, IRoundResolver roundResolver, IDealerPlay dealerPlay, IHand dealerHand, IHand playerHand)
        {
            _deck = deck;
            _wallet = wallet;
            _roundResolver = roundResolver;
            _dealerPlay = dealerPlay;
            DealerHand = dealerHand;
            PlayerHand = playerHand;
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
            _wallet.PlaceBet(amount);
            IsRoundInProgress = true;
            dealInitialCards();
        }

        private void dealInitialCards()
        {
            drawCard(2, PlayerHand);
            drawCard(2, DealerHand);
        }

        private void drawCard(int count, IHand hand)
        {
            for (int i = 0; i < count; i++)
                hand.AddCard(_deck.DrawCard());
        }

        public void Hit()
        {
            if (!_wallet.BetPlaced)
            {
                throw new InvalidOperationException("No bet has been placed. Please place a bet before hitting.");
            }
        }

        public void Stand()
        {
            if (!_wallet.BetPlaced)
            {
                throw new InvalidOperationException("No bet has been placed. Please place a bet before standing.");
            }
        }

        public void DoubleDown()
        {
            if (!_wallet.BetPlaced)
            {
                throw new InvalidOperationException("No bet has been placed. Please place a bet before doubling down.");
            }
        }

    }
}