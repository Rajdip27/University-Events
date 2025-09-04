using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Expressions;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ModelSpecification;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface ICategoryRepository
{
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly UniversityDbContext _context;

    public CategoryRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter)
    {
        try
        {
            var spec = new CategorySpecification(filter);
            var query = SpecificationEvaluator<Category>.GetQuery(
                _context.Categories.AsNoTracking(),
                spec
            );
            var projected = query.ProjectToType<CategoryVm>();
            return await projected.ToPagedListAsync(filter.Page, filter.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving categories: {ex.Message}");
            throw;
        }
       
    }
}
