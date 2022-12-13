using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Api.Models;

public class EnterspeedProductVariantEntity : IEnterspeedEntity
{
    public EnterspeedProductVariantEntity(string id, string type)
    {
        Id = id;
        Type = type;
    }

    public string Id { get; set; }
    public string Type { get; set; }
    public string? Url { get; set; }
    public string[]? Redirects { get; set; }
    public string? ParentId { get; set; }
    public IDictionary<string, IEnterspeedProperty> Properties { get; set; } = new Dictionary<string, IEnterspeedProperty>();
}