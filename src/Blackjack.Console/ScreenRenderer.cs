using Blackjack.Game;

namespace Blackjack.Console;

/// <summary>
/// Draws the whole game screen every frame: a top info bar, the player's
/// cards, a fixed-position action bar (entries dim out when not currently
/// available), and the dealer's cards.
/// </summary>
internal sealed class ScreenRenderer
{
    private const int Width = 96;

    private static readonly (char Key, string Label, Func<IBlackjackGame, bool> IsEnabled)[] Actions =
    {
        ('B', "Place Bet", game => !game.IsRoundInProgress),
        ('H', "Hit", game => game.IsRoundInProgress),
        ('S', "Stand", game => game.IsRoundInProgress),
        ('D', "Double Down", game => game.IsRoundInProgress
            && game.PlayerHand.Cards.Count == 2
            && game.Balance >= game.CurrentBet),
        ('Q', "Quit", _ => true),
    };

    private readonly CardArtGallery _gallery;

    public ScreenRenderer(CardArtGallery gallery)
    {
        _gallery = gallery;
    }

    public void Render(IBlackjackGame game, string status)
    {
        try { System.Console.Clear(); }
        catch (IOException) { }

        WriteHeader(game);
        System.Console.WriteLine();
        WriteHandSection("PLAYER", game.PlayerHand.Cards, PlayerValueLabel(game));
        System.Console.WriteLine();
        WriteActionBar(game, status);
        System.Console.WriteLine();
        WriteHandSection("DEALER", game.DealerHand.Cards, DealerValueLabel(game));
    }

    // TODO(Blackjack.Game): IBlackjackGame doesn't expose the deck or a cards-remaining
    // count, so this needs its own reference to the same IDeck passed into
    // GameFactory.CreateGame(startingBalance, deck) just to show it here. Ideally
    // IBlackjackGame would expose CardsRemaining directly.
    private static void WriteHeader(IBlackjackGame game)
    {
        string lastRound = game.LastOutcome?.ToString() ?? "-";
        WriteLinePadded($" Balance: {game.Balance,-8} Bet: {game.CurrentBet,-6} Last Round: {lastRound,-12} Cards Left: {game.GetCardsRemaining()}");
        System.Console.WriteLine(new string('=', Width));
    }

    private void WriteHandSection(string title, IReadOnlyList<Card> cards, string valueLabel)
    {
        WriteLinePadded($" -- {title} ({valueLabel}) --");

        if (cards.Count == 0)
        {
            for (int row = 0; row < CardArtGallery.CardHeight; row++)
                WriteLinePadded(string.Empty);
            return;
        }

        var blocks = cards.Select(_gallery.GetArt).ToList();
        for (int row = 0; row < CardArtGallery.CardHeight; row++)
        {
            var line = new System.Text.StringBuilder(" ");
            foreach (var block in blocks)
                line.Append(block[row]).Append(' ');
            WriteLinePadded(line.ToString());
        }
    }

    private static void WriteActionBar(IBlackjackGame game, string status)
    {
        System.Console.WriteLine(new string('-', Width));
        WriteLinePadded($" {status}");

        System.Console.Write(' ');
        foreach (var (key, label, isEnabled) in Actions)
        {
            System.Console.ForegroundColor = isEnabled(game) ? ConsoleColor.White : ConsoleColor.DarkGray;
            System.Console.Write($"[{key}] {label}  ");
        }
        System.Console.ResetColor();
        System.Console.WriteLine();
        System.Console.WriteLine(new string('-', Width));
    }

    private static string PlayerValueLabel(IBlackjackGame game) =>
        game.PlayerHand.Cards.Count == 0 ? "-" : game.PlayerHand.Value.ToString();

    private static string DealerValueLabel(IBlackjackGame game)
    {
        if (game.DealerHand.Cards.Count == 0)
            return "-";

        if (game.IsDealerHoleCardRevealed)
            return game.DealerHand.Value.ToString();

        int visible = game.DealerHand.Cards.Where(card => card.IsFaceUp).Sum(CardPipValue);
        return $"{visible} + ?";
    }

    private static int CardPipValue(Card card) => card.Rank switch
    {
        Rank.Jack or Rank.Queen or Rank.King => 10,
        Rank.Ace => 11,
        _ => (int)card.Rank,
    };

    // Pads/truncates every line to a fixed width and clears to end-of-line,
    // so redrawing in place never leaves stale characters from a longer
    // previous frame behind.
    private static void WriteLinePadded(string text)
    {
        if (text.Length > Width)
            text = text[..Width];
        System.Console.WriteLine(text.PadRight(Width));
    }
}
