using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;

namespace Mka.Rag;


// Text from one page (PDF) or the whole file (other formats, PageNumber = null).
public record PageText(int? PageNumber, string Text);


public static class TextExtractor
{
    public static IReadOnlyList<PageText> Extract(string fileName, byte[] content)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            // Some editors (e.g. Notepad) save UTF-8 with a BOM: an invisible U+FEFF at the start.
            // Remove it so the first chunk does not start with a hidden character
            // that breaks string comparisons and shows up in the index.
            ".txt" or ".md" => [new PageText(null, Encoding.UTF8.GetString(content).TrimStart((char)0xFEFF))],
            ".pdf" => ExtractPdf(content),
            ".docx" => [new PageText(null, ExtractDocx(content))],
            _ => throw new NotSupportedException($"File type {ext} is not supported.")
        };
    }

    private static List<PageText> ExtractPdf(byte[] content)
    {
        using var pdf = PdfDocument.Open(content);
        return pdf.GetPages()
        .Select(page => new PageText(page.Number, page.Text))
        .Where(p => !string.IsNullOrWhiteSpace(p.Text)) // scanned pages have no text layer
        .ToList();
    }

    private static string ExtractDocx(byte[] content)
    {
        using var stream = new MemoryStream(content);
        using var doc = WordprocessingDocument.Open(stream, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if(body is null) return "";

        // Keep paragraph breaks, so the chunker can split on them.
        var paragraghs = body.Descendants<Paragraph>()
        .Select(p => p.InnerText)
        .Where(t => t.Length > 0);
        return string.Join("\n\n", paragraghs);
    }
}