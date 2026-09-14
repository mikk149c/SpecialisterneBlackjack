namespace Blackjack.Game;

/// <summary>
/// Shared scoring logic for a hand of cards. <see cref="PlayerHand"/> and
/// <see cref="DealerHand"/> derive from this and add whichever behavior is
/// specific to the player's or the dealer's side of the table.
/// </summary>
public abstract class Hand : IHand
{
    private readonly List<Card> _cards = new();

    public IReadOnlyList<Card> Cards => _cards;

    public void AddCard(Card card)
    {
        _cards.Add(card);
    }

    public int Value
    {
        get
        {
            int total = 0;
            int aceCount = 0;

            foreach (var card in _cards)
            {
                if (card.Rank == Rank.Ace)
                {
                    aceCount++;
                    total += 11;
                }
                else if (card.Rank is Rank.Jack or Rank.Queen or Rank.King)
                {
                    total += 10;
                }
                else
                {
                    total += (int)card.Rank;
                }
            }

            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }

            return total;
        }
    }

    public bool IsBust => Value > 21;

    public bool IsBlackjack => Cards.Count == 2 && Value == 21;
}
