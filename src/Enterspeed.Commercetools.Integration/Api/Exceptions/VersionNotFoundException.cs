namespace Enterspeed.Commercetools.Integration.Api.Exceptions;

public class VersionNotFoundException : Exception
{
    public VersionNotFoundException(string entityName, long actualVersion, long expectedVersion)
        : base($"Version not fund: {entityName} has version {actualVersion}, expected version was {expectedVersion}.")
    {
    }
}