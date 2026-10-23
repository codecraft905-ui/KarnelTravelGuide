using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KarnelTravelGuide.Models;
using KarnelTravelGuide.Models.Data;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

// Admin CRUD for the TourPackages table. Only administrators can open or call anything here.
[Authorize(Roles = "Admin")]
public class TourPackagesAdminController : Controller
{
    readonly ApplicationDbContext _db;
    public TourPackagesAdminController(ApplicationDbContext db) { _db = db; }

    // READ (list)
    [HttpGet]
    public async Task<IActionResult> Index() =>
        View(await _db.TourPackages.AsNoTracking().OrderBy(p => p.Name).ToListAsync());

    // CREATE
    [HttpGet]
    public IActionResult Create() => View("Edit", new TourPackageFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TourPackageFormViewModel vm)
    {
        Clean(vm);
        if (await NameTaken(vm.Name, 0)) ModelState.AddModelError(nameof(vm.Name), "A package with this name already exists.");
        if (!ModelState.IsValid) return View("Edit", vm);

        _db.TourPackages.Add(new TourPackageDb { Name = vm.Name, Offer = vm.Offer, Kw = vm.Kw });
        await _db.SaveChangesAsync();
        TempData["Flash"] = $"Package \"{vm.Name}\" was added.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _db.TourPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) { TempData["Flash"] = "That package was not found."; return RedirectToAction(nameof(Index)); }
        return View(new TourPackageFormViewModel { Id = p.Id, Name = p.Name, Offer = p.Offer, Kw = p.Kw });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TourPackageFormViewModel vm)
    {
        vm.Id = id;                       // the id in the address is the only one we trust
        Clean(vm);
        var p = await _db.TourPackages.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) { TempData["Flash"] = "That package was not found."; return RedirectToAction(nameof(Index)); }
        if (await NameTaken(vm.Name, id)) ModelState.AddModelError(nameof(vm.Name), "Another package already uses this name.");
        if (!ModelState.IsValid) return View("Edit", vm);

        p.Name = vm.Name; p.Offer = vm.Offer; p.Kw = vm.Kw;
        await _db.SaveChangesAsync();
        TempData["Flash"] = $"Package \"{p.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE (the Delete button asks for confirmation first)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.TourPackages.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) { TempData["Flash"] = "That package was not found."; return RedirectToAction(nameof(Index)); }
        _db.TourPackages.Remove(p);
        await _db.SaveChangesAsync();
        TempData["Flash"] = $"Package \"{p.Name}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    static void Clean(TourPackageFormViewModel vm)
    {
        vm.Name = vm.Name?.Trim();
        vm.Offer = vm.Offer?.Trim();
        vm.Kw = vm.Kw?.Trim();
    }

    Task<bool> NameTaken(string name, int exceptId) =>
        string.IsNullOrEmpty(name) ? Task.FromResult(false) : _db.TourPackages.AnyAsync(p => p.Name == name && p.Id != exceptId);
}
