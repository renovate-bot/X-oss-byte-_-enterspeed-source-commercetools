using System;
using System.Threading.Tasks;
using Enterspeed.Commercetools.Integration.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.AzureFunctions.Functions
{
    public class ImportAllFunction
    {
        private readonly IEnterspeedImportAllCategories _enterspeedImportAllCategories;
        private readonly IEnterspeedImportAllProducts _enterspeedImportAllProducts;
        private readonly ILogger<ImportAllFunction> _logger;

        public ImportAllFunction(
            IEnterspeedImportAllCategories enterspeedImportAllCategories,
            IEnterspeedImportAllProducts enterspeedImportAllProducts,
            ILogger<ImportAllFunction> logger)
        {
            _enterspeedImportAllCategories = enterspeedImportAllCategories;
            _enterspeedImportAllProducts = enterspeedImportAllProducts;
            _logger = logger;
        }

        [Function(nameof(ImportAllFunction))]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
            ILogger log)
        {
            _logger.LogDebug("Importing all products and categories");
            var categoriesTask = _enterspeedImportAllCategories.ImportAllCategoriesAsync();
            var productTask = _enterspeedImportAllProducts.ImportAllProductsAsync();

            await Task.WhenAll(categoriesTask, productTask);
        }
    }
}
