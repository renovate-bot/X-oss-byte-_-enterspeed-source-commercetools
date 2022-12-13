using System.Net;
using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Models;
using Enterspeed.Commercetools.Integration.Api.Repositories;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.Domain.Services
{
    public class InMemoryImportAllProducts : IEnterspeedImportAllProducts
    {
        private readonly ICommercetoolsRepository _commercetoolsRepository;
        private readonly IMapper<IProductProjection, EnterspeedProductEntity> _productMapper;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly ILogger<InMemoryImportAllProducts> _logger;

        public InMemoryImportAllProducts(
            ICommercetoolsRepository commercetoolsRepository,
            IMapper<IProductProjection, EnterspeedProductEntity> productMapper,
            IEnterspeedIngestService enterspeedIngestService,
            ILogger<InMemoryImportAllProducts> logger)
        {
            _commercetoolsRepository = commercetoolsRepository;
            _productMapper = productMapper;
            _enterspeedIngestService = enterspeedIngestService;
            _logger = logger;
        }

        public async Task<ImportAllResultModel> ImportAllProductsAsync()
        {
            var products = await _commercetoolsRepository.GetAllProductsAsync();
            var productTasks = products.Select(ImportProduct);

            var errorModels = await Task.WhenAll(productTasks);
            if (errorModels.Length > 0)
            {
                return new ImportAllResultModel
                {
                    Errors = errorModels.ToList()
                };
            }

            return new ImportAllResultModel();
        }

        private async Task<ErrorModel> ImportProduct(IProductProjection product)
        {
            var enterspeedProductModel = await _productMapper.MapAsync(product);
            return SaveEntity(enterspeedProductModel);
        }
        private ErrorModel SaveEntity(IEnterspeedEntity entity)
        {
            try
            {
                // Save entity to Enterspeed
                var response = _enterspeedIngestService.Save(entity);
                if (response.Status != HttpStatusCode.OK)
                {
                    return new ErrorModel()
                    {
                        Id = entity.Id,
                        Exception = response.Exception
                    };
                }
                return new ErrorModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed importing entity with id: {EntityId} to Enterspeed", entity.Id);
                return new ErrorModel
                {
                    Id = entity.Id,
                    Exception = ex
                };
            }
        }
    }
}
