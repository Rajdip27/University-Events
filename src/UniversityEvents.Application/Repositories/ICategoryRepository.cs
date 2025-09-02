using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface ICategoryRepository
{
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(string search, int page, int pageSize);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly UniversityDbContext _context;

    public CategoryRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(string search, int page = 1, int pageSize = 10)
    {
        var query = _context.Categories
                            .AsNoTracking()
                            .Where(x => !x.IsDelete)
                            .ProjectToType<CategoryVm>();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        return await query.OrderBy(p => p.Id)
                          .ToPagedListAsync(page, pageSize);
    }
}
