using commercetools.Sdk.Api.Models.Categories;
using Enterspeed.Commercetools.Integration.Api.Models;

namespace Enterspeed.Commercetools.Integration.Api.Services
{
    public interface IEnterspeedImportAllCategories
    {
        Task<ImportAllResultModel> ImportAllCategoriesAsync();
    }
}
