using commercetools.Sdk.Api.Models.Common;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Extensions;

public static class LocalizedStringExtensions
{
    internal static IEnterspeedProperty ToEnterspeedProperty(this IDictionary<string, string> me)
    {
        return new ObjectEnterspeedProperty(me
            .ToDictionary(
                x => x.Key.Replace('-', '_'), // Enterspeed property name can't contain hyphen, replace with underscore.
                x => new StringEnterspeedProperty(x.Value) as IEnterspeedProperty));
    }
}