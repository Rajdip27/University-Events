using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using UniversityEvents.Application.Caching;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Expressions;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.ModelSpecification;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// Gets the categories asynchronous.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken cancellationToken);
    /// <summary>
    /// Gets the category by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<CategoryVm> GetCategoryByIdAsync(long id,CancellationToken cancellationToken);
}

public class CategoryRepository(UniversityDbContext context, IAppLogger<CategoryRepository> logger, IRedisCacheService redisCacheService) : ICategoryRepository
{
    /// <summary>
    /// Gets the categories asynchronous.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken cancellationToken)
    {
        var cacheKey = $"categories:search={filter.Search ?? "all"}:page={filter.Page}:size={filter.PageSize}";
        try
        {
            logger.LogInfo($"[GetCategoriesAsync] Search='{filter.Search}', Page={filter.Page}, PageSize={filter.PageSize}");

            // Try cache first
            var cached = await redisCacheService.GetDataAsync<PaginationModel<CategoryVm>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                logger.LogInfo($"[GetCategoriesAsync] Cache hit: {cached.Items.Count()} categories.");
                return cached;
            }

            // Build query and project
            var spec = new CategorySpecification(filter);
            var query = SpecificationEvaluator<Category>.GetQuery(context.Categories.AsNoTracking(), spec);
            var result = await query.ProjectToType<CategoryVm>().ToPagedListAsync(filter.Page, filter.PageSize);

            logger.LogInfo($"[GetCategoriesAsync] DB hit: {result.Items.Count()} categories.");

            // Save to cache
            await redisCacheService.SetDataAsync(cacheKey, result, cancellationToken);
            logger.LogInfo($"[GetCategoriesAsync] Cached result for key: {cacheKey}");

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("[GetCategoriesAsync] Failed to retrieve categories.", ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the category by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInfo($"Fetching category by Id: {id}");
            // Using AsNoTracking for read-only operation
            var category = await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            // Handle case where category is not found
            if (category == null)
            {
                logger.LogWarning($"Category with Id {id} not found.");
                return null; 
            }
            // Map to ViewModel
            var categoryVm = category.Adapt<CategoryVm>();
            logger.LogInfo($"Successfully fetched category: {categoryVm.Name} (Id: {categoryVm.Id})");

            return categoryVm;
        }
        catch (Exception ex)
        {

            logger.LogError("An error occurred while retrieving categories.", ex);
            throw;
        }
        
    }
}
