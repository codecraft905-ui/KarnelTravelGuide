using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarnelTravelGuide.Models;
using KarnelTravelGuide.Models.Data;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

// Every action in this controller needs the "Admin" role (set in AccountController when the admin logs in).
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    readonly ApplicationDbContext _db;
    public AdminController(ApplicationDbContext db) { _db = db; }

    // ================= Booking requests =================

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.TotalBookings = await _db.Bookings.CountAsync();

        ViewBag.PendingBookings = await _db.Bookings
            .CountAsync(b => b.Status == BookingStatus.Pending);

        ViewBag.TotalMessages = await _db.ContactMessages.CountAsync();

        ViewBag.UnreadMessages = await _db.ContactMessages
            .CountAsync(m => !m.IsRead);

        ViewBag.TotalTourPackages = await _db.TourPackages.CountAsync();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Index(string status, string q)
    {
        var vm = new AdminDashboardViewModel { Q = q?.Trim() };

        var counts = await _db.Bookings.GroupBy(b => b.Status).Select(g => new { Key = g.Key, N = g.Count() }).ToListAsync();
        int Count(BookingStatus s) => counts.Where(c => c.Key == s).Select(c => c.N).FirstOrDefault();
        vm.Pending = Count(BookingStatus.Pending);
        vm.Confirmed = Count(BookingStatus.Confirmed);
        vm.Rejected = Count(BookingStatus.Rejected);
        vm.Total = vm.Pending + vm.Confirmed + vm.Rejected;

        var rows = _db.Bookings.AsNoTracking().AsQueryable();
        if (Enum.TryParse<BookingStatus>(status, true, out var st) && Enum.IsDefined(st))
        {
            rows = rows.Where(b => b.Status == st);
            vm.Status = st.ToString();
        }
        var term = vm.Q;
        if (!string.IsNullOrEmpty(term))
            rows = rows.Where(b => b.Id.Contains(term) || b.Name.Contains(term) || b.Email.Contains(term) || b.Phone.Contains(term)
                                   || b.Destination.Contains(term) || b.Hotel.Contains(term));

        // pending requests first, then newest first
        vm.Rows = await rows.OrderBy(b => b.Status == BookingStatus.Pending ? 0 : 1).ThenByDescending(b => b.CreatedAtUtc).ToListAsync();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Review(string id)
    {
        var b = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) { TempData["Flash"] = "That booking request was not found."; return RedirectToAction(nameof(Index)); }
        return View(new AdminReviewViewModel { Booking = b, AdminNote = b.AdminNote });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Accept(string id, string adminNote) => Decide(id, BookingStatus.Confirmed, adminNote);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Reject(string id, string adminNote) => Decide(id, BookingStatus.Rejected, adminNote);

    // private = not reachable from the browser; only Accept / Reject above call it
    async Task<IActionResult> Decide(string id, BookingStatus newStatus, string adminNote)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) { TempData["Flash"] = "That booking request was not found."; return RedirectToAction(nameof(Index)); }

        adminNote = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
        if (adminNote != null && adminNote.Length > 500)
        {
            TempData["Flash"] = "The note cannot be longer than 500 characters.";
            return RedirectToAction(nameof(Review), new { id });
        }

        b.Status = newStatus;
        b.AdminNote = adminNote;
        b.DecidedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Flash"] = $"Booking {b.Id} is now {(newStatus == BookingStatus.Confirmed ? "Confirmed" : "Rejected")}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(string id)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == id);
        if (b != null)
        {
            _db.Bookings.Remove(b);
            await _db.SaveChangesAsync();
            TempData["Flash"] = $"Booking {id} was deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ================= Contact messages =================

    [HttpGet]
    public async Task<IActionResult> Messages(string show)
    {
        var all = _db.ContactMessages.AsNoTracking();
        var vm = new AdminMessagesViewModel
        {
            Total = await all.CountAsync(),
            Unread = await all.CountAsync(m => !m.IsRead),
            Inquiries = await all.CountAsync(m => m.Type == "Inquiry"),
            Feedbacks = await all.CountAsync(m => m.Type == "Feedback")
        };

        var rows = all;
        switch (show?.ToLowerInvariant())
        {
            case "unread": rows = rows.Where(m => !m.IsRead); vm.Show = "unread"; break;
            case "inquiry": rows = rows.Where(m => m.Type == "Inquiry"); vm.Show = "inquiry"; break;
            case "feedback": rows = rows.Where(m => m.Type == "Feedback"); vm.Show = "feedback"; break;
        }
        vm.Rows = await rows.OrderByDescending(m => m.CreatedAtUtc).ToListAsync();
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> MarkRead(string id, string show) => SetRead(id, true, show);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> MarkUnread(string id, string show) => SetRead(id, false, show);

    async Task<IActionResult> SetRead(string id, bool read, string show)
    {
        var m = await _db.ContactMessages.FirstOrDefaultAsync(x => x.Id == id);
        if (m != null) { m.IsRead = read; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Messages), new { show });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(string id, string show)
    {
        var m = await _db.ContactMessages.FirstOrDefaultAsync(x => x.Id == id);
        if (m != null)
        {
            _db.ContactMessages.Remove(m);
            await _db.SaveChangesAsync();
            TempData["Flash"] = "Message deleted.";
        }
        return RedirectToAction(nameof(Messages), new { show });
    }
}
