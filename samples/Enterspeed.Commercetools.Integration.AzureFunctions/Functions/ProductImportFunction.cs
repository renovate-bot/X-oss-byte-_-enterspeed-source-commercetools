using System.Threading.Tasks;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.AzureFunctions.Api.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.AzureFunctions.Functions;

public class ProductImportFunction
{
    private readonly IEnterspeedImportService _enterspeedImportService;
    private readonly ILogger<ProductImportFunction> _logger;

    public ProductImportFunction(
        IEnterspeedImportService enterspeedImportService,
        ILogger<ProductImportFunction> logger)
    {
        _enterspeedImportService = enterspeedImportService;
        _logger = logger;
    }

    [Function(nameof(ProductImportFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "commercetools-product-import",
            Connection = "ServiceBusConnectionString")]
        CommercetoolsMessage message)
    {
        _logger.LogDebug("Importing product with id {id}", message.Resource.Id);
        
        await _enterspeedImportService.ImportProductAsync(message.Resource.Id, message.ResourceVersion);
    }
}