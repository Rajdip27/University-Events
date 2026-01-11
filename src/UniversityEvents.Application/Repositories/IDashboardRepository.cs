using Microsoft.EntityFrameworkCore;
using UniversityEvents.Application.ViewModel;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Application.Repositories;

public interface IDashboardRepository
{
    DashboardViewModel GetDashboardData(long? eventId = null);
}

public class DashboardRepository(UniversityDbContext _context) : IDashboardRepository
{
    public DashboardViewModel GetDashboardData(long? eventId = null)
    {

        try
        {
            var registrations = _context.StudentRegistrations.Include(r => r.Event).AsQueryable();

            if (eventId.HasValue)
                registrations = registrations.Where(r => r.EventId == eventId.Value);

            var recentRegistrations = registrations
                .OrderByDescending(r => r.CreatedDate)
                .Take(10)
                .Select(r => new StudentRegistrationVm
                {
                    IdCardNumber = r.IdCardNumber,
                    FullName = r.FullName,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    Department = r.Department,
                    Event = new EventVm { Id = r.Event.Id, Name = r.Event.Name },
                    PaymentStatus = r.PaymentStatus,
                    RegistrationDate = r.CreatedDate
                }).ToList();

            var today = DateTime.Today;
            var last30Days = Enumerable.Range(0, 30)
                .Select(i => today.AddDays(-29 + i))
                .ToList();

            var dailyRegistrations = last30Days
                .Select(date => new DailyRegistration
                {
                    DateLabel = date.ToString("MMM d"),
                    Count = registrations.Count(r => r.CreatedDate.Date == date)
                })
                .ToList();

            var paymentSummary = new PaymentSummary
            {
                Paid = registrations.Count(r => r.PaymentStatus == "Paid"),
                Pending = registrations.Count(r => r.PaymentStatus == "Pending")
            };

            return new DashboardViewModel
            {
                EventId = eventId,
                Events = _context.Events.Select(e => new EventVm { Id = e.Id, Name = e.Name }).ToList(),
                TotalEvents = _context.Events.Count(),
                TotalRegistrations = registrations.Count(),
                TotalPayments = paymentSummary.Paid,
                TotalPendingPayments = registrations.Count(r => r.PaymentStatus != "Paid" &&  r.PaymentStatus != "Free"),
                RecentRegistrations = recentRegistrations,
                DailyRegistrations = dailyRegistrations,
                PaymentSummary = paymentSummary
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine( ex.Message);
            throw;
        }
       
    }
}
