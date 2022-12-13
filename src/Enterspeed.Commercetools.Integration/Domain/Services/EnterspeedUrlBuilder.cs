using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Services;

namespace Enterspeed.Commercetools.Integration.Domain.Services;

public class EnterspeedUrlBuilder : IEnterspeedUrlBuilder
{
    public Task<Dictionary<string, string>> BuildLocalizedUrlsAsync(ICategory category, List<ICategoryReference>? ancestors = null)
    {
        // Loop through all languages with a slug
        return Task.FromResult(category.Slug.ToDictionary(
                k => k.Key,
                v => BuildCategoryUrl(
                    category,
                    ancestors?
                        .Select(x => x.Obj)
                        .Where(x => x != null)
                        .ToList(),
                    v.Key)));
    }

    public Task<Dictionary<string, string>> BuildLocalizedUrlsAsync(IProductProjection product)
    {
        var primaryCategory = product.Categories.FirstOrDefault();

        // Loop through all languages with a slug
        return Task.FromResult(product.Slug
                .ToDictionary(
                    k => k.Key,
                    v =>
                    {
                        var ancestors = primaryCategory?.Obj?.Ancestors
                            ?.Select(a => a.Obj)
                            .Where(c => c != null)
                            .ToList();

                        var categoryUrl = BuildCategoryUrl(primaryCategory?.Obj, ancestors, v.Key);

                        var productSlug = product.Slug[v.Key];

                        return $"{categoryUrl}/{productSlug}/";
                    }));
    }

    private static string BuildCategoryUrl(ICategory? category, IList<ICategory>? ancestors, string locale)
    {
        if (category == null)
        {
            return string.Empty;
        }

        var slugs = new List<string>();

        // Add each category slug upwards in the hierarchy
        var currentCategory = category;
        do
        {
            if (currentCategory.Slug.TryGetValue(locale, out var slug))
            {
                slugs.Add(slug);
            }

            currentCategory = ancestors?.FirstOrDefault(x => x.Id == currentCategory.Parent?.Id);
        }
        while (currentCategory != null);

        // Reverse the slugs to drill down instead of up
        slugs.Reverse();

        var path = $"/{string.Join("/", slugs)}";

        return path;
    }
}