namespace Blackjack.Presentation;

/// <summary>
/// Which colour family a card is drawn in. Front ends map each value to a
/// concrete colour of their own palette.
/// </summary>
public enum CardInk
{
    /// <summary>Clubs, spades and the empty slot: the normal text colour.</summary>
    Neutral,

    /// <summary>Hearts and diamonds.</summary>
    Red,

    /// <summary>The back of a card.</summary>
    CardBack,
}
