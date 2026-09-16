namespace Blackjack.Game;

/// <summary>
/// Card ranks. Numeric ranks carry their face value; Jack/Queen/King are worth
/// 10 and Ace is resolved to 1 or 11 by <see cref="Hand.Value"/>, whichever
/// keeps the hand valid (closest to 21 without busting).
/// </summary>
public enum Rank
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14,
    Blank = 15
}
