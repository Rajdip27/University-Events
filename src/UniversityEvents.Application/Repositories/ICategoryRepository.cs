using System.Diagnostics;
using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Expressions;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.ModelSpecification;
using UniversityEvents.Application.Utilities;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UniversityEvents.Application.Repositories;

public interface ICategoryRepository
{
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter);
    Task<CategoryVm> GetCategoryByIdAsync(long id,CancellationToken cancellationToken);
}

public class CategoryRepository(UniversityDbContext context, IAppLogger<CategoryRepository> logger) : ICategoryRepository
{

    public async Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter)
    {
        try
        {
            logger.LogInfo($"Fetching categories. Search: {filter.Search}, Page: {filter.Page}, PageSize: {filter.PageSize}");

            var spec = new CategorySpecification(filter);
            var query = SpecificationEvaluator<Category>.GetQuery(
                context.Categories.AsNoTracking(),
                spec
            );

            var projected = query.ProjectToType<CategoryVm>();
            var result = await projected.ToPagedListAsync(filter.Page, filter.PageSize);

            logger.LogInfo($"Fetched {result.Items.Count()} categories successfully.");
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while retrieving categories.", ex);
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
