using Blackjack.Game;

namespace Blackjack.Presentation;

/// <summary>
/// Reads the card-art texture strip: 53 fixed-size blocks
/// (7 lines x 11 chars) back to back, ordered by <see cref="Card.Index"/>
/// (0-51), followed by one card-back block at index 52.
/// </summary>
public sealed class CardArtGallery
{
    public const int CardWidth = 11;
    public const int CardHeight = 7;
    private const int CardBackIndex = 52;

    private readonly string[] _lines;

    public CardArtGallery(string path)
        : this(File.ReadAllLines(path))
    {
    }

    public CardArtGallery(IEnumerable<string> lines)
    {
        _lines = lines.ToArray();
    }

    public IReadOnlyList<string> GetArt(Card card) =>
        GetArtByIndex(card.IsFaceUp ? card.Index : CardBackIndex);

    public IReadOnlyList<string> GetCardBack() => GetArtByIndex(CardBackIndex);

    private IReadOnlyList<string> GetArtByIndex(int index)
    {
        int start = index * CardHeight;
        var block = new string[CardHeight];
        Array.Copy(_lines, start, block, 0, CardHeight);
        return block;
    }
}
