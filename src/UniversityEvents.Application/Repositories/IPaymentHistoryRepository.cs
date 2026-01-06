using Microsoft.EntityFrameworkCore;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface IPaymentHistoryRepository
{
    Task<PaymentHistory> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<PaymentHistory> AddAsync(PaymentHistory paymentHistory, CancellationToken cancellationToken = default);
}

public class PaymentHistoryRepository : IPaymentHistoryRepository
{
    private readonly UniversityDbContext _context;

    public PaymentHistoryRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentHistory> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentHistory
            .Include(p => p.Payment) // Include Payment entity if needed
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PaymentHistory> AddAsync(PaymentHistory paymentHistory, CancellationToken cancellationToken = default)
    {
        await _context.PaymentHistory.AddAsync(paymentHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return paymentHistory; // last inserted value
    }
}
