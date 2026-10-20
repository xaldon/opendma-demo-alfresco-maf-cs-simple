using System.Text;
using OpenDMA.Api;

namespace OpenDma.Demo.Alfresco.Maf.Simple;

internal sealed class MarkdownContentReader
{
    public async Task<string?> ReadMarkdownAsync(IOdmaDocument document, CancellationToken cancellationToken)
    {
        if (document.PrimaryContentElement is not IOdmaDataContentElement dataContentElement)
        {
            return null;
        }

        if (!IsTextLike(document, dataContentElement))
        {
            return null;
        }

        var content = dataContentElement.Content;
        if (content is null)
        {
            return null;
        }

        await using var stream = content.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private static bool IsTextLike(IOdmaDocument document, IOdmaDataContentElement contentElement)
    {
        var contentType = document.CombinedContentType ?? string.Empty;
        var fileName = contentElement.FileName ?? document.Title ?? string.Empty;

        return contentType.Contains("text/", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("markdown", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
    }
}
