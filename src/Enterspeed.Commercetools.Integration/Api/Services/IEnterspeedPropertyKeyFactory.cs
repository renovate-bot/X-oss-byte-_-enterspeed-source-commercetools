namespace Enterspeed.Commercetools.Integration.Api.Services;

public interface IEnterspeedPropertyKeyFactory
{
    Task<string> CreatePropertyKeyAsync(string key);
}