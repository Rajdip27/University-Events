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
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken ct);
    Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken ct);
    Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm categoryVm, CancellationToken ct);
    Task<bool> DeleteCategoryAsync(long id, CancellationToken ct);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly UniversityDbContext _context;
    private readonly IAppLogger<CategoryRepository> _logger;
    private readonly IRedisCacheService _redis;

    public CategoryRepository(
        UniversityDbContext context,
        IAppLogger<CategoryRepository> logger,
        IRedisCacheService redis)
    {
        _context = context;
        _logger = logger;
        _redis = redis;
    }

    #region CreateOrUpdate
    public async Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm vm, CancellationToken ct)
    {
        var isUpdate = vm.Id > 0;
        var category = isUpdate
            ? await _context.Categories.FirstOrDefaultAsync(c => c.Id == vm.Id, ct)
            : null;

        if (isUpdate && category is null) return null;

        category ??= vm.Adapt<Category>();
        category.Name = vm.Name;
        category.Description = vm.Description;

        if (isUpdate)
            _context.Categories.Update(category);
        else
            await _context.Categories.AddAsync(category, ct);

        await _context.SaveChangesAsync(ct);

        var categoryVm = category.Adapt<CategoryVm>();
        var itemCacheKey = $"categories:id={category.Id}";

        // Cache single item
        await _redis.SetDataAsync(itemCacheKey, categoryVm, ct);

        // Invalidate list caches
        await _redis.RemoveByPatternAsync("categories:search:", ct);

        _logger.LogInfo(isUpdate
            ? $"Updated Category Id={category.Id}"
            : $"Created Category Id={category.Id}");

        return categoryVm;
    }
    #endregion

    #region Delete
    public async Task<bool> DeleteCategoryAsync(long id, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null) return false;

        category.IsDelete = true;
        await _context.SaveChangesAsync(ct);

        // Remove caches
        await _redis.RemoveDataAsync($"categories:id={id}", ct);
        await _redis.RemoveByPatternAsync("categories:search:", ct);

        _logger.LogInfo($"Deleted Category Id={id}");
        return true;
    }
    #endregion

    #region Get
    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken ct)
    {
        var listCacheKey = $"categories:search:page={filter.Page}&size={filter.PageSize}";
        var cachedList = await _redis.GetDataAsync<PaginationModel<CategoryVm>>(listCacheKey, ct);

        if (cachedList != null)
        {
            _logger.LogInfo($"[GetCategoriesAsync] Fetched {cachedList.Items.Count()} categories from cache.");
            return cachedList;
        }

        var query = _context.Categories.AsNoTracking()
            .Where(c => !c.IsDelete);

        query = SpecificationEvaluator<Category>.GetQuery(query, new CategorySpecification(filter));

        var result = await query
            .ProjectToType<CategoryVm>()
            .ToPagedListAsync(filter.Page, filter.PageSize);

        // Cache the list
        await _redis.SetDataAsync(listCacheKey, result, ct);

        // Also cache each single item for direct lookup
        await Task.WhenAll(
     result.Items.Select(cat =>
     {
         var itemKey = $"categories:id={cat.Id}";
         return _redis.SetDataAsync(itemKey, cat, ct);
     })
 );

        _logger.LogInfo($"[GetCategoriesAsync] Fetched {result.Items.Count()} categories from DB.");
        return result;
    }

    public async Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken ct)
    {
        var itemCacheKey = $"categories:id={id}";
        var cached = await _redis.GetDataAsync<CategoryVm>(itemCacheKey, ct);
        if (cached != null) return cached;

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete, ct);

        if (category is null) return null;

        var categoryVm = category.Adapt<CategoryVm>();

        // Cache single item
        await _redis.SetDataAsync(itemCacheKey, categoryVm, ct);

        return categoryVm;
    }
    #endregion
}
