using Blackjack.Game;

namespace Blackjack.Presentation.Tests;

public class CardArtGalleryTests
{
    private const int BlockCount = 53;

    // Every line says which block and row it belongs to, so a block read from
    // the wrong offset is obvious in the assertion.
    private static readonly string[] Lines = Enumerable.Range(0, BlockCount * CardArtGallery.CardHeight)
        .Select(line => $"block{line / CardArtGallery.CardHeight}-row{line % CardArtGallery.CardHeight}")
        .ToArray();

    private readonly CardArtGallery _gallery = new(Lines);

    [Fact]
    public void GetArt_FaceUpCard_ReturnsBlockAtCardIndex()
    {
        var card = new Card(Rank.Three, Suit.Diamonds) { IsFaceUp = true };

        var art = _gallery.GetArt(card);

        Assert.Equal(CardArtGallery.CardHeight, art.Count);
        Assert.Equal($"block{card.Index}-row0", art[0]);
        Assert.Equal($"block{card.Index}-row6", art[6]);
    }

    [Fact]
    public void GetArt_FaceDownCard_ReturnsCardBack()
    {
        var card = new Card(Rank.Three, Suit.Diamonds) { IsFaceUp = false };

        var art = _gallery.GetArt(card);

        Assert.Equal(_gallery.GetCardBack(), art);
    }

    [Fact]
    public void GetCardBack_ReturnsLastBlock()
    {
        var back = _gallery.GetCardBack();

        Assert.Equal("block52-row0", back[0]);
        Assert.Equal("block52-row6", back[^1]);
    }

    [Fact]
    public void FilePathConstructor_ReadsLinesFromFile()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(path, Lines);

            var gallery = new CardArtGallery(path);

            Assert.Equal("block52-row0", gallery.GetCardBack()[0]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
