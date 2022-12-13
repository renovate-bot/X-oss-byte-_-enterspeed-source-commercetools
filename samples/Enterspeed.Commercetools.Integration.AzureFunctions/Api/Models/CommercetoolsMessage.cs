namespace Enterspeed.Commercetools.Integration.AzureFunctions.Api.Models;

public class CommercetoolsMessage
{
    public CommercetoolsMessage(CommercetoolsResource resource, int resourceVersion)
    {
        Resource = resource;
        ResourceVersion = resourceVersion;
    }

    public CommercetoolsResource Resource { get; set; }
    public int ResourceVersion { get; set; }

    public class CommercetoolsResource
    {
        public string Id { get; set; }
    }
}