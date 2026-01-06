using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.FileServices;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.Application.Repositories;

public interface IStudentRegistrationRepository
{
   
    Task<PaginationModel<StudentRegistrationVm>> GetRegistrationsAsync(Filter filter, CancellationToken ct);
    Task<StudentRegistrationVm> GetRegistrationByIdAsync(long id, CancellationToken ct);

    Task<StudentRegistrationVm> GetStudentRegistrationAsync(long EventId,long UserId, CancellationToken ct);

    Task<StudentRegistrationVm> CreateOrUpdateRegistrationAsync(StudentRegistrationVm vm, CancellationToken ct);
    Task<bool> DeleteRegistrationAsync(long id, CancellationToken ct);
}

public class StudentRegistrationRepository(UniversityDbContext context,IFileService fileService, ISignInHelper signInHelper) : IStudentRegistrationRepository
{
    private readonly UniversityDbContext _context = context;
    public async Task<StudentRegistrationVm> CreateOrUpdateRegistrationAsync(StudentRegistrationVm vm, CancellationToken ct)
    {
        try
        {
            var entity = vm.Id > 0
                ? await _context.StudentRegistrations.FirstOrDefaultAsync(s => s.Id == vm.Id, ct)
                : new StudentRegistration();

            if (vm.Id > 0 && entity == null) return null;

            // Map fields
            entity.EventId = vm.EventId;
            entity.FullName = vm.FullName;
            entity.PhoneNumber = vm.PhoneNumber;
            entity.Email = vm.Email;
            entity.IdCardNumber = vm.IdCardNumber;
            entity.Department = vm.Department;
            entity.UserId = (long)(vm.UserId != 0 ? vm.UserId : (signInHelper.UserId != 0 ? signInHelper.UserId : 0));

            if (vm.ImageFile is not null)
            {
                if (!string.IsNullOrEmpty(entity.PhotoPath))
                {
                    fileService.DeleteFile(entity.PhotoPath, CommonVariables.StudentRegistrationLocation);
                }
                entity.PhotoPath = await fileService.Upload(vm.ImageFile, CommonVariables.StudentRegistrationLocation);
            }
            entity.PaymentStatus = vm.PaymentStatus;

            if (vm.Id > 0)
                _context.StudentRegistrations.Update(entity);
            else
                await _context.StudentRegistrations.AddAsync(entity, ct);

            await _context.SaveChangesAsync(ct);

            return entity.Adapt<StudentRegistrationVm>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    // ✅ Delete (soft delete)
    public async Task<bool> DeleteRegistrationAsync(long id, CancellationToken ct)
    {
        try
        {
            var entity = await _context.StudentRegistrations.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (entity == null) return false;

            entity.IsDelete = true;
            await _context.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    // ✅ Get by Id
    public async Task<StudentRegistrationVm> GetRegistrationByIdAsync(long id, CancellationToken ct)
    {
        try
        {
            var entity = await _context.StudentRegistrations
                .AsNoTracking()
                .Include(s => s.Event) // Include related event if needed
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete, ct);

            return entity?.Adapt<StudentRegistrationVm>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<PaginationModel<StudentRegistrationVm>> GetRegistrationsAsync(Filter filter, CancellationToken ct) =>
    await _context.StudentRegistrations
        .AsNoTracking()
        .Include(s => s.Event)
        .Where(s => !s.IsDelete &&
                   (string.IsNullOrWhiteSpace(filter.Search) ||
                    s.FullName.Contains(filter.Search) ||
                    s.Email.Contains(filter.Search) ||
                    s.PhoneNumber.Contains(filter.Search)) &&
                   (filter.UserId <= 0 || s.UserId == filter.UserId))
        .ProjectToType<StudentRegistrationVm>()
        .ToPagedListAsync(filter.Page, filter.PageSize);

    public async Task<StudentRegistrationVm> GetStudentRegistrationAsync(
     long eventId,
     long userId,
     CancellationToken ct)
    {
        var data= await _context.StudentRegistrations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.EventId == eventId && s.UserId == userId,
                ct);

        return data.Adapt<StudentRegistrationVm>();
    }

}
