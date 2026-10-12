using Blackjack.Game;

namespace Blackjack.Presentation.Tests;

public class CardDisplayTests
{
    private static readonly string[] Lines = Enumerable.Range(0, CardArtLoader.RequiredLineCount)
        .Select(line => $"block{line / CardArtGallery.CardHeight}-row{line % CardArtGallery.CardHeight}")
        .ToArray();

    private readonly CardArtGallery _gallery = new(Lines);

    private static Card FaceUp(Rank rank, Suit suit) => new(rank, suit) { IsFaceUp = true };

    [Theory]
    [InlineData(Suit.Hearts, CardInk.Red)]
    [InlineData(Suit.Diamonds, CardInk.Red)]
    [InlineData(Suit.Clubs, CardInk.Neutral)]
    [InlineData(Suit.Spades, CardInk.Neutral)]
    public void Create_FaceUpCard_InkFollowsSuit(Suit suit, CardInk expected)
    {
        var display = CardDisplay.Create(_gallery, FaceUp(Rank.Ace, suit));

        Assert.Equal(expected, display.Ink);
    }

    [Fact]
    public void Create_FaceUpCard_UsesCardArt()
    {
        var card = FaceUp(Rank.Queen, Suit.Spades);

        var display = CardDisplay.Create(_gallery, card);

        Assert.Equal(_gallery.GetArt(card), display.Lines);
        Assert.Equal(CardArtGallery.CardHeight, display.Lines.Count);
    }

    [Fact]
    public void Create_FaceDownCard_ShowsCardBack()
    {
        var card = new Card(Rank.Ace, Suit.Hearts) { IsFaceUp = false };

        var display = CardDisplay.Create(_gallery, card);

        Assert.Equal(_gallery.GetCardBack(), display.Lines);
        Assert.Equal(CardInk.CardBack, display.Ink);
    }

    [Fact]
    public void Create_ForceBack_ShowsCardBackEvenForFaceUpCard()
    {
        var display = CardDisplay.Create(_gallery, FaceUp(Rank.Ace, Suit.Hearts), forceBack: true);

        Assert.Equal(_gallery.GetCardBack(), display.Lines);
        Assert.Equal(CardInk.CardBack, display.Ink);
    }

    [Fact]
    public void Create_ForceBackWithoutCard_ShowsCardBack()
    {
        var display = CardDisplay.Create(_gallery, null, forceBack: true);

        Assert.Equal(_gallery.GetCardBack(), display.Lines);
    }

    [Fact]
    public void Create_NoCard_HasNoLines()
    {
        var display = CardDisplay.Create(_gallery, null);

        Assert.Empty(display.Lines);
        Assert.Equal(CardInk.Neutral, display.Ink);
    }

    [Fact]
    public void Create_ReflectsFlippedCardWhenCalledAgain()
    {
        var card = new Card(Rank.Five, Suit.Diamonds) { IsFaceUp = false };
        var before = CardDisplay.Create(_gallery, card);

        card.IsFaceUp = true;
        var after = CardDisplay.Create(_gallery, card);

        Assert.Equal(CardInk.CardBack, before.Ink);
        Assert.Equal(CardInk.Red, after.Ink);
    }
}
