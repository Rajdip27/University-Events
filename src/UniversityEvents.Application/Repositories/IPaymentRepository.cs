using Microsoft.EntityFrameworkCore;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface IPaymentRepository
{
    Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default);

}

public class PaymentRepository(UniversityDbContext _context) : IPaymentRepository
{
    public async Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.PaymentHistory) 
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return payment;
    }

}
