using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class DetailsController : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new DetailsViewModel());
}
