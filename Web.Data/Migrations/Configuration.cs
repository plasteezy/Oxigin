using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Web.Data.Model;

namespace Web.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            //SeedAdminUser(context);
        }

        private static void SeedAdminUser(ApplicationDbContext context)
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = new ApplicationUser
            {
                UserName = "andrew@example.com",
                Email = "andrew@example.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                DateCreated = DateTime.Now,
                LastLoginDate = new DateTime(1990, 1, 1),
                Name = "Andrew"
            };

            manager.Create(user, "password");

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("SuperAdmin"));

            manager.AddToRole(user.Id, "SuperAdmin");
        }
    }
}
