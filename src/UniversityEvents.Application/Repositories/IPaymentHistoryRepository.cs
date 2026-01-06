using Mapster;
using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Filters;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.Application.Repositories;

public interface IPaymentHistoryRepository
{
    Task<PaymentHistory> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PaginationModel<PaymentHistory>> GetPaymentHistoryAsync(Filter filter, CancellationToken ct);

    Task<PaymentHistory> AddAsync(PaymentHistory paymentHistory, CancellationToken cancellationToken = default);
}

public class PaymentHistoryRepository(UniversityDbContext _context, ISignInHelper signInHelper) : IPaymentHistoryRepository
{
   

    

    public async Task<PaymentHistory> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentHistory
            .Include(p => p.Payment) // Include Payment entity if needed
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PaymentHistory> AddAsync(PaymentHistory paymentHistory, CancellationToken cancellationToken = default)
    {
        paymentHistory.CreatedBy = signInHelper.UserId??0;
        await _context.PaymentHistory.AddAsync(paymentHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return paymentHistory; // last inserted value
    }

    public async Task<PaginationModel<PaymentHistory>> GetPaymentHistoryAsync(Filter filter, CancellationToken ct)
    {
        var query = _context.PaymentHistory
            .AsNoTracking()
            .Include(s => s.Payment)
            .ThenInclude(p => p.Registration)
            .ThenInclude(r => r.Event)
            .Where(s => !s.IsDelete);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(s =>
                s.Payment.Registration.FullName.Contains(filter.Search) ||
                s.Payment.Registration.Event.Name.Contains(filter.Search));
        }

        // Apply UserId filter
        if (filter.UserId > 0)
        {
            query = query.Where(s => s.Payment.Registration.UserId == filter.UserId);
        }

        // Project and paginate
        var result = await query
     .ProjectToType<PaymentHistory>()
     .ToPagedListAsync(filter.Page, filter.PageSize);


        return result;
    }

}
