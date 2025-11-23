using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;

public interface IEventRepository
{
    Task<PaginationModel<EventVm>> GetEventsAsync(Filter filter, CancellationToken ct);
    Task<EventVm> GetEventByIdAsync(long id, CancellationToken ct);
    Task<EventVm> CreateOrUpdateEventAsync(EventVm vm, CancellationToken ct);
    Task<bool> DeleteEventAsync(long id, CancellationToken ct);
}

public class EventRepository(UniversityDbContext context) : IEventRepository
{
    private readonly UniversityDbContext _context = context;

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
            eventEntity.StartDate = vm.StartDate;
            eventEntity.EndDate = vm.EndDate;
            eventEntity.RegistrationFee = vm.RegistrationFee;
            eventEntity.Slug = vm.Slug;

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
        var query = _context.Events
            .AsNoTracking()
            .Where(e => !e.IsDelete);

        // Optional: Apply filters using Specification pattern if needed
        // query = SpecificationEvaluator<Event>.GetQuery(query, new EventSpecification(filter));

        return await query
            .ProjectToType<EventVm>()
            .ToPagedListAsync(filter.Page, filter.PageSize);
    }
}