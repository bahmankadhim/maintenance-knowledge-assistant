using System.Text;

namespace Mka.Rag;

public record TextChunk(int Index, int? PageNumber, string Content);

public static class TextChunker
{
    // ~1,500 characters is roughly 350-400 tokens: big enough to keep one procedure together,
    // small enough that several chunks fit in one prompt. Overlap keeps facts that sit on a boundary.
    public static List<TextChunk> Split(IReadOnlyList<PageText> pages, int maxChars = 1500, int overlapChars = 200)
    {
        var chunks = new List<TextChunk>();

        foreach (var page in pages)
        {
            var paragraphs = page.Text
                .Replace("\r\n", "\n")
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var current = new StringBuilder();
            foreach (var paragraph in paragraphs)
            {
                foreach (var piece in SplitLong(paragraph, maxChars))
                {
                    // Adding this piece would make the chunk too big: close it and start a new one.
                    if (current.Length > 0 && current.Length + piece.Length + 2 > maxChars)
                    {
                        chunks.Add(new TextChunk(chunks.Count, page.PageNumber, current.ToString()));
                        var overlap = Tail(current.ToString(), overlapChars);
                        current.Clear().Append(overlap);
                    }

                    if (current.Length > 0) current.Append("\n\n");
                    current.Append(piece);
                }
            }

            if (current.Length > 0)
                chunks.Add(new TextChunk(chunks.Count, page.PageNumber, current.ToString()));
        }

        return chunks;
    }

    // A paragraph longer than maxChars is cut at the last space before the limit.
    private static IEnumerable<string> SplitLong(string text, int maxChars)
    {
        while (text.Length > maxChars)
        {
            var cut = text.LastIndexOf(' ', maxChars);
            if (cut <= 0) cut = maxChars;
            yield return text[..cut].Trim();
            text = text[cut..].Trim();
        }
        if (text.Length > 0) yield return text;
    }

    // The last ~length characters, starting at a word boundary.
    private static string Tail(string text, int length)
    {
        if (text.Length <= length) return text;
        var start = text.IndexOf(' ', text.Length - length);
        return start < 0 ? text[^length..] : text[(start + 1)..];
    }
}