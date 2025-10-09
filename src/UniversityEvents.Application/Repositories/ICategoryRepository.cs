using Mapster;
using Microsoft.EntityFrameworkCore;
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
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken cancellationToken);
    Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken cancellationToken);
    Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm categoryVm, CancellationToken cancellationToken);
    Task<bool> DeleteCategoryAsync(long id, CancellationToken cancellationToken);
}

public class CategoryRepository(
    UniversityDbContext context,
    IAppLogger<CategoryRepository> logger,
    IRedisCacheService redisCacheService) : ICategoryRepository
{
    private readonly UniversityDbContext _context = context;
    private readonly IAppLogger<CategoryRepository> _logger = logger;
    private readonly IRedisCacheService _redis = redisCacheService;

    #region CreateOrUpdate
    public async Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm vm, CancellationToken ct)
    {
        bool isUpdate = vm.Id > 0;

        var category = isUpdate
            ? await _context.Categories.FirstOrDefaultAsync(c => c.Id == vm.Id, ct)
            : null;

        if (isUpdate && category is null)
            return null;

        category ??= vm.Adapt<Category>();
        category.Name = vm.Name;
        category.Description = vm.Description;

        if (isUpdate)
            _context.Categories.Update(category);
        else
            await _context.Categories.AddAsync(category, ct);

        await _context.SaveChangesAsync(ct);

        var resultVm = category.Adapt<CategoryVm>();
        var cacheKey = $"categories:id={category.Id}";

        // Update single item cache
        await _redis.SetDataAsync(cacheKey, resultVm, ct);

        // Invalidate list cache (all search pages)
        await _redis.RemoveByPatternAsync("categories:search:", ct);

        _logger.LogInfo(isUpdate
            ? $"Updated Category Id={category.Id}"
            : $"Created Category Id={category.Id}");

        return resultVm;
    }
    #endregion

    #region Delete
    public async Task<bool> DeleteCategoryAsync(long id, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return false;

        category.IsDelete = true;
        await _context.SaveChangesAsync(ct);

        // Remove single item cache
        await _redis.RemoveDataAsync($"categories:id={id}", ct);

        // Remove all list caches
        await _redis.RemoveByPatternAsync("categories:search:", ct);

        _logger.LogInfo($"Deleted Category Id={id}");
        return true;
    }
    #endregion

    #region Get
    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken ct)
    {
        // Always fetch real-time data from DB to prevent stale cache issues
        var query = _context.Categories.AsNoTracking()
            .Where(c => !c.IsDelete);

        query = SpecificationEvaluator<Category>.GetQuery(query, new CategorySpecification(filter));

        var result = await query
            .ProjectToType<CategoryVm>()
            .ToPagedListAsync(filter.Page, filter.PageSize);

        _logger.LogInfo($"[GetCategoriesAsync] Fetched {result.Items.Count()} categories from DB.");

        return result;
    }

    public async Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken ct)
    {
        var cacheKey = $"categories:id={id}";
        var cached = await _redis.GetDataAsync<CategoryVm>(cacheKey, ct);

        if (cached != null)
            return cached;

        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => !c.IsDelete)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (category is null)
            return null;

        var categoryVm = category.Adapt<CategoryVm>();

        // Cache single item
        await _redis.SetDataAsync(cacheKey, categoryVm, ct);

        return categoryVm;
    }
    #endregion
}
