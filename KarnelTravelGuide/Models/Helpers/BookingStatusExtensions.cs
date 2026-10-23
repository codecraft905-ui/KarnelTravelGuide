using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.Helpers;

public static class BookingStatusExtensions
{
    public static string Css(this BookingStatus s) => s.ToString().ToLowerInvariant();
    public static string Label(this BookingStatus s) => s switch
    {
        BookingStatus.Confirmed => "Confirmed",
        BookingStatus.Rejected => "Rejected",
        _ => "Pending"
    };
}
