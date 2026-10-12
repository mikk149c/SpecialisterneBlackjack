using Blackjack.Presentation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;

namespace Blackjack.WinUI.Controls;

/// <summary>
/// Draw-pile picture: a "DECK" caption over a stack of offset card backs with the
/// cards-remaining count (or "REFILLED") on the top card. The layout comes from
/// <see cref="DeckStackLayout"/>; this control only binds its properties to it.
/// </summary>
public sealed partial class DeckStack : UserControl
{
    public static readonly DependencyProperty CardsRemainingProperty = DependencyProperty.Register(
        nameof(CardsRemaining), typeof(int), typeof(DeckStack),
        new PropertyMetadata(0, OnLayoutInputChanged));

    public static readonly DependencyProperty JustRefilledProperty = DependencyProperty.Register(
        nameof(JustRefilled), typeof(bool), typeof(DeckStack),
        new PropertyMetadata(false, OnLayoutInputChanged));

    public static readonly DependencyProperty CardBackLinesProperty = DependencyProperty.Register(
        nameof(CardBackLines), typeof(IReadOnlyList<string>), typeof(DeckStack),
        new PropertyMetadata(null, OnLayoutInputChanged));

    public DeckStack()
    {
        InitializeComponent();
        Refresh();
    }

    public int CardsRemaining
    {
        get => (int)GetValue(CardsRemainingProperty);
        set => SetValue(CardsRemainingProperty, value);
    }

    /// <summary>True for exactly one state update after the deck was refilled; the caller owns that lifetime.</summary>
    public bool JustRefilled
    {
        get => (bool)GetValue(JustRefilledProperty);
        set => SetValue(JustRefilledProperty, value);
    }

    /// <summary>
    /// The card-back art, supplied by the caller: <see cref="CardArtGallery.CardHeight"/> lines of at
    /// least <see cref="CardArtGallery.CardWidth"/> characters, otherwise an <see cref="ArgumentException"/>
    /// is thrown. <c>null</c> draws an empty stack.
    /// </summary>
    public IReadOnlyList<string>? CardBackLines
    {
        get => (IReadOnlyList<string>?)GetValue(CardBackLinesProperty);
        set => SetValue(CardBackLinesProperty, value);
    }

    private static void OnLayoutInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        ((DeckStack)sender).Refresh();

    private void Refresh()
    {
        TitleText.Text = DeckStackLayout.TitleLine;

        var cardBack = CardBackLines;
        StackText.Text = cardBack is null
            ? string.Empty
            : string.Join('\n', DeckStackLayout.Build(cardBack, DeckStackLayout.LabelFor(CardsRemaining, JustRefilled)));

        AutomationProperties.SetName(this, DeckStackLayout.AccessibleName(CardsRemaining, JustRefilled));
        VisualStateManager.GoToState(this, JustRefilled ? "Refilled" : "Normal", useTransitions: false);
    }
}
