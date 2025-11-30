using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ViewModel;

namespace UniversityEvents.Application.Repositories;

public interface IStudentRegistrationRepository
{
    Task<PaginationModel<StudentRegistrationVm>> GetCategoriesAsync(Filter filter, CancellationToken ct);
    Task<StudentRegistrationVm> GetCategoryByIdAsync(long id, CancellationToken ct);
    Task<StudentRegistrationVm> CreateOrUpdateCategoryAsync(StudentRegistrationVm vm, CancellationToken ct);
    Task<bool> DeleteCategoryAsync(long id, CancellationToken ct);
}

public class StudentRegistrationRepository : IStudentRegistrationRepository
{
    public Task<StudentRegistrationVm> CreateOrUpdateCategoryAsync(StudentRegistrationVm vm, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCategoryAsync(long id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<PaginationModel<StudentRegistrationVm>> GetCategoriesAsync(Filter filter, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<StudentRegistrationVm> GetCategoryByIdAsync(long id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

