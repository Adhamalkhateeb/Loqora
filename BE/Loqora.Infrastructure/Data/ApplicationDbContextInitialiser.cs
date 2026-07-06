using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Loqora.Domain.Identity;
using Loqora.Infrastructure.Identity;

namespace Loqora.Infrastructure.Data;

internal class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
AppDbContext context,
UserManager<AppUser> userManager,
RoleManager<AppRole> roleManager
)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<AppRole> _roleManager = roleManager;

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var adminRole = new AppRole { Name = nameof(Role.Admin) };
        var staffRole = new AppRole { Name = nameof(Role.Staff) };
        var guestRole = new AppRole { Name = nameof(Role.Guest) };

        if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await _roleManager.CreateAsync(adminRole);
        }

        if (_roleManager.Roles.All(r => r.Name != staffRole.Name))
        {
            await _roleManager.CreateAsync(staffRole);
        }

        if (_roleManager.Roles.All(r => r.Name != guestRole.Name))
        {
            await _roleManager.CreateAsync(guestRole);
        }

        var admin = AppUser.Create("Admin", "", "admin@localhost.com");
        admin.EmailConfirmed = true;


        if (_userManager.Users.All(u => u.Email != admin.Email))
        {
            await _userManager.CreateAsync(admin, "P@ssword1234");

            if (!string.IsNullOrWhiteSpace(adminRole.Name))
            {
                await _userManager.AddToRolesAsync(admin, [adminRole.Name]);
            }
        }
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}