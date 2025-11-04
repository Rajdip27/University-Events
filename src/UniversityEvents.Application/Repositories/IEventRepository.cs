using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Filters;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface IEventRepository
{
    Task<PaginationModel<EventVm>> GetEventAsync(Filter filter, CancellationToken ct);
    Task<EventVm> GetEventByIdAsync(long id, CancellationToken ct);
    Task<EventVm> CreateOrUpdateEventAsync(EventVm categoryVm, CancellationToken ct);
    Task<bool> DeleteEventAsync(long id, CancellationToken ct);
}

public class EventRepository(UniversityDbContext context) : IEventRepository
{
    public async Task<bool> DeleteEventAsync(long id, CancellationToken ct)
    {
        try
        {
            var exitingEvent = await context.Events.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (exitingEvent == null) return false;
            exitingEvent.IsDelete = true;
            await context.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
       
    }
    public Task<EventVm> GetEventByIdAsync(long id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
    public Task<PaginationModel<EventVm>> GetEventAsync(Filter filter, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
    public Task<EventVm> CreateOrUpdateEventAsync(EventVm categoryVm, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
