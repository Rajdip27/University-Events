using Microsoft.EntityFrameworkCore;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.Application.Repositories;

public interface IPaymentRepository
{
    Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default);

}

public class PaymentRepository(UniversityDbContext _context, ISignInHelper signInHelper) : IPaymentRepository
{
    public async Task<Payment> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.PaymentHistory) 
            .Include(p => p.Registration) 
            .Include(p => p.Registration.Event) 
            .FirstOrDefaultAsync(p => p.RegistrationId == id, cancellationToken);
    }

    public async Task<Payment> AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        payment.CreatedBy = signInHelper.UserId??0;
        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return payment;
    }

}
