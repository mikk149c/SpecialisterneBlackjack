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
/// same instances raises no change. After the engine flips or deals cards the
/// owner must call <see cref="Refresh"/> (re-assigning <see cref="Cards"/> with
/// a different list instance also works).
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

    public HandSection()
    {
        InitializeComponent();
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

    // Assigning the same instance raises no change, hence the explicit refresh.
    private static void ShowCard(CardView view, Card card)
    {
        view.Card = card;
        view.Refresh();
    }
}
