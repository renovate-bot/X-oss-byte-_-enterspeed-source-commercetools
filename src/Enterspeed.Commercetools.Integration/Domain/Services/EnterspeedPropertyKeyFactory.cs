using Enterspeed.Commercetools.Integration.Api.Services;

namespace Enterspeed.Commercetools.Integration.Domain.Services;

public class EnterspeedPropertyKeyFactory : IEnterspeedPropertyKeyFactory
{
    public Task<string> CreatePropertyKeyAsync(string key)
    {
        // Enterspeed property name can't contain hyphen, replace with underscore.
        return Task.FromResult(key.Replace('-', '_'));
    }
}