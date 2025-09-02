using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Application.Repositories;

public interface ICategoryRepository
{
    Task<CategoryVm>GetAllCategoryAsync(CancellationToken cancellationToken = default);
}
public class CategoryRepository : ICategoryRepository
{
    public Task<CategoryVm> GetAllCategoryAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
