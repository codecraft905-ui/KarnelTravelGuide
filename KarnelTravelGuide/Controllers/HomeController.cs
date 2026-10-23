using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View(new HomeViewModel());

    [HttpGet("About")]
    public IActionResult About() => View();

    [HttpGet("Information")]
    public IActionResult Information() => View(new InformationViewModel());
}
