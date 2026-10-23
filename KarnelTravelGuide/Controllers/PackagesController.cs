using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class PackagesController : Controller
{
    [HttpGet("Packages")]
    public IActionResult Index() => View();

    [HttpGet("Packages/Details")]
    public IActionResult Details() => View(new PackageViewModel());
}
