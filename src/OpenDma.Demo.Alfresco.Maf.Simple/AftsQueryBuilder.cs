using System.Text;
using System.Text.RegularExpressions;

namespace OpenDma.Demo.Alfresco.Maf.Simple;

internal sealed class AftsQueryBuilder
{
    private static readonly Regex TokenCharacters = new(@"[\p{L}\p{N}_\-]+", RegexOptions.Compiled);
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "about", "alfresco", "and", "are", "between", "could", "does", "for", "from",
        "how", "into", "say", "says", "should", "that", "the", "this", "was", "what",
        "when", "where", "which", "who", "why", "with", "would",
    };

    public string? Build(string input, string? site)
    {
        var terms = ExtractTerms(input);
        if (terms.Length == 0)
        {
            return null;
        }

        var query = new StringBuilder();
        query.Append("TEXT:(");

        for (var index = 0; index < terms.Length; index++)
        {
            if (index > 0)
            {
                query.Append(" AND ");
            }

            query.Append('"');
            query.Append(EscapeQuotedValue(terms[index]));
            query.Append('"');
        }

        query.Append(')');

        if (!string.IsNullOrWhiteSpace(site))
        {
            query.Append(" AND SITE:\"");
            query.Append(EscapeQuotedValue(site));
            query.Append('"');
        }

        return query.ToString();
    }

    public string[] ExtractTerms(string input)
    {
        return TokenCharacters
            .Matches(input)
            .Select(match => match.Value.Trim())
            .Where(term => term.Length > 2 && !StopWords.Contains(term))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToArray();
    }

    private static string EscapeQuotedValue(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
    }
}
