using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Application.Services;
using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Infrastructure.Database;
using GLA_ParkingManagement.Infrastructure.Mapper;
using GLA_ParkingManagement.Infrastructure.Repository.Implementation;
using GLA_ParkingManagement.Infrastructure.Repository.Interfaces;
using GLA_ParkingManagement.Infrastructure.Seeder;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Db Context configuration.
builder.Services.AddDbContext<ParkingManagementDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
#region Service Configuration
builder.Services.AddIdentity<AppUser,IdentityRole>(options =>
{
    // Optional: Configure Password strength
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Optional: Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<ParkingManagementDbContext>() // Links Identity to your DB
    .AddDefaultTokenProviders(); // Enables password reset and 2FA tokens

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();
#endregion



// Session Configuration
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;     // Security: prevents client-side scripts from accessing the cookie
    options.Cookie.IsEssential = true;  // Required for session to work without explicit cookie consent
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedDefaultData(scope.ServiceProvider);
}
app.Run();
