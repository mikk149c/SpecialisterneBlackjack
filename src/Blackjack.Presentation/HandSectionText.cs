using System.Globalization;

namespace Blackjack.Presentation;

/// <summary>
/// The text a hand section shows and announces: the same wording in every
/// front end, so the console and WinUI headers cannot drift apart.
/// </summary>
public static class HandSectionText
{
    /// <summary>The header line, e.g. <c>-- PLAYER (21) --</c>.</summary>
    public static string Header(string title, string valueLabel) => $"-- {title} ({valueLabel}) --";

    /// <summary>
    /// A screen-reader description of the whole hand, e.g.
    /// <c>Player hand, 2 cards, value 21</c> or <c>Dealer hand, 2 cards, value 10 + ?</c>.
    /// It is built from the value label the caller already holds, so a face-down
    /// card is never revealed by it.
    /// </summary>
    public static string AccessibleName(string title, string valueLabel, int cardCount)
    {
        string subject = title.Length == 0 ? "Hand" : $"{ToSentenceCase(title)} hand";

        if (cardCount <= 0)
            return $"{subject}, empty";

        string cards = cardCount == 1 ? "1 card" : $"{cardCount} cards";
        return $"{subject}, {cards}, value {valueLabel}";
    }

    // Titles are shouted in the header ("PLAYER"); read aloud they are plain words.
    private static string ToSentenceCase(string text)
    {
        var lower = text.ToLower(CultureInfo.InvariantCulture);
        return char.ToUpper(lower[0], CultureInfo.InvariantCulture) + lower[1..];
    }
}
