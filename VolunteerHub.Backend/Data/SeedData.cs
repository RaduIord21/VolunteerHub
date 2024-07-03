using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VolunteerHub.Backend.Helpers;
using VolunteerHub.DataModels.Models;


namespace VolunteerHub.Backend.Data
{
    public static class SeedData
    {
        private static long EnsureOrganization(IServiceProvider serviceProvider, string organizationName, string adress, string contact)
        {
            var organizationManager = serviceProvider.GetService<OrganizationManager>();
            Organization organization = new Organization
            {
                Name = organizationName,
                Adress = adress,
                Contact = contact,
                CreatedAt = DateTime.UtcNow
            };
            return organizationManager.AddOrganization(organization);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName, string email, long? organizationId = null)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();
            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = UserName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0

                };
                await userManager.CreateAsync(user, "Parola123!");
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityRole> EnsureRole(IServiceProvider serviceProvider,
                                                                       string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityRole? IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = new IdentityRole(role);
                await roleManager.CreateAsync(IR);
            }
            else
            {
                IR = await roleManager.FindByNameAsync(role);
            }
            return IR;
        }
        public static async Task<IdentityResult> EnsureUserToRole(IServiceProvider serviceProvider, string uid, IdentityRole role)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();
            IdentityResult IR;
            if (userManager == null)
            {
                throw new Exception("userManager is null");
            }

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role.Name);

            return IR;
        }

        public static void SeedDB(VolunteerHubContext context, string adminID)
        {
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            context.Users.AddRange(
                new User
                {
                    UserName = "Volunteer1",
                    Email = "volunteer1@test.com"
                },
                new User
                {
                    UserName = "Volunteer2",
                    Email = "volunteer2@test.com"
                }
             );
            context.SaveChanges();
        }
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new VolunteerHubContext(
                serviceProvider.GetRequiredService<DbContextOptions<VolunteerHubContext>>()))
            {
                var organizationId = EnsureOrganization(serviceProvider, "Admin", "N/A", "admin");
                var adminID = await EnsureUser(serviceProvider, testUserPw, "Admin", "admin@test.com", organizationId);
                var role = await EnsureRole(serviceProvider, Constants.AdministratorRole);
                await EnsureUserToRole(serviceProvider, adminID, role);


                organizationId = EnsureOrganization(serviceProvider, "Org1", "Aici", "Pronto");
                // allowed user can create and edit contacts that they create
                var coordinatorId = await EnsureUser(serviceProvider, testUserPw, "TestCoordonator", "organizer@test.com", organizationId);

                role = await EnsureRole(serviceProvider, Constants.CoordinatorRole);
                await EnsureUserToRole(serviceProvider, coordinatorId, role);

                var volunteerId = await EnsureUser(serviceProvider, testUserPw, "TestVoluntar", "volunteer@test.com", organizationId);
                role = await EnsureRole(serviceProvider, Constants.VolunteerRole);
                await EnsureUserToRole(serviceProvider, volunteerId, role);
                SeedDB(context, coordinatorId);
            }
        }
    }
}
