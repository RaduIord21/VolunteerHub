using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
                Contact = contact
            };
            return organizationManager.AddOrganization(organization);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName, long? organizationId = null)
        {
            var userManager = serviceProvider.GetService<UserManager<User>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new User
                {
                    UserName = UserName,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                    
                };
                await userManager.CreateAsync(user,"Parola123!");
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<User>>();

            if (userManager == null)
            {
                throw new Exception("userManager is null");
            }

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

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
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@test.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdministratorRole);

                var organizationId = EnsureOrganization(serviceProvider, "Org1","Aici", "Pronto");
                // allowed user can create and edit contacts that they create
                var coordinatorId = await EnsureUser(serviceProvider, testUserPw, "manager@contoso.com");
                await EnsureRole(serviceProvider, coordinatorId, Constants.CoordinatorRole);

                SeedDB(context, coordinatorId);
            }
        }
    }
}
