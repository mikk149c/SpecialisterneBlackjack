using Blackjack.Game;

namespace Blackjack.Presentation;

/// <summary>Formats the value shown next to a hand's title.</summary>
internal static class HandValueLabels
{
    private const string NoCards = "-";
    private const int FaceCardValue = 10;
    private const int AceValue = 11;

    public static string ForPlayer(IHand hand) =>
        hand.Cards.Count == 0 ? NoCards : hand.Value.ToString();

    // While the hole card is hidden only the face-up total is known, so the
    // label shows it followed by "?" for the unknown card.
    public static string ForDealer(IHand hand, bool isHoleCardRevealed)
    {
        if (hand.Cards.Count == 0)
            return NoCards;

        if (isHoleCardRevealed)
            return hand.Value.ToString();

        int visibleTotal = hand.Cards.Where(card => card.IsFaceUp).Sum(PipValue);
        return $"{visibleTotal} + ?";
    }

    private static int PipValue(Card card) => card.Rank switch
    {
        Rank.Jack or Rank.Queen or Rank.King => FaceCardValue,
        Rank.Ace => AceValue,
        _ => (int)card.Rank,
    };
}
