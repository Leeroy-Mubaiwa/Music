using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Music.Areas.Identity.Data;
using Music.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("musicConn") ?? throw new InvalidOperationException("Connection string 'MusicIdentityContextConnection' not found.");

builder.Services.AddDbContext<MusicIdentityContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<MusicDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<MusicUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<MusicIdentityContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
