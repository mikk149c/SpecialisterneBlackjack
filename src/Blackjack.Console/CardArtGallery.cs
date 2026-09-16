using Blackjack.Game;

namespace Blackjack.Console;

/// <summary>
/// Reads Assets/CardArt.txt as a texture strip: 53 fixed-size blocks
/// (7 lines x 11 chars) back to back, ordered by <see cref="Card.Index"/>
/// (0-51), followed by one card-back block at index 52.
/// </summary>
internal sealed class CardArtGallery
{
    public const int CardWidth = 11;
    public const int CardHeight = 7;
    private const int CardBackIndex = 52;

    private readonly string[] _lines;

    public CardArtGallery(string path)
    {
        _lines = File.ReadAllLines(path);
    }

    public IReadOnlyList<string> GetArt(Card card) =>
        GetArtByIndex(card.IsFaceUp ? card.Index : CardBackIndex);

    private IReadOnlyList<string> GetArtByIndex(int index)
    {
        int start = index * CardHeight;
        var block = new string[CardHeight];
        Array.Copy(_lines, start, block, 0, CardHeight);
        return block;
    }
}
