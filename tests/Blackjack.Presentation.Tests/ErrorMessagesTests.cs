namespace Blackjack.Presentation.Tests;

public class ErrorMessagesTests
{
    [Fact]
    public void Unexpected_IncludesTheExceptionMessage()
    {
        string message = ErrorMessages.Unexpected(new InvalidOperationException("deck exploded"));

        Assert.Equal("Something went wrong: deck exploded", message);
    }

    [Fact]
    public void Unexpected_ExceptionWithoutMessage_NamesTheExceptionType()
    {
        string message = ErrorMessages.Unexpected(new ExceptionWithBlankMessage());

        Assert.Contains(nameof(ExceptionWithBlankMessage), message);
    }

    [Fact]
    public void Startup_CardArtFailure_SaysTheArtCouldNotBeLoadedAndKeepsTheReason()
    {
        string message = ErrorMessages.Startup(new CardArtLoadException("file 'CardArt.txt' is missing"));

        Assert.StartsWith("The card art could not be loaded.", message);
        Assert.Contains("file 'CardArt.txt' is missing", message);
    }

    [Fact]
    public void Startup_AnyOtherFailure_SaysSomethingWentWrong()
    {
        string message = ErrorMessages.Startup(new InvalidOperationException("boom"));

        Assert.Equal("Something went wrong while starting the game: boom", message);
    }

    private sealed class ExceptionWithBlankMessage : Exception
    {
        public override string Message => " ";
    }
}
