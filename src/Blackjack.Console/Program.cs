using Blackjack.Console;
using Blackjack.Presentation;

TrySetOutputEncoding();
TrySetCursorVisible(false);
TrySetTitle("Blackjack");

var cardArtPath = Path.Combine(AppContext.BaseDirectory, "Assets", "CardArt.txt");
var renderer = new ScreenRenderer(new CardArtGallery(cardArtPath));
var controller = GameController.CreateDefault();

var running = true;

while (running)
{
    renderer.Render(controller);
    var key = System.Console.ReadKey(intercept: true).Key;
    running = HandleKey(key, controller);
}

TrySetCursorVisible(true);
System.Console.WriteLine();
System.Console.WriteLine("Thanks for playing!");

// Returns false when the player asked to quit.
static bool HandleKey(ConsoleKey key, GameController controller)
{
    switch (key)
    {
        case ConsoleKey.H:
            controller.Hit();
            break;

        case ConsoleKey.S:
            controller.Stand();
            break;

        case ConsoleKey.D:
            controller.DoubleDown();
            break;

        case ConsoleKey.B when controller.CanPlaceBet:
            controller.PlaceBet(PromptForBet(controller.Balance));
            break;

        case ConsoleKey.Q:
            return false;
    }

    return true;
}

static string? PromptForBet(int balance)
{
    TrySetCursorVisible(true);
    System.Console.Write($"\n {BetPrompt.Message(balance)} ");
    var input = System.Console.ReadLine();
    TrySetCursorVisible(false);
    return input;
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
