using System.Net;
using commercetools.Sdk.Api.Models.Categories;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Models;
using Enterspeed.Commercetools.Integration.Api.Repositories;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.Domain.Services
{
    public class InMemoryImportAllCategories : IEnterspeedImportAllCategories
    {
        private readonly ICommercetoolsRepository _commercetoolsRepository;
        private readonly IMapper<ICategory, EnterspeedCategoryEntity> _categoryMapper;
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly ILogger<InMemoryImportAllCategories> _logger;

        public InMemoryImportAllCategories(
            ICommercetoolsRepository commercetoolsRepository,
            IMapper<ICategory, EnterspeedCategoryEntity> categoryMapper,
            IEnterspeedIngestService enterspeedIngestService,
            ILogger<InMemoryImportAllCategories> logger)
        {
            _commercetoolsRepository = commercetoolsRepository;
            _categoryMapper = categoryMapper;
            _enterspeedIngestService = enterspeedIngestService;
            _logger = logger;
        }

        public async Task<ImportAllResultModel> ImportAllCategoriesAsync()
        {
            var categories = await _commercetoolsRepository.GetAllCategoriesAsync();
            var categoryTasks = categories.Select(ImportCategory);

            var errorModels = await Task.WhenAll(categoryTasks);
            if (errorModels.Length > 0)
            {
                return new ImportAllResultModel()
                {
                    Errors = errorModels.ToList()
                };
            }

            return new ImportAllResultModel();
        }

        private async Task<ErrorModel> ImportCategory(ICategory category)
        {
            var enterspeedCategoryModel = await _categoryMapper.MapAsync(category);
            return SaveEntity(enterspeedCategoryModel);
        }

        private ErrorModel SaveEntity(IEnterspeedEntity entity)
        {
            try
            {
                // Save entity to Enterspeed
                var response = _enterspeedIngestService.Save(entity);
                if (response.Status != HttpStatusCode.OK)
                {
                    return new ErrorModel
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
