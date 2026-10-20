using OpenDMA.Api;
using OpenDMA.Remote;

namespace OpenDma.Demo.Alfresco.Maf.Simple;

internal sealed class OpenDmaSessionFactory(AppSettings settings)
{
    public IOdmaSession Connect()
    {
        return RemoteSessionFactory.Connect(
            settings.OpenDmaEndpoint,
            settings.OpenDmaUsername,
            settings.OpenDmaPassword);
    }
}
