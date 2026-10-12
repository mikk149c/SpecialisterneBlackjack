using Blackjack.Game;
using Blackjack.Presentation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Blackjack.WinUI.Controls;

/// <summary>
/// Draws one card as the 11x7 ASCII art the console app uses. It only displays
/// the card it is given. <see cref="Card.IsFaceUp"/> is mutable and cannot be
/// observed, so the owner calls <see cref="Refresh"/> (or reassigns
/// <see cref="Card"/>) after the engine changes a card.
/// </summary>
public sealed partial class CardView : UserControl
{
    public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
        nameof(Card), typeof(Card), typeof(CardView), new PropertyMetadata(null, OnDisplayInputChanged));

    public static readonly DependencyProperty ForceBackProperty = DependencyProperty.Register(
        nameof(ForceBack), typeof(bool), typeof(CardView), new PropertyMetadata(false, OnDisplayInputChanged));

    public CardView()
    {
        InitializeComponent();
        // Nothing is drawn until a card is set, so an empty slot does not need the art file.
        ActualThemeChanged += (_, _) => Refresh();
    }

    public Card? Card
    {
        get => (Card?)GetValue(CardProperty);
        set => SetValue(CardProperty, value);
    }

    /// <summary>Show the card back whatever <see cref="Card"/> is.</summary>
    public bool ForceBack
    {
        get => (bool)GetValue(ForceBackProperty);
        set => SetValue(ForceBackProperty, value);
    }

    public void Refresh()
    {
        var display = CardDisplay.Create(CardArtService.Gallery, Card, ForceBack);
        ArtText.Text = string.Join('\n', display.Lines);
        ApplyInk(display.Ink);
    }

    private static void OnDisplayInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        ((CardView)sender).Refresh();

    private void ApplyInk(CardInk ink)
    {
        switch (ink)
        {
            case CardInk.Red:
                ArtText.Foreground = ThemedBrush("CardRedBrush");
                break;
            case CardInk.CardBack:
                ArtText.Foreground = ThemedBrush("CardBackBrush");
                break;
            default:
                // Inherit the theme's text colour.
                ArtText.ClearValue(TextBlock.ForegroundProperty);
                break;
        }
    }

    // ResourceDictionary's indexer ignores the element's theme, so pick the
    // dictionary for the theme this control is actually shown in.
    private Brush ThemedBrush(string key)
    {
        string themeKey = ActualTheme == ElementTheme.Dark ? "Dark" : "Light";
        var themeDictionary = (ResourceDictionary)Resources.ThemeDictionaries[themeKey];
        return (Brush)themeDictionary[key];
    }
}
