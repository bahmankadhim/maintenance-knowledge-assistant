using Mka.Rag;

namespace Mka.Api.Tests;

public class TextChunkerTests
{
    [Fact]
    public void ShortText_ReturnsOneChunk()
    {
        var pages = new[] { new PageText(null, "Check the seal every 500 hours.") };

        var chunks = TextChunker.Split(pages);

        Assert.Single(chunks);
        Assert.Equal(0, chunks[0].Index);
        Assert.Equal("Check the seal every 500 hours.", chunks[0].Content);
    }

    [Fact]
    public void LongText_IsSplit_AndChunksStayUnderLimit()
    {
        var paragraph = string.Join(' ', Enumerable.Repeat("pump seal vibration bearing", 20));
        var text = string.Join("\n\n", Enumerable.Repeat(paragraph, 10));
        var pages = new[] { new PageText(3, text) };

        var chunks = TextChunker.Split(pages, maxChars: 1500, overlapChars: 200);

        Assert.True(chunks.Count > 1);
        Assert.All(chunks, c => Assert.True(c.Content.Length <= 1702));
        Assert.All(chunks, c => Assert.Equal(3, c.PageNumber));
        Assert.Equal(Enumerable.Range(0, chunks.Count), chunks.Select(c => c.Index));
    }

    [Fact]
    public void PageNumbers_ArePreserved()
    {
        var pages = new[]
        {
            new PageText(1, "Page one text."),
            new PageText(2, "Page two text.")
        };

        var chunks = TextChunker.Split(pages);

        Assert.Equal(new int?[] { 1, 2 }, chunks.Select(c => c.PageNumber));
    }

    [Fact]
    public void EmptyText_ReturnsNoChunks()
    {
        var pages = new[] { new PageText(1, "   ") };

        var chunks = TextChunker.Split(pages);

        Assert.Empty(chunks);
    }
}