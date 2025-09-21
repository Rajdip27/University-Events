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
    Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken cancellationToken);


    Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm categoryVm, CancellationToken cancellationToken);
}

public class CategoryRepository(UniversityDbContext context, IAppLogger<CategoryRepository> logger, IRedisCacheService redisCacheService) : ICategoryRepository
{
    /// <summary>
    /// Creates a new category or updates an existing category asynchronously based on the provided category view
    /// model.
    /// </summary>
    /// <param name="categoryVm">The category view model containing the details of the category to create or update. If the <c>Id</c>
    /// property is greater than 0, the method updates the existing category; otherwise, it creates a new category.
    /// Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CategoryVm"/>
    /// representing the created or updated category, or <see langword="null"/> if the category to update does not
    /// exist.</returns>
    public async Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm categoryVm, CancellationToken cancellationToken)
    {
        // Remove single category cache if exists
        if (categoryVm.Id > 0)
            await redisCacheService.RemoveDataAsync($"categories:id={categoryVm.Id}", cancellationToken);

        // Optionally remove all cached lists
        await redisCacheService.RemoveDataAsync("categories:search*", cancellationToken); // if pattern removal supported

        Category category;
        if (categoryVm.Id > 0)
        {
            category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryVm.Id, cancellationToken);
            if (category == null) return null;
            category.Name = categoryVm.Name;
            category.Description = categoryVm.Description;
            context.Categories.Update(category);
        }
        else
        {
            category = categoryVm.Adapt<Category>();
            context.Categories.Add(category);
        }

        await context.SaveChangesAsync(cancellationToken);

        var resultVm = category.Adapt<CategoryVm>();
        await redisCacheService.SetDataAsync($"categories:id={resultVm.Id}", resultVm, cancellationToken);
        logger.LogInfo(categoryVm.Id > 0
            ? $"Updated Category Id={resultVm.Id}"
            : $"Created Category Id={resultVm.Id}");

        return resultVm;
    }


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

