namespace Blackjack.Presentation;

/// <summary>
/// The card-art file could not be used. The message names the file and the
/// problem so a front end can show it to the user as is.
/// </summary>
public sealed class CardArtLoadException : Exception
{
    public CardArtLoadException(string message)
        : base(message)
    {
    }

    public CardArtLoadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
