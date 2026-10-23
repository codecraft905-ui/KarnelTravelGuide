using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class ListingController : Controller
{
    // /tourist-spots  /hotels  /restaurants  /resorts  /travel
    [HttpGet("{cat:regex(^(tourist-spots|hotels|restaurants|resorts|travel)$)}")]
    public IActionResult Index() => View(new ListingViewModel());
}
