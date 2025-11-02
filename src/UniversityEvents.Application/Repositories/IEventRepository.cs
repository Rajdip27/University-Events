using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Application.Repositories;

public interface IEventRepository
{
    Task<PaginationModel<EventVm>> GetCategoriesAsync(Filter filter, CancellationToken ct);
    Task<EventVm> GetCategoryByIdAsync(long id, CancellationToken ct);
    Task<EventVm> CreateOrUpdateCategoryAsync(EventVm categoryVm, CancellationToken ct);
    Task<bool> DeleteCategoryAsync(long id, CancellationToken ct);
}
