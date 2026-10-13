using System.ComponentModel;
using System.Diagnostics;
using Blackjack.Game;

namespace Blackjack.Presentation;

/// <summary>
/// Everything a front end needs to show and drive one blackjack session:
/// display state, which actions are enabled, and the action methods. It owns
/// the status text and the deck-refill rule so every UI behaves identically.
/// Action methods never throw: a rule violation puts its reason in
/// <see cref="Status"/>, an unexpected engine exception is logged and reported
/// there as "Something went wrong", and an action that is not currently
/// allowed is ignored.
/// </summary>
public sealed class GameController : INotifyPropertyChanged
{
    private const int MinimumCardsBeforeDeal = 15;
    private const int InitialHandSize = 2;
    private const string NoLastRound = "-";

    private readonly IBlackjackGame _game;

    private string _status = string.Empty;
    private int _balance;
    private int _currentBet;
    private string _lastRoundLabel = string.Empty;
    private bool _isRoundInProgress;
    private CardState[] _playerCardStates = Array.Empty<CardState>();
    private CardState[] _dealerCardStates = Array.Empty<CardState>();
    private IReadOnlyList<Card> _playerCards = Array.Empty<Card>();
    private IReadOnlyList<Card> _dealerCards = Array.Empty<Card>();
    private string _playerValueLabel = string.Empty;
    private string _dealerValueLabel = string.Empty;
    private int _cardsRemaining;
    private bool _justRefilled;
    private bool _canPlaceBet;
    private bool _canHit;
    private bool _canStand;
    private bool _canDoubleDown;

    public GameController(IBlackjackGame game)
    {
        _game = game;
        _cardsRemaining = _game.GetCardsRemaining();
        CaptureState(StatusMessages.Initial, raiseChanges: false);
    }

    /// <summary>A controller for a standard session: default balance, shuffled deck.</summary>
    public static GameController CreateDefault()
    {
        IBlackjackGame game = new GameFactory().CreateGame(GameFactory.DefaultStartingBalance);
        game.ShuffleDeck();
        return new GameController(game);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Status => _status;
    public int Balance => _balance;
    public int CurrentBet => _currentBet;
    public string LastRoundLabel => _lastRoundLabel;
    public bool IsRoundInProgress => _isRoundInProgress;
    public IReadOnlyList<Card> PlayerCards => _playerCards;
    public IReadOnlyList<Card> DealerCards => _dealerCards;
    public string PlayerValueLabel => _playerValueLabel;
    public string DealerValueLabel => _dealerValueLabel;
    public int CardsRemaining => _cardsRemaining;

    /// <summary>True when <see cref="CardsRemaining"/> went up since the previous state refresh, i.e. the deck was refilled.</summary>
    public bool JustRefilled => _justRefilled;

    public bool CanPlaceBet => _canPlaceBet;
    public bool CanHit => _canHit;
    public bool CanStand => _canStand;
    public bool CanDoubleDown => _canDoubleDown;

    public void Hit()
    {
        if (!_game.IsRoundInProgress)
        {
            IgnoreAction();
            return;
        }

        Perform(() =>
        {
            _game.Hit();
            return _game.IsRoundInProgress
                ? StatusMessages.PlayerTurn
                : StatusMessages.DescribeOutcome(_game.LastOutcome);
        });
    }

    public void Stand()
    {
        if (!_game.IsRoundInProgress)
        {
            IgnoreAction();
            return;
        }

        Perform(() =>
        {
            _game.Stand();
            return StatusMessages.DescribeOutcome(_game.LastOutcome);
        });
    }

    public void DoubleDown()
    {
        if (!_game.IsRoundInProgress || _game.PlayerHand.Cards.Count != InitialHandSize)
        {
            IgnoreAction();
            return;
        }

        Perform(() =>
        {
            try
            {
                _game.DoubleDown();
                return StatusMessages.DescribeOutcome(_game.LastOutcome);
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }
        });
    }

    public void PlaceBet(string? input)
    {
        if (int.TryParse(input, out int amount))
            PlaceBet(amount);
        else if (_game.IsRoundInProgress)
            IgnoreAction();
        else
            Commit(StatusMessages.InvalidBetAmount);
    }

    public void PlaceBet(int amount)
    {
        if (_game.IsRoundInProgress)
        {
            IgnoreAction();
            return;
        }

        Perform(() =>
        {
            // Topping up the deck here, before dealing, keeps a fresh deck from ever
            // being appended behind cards that are already on the table.
            RefillDeckIfLow();

            try
            {
                _game.PlaceBet(amount);
                return StatusMessages.PlayerTurn;
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentOutOfRangeException)
            {
                return ex.Message;
            }
        });
    }

    // The boundary for everything the engine does on the player's behalf. Rule violations are
    // turned into status text by the callers; anything else is a fault the player cannot fix, so it
    // is logged and shown as "something went wrong" instead of crashing the front end. It is not
    // swallowed: the state is re-read and the message is visible.
    private void Perform(Func<string> action)
    {
        string status;
        try
        {
            status = action();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unexpected error during a game action: {ex}");
            status = ErrorMessages.Unexpected(ex);
        }

        Commit(status);
    }

    // A disallowed action still refreshes the state, so JustRefilled lasts
    // exactly one action.
    private void IgnoreAction() => Commit(_status);

    private void Commit(string status)
    {
        if (!_game.IsRoundInProgress)
            RefillDeckIfLow();

        CaptureState(status, raiseChanges: true);
    }

    private void RefillDeckIfLow()
    {
        if (_game.GetCardsRemaining() >= MinimumCardsBeforeDeal)
            return;

        _game.NewDeck();
        _game.ShuffleDeck();
    }

    // Reads the whole game state, then raises PropertyChanged for each value that
    // differs from the previous capture. Handlers only run once every field is
    // up to date, so they never observe a half-updated controller.
    private void CaptureState(string status, bool raiseChanges)
    {
        var changed = new List<string>();
        int cardsRemaining = _game.GetCardsRemaining();
        bool justRefilled = cardsRemaining > _cardsRemaining;

        Update(ref _status, status, nameof(Status), changed);
        Update(ref _balance, _game.Balance, nameof(Balance), changed);
        Update(ref _currentBet, _game.CurrentBet, nameof(CurrentBet), changed);
        Update(ref _lastRoundLabel, _game.LastOutcome?.ToString() ?? NoLastRound, nameof(LastRoundLabel), changed);
        Update(ref _isRoundInProgress, _game.IsRoundInProgress, nameof(IsRoundInProgress), changed);
        UpdateCards(ref _playerCardStates, ref _playerCards, _game.PlayerHand, nameof(PlayerCards), changed);
        UpdateCards(ref _dealerCardStates, ref _dealerCards, _game.DealerHand, nameof(DealerCards), changed);
        Update(ref _playerValueLabel, HandValueLabels.ForPlayer(_game.PlayerHand), nameof(PlayerValueLabel), changed);
        Update(ref _dealerValueLabel,
            HandValueLabels.ForDealer(_game.DealerHand, _game.IsDealerHoleCardRevealed),
            nameof(DealerValueLabel),
            changed);
        Update(ref _cardsRemaining, cardsRemaining, nameof(CardsRemaining), changed);
        Update(ref _justRefilled, justRefilled, nameof(JustRefilled), changed);
        Update(ref _canPlaceBet, !_game.IsRoundInProgress, nameof(CanPlaceBet), changed);
        Update(ref _canHit, _game.IsRoundInProgress, nameof(CanHit), changed);
        Update(ref _canStand, _game.IsRoundInProgress, nameof(CanStand), changed);
        Update(ref _canDoubleDown, CanDoubleDownNow(), nameof(CanDoubleDown), changed);

        if (!raiseChanges)
            return;

        foreach (string name in changed)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private bool CanDoubleDownNow() =>
        _game.IsRoundInProgress
        && _game.PlayerHand.Cards.Count == InitialHandSize
        && _game.Balance >= _game.CurrentBet;

    private static void Update<T>(ref T field, T value, string propertyName, List<string> changed)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        changed.Add(propertyName);
    }

    private static void UpdateCards(
        ref CardState[] states,
        ref IReadOnlyList<Card> cards,
        IHand hand,
        string propertyName,
        List<string> changed)
    {
        var current = hand.Cards.Select(card => new CardState(card, card.IsFaceUp)).ToArray();
        if (states.SequenceEqual(current))
            return;

        states = current;
        cards = current.Select(state => state.Card).ToArray();
        changed.Add(propertyName);
    }

    // Card.Equals ignores IsFaceUp, so the flag is tracked separately for a
    // revealed hole card to count as a change.
    private readonly record struct CardState(Card Card, bool IsFaceUp);
}
