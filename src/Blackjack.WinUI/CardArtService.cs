using Blackjack.Presentation;

namespace Blackjack.WinUI;

/// <summary>
/// Loads <c>Assets\CardArt.txt</c> once and hands out the shared gallery.
/// Throws <see cref="CardArtLoadException"/> (with a message fit for a dialog)
/// when the file is missing or incomplete.
/// </summary>
public static class CardArtService
{
    private static readonly Lazy<CardArtGallery> SharedGallery = new(
        () => CardArtLoader.Load(Path.Combine(AppContext.BaseDirectory, "Assets", "CardArt.txt")));

    public static CardArtGallery Gallery => SharedGallery.Value;
}
