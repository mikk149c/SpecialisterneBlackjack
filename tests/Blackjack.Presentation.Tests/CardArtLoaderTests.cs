namespace Blackjack.Presentation.Tests;

public sealed class CardArtLoaderTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"cardart-{Guid.NewGuid():N}.txt");

    public void Dispose()
    {
        File.Delete(_path);
    }

    [Fact]
    public void Load_CompleteFile_ReturnsGalleryReadingTheFile()
    {
        File.WriteAllLines(_path, Enumerable.Range(0, CardArtLoader.RequiredLineCount).Select(line => $"line{line}"));

        var gallery = CardArtLoader.Load(_path);

        Assert.Equal("line364", gallery.GetCardBack()[0]);
    }

    [Fact]
    public void Load_FileWithTooFewLines_ThrowsNamingFileAndCounts()
    {
        File.WriteAllLines(_path, Enumerable.Repeat("x", CardArtLoader.RequiredLineCount - 1));

        var exception = Assert.Throws<CardArtLoadException>(() => CardArtLoader.Load(_path));

        Assert.Contains(_path, exception.Message);
        Assert.Contains((CardArtLoader.RequiredLineCount - 1).ToString(), exception.Message);
        Assert.Contains(CardArtLoader.RequiredLineCount.ToString(), exception.Message);
    }

    [Fact]
    public void Load_MissingFile_ThrowsCardArtLoadExceptionWithInnerException()
    {
        var exception = Assert.Throws<CardArtLoadException>(() => CardArtLoader.Load(_path));

        Assert.Contains(_path, exception.Message);
        Assert.IsAssignableFrom<IOException>(exception.InnerException);
    }

    [Fact]
    public void Load_MissingDirectory_ThrowsCardArtLoadException()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "CardArt.txt");

        Assert.Throws<CardArtLoadException>(() => CardArtLoader.Load(path));
    }
}
