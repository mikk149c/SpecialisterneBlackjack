using Blackjack.Game;
using Blackjack.Presentation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;

namespace Blackjack.WinUI.Controls;

/// <summary>
/// One side of the table: a <c>-- TITLE (value) --</c> header over the hand's
/// cards, side by side in deal order. It shows what it is given and computes
/// nothing: the caller supplies the title, the already-formatted value label
/// (<see cref="GameController"/>'s player/dealer labels) and the cards.
/// <para>
/// <see cref="Card.IsFaceUp"/> is mutable and cannot be observed, and
/// <see cref="Card"/> has reference equality, so assigning a list holding the
/// same instances raises no change. <c>GameController.UpdateCards</c> already
/// returns a new array whenever a card's <c>IsFaceUp</c> changes, so binding
/// <see cref="Cards"/> to <c>PlayerCards</c>/<c>DealerCards</c> re-renders
/// without help. <see cref="Refresh"/> is only needed by callers that hold on
/// to the same list instance and mutate cards behind it.
/// </para>
/// <para>
/// More cards than fit scroll horizontally; the scroll viewer then becomes a
/// tab stop with a focus visual and scrolls with the arrow keys.
/// </para>
/// </summary>
public sealed partial class HandSection : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(HandSection),
        new PropertyMetadata(string.Empty, OnDisplayInputChanged));

    public static readonly DependencyProperty ValueLabelProperty = DependencyProperty.Register(
        nameof(ValueLabel), typeof(string), typeof(HandSection),
        new PropertyMetadata(string.Empty, OnDisplayInputChanged));

    public static readonly DependencyProperty CardsProperty = DependencyProperty.Register(
        nameof(Cards), typeof(IReadOnlyList<Card>), typeof(HandSection),
        new PropertyMetadata(null, OnDisplayInputChanged));

    // One character cell of the 14 px monospace font the cards are drawn in.
    private const double CardGap = 8;

    // CardView.Height (7 art lines x 16 px): an empty row keeps the height of a dealt one.
    private const double CardRowHeight = 112;

    public HandSection()
    {
        InitializeComponent();
        CardRow.Spacing = CardGap;
        CardScroller.MinHeight = CardRowHeight;
        CardScroller.SizeChanged += (_, _) => UpdateScrollerTabStop();
        CardRow.SizeChanged += (_, _) => UpdateScrollerTabStop();
        Refresh();
    }

    /// <summary>The section name as it appears in the header, e.g. <c>PLAYER</c>.</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>The value text shown in the header, supplied by the caller (e.g. <c>21</c>, <c>-</c>, <c>10 + ?</c>).</summary>
    public string ValueLabel
    {
        get => (string)GetValue(ValueLabelProperty);
        set => SetValue(ValueLabelProperty, value);
    }

    /// <summary>The hand's cards in deal order. <c>null</c> or empty shows the empty section.</summary>
    public IReadOnlyList<Card>? Cards
    {
        get => (IReadOnlyList<Card>?)GetValue(CardsProperty);
        set => SetValue(CardsProperty, value);
    }

    /// <summary>
    /// Re-renders the header and every card from their current state. Call it
    /// after the engine changes cards in place (a flipped hole card, a hit)
    /// because those changes cannot be observed.
    /// </summary>
    public void Refresh()
    {
        string title = Title ?? string.Empty;
        string valueLabel = ValueLabel ?? string.Empty;
        var cards = Cards ?? [];

        HeaderText.Text = HandSectionText.Header(title, valueLabel);
        AutomationProperties.SetName(this, HandSectionText.AccessibleName(title, valueLabel, cards.Count));

        MatchCardViewCount(cards.Count);
        for (int index = 0; index < cards.Count; index++)
            ShowCard((CardView)CardRow.Children[index], cards[index]);
    }

    private static void OnDisplayInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        ((HandSection)sender).Refresh();

    private void MatchCardViewCount(int count)
    {
        while (CardRow.Children.Count > count)
            CardRow.Children.RemoveAt(CardRow.Children.Count - 1);

        while (CardRow.Children.Count < count)
            CardRow.Children.Add(new CardView());
    }

    // Assigning the same instance raises no change, so only then refresh explicitly
    // (a different card already re-renders through the property callback).
    private static void ShowCard(CardView view, Card card)
    {
        if (ReferenceEquals(view.Card, card))
            view.Refresh();
        else
            view.Card = card;
    }

    // Only a row wider than its viewport needs keyboard focus; otherwise it would be a useless tab stop.
    private void UpdateScrollerTabStop() =>
        CardScroller.IsTabStop = CardScroller.ExtentWidth > CardScroller.ViewportWidth;
}
