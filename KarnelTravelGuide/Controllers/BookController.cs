using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarnelTravelGuide.Helpers;
using KarnelTravelGuide.Models;
using KarnelTravelGuide.Models.Data;
using KarnelTravelGuide.Services;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

// Visitors who are not logged in are sent to the Sign Up page (see LoginPath in Program.cs).
// After sign up + login they land back here with the booking form.
[Authorize]
public class BookController : Controller
{
    readonly ICatalogService _catalog;
    readonly ApplicationDbContext _db;
    public BookController(ICatalogService catalog, ApplicationDbContext db) { _catalog = catalog; _db = db; }

    [HttpGet]
    public IActionResult Index(string item)
    {
        var vm = new BookViewModel
        {
            Name = User.Identity?.Name,
            Email = User.FindFirstValue(ClaimTypes.Email),
            // /Book?item=<package or place> comes from the Book Now buttons on package cards
            Destination = (_catalog.FindPackage(item) ?? _catalog.FirstPackageWith(item))?.Name
        };
        return View(vm);
    }

    // "Request Received" page. Redirected to after saving, so refreshing the page can never create a second booking.
    [HttpGet]
    public async Task<IActionResult> Done(string id)
    {
        var me = User.FindFirstValue(ClaimTypes.Email);
        var b = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.AccountEmail == me);
        if (b == null) return RedirectToAction(nameof(Index));
        return View("Index", new BookViewModel { Name = b.Name, Email = b.Email, Done = b });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(BookViewModel vm)
    {
        // Spam trap: bots fill the hidden field. Pretend it worked, save nothing.
        if (!string.IsNullOrEmpty(vm.Website)) return RedirectToAction("Index", "Home");

        // Identity ALWAYS comes from the login cookie, never from the posted form.
        var me = User.FindFirstValue(ClaimTypes.Email);
        vm.Name = User.Identity?.Name;
        vm.Email = me;
        ModelState.Remove(nameof(vm.Name));
        ModelState.Remove(nameof(vm.Email));

        var pkg = _catalog.FindPackage(vm.Destination);
        if (!string.IsNullOrWhiteSpace(vm.Destination) && pkg == null)
            ModelState.AddModelError(nameof(vm.Destination), "Please choose a valid package.");
        if (vm.TravelDate.HasValue && vm.TravelDate.Value.Date < Pk.Today)
            ModelState.AddModelError(nameof(vm.TravelDate), "Travel date cannot be in the past.");
        if (pkg != null && vm.TravelDate.HasValue && ModelState.IsValid)
        {
            // the same customer cannot send the same package for the same date twice (a rejected request may be re-sent)
            var date = vm.TravelDate.Value.Date;
            var duplicate = await _db.Bookings.AnyAsync(b => b.AccountEmail == me && b.Destination == pkg.Name
                                                             && b.TravelDate == date && b.Status != BookingStatus.Rejected);
            if (duplicate)
                ModelState.AddModelError("", "You already have a booking request for this package on that date. Please check My Booking.");
        }
        if (!ModelState.IsValid) return View(vm);

        var booking = new Booking
        {
            AccountEmail = me,
            CreatedAtUtc = DateTime.UtcNow,
            Name = vm.Name.Trim(),
            Email = me,
            Phone = vm.Phone.Trim(),
            Destination = pkg.Name,
            Kind = "Tour Package",
            Included = _catalog.Summary(pkg),          // snapshot of what the package included at booking time
            TravelDate = vm.TravelDate.Value.Date,
            People = vm.People.Value,
            Message = string.IsNullOrWhiteSpace(vm.Message) ? null : vm.Message.Trim(),
            Status = BookingStatus.Pending
        };

        // Reference like KTG-7K2M9Q. In the (very unlikely) case it already exists, try a new one.
        for (var attempt = 0; ; attempt++)
        {
            booking.Id = NewRef();
            _db.Bookings.Add(booking);
            try { await _db.SaveChangesAsync(); break; }
            catch (DbUpdateException) when (attempt < 4) { _db.Entry(booking).State = EntityState.Detached; }
        }

        return RedirectToAction(nameof(Done), new { id = booking.Id });
    }

    static string NewRef()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";   // no 0/O/1/I to avoid mix-ups
        var s = new char[6];
        for (var i = 0; i < s.Length; i++) s[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        return "KTG-" + new string(s);
    }
}
