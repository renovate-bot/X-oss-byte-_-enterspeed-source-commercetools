namespace Enterspeed.Commercetools.Integration.Api.Mappers;

public interface IMapper<in TSource, TDest>
{
    Task<TDest> MapAsync(TSource source);
}