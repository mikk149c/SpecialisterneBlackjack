using System.Text;

namespace Blackjack.Presentation.Tests;

public class DeckStackLayoutTests
{
    // Distinct characters per cell so a layer drawn at the wrong offset or in the wrong order shows up.
    private static readonly string[] CardBack = Enumerable.Range(0, CardArtGallery.CardHeight)
        .Select(row => new string((char)('a' + row), CardArtGallery.CardWidth))
        .ToArray();

    [Fact]
    public void Dimensions_AreCardSizePlusOneCellPerExtraLayer()
    {
        Assert.Equal(CardArtGallery.CardWidth + 2, DeckStackLayout.Width);
        Assert.Equal(CardArtGallery.CardHeight + 2, DeckStackLayout.Height);
        Assert.Equal(3, DeckStackLayout.LabelRow);
    }

    [Fact]
    public void Build_ReturnsFixedFootprint()
    {
        var lines = DeckStackLayout.Build(CardBack, "42");

        Assert.Equal(DeckStackLayout.Height, lines.Count);
        Assert.All(lines, line => Assert.Equal(DeckStackLayout.Width, line.Length));
    }

    [Fact]
    public void Build_LayersCardBacksOneCellRightAndDownPerLayer()
    {
        var lines = DeckStackLayout.Build(CardBack, "42");

        Assert.Equal("aaaaaaaaaaa  ", lines[0]);
        Assert.Equal("bbbbbbbbbbba ", lines[1]);
        Assert.Equal("cccccccccccba", lines[2]);
        Assert.Equal("eeeeeeeeeeedc", lines[4]);
        Assert.Equal(" gggggggggggf", lines[7]);
        Assert.Equal("  ggggggggggg", lines[8]);
    }

    [Fact]
    public void Build_TopCardCoversLayersBeneathIt()
    {
        var lines = DeckStackLayout.Build(CardBack, "42");

        // The middle layer's first row ('a') would start at column 1 of row 1,
        // but the top card's row 1 ('b') is drawn over it.
        Assert.Equal("bbbbbbbbbbb", lines[1][..CardArtGallery.CardWidth]);
    }

    [Fact]
    public void Build_WritesCountCentredInsideBorderOnMiddleRow()
    {
        var lines = DeckStackLayout.Build(CardBack, "42");

        Assert.Equal("|   42    |", lines[DeckStackLayout.LabelRow][..CardArtGallery.CardWidth]);
    }

    [Fact]
    public void Build_KeepsLayerArtRightOfTheLabelOverlay()
    {
        var lines = DeckStackLayout.Build(CardBack, "42");

        // Columns 11-12 on the label row belong to the layers beneath the top card.
        Assert.Equal("cb", lines[DeckStackLayout.LabelRow][CardArtGallery.CardWidth..]);
    }

    [Fact]
    public void Build_LongLabelIsTruncatedToTheCardInterior()
    {
        var lines = DeckStackLayout.Build(CardBack, "REFILLED-TOO-LONG");

        Assert.Equal("|REFILLED-|", lines[DeckStackLayout.LabelRow][..CardArtGallery.CardWidth]);
    }

    [Fact]
    public void Build_MatchesTheOriginalConsoleRendering()
    {
        foreach (var label in new[] { "52", "7", "REFILLED", "" })
        {
            var expected = OriginalConsoleRendering(CardBack, label);

            Assert.Equal(expected, DeckStackLayout.Build(CardBack, label));
        }
    }

    [Fact]
    public void TitleLine_CentresDeckOverTheStack()
    {
        Assert.Equal("    DECK     ", DeckStackLayout.TitleLine);
    }

    [Theory]
    [InlineData(52, false, "52")]
    [InlineData(0, false, "0")]
    [InlineData(14, true, "REFILLED")]
    public void LabelFor_ShowsCountUnlessJustRefilled(int cardsRemaining, bool justRefilled, string expected)
    {
        Assert.Equal(expected, DeckStackLayout.LabelFor(cardsRemaining, justRefilled));
    }

    // Verbatim port of Blackjack.Console.ScreenRenderer's deck drawing before it was
    // moved into DeckStackLayout, kept here to prove the output did not change.
    private static string[] OriginalConsoleRendering(IReadOnlyList<string> cardBack, string label)
    {
        const int stackLayers = 3;
        const int deckWidth = CardArtGallery.CardWidth + (stackLayers - 1);
        const int deckHeight = CardArtGallery.CardHeight + (stackLayers - 1);
        const int deckLabelRow = CardArtGallery.CardHeight / 2;

        var grid = new char[deckHeight, deckWidth];
        for (int r = 0; r < deckHeight; r++)
            for (int c = 0; c < deckWidth; c++)
                grid[r, c] = ' ';

        for (int layer = stackLayers - 1; layer >= 0; layer--)
            for (int r = 0; r < CardArtGallery.CardHeight; r++)
                for (int c = 0; c < CardArtGallery.CardWidth; c++)
                    grid[r + layer, c + layer] = cardBack[r][c];

        var stack = new string[deckHeight];
        for (int r = 0; r < deckHeight; r++)
        {
            var sb = new StringBuilder(deckWidth);
            for (int c = 0; c < deckWidth; c++)
                sb.Append(grid[r, c]);
            stack[r] = sb.ToString();
        }

        string overlay = "|" + Center(label, CardArtGallery.CardWidth - 2) + "|";
        stack[deckLabelRow] = overlay + stack[deckLabelRow][CardArtGallery.CardWidth..];
        return stack;
    }

    private static string Center(string text, int width)
    {
        if (text.Length >= width)
            return text[..width];

        int totalPad = width - text.Length;
        int left = totalPad / 2;
        int right = totalPad - left;
        return new string(' ', left) + text + new string(' ', right);
    }
}
