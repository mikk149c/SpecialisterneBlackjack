using System.Text;

namespace Blackjack.Presentation;

/// <summary>
/// Composes the draw-pile picture shared by every front end: a few offset
/// copies of the card back layered into one text grid, with the cards-remaining
/// count (or the refill notice) written on the middle row of the top card.
/// </summary>
public static class DeckStackLayout
{
    public const int Layers = 3;
    public const int Width = CardArtGallery.CardWidth + (Layers - 1);
    public const int Height = CardArtGallery.CardHeight + (Layers - 1);
    public const int LabelRow = CardArtGallery.CardHeight / 2;

    /// <summary>Rows taken by the caption above the stack.</summary>
    public const int TitleRows = 1;

    /// <summary>Caption plus stack: the full vertical footprint of the draw pile.</summary>
    public const int TotalHeight = TitleRows + Height;

    public const string Title = "DECK";
    public const string RefilledLabel = "REFILLED";

    private const char Background = ' ';
    private const char CardBorder = '|';

    /// <summary>The caption shown above the stack, centred over its footprint.</summary>
    public static string TitleLine => Center(Title, Width);

    /// <summary>The text written on the top card: the count, or the refill notice for one frame.</summary>
    public static string LabelFor(int cardsRemaining, bool justRefilled) =>
        justRefilled ? RefilledLabel : cardsRemaining.ToString();

    /// <summary>
    /// A screen-reader description of the pile, since the drawn art carries no
    /// meaning when read aloud.
    /// </summary>
    public static string AccessibleName(int cardsRemaining, bool justRefilled) =>
        justRefilled
            ? "Deck refilled"
            : $"Deck, {cardsRemaining} {(cardsRemaining == 1 ? "card" : "cards")} remaining";

    /// <summary>
    /// Builds <see cref="Height"/> lines of <see cref="Width"/> characters: the
    /// card back drawn <see cref="Layers"/> times, each copy one cell right and
    /// down from the one beneath it, with <paramref name="label"/> centred
    /// inside the top card's border on <see cref="LabelRow"/>.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="cardBack"/> is not <see cref="CardArtGallery.CardHeight"/> lines
    /// of at least <see cref="CardArtGallery.CardWidth"/> characters.
    /// </exception>
    public static IReadOnlyList<string> Build(IReadOnlyList<string> cardBack, string label)
    {
        EnsureCardBackFits(cardBack);
        var lines = LayerCardBacks(cardBack);
        lines[LabelRow] = Overlay(label) + lines[LabelRow][CardArtGallery.CardWidth..];
        return lines;
    }

    private static void EnsureCardBackFits(IReadOnlyList<string> cardBack)
    {
        ArgumentNullException.ThrowIfNull(cardBack);

        if (cardBack.Count < CardArtGallery.CardHeight
            || cardBack.Take(CardArtGallery.CardHeight).Any(line => line.Length < CardArtGallery.CardWidth))
        {
            throw new ArgumentException(
                $"Card back must be {CardArtGallery.CardHeight} lines of at least {CardArtGallery.CardWidth} characters.",
                nameof(cardBack));
        }
    }

    // Draws the bottom layer first so each upper layer covers the one below.
    private static string[] LayerCardBacks(IReadOnlyList<string> cardBack)
    {
        var grid = new char[Height, Width];
        for (int row = 0; row < Height; row++)
            for (int column = 0; column < Width; column++)
                grid[row, column] = Background;

        for (int layer = Layers - 1; layer >= 0; layer--)
            for (int row = 0; row < CardArtGallery.CardHeight; row++)
                for (int column = 0; column < CardArtGallery.CardWidth; column++)
                    grid[row + layer, column + layer] = cardBack[row][column];

        var lines = new string[Height];
        for (int row = 0; row < Height; row++)
        {
            var line = new StringBuilder(Width);
            for (int column = 0; column < Width; column++)
                line.Append(grid[row, column]);
            lines[row] = line.ToString();
        }
        return lines;
    }

    private static string Overlay(string label) =>
        CardBorder + Center(label, CardArtGallery.CardWidth - 2) + CardBorder;

    // Truncates text that is too long, so a label never spills outside the card.
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
