using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Product_Inventory.Models;

namespace Product_Inventory.Data
{
	public class DbInitializer
	{
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
		{
			_db = db;
			_roleManager = roleManager;
			_userManager = userManager;
		}
		public void Seed()
		{
            //Applying any remaining migrations
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }

            //Checking if roles already exist in Database
            if (!_db.Roles.Any(role => role.Name=="admin" || role.Name=="user"))
            {
                //Seeding roles
                var adminRole = new IdentityRole("admin");
                var userRole = new IdentityRole("user");
                _roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
                _roleManager.CreateAsync(userRole).GetAwaiter().GetResult();
            }

            //Checking if the admin user already exists in Database
            if (!_db.Users.Any(user => user.Email == "admin@gmail.com"))
            {
                //Seeding Admin User
                var adminUser = new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };
                var result = _userManager.CreateAsync(adminUser, "Admin123@").GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    //Assigning admin role to user admin@gmail.com
                    var adminUserEntry = _userManager.FindByEmailAsync("admin@gmail.com").GetAwaiter().GetResult(); //User to whom the admin role is to be assigned
                    if (adminUserEntry != null)
                    {
                        var res = _userManager.AddToRoleAsync(adminUserEntry, "admin").GetAwaiter().GetResult();
                    }
                }
            }
        }
	}
}

