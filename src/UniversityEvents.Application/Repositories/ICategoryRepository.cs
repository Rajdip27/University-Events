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
using UniversityEvents.Application.Logging;

namespace UniversityEvents.Application.Repositories;

public interface ICategoryRepository
{
    Task<PaginationModel<CategoryVm>> GetCategoriesAsync(Filter filter);
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
}
