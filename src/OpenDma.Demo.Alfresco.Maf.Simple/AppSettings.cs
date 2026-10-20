namespace OpenDma.Demo.Alfresco.Maf.Simple;

internal sealed class AppSettings
{
    public string OpenDmaEndpoint { get; init; } = "http://localhost:7070/opendma/alf";

    public string OpenDmaUsername { get; init; } = "admin";

    public string OpenDmaPassword { get; init; } = "admin";

    public string OpenDmaRepositoryId { get; init; } = "Alfresco";

    public string AlfrescoSite { get; init; } = "project-apollo";

    public string? AlfrescoShareUrl { get; init; } = "http://localhost:8080/share";

    public int MaxSearchResults { get; init; } = 8;

    public int MaxExcerptLength { get; init; } = 1500;

    public string OpenAiModel { get; init; } = "gpt-4o-mini";

    public string? OpenAiApiKey { get; init; }

    public static AppSettings FromEnvironment()
    {
        return new AppSettings
        {
            OpenDmaEndpoint = Get("OPENDMA_ENDPOINT", "http://localhost:7070/opendma/alf"),
            OpenDmaUsername = Get("OPENDMA_USERNAME", "admin"),
            OpenDmaPassword = Get("OPENDMA_PASSWORD", "admin"),
            OpenDmaRepositoryId = Get("OPENDMA_REPOSITORY_ID", "Alfresco"),
            AlfrescoSite = Get("ALFRESCO_SITE", "project-apollo"),
            AlfrescoShareUrl = GetNullable("ALFRESCO_SHARE_URL") ?? "http://localhost:8080/share",
            MaxSearchResults = GetInt("MAX_SEARCH_RESULTS", 8),
            MaxExcerptLength = GetInt("MAX_EXCERPT_LENGTH", 1500),
            OpenAiModel = Get("OPENAI_MODEL", "gpt-4o-mini"),
            OpenAiApiKey = GetNullable("OPENAI_API_KEY"),
        };
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(OpenAiApiKey))
        {
            throw new InvalidOperationException("OPENAI_API_KEY is required.");
        }
    }

    private static string Get(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    private static string? GetNullable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static int GetInt(string name, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return int.TryParse(value, out var parsed) ? parsed : defaultValue;
    }
}
