using Blackjack.Console;
using Blackjack.Game;

TrySetOutputEncoding();
TrySetCursorVisible(false);
TrySetTitle("Blackjack");

var cardArtPath = Path.Combine(AppContext.BaseDirectory, "Assets", "CardArt.txt");
var gallery = new CardArtGallery(cardArtPath);
var renderer = new ScreenRenderer(gallery);

var factory = new GameFactory();


IBlackjackGame game = factory.CreateGame(GameFactory.DefaultStartingBalance);
game.ShuffleDeck();

var status = "Place your bet to start the first round.";
var running = true;

while (running)
{
    // `deck` is kept as a side reference purely so the header can show cards
    // remaining - see the TODO in ScreenRenderer.WriteHeader.
    renderer.Render(game, status);
    var key = System.Console.ReadKey(intercept: true).Key;
    status = HandleKey(key, game, status, ref running);
    if(game.GetCardsRemaining() < 15)
    {
        game.NewDeck();
        game.ShuffleDeck();
    }
}

TrySetCursorVisible(true);
System.Console.WriteLine();
System.Console.WriteLine("Thanks for playing!");

static string HandleKey(ConsoleKey key, IBlackjackGame game, string currentStatus, ref bool running)
{
    switch (key)
    {
        case ConsoleKey.H when game.IsRoundInProgress:
            game.Hit();
            return game.IsRoundInProgress
                ? "Your turn: Hit, Stand, or Double Down."
                : DescribeOutcome(game);

        case ConsoleKey.S when game.IsRoundInProgress:
            game.Stand();
            return DescribeOutcome(game);

        case ConsoleKey.D when game.IsRoundInProgress && game.PlayerHand.Cards.Count == 2:
            try
            {
                game.DoubleDown();
                return DescribeOutcome(game);
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }

        case ConsoleKey.B when !game.IsRoundInProgress:
            return PromptForBet(game);

        case ConsoleKey.Q:
            running = false;
            return "Goodbye!";

        default:
            return currentStatus;
    }
}

static string PromptForBet(IBlackjackGame game)
{
    TrySetCursorVisible(true);
    System.Console.Write($"\n Enter bet amount (balance {game.Balance}): ");
    var input = System.Console.ReadLine();
    TrySetCursorVisible(false);

    if (!int.TryParse(input, out var amount))
        return "Invalid bet amount - enter a whole number.";

    try
    {
        game.PlaceBet(amount);
        return "Your turn: Hit, Stand, or Double Down.";
    }
    catch (Exception ex) when (ex is InvalidOperationException or ArgumentOutOfRangeException)
    {
        return ex.Message;
    }
}

static void TrySetCursorVisible(bool visible)
{
    try { System.Console.CursorVisible = visible; }
    catch (IOException) { }
    catch (PlatformNotSupportedException) { }
}

static void TrySetOutputEncoding()
{
    try { System.Console.OutputEncoding = System.Text.Encoding.UTF8; }
    catch (IOException) { }
}

static void TrySetTitle(string title)
{
    try { System.Console.Title = title; }
    catch (IOException) { }
    catch (PlatformNotSupportedException) { }
}

static string DescribeOutcome(IBlackjackGame game) => game.LastOutcome switch
{
    RoundOutcome.PlayerWin => "You win! Press [B] to play again.",
    RoundOutcome.DealerWin => "Dealer wins. Press [B] to play again.",
    RoundOutcome.Blackjack => "Blackjack! You win 3:2. Press [B] to play again.",
    RoundOutcome.Push => "Push - bet returned. Press [B] to play again.",
    _ => "Round resolved. Press [B] to play again.",
};
