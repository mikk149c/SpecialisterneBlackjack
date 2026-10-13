namespace Blackjack.Presentation;

/// <summary>
/// Loads the card-art texture strip and checks that it holds every card block
/// before a <see cref="CardArtGallery"/> is built from it.
/// </summary>
public static class CardArtLoader
{
    /// <summary>52 cards plus one card back.</summary>
    public const int BlockCount = 53;

    public const int RequiredLineCount = BlockCount * CardArtGallery.CardHeight;

    public static CardArtGallery Load(string path)
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new CardArtLoadException($"Card art file '{path}' could not be read: {exception.Message}", exception);
        }

        if (lines.Length < RequiredLineCount)
        {
            throw new CardArtLoadException(
                $"Card art file '{path}' has {lines.Length} lines but at least {RequiredLineCount} " +
                $"({BlockCount} cards x {CardArtGallery.CardHeight} lines) are required.");
        }

        return new CardArtGallery(lines);
    }
}
