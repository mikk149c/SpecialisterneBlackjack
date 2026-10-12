using Blackjack.Game;

namespace Blackjack.Presentation;

/// <summary>
/// What a single card slot shows: the art lines and the colour family to draw
/// them in. A slot without a card has no lines but still occupies its space.
/// </summary>
public sealed class CardDisplay
{
    private static readonly IReadOnlyList<string> NoLines = [];

    public IReadOnlyList<string> Lines { get; }

    public CardInk Ink { get; }

    private CardDisplay(IReadOnlyList<string> lines, CardInk ink)
    {
        Lines = lines;
        Ink = ink;
    }

    public static CardDisplay Create(CardArtGallery gallery, Card? card, bool forceBack = false)
    {
        if (forceBack)
        {
            return new CardDisplay(gallery.GetCardBack(), CardInk.CardBack);
        }

        if (card is null)
        {
            return new CardDisplay(NoLines, CardInk.Neutral);
        }

        return new CardDisplay(gallery.GetArt(card), InkFor(card));
    }

    private static CardInk InkFor(Card card) => card.Suit switch
    {
        Suit.Hearts or Suit.Diamonds => CardInk.Red,
        Suit.Clubs or Suit.Spades => CardInk.Neutral,
        _ => CardInk.CardBack,
    };
}
