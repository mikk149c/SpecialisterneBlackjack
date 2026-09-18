namespace Blackjack.Game
{
    public class Game : IBlackjackGame
    {
        private readonly IDeck _deck;
        private readonly IWallet _wallet;
        private readonly IRoundResolver _roundResolver;

        public int Balance => _wallet.Balance;

        public int CurrentBet => _wallet.Bet;

        public bool IsRoundInProgress {get; set;}

        public IHand PlayerHand {get; private set;}

        public IHand DealerHand {get; private set;}

        public bool IsDealerHoleCardRevealed => DealerHand.Cards.All(card => card.IsFaceUp);

        public RoundOutcome? LastOutcome {get; private set;}


        public Game(IDeck deck, IWallet wallet, IRoundResolver roundResolver, IHand dealerHand, IHand playerHand)
        {
            _deck = deck;
            _wallet = wallet;
            _roundResolver = roundResolver;
            DealerHand = dealerHand;
            PlayerHand = playerHand;
            IsRoundInProgress = false;
        }

        public void ShuffleDeck()
        {
            _deck.Shuffle();
        }

        public void NewDeck()
        {
            _deck.RefreshDeck();
        }

        public void PlayRound()
        {
            // Implementation of the game round logic goes here.
            // This would include dealing cards, allowing player actions,
            // invoking the dealer's play, and resolving the round.
        }

        public void PlaceBet(int amount)
        {
            if (IsRoundInProgress)
                throw new InvalidOperationException("A round is already in progress. Please wait for it to finish before placing a new bet.");
            
            PlayerHand = new PlayerHand();
            DealerHand = new DealerHand();
            _wallet.PlaceBet(amount);
            IsRoundInProgress = true;
            dealInitialCards();
        }

        private void dealInitialCards()
        {
            drawCard(1, PlayerHand);
            drawCard(1, DealerHand);
            drawCard(1, PlayerHand);
            drawCard(1, DealerHand);
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

            drawCard(1, PlayerHand);

            if (PlayerHand.IsBust)
            {
                endRound();
            }
        }

        public void Stand()
        {
            if (!_wallet.BetPlaced)
            {
                throw new InvalidOperationException("No bet has been placed. Please place a bet before standing.");
            }

            endRound();
        }

        private void endRound()
        {
            DealerHand.RevealHoleCard();
            while (!DealerHand.IsBust && DealerHand.Value < 17 && _deck.HasCards())
            {
                drawCard(1, DealerHand);
            }
            IsRoundInProgress = false;
            LastOutcome = _roundResolver.Resolve(PlayerHand, DealerHand);
            _wallet.Settle(LastOutcome.Value);
        }


        public void DoubleDown()
        {
            if (!_wallet.BetPlaced)
            {
                throw new InvalidOperationException("No bet has been placed. Please place a bet before doubling down.");
            }

            if (PlayerHand.Cards.Count != 2)
            {
                throw new InvalidOperationException("Double down is only allowed on the initial two cards.");
            }

            _wallet.PlaceBet(_wallet.Bet);

            drawCard(1, PlayerHand);

            endRound();
        }

        public int GetCardsRemaining()
        {
            return _deck.CardsRemaining;
        }
    }
}