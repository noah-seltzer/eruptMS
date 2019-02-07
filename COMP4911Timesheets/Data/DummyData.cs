using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.Data
{
    public class DummyData
    {
        public static async Task Initialize(ApplicationDbContext context,
                              UserManager<Employee> userManager,
                              RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            String adminId1 = "";

            string adminRole = "Admin";
            string adminDescription = "This is the administrator role";

            string employeeRole = "Employee";
            string employeeDescription = "This is an Employee role";

            string password = "P@$$w0rd";

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(adminRole, adminDescription, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(employeeRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(employeeRole, employeeDescription, DateTime.Now));
            }

            if (await userManager.FindByNameAsync("555") == null)
            {
                var user = new Employee
                {
                    UserName = "555",
                    EmployeeId = 555,
                    Email = "test@test.t",
                    FirstName = "John",
                    LastName = "Doe",
                    Title = 622,
                    CreatedTime = DateTime.Now,
                    FlexTime = 0,
                    VacationTime = 0,
                    Status = 1
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, employeeRole);
                }
            }

            if (await userManager.FindByNameAsync("001") == null)
            {
                var user = new Employee
                {
                    UserName = "001",
                    EmployeeId = 001,
                    Email = "admin@admin.a",
                    FirstName = "Jane",
                    LastName = "Doe",
                    Title = 0,
                    CreatedTime = DateTime.Now,
                    FlexTime = 0,
                    VacationTime = 0,
                    Status = 1
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, adminRole);
                }
                adminId1 = user.Id;
            }
        }
    }
}
