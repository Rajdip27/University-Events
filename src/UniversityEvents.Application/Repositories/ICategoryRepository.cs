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
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken cancellationToken);
    Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken cancellationToken);
    Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm categoryVm, CancellationToken cancellationToken);
    Task<bool> DeleteCategoryAsync(long id, CancellationToken cancellationToken);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly UniversityDbContext _context;
    private readonly IAppLogger<CategoryRepository> _logger;
    private readonly IRedisCacheService _redisCacheService;

    public CategoryRepository(UniversityDbContext context, IAppLogger<CategoryRepository> logger, IRedisCacheService redisCacheService)
        => (_context, _logger, _redisCacheService) = (context, logger, redisCacheService);

    public async Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm vm, CancellationToken ct)
    {
        bool isUpdate = vm.Id > 0;
        var category = isUpdate
            ? await _context.Categories.FirstOrDefaultAsync(c => c.Id == vm.Id, ct)
            : null;

        if (isUpdate && category is null) return null;

        category ??= vm.Adapt<Category>();
        category.Name = vm.Name;
        category.Description = vm.Description;

        if (isUpdate) _context.Categories.Update(category);
        else await _context.Categories.AddAsync(category, ct);

        await _context.SaveChangesAsync(ct);

        var resultVm = category.Adapt<CategoryVm>();
        var cacheKey = $"categories:id={category.Id}";

        if (isUpdate) await _redisCacheService.RemoveDataAsync(cacheKey, ct);
        await _redisCacheService.SetDataAsync(cacheKey, resultVm, ct);

        _logger.LogInfo(isUpdate
            ? $"Updated Category Id={category.Id}"
            : $"Created Category Id={category.Id}");

        return resultVm;
    }

    public async Task<bool> DeleteCategoryAsync(long id, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null) return false;

        category.IsDelete = true;
        await _context.SaveChangesAsync(ct);

        // Remove single item cache
        await _redisCacheService.RemoveDataAsync($"categories:id={id}", ct);

        // Remove all list caches
        await _redisCacheService.RemoveDataAsync("categories:search:*", ct);

        _logger.LogInfo($"Deleted Category Id={id}");
        return true;
    }


    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken ct)
    {
        var cacheKey = $"categories:search={filter.Search ?? "all"}:page={filter.Page}:size={filter.PageSize}";
        var cached = await _redisCacheService.GetDataAsync<PaginationModel<CategoryVm>>(cacheKey, ct);
        if (cached != null)
        {
            _logger.LogInfo($"[GetCategoriesAsync] Cache hit: {cached.Items.Count()} categories.");
            return cached;
        }
        var query = SpecificationEvaluator<Category>.GetQuery(_context.Categories.AsNoTracking(), new CategorySpecification(filter));
        var result = await query.ProjectToType<CategoryVm>().ToPagedListAsync(filter.Page, filter.PageSize);

        await _redisCacheService.SetDataAsync(cacheKey, result, ct);
        _logger.LogInfo($"[GetCategoriesAsync] Cache miss: Cached {result.Items.Count()} categories.");
        return result;
    }

    public async Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken ct)
    {
        var cacheKey = $"categories:id={id}";
        var cached = await _redisCacheService.GetDataAsync<CategoryVm>(cacheKey, ct);
        if (cached != null) return cached;

        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null) return null;

        var categoryVm = category.Adapt<CategoryVm>();
        await _redisCacheService.SetDataAsync(cacheKey, categoryVm, ct);
        return categoryVm;
    }
}
