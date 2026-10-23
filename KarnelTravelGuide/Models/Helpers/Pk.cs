using System.Globalization;

namespace KarnelTravelGuide.Helpers;

// Pakistan time helpers (falls back to the server's local time if the time zone is missing)
public static class Pk
{
    static readonly TimeZoneInfo Tz = Find();
    static TimeZoneInfo Find()
    {
        foreach (var id in new[] { "Asia/Karachi", "Pakistan Standard Time" })
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch { }
        return TimeZoneInfo.Local;
    }
    public static DateTime Today => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Tz).Date;
    public static string Stamp(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Tz).ToString("dd MMM yyyy, hh:mm tt", CultureInfo.InvariantCulture);
    public static string Day(DateTime d) => d.ToString("ddd, dd MMM yyyy", CultureInfo.InvariantCulture);
}
