using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.Models;
using KarnelTravelGuide.Models.Data;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class ContactController : Controller
{
    readonly ApplicationDbContext _db;
    public ContactController(ApplicationDbContext db) { _db = db; }

    // The Contact page itself stays public (branches, FAQs, phone numbers)
    [HttpGet]
    public IActionResult Index(string type, bool sent = false) => View(new ContactViewModel
    {
        Name = User.Identity?.Name,
        Email = User.FindFirstValue(ClaimTypes.Email),
        Type = type == "Feedback" ? "Feedback" : "Inquiry",
        Sent = sent
    });

    // "Send Feedback" in the menu points here: Sign Up first, then the feedback form opens
    [Authorize, HttpGet("Feedback")]
    public IActionResult Feedback() => Redirect("/Contact?type=Feedback#form");

    // Sending a message needs an account. The message is saved and the admin reads it in Admin > Messages.
    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel vm)
    {
        if (vm.Type != "Inquiry" && vm.Type != "Feedback")
            ModelState.AddModelError(nameof(vm.Type), "Please choose a valid message type.");
        if (!ModelState.IsValid) return View(vm);

        var feedback = vm.Type == "Feedback";
        _db.ContactMessages.Add(new ContactMessage
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedAtUtc = DateTime.UtcNow,
            Name = vm.Name.Trim(),
            Email = vm.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(vm.Phone) ? null : vm.Phone.Trim(),
            Type = vm.Type,
            Rating = feedback ? vm.Rating : null,
            Text = vm.Message.Trim(),
            Destination = string.IsNullOrWhiteSpace(vm.Destination) ? null : vm.Destination.Trim(),
            TravelDate = feedback ? null : vm.TravelDate,
            People = feedback ? null : vm.People,
            IsRead = false
        });
        await _db.SaveChangesAsync();

        // redirect so that refreshing the page cannot send the same message twice
        return RedirectToAction(nameof(Index), new { sent = true });
    }
}
