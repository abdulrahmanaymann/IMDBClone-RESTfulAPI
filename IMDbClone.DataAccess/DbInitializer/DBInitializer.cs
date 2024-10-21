using IMDbClone.Common;
using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IMDbClone.DataAccess.DbInitializer
{
    public class DBInitializer : IDBInitializer
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DBInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        public async Task Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Roles.User).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.User)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Admin)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "ezio",
                    Email = "ezio@gmail.com",
                    Name = "Ezio Auditore"
                }, "Admin123!").GetAwaiter().GetResult();


                ApplicationUser? user = await _context.ApplicationUsers
                            .FirstOrDefaultAsync(u => u.Email == "ezio@gmail.com");

                _userManager.AddToRoleAsync(user, Roles.Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}