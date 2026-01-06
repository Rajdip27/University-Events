using Microsoft.EntityFrameworkCore;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.Application.Repositories;

public interface IPaymentHistoryRepository
{
    Task<PaymentHistory> GetByIdAsync(long id, CancellationToken cancellationToken = default);

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
}
