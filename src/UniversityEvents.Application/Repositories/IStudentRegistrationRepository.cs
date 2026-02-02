using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.FileServices;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace UniversityEvents.Application.Repositories;

public interface IStudentRegistrationRepository
{
   
    Task<PaginationModel<StudentRegistrationVm>> GetRegistrationsAsync(Filter filter, CancellationToken ct);
    Task<StudentRegistrationVm> GetRegistrationByIdAsync(long id, CancellationToken ct);

    Task<StudentRegistrationVm> GetStudentRegistrationAsync(long EventId,long UserId, CancellationToken ct);

    Task<StudentRegistrationVm> CreateOrUpdateRegistrationAsync(StudentRegistrationVm vm, CancellationToken ct);
    Task<bool> DeleteRegistrationAsync(long id, CancellationToken ct);

    Task<IEnumerable<SelectListItem>> StudentRegistrationDropdown(long userId);
    Task<IEnumerable<SelectListItem>> StudentRegistrationDropdown();
}

public class StudentRegistrationRepository(UniversityDbContext _context, IFileService fileService, ISignInHelper signInHelper) : IStudentRegistrationRepository
{
    public async Task<StudentRegistrationVm> CreateOrUpdateRegistrationAsync(StudentRegistrationVm vm, CancellationToken ct)
    {
        try
        {
            var entity = vm.Id > 0
                ? await _context.StudentRegistrations.FirstOrDefaultAsync(s => s.Id == vm.Id, ct)
                : new StudentRegistration();

            if(vm.EventId>0)
            {
                entity.Event = await _context.Events.FirstOrDefaultAsync(e => e.Id == vm.EventId, ct);
               
            }
            if (vm.Id > 0 && entity == null) return null;

            // Map fields
            entity.EventId = vm.EventId;
            entity.FullName = vm.FullName;
            entity.PhoneNumber = vm.PhoneNumber;
            entity.Email = vm.Email;
            entity.IdCardNumber = vm.IdCardNumber;
            entity.Department = vm.Department;
            entity.PaymentStatus = entity.Event.IsFree ? "Free" : "Pending";
            entity.UserId = (long)(vm.UserId != 0 ? vm.UserId : (signInHelper.UserId != 0 ? signInHelper.UserId : 0));
            if (vm.ImageFile is not null)
            {
                if (!string.IsNullOrEmpty(entity.PhotoPath))
                {
                    fileService.DeleteFile(entity.PhotoPath, CommonVariables.StudentRegistrationLocation);
                }
                entity.PhotoPath = await fileService.Upload(vm.ImageFile, CommonVariables.StudentRegistrationLocation);
            }
            //entity.PaymentStatus = vm.PaymentStatus;

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

    public async Task<PaginationModel<StudentRegistrationVm>> GetRegistrationsAsync(Filter filter, CancellationToken ct) 
    {
        try
        {
            var query = _context.StudentRegistrations
                       .AsNoTracking()
                       .Include(s => s.Event)
                       .Where(s => !s.IsDelete);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(s =>
                    s.FullName.Contains(filter.Search) ||
                    s.Email.Contains(filter.Search) ||
                    s.PhoneNumber.Contains(filter.Search));
            }
            if (filter.UserId > 0)
                query = query.Where(s => s.UserId == filter.UserId);
            if (filter.EventId > 0)
                query = query.Where(s => s.EventId == filter.EventId);
            if (filter.StudentId > 0)
                query = query.Where(s => s.Id == filter.StudentId);
            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(s => s.PaymentStatus == filter.Status);
            var projected = query.ProjectToType<StudentRegistrationVm>();
            return await projected.ToPagedListAsync(filter.Page, filter.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        

    }

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

    public async Task<IEnumerable<SelectListItem>> StudentRegistrationDropdown(long userId)
    {
        var query = _context.StudentRegistrations
                        .Where(x => !x.IsDelete);
        if (userId > 0)
            query = query.Where(x => x.UserId == userId);
        var list = await query
            .Select(x => new SelectListItem
            {
                Text = x.FullName,
                Value = x.Id.ToString()
            })
            .ToListAsync();
        return list;
    }

    public async Task<IEnumerable<SelectListItem>> StudentRegistrationDropdown()
    {
        var query = _context.StudentRegistrations
                        .Where(x => !x.IsDelete);
        var list = await query
            .Select(x => new SelectListItem
            {
                Text = x.FullName,
                Value = x.Id.ToString()
            })
            .ToListAsync();
        return list;
    }
}
