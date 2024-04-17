using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Product_Inventory.Data;
using Product_Inventory.Models;
using Product_Inventory.Repositories;
using Product_Inventory.Repositories.IRepos;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationDbContext> //For ApplicationDbContext
(
    options => options.UseSqlServer(connectionString)
);
builder.Services.AddIdentity<AppUser, IdentityRole>() //For Identity
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<DbInitializer>(); //For DbInitializer to Seed Data
builder.Services.AddAutoMapper(typeof(Program).Assembly); //For automapper
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) //It'll call Seed method to store initial roles (admin & user) and admin user into DB
{
    var services = scope.ServiceProvider.GetRequiredService<DbInitializer>;
    services.Invoke().Seed();
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();

