namespace Blackjack.Presentation;

/// <summary>
/// Wording for failures the player did not cause (a broken art file, an engine fault),
/// so every front end tells the user in the same way that something went wrong.
/// Rule violations such as an invalid bet are not errors and never come through here.
/// </summary>
public static class ErrorMessages
{
    public const string StartupTitle = "Blackjack cannot start";

    /// <summary>An unexpected exception while the game is running, for the status line.</summary>
    public static string Unexpected(Exception exception) =>
        $"Something went wrong: {Describe(exception)}";

    /// <summary>A failure while starting up, for an error dialog before the game exists.</summary>
    public static string Startup(Exception exception) =>
        exception is CardArtLoadException
            ? $"The card art could not be loaded. {exception.Message}"
            : $"Something went wrong while starting the game: {Describe(exception)}";

    private static string Describe(Exception exception) =>
        string.IsNullOrWhiteSpace(exception.Message) ? exception.GetType().Name : exception.Message;
}
