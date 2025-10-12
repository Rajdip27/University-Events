using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.Caching;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Expressions;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Helpers;
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
    private readonly IRedisCacheHelper _cacheHelper;

    public CategoryRepository(UniversityDbContext context, IRedisCacheHelper cacheHelper)
    {
        _context = context;
        _cacheHelper = cacheHelper;
    }

    private string ItemKey(long id) => $"categories:id={id}";
    private string ListPattern => "categories:search:";

    // Create or Update
    public async Task<CategoryVm> CreateOrUpdateCategoryAsync(CategoryVm vm, CancellationToken ct)
    {
        var category = vm.Id > 0
            ? await _context.Categories.FirstOrDefaultAsync(c => c.Id == vm.Id, ct)
            : new Category();

        if (vm.Id > 0 && category == null) return null;

        category.Name = vm.Name;
        category.Description = vm.Description;

        if (vm.Id > 0) _context.Categories.Update(category);
        else await _context.Categories.AddAsync(category, ct);

        await _context.SaveChangesAsync(ct);

        var vmUpdated = category.Adapt<CategoryVm>();

        // ✅ Update Redis instantly
        await _cacheHelper.UpdateItemAndInvalidateListAsync(ItemKey(vmUpdated.Id), vmUpdated, ListPattern, ct);

        return vmUpdated;
    }

    // Delete
    public async Task<bool> DeleteCategoryAsync(long id, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null) return false;

        category.IsDelete = true;
        await _context.SaveChangesAsync(ct);

        // ✅ Remove from Redis immediately
        await _cacheHelper.RemoveItemAndInvalidateListAsync(ItemKey(id), ListPattern, ct);

        return true;
    }

    // Get by Id
    public async Task<CategoryVm> GetCategoryByIdAsync(long id, CancellationToken ct)
    {
        var cached = await _cacheHelper.GetAsync<CategoryVm>(ItemKey(id), ct);
        if (cached != null) return cached;

        var category = await _context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete, ct);
        if (category == null) return null;

        var vm = category.Adapt<CategoryVm>();
        await _cacheHelper.SetAsync(ItemKey(id), vm, ct); // set in Redis

        return vm;
    }

    // Get paginated list
    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter, CancellationToken ct)
    {
        var listKey = $"{ListPattern}page={filter.Page}&size={filter.PageSize}";
        var cached = await _cacheHelper.GetAsync<PaginationModel<CategoryVm>>(listKey, ct);
        if (cached != null) return cached;

        var query = _context.Categories.AsNoTracking().Where(c => !c.IsDelete);
        query = SpecificationEvaluator<Category>.GetQuery(query, new CategorySpecification(filter));

        var result = await query.ProjectToType<CategoryVm>()
            .ToPagedListAsync(filter.Page, filter.PageSize);

        // ✅ Store list and individual items in Redis
        await _cacheHelper.SetAsync(listKey, result, ct);
        foreach (var cat in result.Items)
            await _cacheHelper.SetAsync(ItemKey(cat.Id), cat, ct);

        return result;
    }
}
