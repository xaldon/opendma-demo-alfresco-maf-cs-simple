using System.Text.RegularExpressions;
using Microsoft.Agents.AI;
using OpenDMA.Api;

namespace OpenDma.Demo.Alfresco.Maf.Simple;

internal sealed partial class AlfrescoTextSearchProvider(
    AppSettings settings,
    OpenDmaSessionFactory sessionFactory,
    AftsQueryBuilder queryBuilder,
    MarkdownContentReader contentReader,
    AlfrescoLinkBuilder linkBuilder)
{
    public async Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken)
    {
        var terms = queryBuilder.ExtractTerms(query);
        var aftsQuery = queryBuilder.Build(query, settings.AlfrescoSite);
        var results = new List<TextSearchProvider.TextSearchResult>();

        Console.WriteLine();
        Console.WriteLine($"> Search terms: {string.Join(' ', terms)}");

        if (aftsQuery is null)
        {
            Console.WriteLine("> AFTS query: skipped (no usable search terms).");
            Console.WriteLine("> Found 0 documents.");
            return results;
        }

        Console.WriteLine($"> AFTS query: {aftsQuery}");

        var session = sessionFactory.Connect();

        try
        {
            var searchResult = session.Search(
                new OdmaId(settings.OpenDmaRepositoryId),
                OdmaQName.FromString("alfresco:afts"),
                aftsQuery);

            var documents = searchResult.GetObjects().OfType<IOdmaDocument>().ToList();
            Console.WriteLine($"> Found {documents.Count} documents.");

            foreach (var document in documents)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var markdown = await contentReader.ReadMarkdownAsync(document, cancellationToken);
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    continue;
                }

                var excerpt = ExtractExcerpt(markdown, terms, settings.MaxExcerptLength);
                if (string.IsNullOrWhiteSpace(excerpt))
                {
                    continue;
                }

                results.Add(new TextSearchProvider.TextSearchResult
                {
                    Text = excerpt,
                    SourceName = document.Title ?? document.Id.ToString(),
                    SourceLink = linkBuilder.BuildDocumentLink(document.Id),
                    RawRepresentation = new
                    {
                        objectId = document.Id.ToString(),
                        contentType = document.CombinedContentType,
                        checkedOut = document.CheckedOut,
                        aftsQuery,
                    },
                });

                if (results.Count >= settings.MaxSearchResults)
                {
                    break;
                }
            }
        }
        finally
        {
            session.Close();
        }

        return results;
    }

    private static string ExtractExcerpt(string markdown, IReadOnlyCollection<string> terms, int maxLength)
    {
        var normalized = NewLineRegex().Replace(markdown, "\n").Trim();
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        var index = FindFirstTermIndex(normalized, terms);
        if (index < 0)
        {
            return normalized[..maxLength].Trim();
        }

        var start = Math.Max(0, index - maxLength / 3);
        var end = Math.Min(normalized.Length, start + maxLength);
        start = Math.Max(0, end - maxLength);

        var excerpt = normalized[start..end].Trim();
        if (start > 0)
        {
            excerpt = "..." + excerpt;
        }

        if (end < normalized.Length)
        {
            excerpt += "...";
        }

        return excerpt;
    }

    private static int FindFirstTermIndex(string text, IEnumerable<string> terms)
    {
        var first = -1;

        foreach (var term in terms)
        {
            var index = text.IndexOf(term, StringComparison.OrdinalIgnoreCase);
            if (index >= 0 && (first < 0 || index < first))
            {
                first = index;
            }
        }

        return first;
    }

    [GeneratedRegex(@"\r\n?")]
    private static partial Regex NewLineRegex();
}
