using Enterspeed.Commercetools.Integration.Api.Models;

namespace Enterspeed.Commercetools.Integration.Api.Services
{
    public interface IEnterspeedImportAllProducts
    {
        Task<ImportAllResultModel> ImportAllProductsAsync();
    }
}
