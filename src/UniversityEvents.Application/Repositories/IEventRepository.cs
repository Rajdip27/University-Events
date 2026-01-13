using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using OfficeOpenXml.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Expressions;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.FileServices;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ModelSpecification;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public interface IEventRepository
{
    Task<PaginationModel<EventVm>> GetEventsAsync(Filter filter, CancellationToken ct);
    Task<EventVm> GetEventByIdAsync(long id, CancellationToken ct);
    Task<EventVm> CreateOrUpdateEventAsync(EventVm vm, CancellationToken ct);
    Task<bool> DeleteEventAsync(long id, CancellationToken ct);
    Task<List<CategoryVm>> GetAllCategoriesAsync(CancellationToken ct);
    Task<IEnumerable<SelectListItem>> CategoryDropdown();
    Task<IEnumerable<SelectListItem>> EventDropdown();
    Task<List<EventVm>> GetAllAsync(params Expression<Func<Event, object>>[] includes);
    Task<EventVm> GetByIdAsync(long id, params Expression<Func<Event, object>>[] includes);
    Task<List<MealTokenVm>> PrintMealTokens(Filter filter);

}

public class EventRepository(UniversityDbContext _context, IFileService fileService) : IEventRepository
{
    // ✅ Create or Update
    public async Task<EventVm> CreateOrUpdateEventAsync(EventVm vm, CancellationToken ct)
    {
        try
        {
            var eventEntity = vm.Id > 0
            ? await _context.Events.FirstOrDefaultAsync(e => e.Id == vm.Id, ct)
            : new Event();

            if (vm.Id > 0 && eventEntity == null) return null;

            // Map fields
            eventEntity.Name = vm.Name;
            eventEntity.Description = vm.Description;
            eventEntity.CategoryId = vm.CategoryId;
            eventEntity.StartDate = vm.StartDate.ToUtc();
            eventEntity.EndDate = vm.EndDate.ToUtc();
            eventEntity.RegistrationFee = vm.RegistrationFee;
            eventEntity.Slug = vm.Slug;
            eventEntity.MealsOffered = vm.MealsOffered;
            eventEntity.IsFree = vm.IsFree;

            // Handle Image Upload
            if (vm.ImageFile != null)
            {
                var imageUrl = await fileService.Upload(vm.ImageFile, CommonVariables.EventLocation);
                eventEntity.ImageUrl = imageUrl;
            }

            if (vm.Id > 0)
                _context.Events.Update(eventEntity);
            else
                await _context.Events.AddAsync(eventEntity, ct);

            await _context.SaveChangesAsync(ct);

            return eventEntity.Adapt<EventVm>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    // ✅ Delete (soft delete)
    public async Task<bool> DeleteEventAsync(long id, CancellationToken ct)
    {
        try
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (eventEntity == null) return false;
            eventEntity.IsDelete = true;
            await _context.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    public async Task<List<CategoryVm>> GetAllCategoriesAsync(CancellationToken ct)
    {
        try
        {
            var data = await _context.Categories
                        .AsNoTracking()
                        .ProjectToType<CategoryVm>()
                        .ToListAsync();
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    // ✅ Get by Id
    public async Task<EventVm> GetEventByIdAsync(long id, CancellationToken ct)
    {
        try
        {
            var eventEntity = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDelete, ct);

            return eventEntity?.Adapt<EventVm>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    // ✅ Get paginated list
    public async Task<PaginationModel<EventVm>> GetEventsAsync(Filter filter, CancellationToken ct)
    {
        try
        {
            var query = _context.Events
                        .AsNoTracking()
                        .Where(e => !e.IsDelete);

            // Optional: Apply filters using Specification pattern if needed
            query = SpecificationEvaluator<Event>.GetQuery(query, new EventSpecification(filter));

            return await query
                .ProjectToType<EventVm>()
                .ToPagedListAsync(filter.Page, filter.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<SelectListItem>> CategoryDropdown()
    {
        var list = await _context.Set<Category>()
            .Where(x => !x.IsDelete)
            .Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
            .ToListAsync();

        return list;
    }

    public async Task<List<EventVm>> GetAllAsync(params Expression<Func<Event, object>>[] includes)
    {
        try
        {
            DateTimeOffset todayUtc = DateTimeOffset.UtcNow.Date;
            IQueryable<Event> query = _context.Events.AsQueryable().AsNoTracking().Where(e => !e.IsDelete && e.EndDate >= todayUtc);
            // Apply includes
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query
                 .ProjectToType<EventVm>()
                 .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    public async Task<EventVm> GetByIdAsync(long id, params Expression<Func<Event, object>>[] includes)
    {
        try
        {
            IQueryable<Event> query = _context.Events
                .AsNoTracking()
                .Where(e => !e.IsDelete && e.Id == id);

            // Apply Includes
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query
                .ProjectToType<EventVm>()
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<SelectListItem>> EventDropdown()
    {
        try
        {
            var list = await _context.Events
             .Where(x => !x.IsDelete)
             .Select(x => new SelectListItem
             {
                 Text = x.Name,
                 Value = x.Id.ToString()
             })
             .ToListAsync();
            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    public async Task<List<MealTokenVm>> PrintMealTokens(Filter filter)
    {
        try
        {
            // Step 1: Fetch registrations from DB first
            var query = _context.StudentRegistrations
                .Include(r => r.Event)
                .AsNoTracking()
                .AsQueryable();

            if (filter.StudentId > 0)
            {
                query = query.Where(r => r.Id == filter.StudentId);
            }

            if (filter.EventId > 0)
            {
                query = query.Where(r => r.Event.Id == filter.EventId);
            }

            var registrations = await query.ToListAsync(); 

            var data = registrations
                .SelectMany(r => new[]
                {
                new { MealValue = 1, MealName = "Breakfast" },
                new { MealValue = 2, MealName = "Lunch" },
                new { MealValue = 4, MealName = "Dinner" }
                }, (r, m) => new MealTokenVm
                {
                    EventName = r.Event.Name,
                    RegistrationName = r.FullName,
                    IdCardNumber = r.IdCardNumber,
                    PhoneNumber = r.PhoneNumber,
                    Email = r.Email,
                    Department = r.Department,
                    MealName = ((r.Event.MealsOffered & m.MealValue) == m.MealValue) ? m.MealName : null,
                })
                .Where(x => x.MealName != null)
                .OrderBy(x => x.EventName)
                .ThenBy(x => x.RegistrationName)
                .ThenBy(x => x.MealName)
                .ToList();

            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new List<MealTokenVm>();
        }
    }


}
