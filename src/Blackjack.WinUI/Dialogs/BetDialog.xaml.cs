using Blackjack.Presentation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Blackjack.WinUI.Dialogs;

/// <summary>
/// Asks for a bet. It returns the raw text unchanged: parsing and validation are owned by
/// <see cref="GameController.PlaceBet(string?)"/>, so the dialog has no rules of its own.
/// </summary>
public sealed partial class BetDialog : ContentDialog
{
    public BetDialog(XamlRoot xamlRoot, int balance)
    {
        InitializeComponent();
        XamlRoot = xamlRoot;
        PromptText.Text = BetPrompt.Message(balance);
        Opened += (_, _) => BetInput.Focus(FocusState.Programmatic);
    }

    /// <summary>Shows the dialog; returns the entered text, or null when the player cancelled.</summary>
    public async Task<string?> AskAsync()
    {
        ContentDialogResult result = await ShowAsync();
        return result == ContentDialogResult.Primary ? BetInput.Text : null;
    }
}
