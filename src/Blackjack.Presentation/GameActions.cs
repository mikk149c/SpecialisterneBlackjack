namespace Blackjack.Presentation;

/// <summary>
/// The five player actions in display order: the one definition of their keys,
/// labels and enable rules, shared by the console and WinUI action bars.
/// </summary>
public static class GameActions
{
    public static GameAction PlaceBet { get; } = new('B', "Place Bet", controller => controller.CanPlaceBet);
    public static GameAction Hit { get; } = new('H', "Hit", controller => controller.CanHit);
    public static GameAction Stand { get; } = new('S', "Stand", controller => controller.CanStand);
    public static GameAction DoubleDown { get; } = new('D', "Double Down", controller => controller.CanDoubleDown);
    public static GameAction Quit { get; } = new('Q', "Quit", _ => true);

    /// <summary>All actions in the order they appear in the action bar.</summary>
    public static IReadOnlyList<GameAction> All { get; } = new[] { PlaceBet, Hit, Stand, DoubleDown, Quit };
}
