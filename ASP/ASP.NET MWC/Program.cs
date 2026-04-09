using ASP.NET_MWC.Models;
using ASP.NET_MWC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserStore>();

// Session — pro sledování přihlášení
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Map /pricing.html to the MVC action directly so the URL works as-is
app.MapControllerRoute(
    name: "pricing",
    pattern: "pricing.html",
    defaults: new { controller = "Home", action = "Pricing" });

// Map /bomb.html to the MVC action
app.MapControllerRoute(
    name: "bomb",
    pattern: "bomb.html",
    defaults: new { controller = "Home", action = "Bomb" });

app.UseStaticFiles(); // serve dynamically-uploaded files (comment images, etc.)
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
