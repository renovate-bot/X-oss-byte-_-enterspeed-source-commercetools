using System.Threading.Tasks;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.AzureFunctions.Api.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.AzureFunctions.Functions;

public class CategoryImportFunction
{
    private readonly IEnterspeedImportService _enterspeedImportService;
    private readonly ILogger<CategoryImportFunction> _logger;

    public CategoryImportFunction(
        IEnterspeedImportService enterspeedImportService,
        ILogger<CategoryImportFunction> logger)
    {
        _enterspeedImportService = enterspeedImportService;
        _logger = logger;
    }

    [Function(nameof(CategoryImportFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            "commercetools-category-import",
            Connection = "ServiceBusConnectionString")]
        CommercetoolsMessage message)
    {
        _logger.LogDebug("Importing category with id {id}", message.Resource.Id);

        await _enterspeedImportService.ImportCategoryAsync(message.Resource.Id, message.ResourceVersion);
    }
}