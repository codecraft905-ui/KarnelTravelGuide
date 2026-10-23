using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class SearchController : Controller
{
    [HttpGet]
    public IActionResult Index([FromQuery] SearchViewModel vm) => View(vm);

    [HttpGet("AdvancedSearch")]
    public IActionResult Advanced([FromQuery] SearchViewModel vm) => View(vm);
}
