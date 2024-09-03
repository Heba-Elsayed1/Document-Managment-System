using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DataSeeder
    {
        public static async Task Seed(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger<DataSeeder> logger)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Roles
            if (!roleManager.Roles.Any())
            {
                logger.LogInformation("Seeding roles...");
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" });
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "User", NormalizedName = "USER" });
            }

            // Seed Users
            if (!userManager.Users.Any())
            {
                // Create Admin
                logger.LogInformation("Seeding admin user...");
                User admin = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    PhoneNumber = "123-456-7890",
                    FullName = "Admin User",
                    Gender = "Admin",
                    NID = "1234567890987"
                };

                var resultAdmin = await userManager.CreateAsync(admin, "AdminPassword123!");
                if (resultAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                // Create Regular User
                logger.LogInformation("Seeding regular user...");
                User regularUser = new User
                {
                    UserName = "regular.user",
                    Email = "user@example.com",
                    PhoneNumber = "098-765-4321",
                    FullName = "Regular User",
                    Gender = "User",
                    NID = "5432123467890"
                };

                var resultUser = await userManager.CreateAsync(regularUser, "UserPassword123!");
                if (resultUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }

                // Seed Workspaces
                logger.LogInformation("Seeding workspace...");
                Workspace userWorkspace = new Workspace
                {
                    Name = "User Workspace",
                    UserId = regularUser.Id
                };

                context.Workspaces.Add(userWorkspace);
                await context.SaveChangesAsync(); // Save to get userWorkspace.Id

                // Seed Folders
                logger.LogInformation("Seeding folders...");
                Folder folder1 = new Folder
                {
                    Name = "Folder1",
                    WorkspaceId = userWorkspace.Id,
                    IsPublic = true
                };
                context.Folders.Add(folder1);

                Folder folder2 = new Folder
                {
                    Name = "Folder2",
                    WorkspaceId = userWorkspace.Id,
                    IsPublic = false
                };
                context.Folders.Add(folder2);

                await context.SaveChangesAsync(); // Save to get folder Ids

                // Log folder IDs for debugging purposes
                logger.LogInformation($"Folder1 ID: {folder1.Id}");
                logger.LogInformation($"Folder2 ID: {folder2.Id}");

                // Seed Documents
                logger.LogInformation("Seeding documents...");
                Document document1 = new Document
                {
                    Name = "Document1",
                    Type = "pdf",
                    CreationDate = DateTime.Now,
                    FolderId = folder1.Id
                };
                context.Documents.Add(document1);

                Document document2 = new Document
                {
                    Name = "Document2",
                    Type = "docx",
                    CreationDate = DateTime.Now,
                    FolderId = folder2.Id
                };
                context.Documents.Add(document2);

                await context.SaveChangesAsync(); // Final save to commit all changes

                logger.LogInformation("Finished seeding data.");
            }
        }
    }
}