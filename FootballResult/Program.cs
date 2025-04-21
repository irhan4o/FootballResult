using FootballResult.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FootballResult.Models.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FootballResultDB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FootballResultDB>();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    // ��������� �� ����
    string[] roleNames = { "Admin", "User", "Athlete" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // ��������� �� �������������
    string adminEmail = "admin@football.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByNameAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "RentaCar",
            LastName = "Admin",
            IdentificationNumber = "1234567890"
        };
        var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
        if (createAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

    // Creating role Athlete
    string athleteEmail = "athlete@football.com";
    string athletePassword = "Athlete123!";
    var athleteUser = await userManager.FindByNameAsync(athleteEmail);
    if (athleteUser == null)
    {
        var newAthlete = new User
        {
            UserName = athleteEmail,
            Email = athleteEmail,
            EmailConfirmed = true,
            FirstName = "FootballResult",
            LastName = "Athlete",
            IdentificationNumber = "9876543210"
        };
        var createAthlete = await userManager.CreateAsync(newAthlete, athletePassword);
        if (createAthlete.Succeeded)
        {
            await userManager.AddToRoleAsync(newAthlete, "Athelete");
        }
    }
}

