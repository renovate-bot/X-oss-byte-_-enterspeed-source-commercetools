namespace Enterspeed.Commercetools.Integration.Api.Services;

public interface IEnterspeedImportService
{
    Task ImportProductAsync(string productId, long version);
    Task ImportCategoryAsync(string categoryId, long version);
}