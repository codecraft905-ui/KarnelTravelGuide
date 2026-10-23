using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.ViewModels;

public class MyBookingViewModel
{
    public string Ref { get; set; }
    public string Email { get; set; }
    public List<Booking> Mine { get; set; } = new();
}
