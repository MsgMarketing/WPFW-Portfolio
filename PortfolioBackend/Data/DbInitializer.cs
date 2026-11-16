using Microsoft.AspNetCore.Identity;

namespace PortfolioBackend.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = ["Gast", "Editor", "Admin"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Lokale testaccounts voor Editor en Admin (wachtwoorden via config/User Secrets)
        var editorEmail = configuration["SeedUsers:EditorEmail"] ?? "editor@portfolio.local";
        var editorPassword = configuration["SeedUsers:EditorPassword"];

        if (!string.IsNullOrWhiteSpace(editorPassword) && await userManager.FindByEmailAsync(editorEmail) == null)
        {
            var editorUser = new IdentityUser { UserName = editorEmail, Email = editorEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(editorUser, editorPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(editorUser, "Editor");
            }
        }

        var adminEmail = configuration["SeedUsers:AdminEmail"] ?? "admin@portfolio.local";
        var adminPassword = configuration["SeedUsers:AdminPassword"];

        if (!string.IsNullOrWhiteSpace(adminPassword) && await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}