using Blackjack.Game;

namespace Blackjack.Presentation;

/// <summary>
/// Every status line the game shows the player, in one place so that each
/// front end displays identical wording.
/// </summary>
public static class StatusMessages
{
    public const string Initial = "Place your bet to start the first round.";
    public const string InvalidBetAmount = "Invalid bet amount - enter a whole number.";
    public const string PlayerTurn = "Your turn: Hit, Stand, or Double Down.";

    public static string DescribeOutcome(RoundOutcome? outcome) => outcome switch
    {
        RoundOutcome.PlayerWin => "You win! Press [B] to play again.",
        RoundOutcome.DealerWin => "Dealer wins. Press [B] to play again.",
        RoundOutcome.Blackjack => "Blackjack! You win 3:2. Press [B] to play again.",
        RoundOutcome.Push => "Push - bet returned. Press [B] to play again.",
        _ => "Round resolved. Press [B] to play again.",
    };
}
