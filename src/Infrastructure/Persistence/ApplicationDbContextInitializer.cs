using System.Reflection;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsRelational())
                await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedTenantsAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedDataAsync();
            context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static IEnumerable<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = string.Empty;
            var moduleDescription = string.Empty;

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fi in fields)
            {
                var propertyValue = fi.GetValue(null);

                if (propertyValue is not null)
                    allPermissions.Add((string)propertyValue);
            }
        }

        return allPermissions;
    }




    private async Task SeedTenantsAsync()
    {
        if (await context.Tenants.AnyAsync()) return;

        logger.LogInformation("Seeding tenants...");
        var tenants = new[]
        {
                new Tenant { Name = "Master", Description = "Master Site" },
                new Tenant { Name = "Slave", Description = "Slave Site" }
            };

        await context.Tenants.AddRangeAsync(tenants);
        await context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var adminRoleName = RoleName.Admin;
        var userRoleName = RoleName.Basic;

        if (await roleManager.RoleExistsAsync(adminRoleName)) return;

        logger.LogInformation("Seeding roles...");
        var administratorRole = new ApplicationRole(adminRoleName)
        {
            Description = "Admin Group",
            TenantId = (await context.Tenants.FirstAsync()).Id
        };
        var userRole = new ApplicationRole(userRoleName)
        {
            Description = "Basic Group",
            TenantId = (await context.Tenants.FirstAsync()).Id
        };

        await roleManager.CreateAsync(administratorRole);
        await roleManager.CreateAsync(userRole);

        var permissions = GetAllPermissions();

        foreach (var permission in permissions)
        {
            var claim = new Claim(ApplicationClaimTypes.Permission, permission);
            await roleManager.AddClaimAsync(administratorRole, claim);

            if (permission.StartsWith("Permissions.Products"))
            {
                await roleManager.AddClaimAsync(userRole, claim);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await userManager.Users.AnyAsync()) return;

        logger.LogInformation("Seeding users...");
        var adminUser = new ApplicationUser
        {
            UserName = UserName.Administrator,
            Provider = "Local",
            IsActive = true,
            TenantId = (await context.Tenants.FirstAsync()).Id,
            DisplayName = UserName.Administrator,
            Email = "admin@example.com",
            EmailConfirmed = true,
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80",
            LanguageCode="en-US",
            TimeZoneId= "Asia/Shanghai",
            TwoFactorEnabled = false
        };

        var demoUser = new ApplicationUser
        {
            UserName = UserName.Demo,
            IsActive = true,
            Provider = "Local",
            TenantId = (await context.Tenants.FirstAsync()).Id,
            DisplayName = UserName.Demo,
            Email = "demo@example.com",
            EmailConfirmed = true,
            LanguageCode = "de-DE",
            TimeZoneId = "Europe/Berlin",
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80"
        };

        await userManager.CreateAsync(adminUser, UserName.DefaultPassword);
        await userManager.AddToRoleAsync(adminUser, RoleName.Admin);

        await userManager.CreateAsync(demoUser, UserName.DefaultPassword);
        await userManager.AddToRoleAsync(demoUser, RoleName.Basic);
    }

    private async Task SeedDataAsync()
    {
        if (!await context.PicklistSets.AnyAsync())
        {

            logger.LogInformation("Seeding key values...");
            var keyValues = new[]
            {
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "initialization",
                    Text = "Initialization",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "processing",
                    Text = "Processing",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "pending",
                    Text = "Pending",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "done",
                    Text = "Done",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Brand,
                    Value = "Apple",
                    Text = "Apple",
                    Description = "Brand of production"
                },
                new PicklistSet
                {
                    Name = Picklist.Brand,
                    Value = "Google",
                    Text = "Google",
                    Description = "Brand of production"
                },
                new PicklistSet
                {
                    Name = Picklist.Brand,
                    Value = "Microsoft",
                    Text = "Microsoft",
                    Description = "Brand of production"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "EA",
                    Text = "EA",
                    Description = "Unit of product"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "KM",
                    Text = "KM",
                    Description = "Unit of product"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "PC",
                    Text = "PC",
                    Description = "Unit of product"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "L",
                    Text = "L",
                    Description = "Unit of product"
                }
            };

            await context.PicklistSets.AddRangeAsync(keyValues);
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {

            logger.LogInformation("Seeding products...");
            var products = new[]
            {
                new Product
                {
                    Brand = "Apple",
                    Name = "IPhone 13 Pro",
                    Description =
                    "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.",
                    Unit = "EA",
                    Price = 999.98m
                },
                new Product
                {
                    Brand = "Sony",
                    Name = "WH-1000XM4",
                    Description = "Sony WH-1000XM4 Wireless Noise-Canceling Over-Ear Headphones. Features industry-leading noise cancellation, up to 30 hours of battery life, touch sensor controls.",
                    Unit = "EA",
                    Price = 349.99m
                },
                new Product
                {
                    Brand = "Nintendo",
                    Name = "Switch OLED Model",
                    Description = "Nintendo Switch OLED Model console. Released October 2021. Features 7″ OLED screen, 64GB internal storage, enhanced audio, dock with wired LAN port.",
                    Unit = "EA",
                    Price = 349.99m
                },
                new Product
                {
                    Brand = "Apple",
                    Name = "MacBook Air M1",
                    Description = "Apple MacBook Air with M1 chip. Features 13.3″ Retina display, Apple M1 chip with 8‑core CPU, 8GB RAM, 256GB SSD storage, up to 18 hours of battery life.",
                    Unit = "EA",
                    Price = 999.99m
                }

            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}