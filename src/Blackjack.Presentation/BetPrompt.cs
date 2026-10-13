namespace Blackjack.Presentation;

/// <summary>
/// The wording of the bet prompt, shared by the console line prompt and the WinUI
/// dialog. Parsing and validation of the answer belong to <see cref="GameController"/>.
/// </summary>
public static class BetPrompt
{
    public const string Title = "Place Bet";
    public const string ConfirmLabel = "Deal";
    public const string CancelLabel = "Cancel";

    public static string Message(int balance) => $"Enter bet amount (balance {balance}):";
}
