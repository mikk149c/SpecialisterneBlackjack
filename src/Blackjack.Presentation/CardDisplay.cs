using Blackjack.Game;

namespace Blackjack.Presentation;

/// <summary>
/// What a single card slot shows: the art lines, the colour family to draw
/// them in and the text a screen reader announces. A slot without a card has
/// no lines but still occupies its space.
/// </summary>
public sealed class CardDisplay
{
    public const string FaceDownDescription = "Face-down card";
    public const string EmptySlotDescription = "Empty card slot";

    /// <summary>An empty slot. Needs no card art, so it can be used before the art file is loaded.</summary>
    public static readonly CardDisplay Empty = new([], CardInk.Neutral, EmptySlotDescription);

    public IReadOnlyList<string> Lines { get; }

    public CardInk Ink { get; }

    public string Description { get; }

    private CardDisplay(IReadOnlyList<string> lines, CardInk ink, string description)
    {
        Lines = lines;
        Ink = ink;
        Description = description;
    }

    public static CardDisplay Create(CardArtGallery gallery, Card? card, bool forceBack = false)
    {
        if (forceBack)
        {
            return new CardDisplay(gallery.GetCardBack(), CardInk.CardBack, FaceDownDescription);
        }

        if (card is null)
        {
            return Empty;
        }

        return new CardDisplay(gallery.GetArt(card), InkFor(card), DescriptionFor(card));
    }

    private static CardInk InkFor(Card card) => card.Suit switch
    {
        Suit.Hearts or Suit.Diamonds => CardInk.Red,
        Suit.Clubs or Suit.Spades => CardInk.Neutral,
        Suit.Blank => CardInk.CardBack,
        _ => throw new ArgumentOutOfRangeException(nameof(card), card.Suit, "Unknown suit."),
    };

    private static string DescriptionFor(Card card) =>
        card.IsFaceUp ? $"{card.Rank} of {card.Suit}" : FaceDownDescription;
}
