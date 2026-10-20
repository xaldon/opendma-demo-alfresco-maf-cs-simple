using OpenDMA.Api;

namespace OpenDma.Demo.Alfresco.Maf.Simple;

internal sealed class AlfrescoLinkBuilder(AppSettings settings)
{
    public string? BuildDocumentLink(OdmaId objectId)
    {
        if (string.IsNullOrWhiteSpace(settings.AlfrescoShareUrl))
        {
            return null;
        }

        var baseUrl = settings.AlfrescoShareUrl.TrimEnd('/');
        var id = objectId.ToString();

        if (id.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
        {
            id = id["node:".Length..];
        }

        return $"{baseUrl}/page/document-details?nodeRef=workspace://SpacesStore/{Uri.EscapeDataString(id)}";
    }
}
