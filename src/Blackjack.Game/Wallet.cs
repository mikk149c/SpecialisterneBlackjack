namespace Blackjack.Game
{
    public class Wallet : IWallet
    {
        public int Balance { get; set; }
        public int Bet { get; set; }
        public bool BetPlaced => Bet > 0;

        public Wallet(int balance)
        {
            Balance = balance;
        }

        public void PlaceBet(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be positive.");
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient balance to place the bet.");

            Balance -= amount;
            Bet += amount;
        }

        public int Settle(RoundOutcome outcome)
        {
            int payout = outcome switch
            {
                RoundOutcome.PlayerWin => Bet*2,
                RoundOutcome.Blackjack => (int)(Bet * 2.5),
                RoundOutcome.Push => Bet,
                RoundOutcome.DealerWin => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(outcome), "Invalid round outcome.")
            };

            Balance += payout;
            Bet = 0;
            return payout;
        }

        public void Deposit(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");

            Balance += amount;
        }
    }
}