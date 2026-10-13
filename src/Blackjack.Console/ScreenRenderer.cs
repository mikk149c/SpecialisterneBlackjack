using Blackjack.Game;
using Blackjack.Presentation;

namespace Blackjack.Console;

/// <summary>
/// Draws the whole game screen every frame: a top info bar, a deck of cards
/// to the left of the play area (with the remaining-card count shown on its
/// top card), a fixed-position action bar (entries dim out when not
/// currently available), and the player's / dealer's cards.
/// </summary>
internal sealed class ScreenRenderer
{
    private const int Width = 96;
    private const int DeckWidth = DeckStackLayout.Width;
    private const int LeftMargin = 1;
    private const int Gap = 2;
    private const int TableX = LeftMargin + DeckWidth + Gap;
    private const int TableWidth = Width - TableX;
    private const int PlayAreaTop = 3;

    private readonly CardArtGallery _gallery;

    public ScreenRenderer(CardArtGallery gallery)
    {
        _gallery = gallery;
    }

    public void Render(GameController controller)
    {
        try { System.Console.Clear(); }
        catch (IOException) { }

        WriteHeader(controller);

        WriteDeckColumn(LeftMargin, PlayAreaTop, controller.CardsRemaining, controller.JustRefilled);

        WriteHandSectionAt(TableX, PlayAreaTop, "PLAYER", controller.PlayerCards, controller.PlayerValueLabel);
        WriteActionBarAt(TableX, PlayAreaTop + 9, controller);
        WriteHandSectionAt(TableX, PlayAreaTop + 14, "DEALER", controller.DealerCards, controller.DealerValueLabel);

        System.Console.SetCursorPosition(0, PlayAreaTop + 22);
    }

    private static void WriteHeader(GameController controller)
    {
        WriteAt(0, 0, PadTrunc(HeaderBarText.ConsoleLine(controller.Balance, controller.CurrentBet, controller.LastRoundLabel), Width));
        WriteAt(0, 1, new string('=', Width));
    }

    private void WriteDeckColumn(int x, int y, int cardsRemaining, bool justRefilled)
    {
        ConsoleColor color = justRefilled ? ConsoleColor.Green : ConsoleColor.DarkYellow;

        WriteAt(x, y, PadTrunc(DeckStackLayout.TitleLine, DeckWidth), color);

        var label = DeckStackLayout.LabelFor(cardsRemaining, justRefilled);
        var stack = DeckStackLayout.Build(_gallery.GetCardBack(), label);

        for (int row = 0; row < stack.Count; row++)
            WriteAt(x, y + 1 + row, PadTrunc(stack[row], DeckWidth), color);
    }

    private void WriteHandSectionAt(int x, int y, string title, IReadOnlyList<Card> cards, string valueLabel)
    {
        WriteAt(x, y, PadTrunc($" {HandSectionText.Header(title, valueLabel)}", TableWidth));

        if (cards.Count == 0)
        {
            for (int row = 0; row < CardArtGallery.CardHeight; row++)
                WriteAt(x, y + 1 + row, PadTrunc(string.Empty, TableWidth));
            return;
        }

        var blocks = cards.Select(_gallery.GetArt).ToList();
        for (int row = 0; row < CardArtGallery.CardHeight; row++)
        {
            var line = new System.Text.StringBuilder(" ");
            foreach (var block in blocks)
                line.Append(block[row]).Append(' ');
            WriteAt(x, y + 1 + row, PadTrunc(line.ToString(), TableWidth));
        }
    }

    private static void WriteActionBarAt(int x, int y, GameController controller)
    {
        WriteAt(x, y, new string('-', TableWidth));
        WriteAt(x, y + 1, PadTrunc($" {controller.Status}", TableWidth));

        System.Console.SetCursorPosition(x, y + 2);
        System.Console.Write(' ');
        int used = 1;
        foreach (var action in GameActions.All)
        {
            string segment = $"{action.DisplayText}  ";
            System.Console.ForegroundColor = action.IsEnabled(controller) ? ConsoleColor.White : ConsoleColor.DarkGray;
            System.Console.Write(segment);
            used += segment.Length;
        }
        System.Console.ResetColor();
        if (used < TableWidth)
            System.Console.Write(new string(' ', TableWidth - used));

        WriteAt(x, y + 3, new string('-', TableWidth));
    }

    // Pads/truncates a line to a fixed width so redrawing in place never
    // leaves stale characters from a longer previous frame behind, even if
    // Console.Clear() silently failed.
    private static string PadTrunc(string text, int width)
    {
        if (text.Length > width)
            text = text[..width];
        return text.PadRight(width);
    }

    private static void WriteAt(int x, int y, string text, ConsoleColor? color = null)
    {
        System.Console.SetCursorPosition(x, y);
        if (color.HasValue)
            System.Console.ForegroundColor = color.Value;
        System.Console.Write(text);
        if (color.HasValue)
            System.Console.ResetColor();
    }
}
