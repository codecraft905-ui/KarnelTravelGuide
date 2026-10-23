// Karnel Travel Guide - ASP.NET Core MVC (.NET 8) with SQL Server (EF Core 8).
using Microsoft.EntityFrameworkCore;
using KarnelTravelGuide.Models.Data;
using KarnelTravelGuide.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// SQL Server: KarnelTravelGuideDB (connection string "DefaultConnection" in appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Not logged in? Book a Tour, My Booking, Book Now and Send Feedback all send the visitor to the Sign Up page first
// (with ?ReturnUrl=...), and after Sign Up -> Login they come back to the page they asked for.
builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", o =>
{
    o.LoginPath = "/Register";
    o.AccessDeniedPath = "/Login";
    o.Cookie.Name = "ktg.auth";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
});
builder.Services.AddScoped<UserStore>();   // accounts are saved in SQL Server (uses the DbContext, so it is scoped)
// Site content + helpers used by controllers and views
builder.Services.AddTransient<EmailService>();
builder.Services.AddSingleton<ICatalogService, CatalogService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<NavigationService>();

var app = builder.Build();
app.Services.GetRequiredService<ICatalogService>().Validate();    // fails fast if a package names a place that does not exist
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
