namespace UniversityEvents.Application.ViewModel;

public class DashboardViewModel
{
    public long? EventId { get; set; } // optional filter
    public List<EventVm> Events { get; set; } // dropdown list
    public int TotalEvents { get; set; }
    public int TotalRegistrations { get; set; }
    public int TotalPendingPayments { get; set; }
    public int TotalPayments { get; set; }
    public List<StudentRegistrationVm> RecentRegistrations { get; set; }

    public List<DailyRegistration> DailyRegistrations { get; set; }
    public PaymentSummary PaymentSummary { get; set; }
}

public class DailyRegistration
{
    public string DateLabel { get; set; } // e.g., "Jan 1"
    public int Count { get; set; }
}

public class PaymentSummary
{
    public int Paid { get; set; }
    public int Pending { get; set; }
}
