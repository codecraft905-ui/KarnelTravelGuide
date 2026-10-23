using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

public class AdminDashboardViewModel
{
    public string Status { get; set; }   // Pending | Confirmed | Rejected | (all)
    public string Q { get; set; }        // search text
    public List<Booking> Rows { get; set; } = new();
    public int Total, Pending, Confirmed, Rejected;
}

public class AdminMessagesViewModel
{
    public string Show { get; set; }     // unread | inquiry | feedback | (all)
    public List<ContactMessage> Rows { get; set; } = new();
    public int Total, Unread, Inquiries, Feedbacks;
}

public class AdminReviewViewModel
{
    public Booking Booking { get; set; }
    public string AdminNote { get; set; }
}
