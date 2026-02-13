using EMS.Api.Models;

namespace EMS.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.Roles.Any())
        {
            db.Roles.AddRange(
                new Role { RoleName = "Admin" },
                new Role { RoleName = "Staff" },
                new Role { RoleName = "Manager" },
                new Role { RoleName = "HR" }
            );

            db.SaveChanges();
        }
    }
}
