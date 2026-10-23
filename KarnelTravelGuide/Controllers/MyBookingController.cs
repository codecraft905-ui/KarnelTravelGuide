using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarnelTravelGuide.Models;
using KarnelTravelGuide.Models.Data;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

// A customer only ever sees THEIR OWN bookings (matched on the logged-in account's email).
[Authorize]
public class MyBookingController : Controller
{
    readonly ApplicationDbContext _db;
    public MyBookingController(ApplicationDbContext db) { _db = db; }

    Task<List<Booking>> OwnBookings()
    {
        var me = User.FindFirstValue(ClaimTypes.Email);
        return _db.Bookings.AsNoTracking().Where(b => b.AccountEmail == me).OrderByDescending(b => b.CreatedAtUtc).ToListAsync();
    }

    [HttpGet("MyBooking")]
    public async Task<IActionResult> Index() =>
        View(new MyBookingViewModel { Email = User.FindFirstValue(ClaimTypes.Email), Mine = await OwnBookings() });

    // "Find My Booking": looks the reference up inside the logged-in user's own bookings only
    [HttpPost("MyBooking"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(MyBookingViewModel vm)
    {
        var own = await OwnBookings();
        var r = vm.Ref?.Trim();
        var found = own.Where(b => string.Equals(b.Id, r, StringComparison.OrdinalIgnoreCase)).ToList();
        if (found.Count == 0) { ModelState.AddModelError("", "No booking with that reference was found in your account."); vm.Mine = own; }
        else vm.Mine = found;
        return View(vm);
    }
}
