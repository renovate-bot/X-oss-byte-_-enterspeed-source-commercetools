using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Exceptions;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Models;
using Enterspeed.Commercetools.Integration.Api.Repositories;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.Domain.Models;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.Domain.Services;

public class EnterspeedImportService : IEnterspeedImportService
{
    private readonly ICommercetoolsRepository _commercetoolsRepository;
    private readonly IEnterspeedIngestService _enterspeedIngestService;
    private readonly ILogger<EnterspeedImportService> _logger;
    private readonly IMapper<IProductProjection, EnterspeedProductEntity> _productMapper;
    private readonly IMapper<ICategory, EnterspeedCategoryEntity> _categoryMapper;
    private readonly IMapper<ProductVariantMappingContext, EnterspeedProductVariantEntity> _variantMapper;

    public EnterspeedImportService(
        ICommercetoolsRepository commercetoolsRepository,
        IEnterspeedIngestService enterspeedIngestService,
        ILogger<EnterspeedImportService> logger,
        IMapper<IProductProjection, EnterspeedProductEntity> productMapper,
        IMapper<ICategory, EnterspeedCategoryEntity> categoryMapper,
        IMapper<ProductVariantMappingContext, EnterspeedProductVariantEntity> variantMapper)
    {
        _commercetoolsRepository = commercetoolsRepository;
        _enterspeedIngestService = enterspeedIngestService;
        _logger = logger;
        _productMapper = productMapper;
        _categoryMapper = categoryMapper;
        _variantMapper = variantMapper;
    }

    public async Task ImportProductAsync(string productId, long version)
    {
        var product = await _commercetoolsRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            // Product was deleted in commercetools, so delete it in Enterspeed as well
            DeleteEntity(productId);
            
            // We should delete variants here, but there is no way to query Enterspeed to fetch the old product, so we can clean up the variants.
            return;
        }

        if (product.Version < version)
        {
            // The requested version isn't published by Commercetools yet
            throw new VersionNotFoundException("ProductProjection", product.Version, version);
        }

        // Map Commercetools product to Enterspeed model
        var enterspeedProductModel = await _productMapper.MapAsync(product);

        // Save product in Enterspeed
        SaveEntity(enterspeedProductModel);

        // Map variants
        var variants = new List<IProductVariant>(product.Variants)
        {
            product.MasterVariant
        };

        var enterspeedVariantModels = await Task.WhenAll(variants.Select(x => _variantMapper.MapAsync(new ProductVariantMappingContext(product, x))));

        // Save variants
        foreach (var variant in enterspeedVariantModels)
        {
            SaveEntity(variant);
        }

    }

    public async Task ImportCategoryAsync(string categoryId, long version)
    {
        var category = await _commercetoolsRepository.GetCategoryByIdAsync(categoryId);
        if (category == null)
        {
            // Category was deleted in Commercetools, so delete it in Enterspeed as well
            DeleteEntity(categoryId);
            return;
        }

        if (category.Version < version)
        {
            // The requested version isn't published by Commercetools yet
            throw new VersionNotFoundException("Category", category.Version, version);
        }

        // Map Commercetools category to Enterspeed model
        var enterspeedCategoryModel = await _categoryMapper.MapAsync(category);

        // Save category in Enterspeed
        SaveEntity(enterspeedCategoryModel);
    }

    private void SaveEntity(IEnterspeedEntity entity)
    {
        try
        {
            // Save entity to Enterspeed
            var response = _enterspeedIngestService.Save(entity);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed importing entity with id: {EntityId} to Enterspeed", entity.Id);
            throw;
        }
    }

    private void DeleteEntity(string entityId)
    {
        try
        {
            // Delete entity from Enterspeed
            var response = _enterspeedIngestService.Delete(entityId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed deleting entity with id: {EntityId} from Enterspeed", entityId);
            throw;
        }
    }
}