using commercetools.Base.Client;
using commercetools.Base.Client.Error;
using commercetools.Sdk.Api.Extensions;
using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Repositories;
using Enterspeed.Commercetools.Integration.Configuration;

namespace Enterspeed.Commercetools.Integration.Domain.Repositories;

public class CommercetoolsRepository : ICommercetoolsRepository
{
    private readonly IClient _client;
    private readonly EnterspeedCommercetoolsConfiguration _configuration;

    public CommercetoolsRepository(IClient client, EnterspeedCommercetoolsConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<ICategory?> GetCategoryByIdAsync(string categoryId)
    {
        try
        {
            var category = await _client
                .WithApi(_configuration.CommercetoolsProjectKey)
                .Categories()
                .WithId(categoryId)
                .Get()
                .WithExpand("ancestors[*]")
                .ExecuteAsync();

            return category;
        }
        catch (NotFoundException)
        {
            return null;
        }
    }

    public async Task<IProductProjection?> GetProductByIdAsync(string productId)
    {
        try
        {
            var product = await _client
                .WithApi(_configuration.CommercetoolsProjectKey)
                .ProductProjections()
                .WithId(productId)
                .Get()
                .WithExpand("productType")
                .WithExpand("categories[*].ancestors[*]")
                .ExecuteAsync();

            return product;
        }
        catch (NotFoundException)
        {
            return null;
        }
    }

    public async Task<List<IProductProjection>> GetAllProductsAsync()
    {
        var canContinue = true;
        var results = new List<IProductProjection>();
        var limit = 500;
        string? lastId = null;

        while (canContinue)
        {
            var responseResult = new List<IProductProjection>();

            if (lastId == null)
            {
                var response = await _client
                    .WithApi(_configuration.CommercetoolsProjectKey)
                    .ProductProjections()
                    .Get()
                    .WithExpand("productType")
                    .WithExpand("categories[*].ancestors[*]")
                    .WithWithTotal(false)
                    .WithSort("id asc")
                    .WithLimit(limit)
                    .ExecuteAsync();

                responseResult = response.Results;
            }
            else
            {
                var response = await _client
                    .WithApi(_configuration.CommercetoolsProjectKey)
                    .ProductProjections()
                    .Get()
                    .WithExpand("productType")
                    .WithExpand("categories[*].ancestors[*]")
                    .WithWithTotal(false)
                    .WithSort("id asc")
                    .WithLimit(limit)
                    .WithWhere($"id > \"{lastId}\"")
                    .ExecuteAsync();

                responseResult.AddRange(response.Results);
            }

            canContinue = responseResult.Count == limit;
            lastId = responseResult.LastOrDefault()?.Id;
            results.AddRange(responseResult);
        }

        return results;
        
    }

    public async Task<List<ICategory>> GetAllCategoriesAsync()
    {
        var canContinue = true;
        var results = new List<ICategory>();
        var limit = 500;
        string? lastId = null;

        while (canContinue)
        {
            var responseResult = new List<ICategory>();

            if (lastId == null)
            {
                var response = await _client
                    .WithApi(_configuration.CommercetoolsProjectKey)
                    .Categories()
                    .Get()
                    .WithWithTotal(false)
                    .WithSort("id asc")
                    .WithLimit(limit)
                    .ExecuteAsync();

                responseResult = response.Results;
            }
            else
            {
                var response = await _client
                    .WithApi(_configuration.CommercetoolsProjectKey)
                    .Categories()
                    .Get()
                    .WithWithTotal(false)
                    .WithLimit(limit)
                    .ExecuteAsync();

                responseResult.AddRange(response.Results);
            }

            canContinue = responseResult.Count == limit;
            lastId = responseResult.LastOrDefault()?.Id;
            results.AddRange(responseResult);
        }

        return results;
        
    }
}