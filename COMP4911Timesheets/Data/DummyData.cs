using System;
using System.Collections.Generic;
using System.Linq;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using COMP4911Timesheets.Controllers;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace COMP4911Timesheets.Data
{
    public class DummyData
    {
        public static async void InitializeAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<Employee>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                //context.Database.Migrate();

                // Look for any teams.
                if (context.Employees.Any())
                {
                    return;   // DB has already been seeded
                }

                var password = "Password123!";

                var applicationRoles = GetApplicationRoles().ToArray();
                foreach (var applicationRole in applicationRoles)
                {
                    await roleManager.CreateAsync(applicationRole);
                }

                var admins = GetAdminEmployees().ToArray();
                foreach (var admin in admins)
                {
                    await userManager.CreateAsync(admin, password);
                }

                var approversAndSupervisors = GetSupervisorAndApproverEmployees(context).ToArray();
                foreach (var approverAndSupervisor in approversAndSupervisors)
                {
                    await userManager.CreateAsync(approverAndSupervisor, password);
                }

                var normalEmployees = GetNormalEmployees(context).ToArray();
                foreach (var normalEmployee in normalEmployees)
                {
                    await userManager.CreateAsync(normalEmployee, password);
                }

                var employees = context.Employees.OrderBy(e => e.EmployeeId).ToList();
                foreach (var employee in employees)
                {
                    if (employee.EmployeeId == 1)
                    {
                        await userManager.AddToRoleAsync(employee, "AD");
                    }
                    else if (employee.EmployeeId == 2)
                    {
                        await userManager.AddToRolesAsync(employee, new List<string>() {
                            "HR", "PM", "RE", "RE", "RE", "RE", "RE", "TA", "LM", "TA", "LM"
                        });
                        await userManager.AddToRoleAsync(employee, "EM");
                    }
                    else if (employee.EmployeeId == 3)
                    {
                        await userManager.AddToRolesAsync(employee, new List<string>() {
                            "TA", "LM", "TA", "LM", "TA", "LM", "TA", "LM"
                        });
                        await userManager.AddToRoleAsync(employee, "EM");
                    }
                    else if (employee.EmployeeId == 4)
                    {
                        await userManager.AddToRolesAsync(employee, new List<string>() {
                            "PA", "TA", "LM", "TA", "LM", "TA", "LM", "TA", "LM", "TA", "LM"
                        });
                        await userManager.AddToRoleAsync(employee, "EM");
                    }
                    await userManager.AddToRoleAsync(employee, "EM");
                    await userManager.AddToRoleAsync(employee, "EM");
                    await userManager.AddToRoleAsync(employee, "EM");
                    await userManager.AddToRoleAsync(employee, "EM");
                }

                var payGrades = GetPayGrades().ToArray();
                context.PayGrades.AddRange(payGrades);
                context.SaveChanges();

                var employeePays = GetEmployeePays(context).ToArray();
                context.EmployeePays.AddRange(employeePays);
                context.SaveChanges();

                var projects = GetProjects().ToArray();
                context.Projects.AddRange(projects);
                context.SaveChanges();

                var workPackages = GetWorkPackages(context).ToArray();
                context.WorkPackages.AddRange(workPackages);
                context.SaveChanges();

                var managedWorkPackages = GetManagedWorkPackages(context).ToArray();
                context.WorkPackages.AddRange(managedWorkPackages);
                context.SaveChanges();

                var projectEmployees = GetProjectEmployees(context).ToArray();
                context.ProjectEmployees.AddRange(projectEmployees);
                context.SaveChanges();
            }
        }

        public static List<ApplicationRole> GetApplicationRoles()
        {
            List<ApplicationRole> applicationRoles = new List<ApplicationRole>()
            {
                new ApplicationRole
                {
                    Name="AD",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="HR",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="PM",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="PA",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="LM",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="TA",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="RE",
                    CreatedDate=new DateTime(2018, 1, 1)
                },
                new ApplicationRole
                {
                    Name="EM",
                    CreatedDate=new DateTime(2018, 1, 1)
                }
            };
            return applicationRoles;
        }

        public static List<Employee> GetAdminEmployees()
        {
            List<Employee> employees = new List<Employee>()
            {
                new Employee
                {
                    Email="cameron.lay@infosys.ca",
                    UserName="cameron.lay@infosys.ca",
                    FirstName="Cameron",
                    LastName="Lay",
                    Title=Employee.ADMIN,
                    CreatedTime=new DateTime(2018, 3, 29),
                    FlexTime=0,
                    VacationTime=104,
                    Status=Employee.CURRENTLY_EMPLOYEED
                }, // AD
                new Employee
                {
                    Email="edmund.ham@infosys.ca",
                    UserName="edmund.ham@infosys.ca",
                    FirstName="Edmund",
                    LastName="Ham",
                    Title=Employee.HR_MANAGER,
                    CreatedTime=new DateTime(2018, 4, 23),
                    FlexTime=0,
                    VacationTime=96,
                    Status=Employee.CURRENTLY_EMPLOYEED
                } // HR, PM, RE
            };
            return employees;
        }

        public static List<Employee> GetSupervisorAndApproverEmployees(ApplicationDbContext context)
        {
            List<Employee> employees = new List<Employee>()
            {
                new Employee
                {
                    Email="ruize.ma@infosys.ca",
                    UserName="ruize.ma@infosys.ca",
                    FirstName="Ruize",
                    LastName="Ma",
                    Title=Employee.LINE_MANAGER,
                    CreatedTime=new DateTime(2018, 12, 25),
                    FlexTime=0,
                    VacationTime=32,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 2).First().Id
                }, // PM, PA
                new Employee
                {
                    Email="yang.tong@infosys.ca",
                    UserName="yang.tong@infosys.ca",
                    FirstName="Yang",
                    LastName="Tong",
                    Title=Employee.SUPERVISOR,
                    CreatedTime=new DateTime(2019, 1, 1),
                    FlexTime=0,
                    VacationTime=32,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 2).First().Id
                } // PM, PA, RE
            };
            return employees;
        }

        public static List<Employee> GetNormalEmployees(ApplicationDbContext context)
        {
            List<Employee> employees = new List<Employee>()
            {
                new Employee
                {
                    Email="bruce.link@infosys.ca",
                    UserName="bruce.link@infosys.ca",
                    FirstName="Bruce",
                    LastName="Link",
                    Title=Employee.BUSINESS_ANALYST,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 3).First().Id
                },
                new Employee
                {
                    Email="garth.nelson@infosys.ca",
                    UserName="garth.nelson@infosys.ca",
                    FirstName="Garth",
                    LastName="Nelson",
                    Title=Employee.TECHNICAL_WRITER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 4).First().Id
                },
                new Employee
                {
                    Email="danny.diiorio@infosys.ca",
                    UserName="danny.diiorio@infosys.ca",
                    FirstName="Danny",
                    LastName="Di Iorio",
                    Title=Employee.SOFTWARE_ARCHITECT,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 4).First().Id
                }, // PA
                new Employee
                {
                    Email="tony.pacheco@infosys.ca",
                    UserName="tony.pacheco@infosys.ca",
                    FirstName="Tony",
                    LastName="Pacheco",
                    Title=Employee.SOFTWARE_DEVELOPER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 4).First().Id
                }, // RE
                new Employee
                {
                    Email="yipan.wu@infosys.ca",
                    UserName="yipan.wu@infosys.cs",
                    FirstName="Yipan",
                    LastName="Wu",
                    Title=Employee.SOFTWARE_TESTER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 3).First().Id
                }, // RE
                new Employee
                {
                    Email="david.lee@infosys.ca",
                    UserName="david.lee@infosys.ca",
                    FirstName="David",
                    LastName="Lee",
                    Title=Employee.SENIOR_SOFTWARE_DEVELOPER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 3).First().Id
                },
                new Employee
                {
                    Email="andrew.le@infosys.ca",
                    UserName="andrew.le@infosys.ca",
                    FirstName="Andrew",
                    LastName="Le",
                    Title=Employee.JUNIOR_SOFTWARE_DEVELOPER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 4).First().Id
                },
                new Employee
                {
                    Email="noah.seltzer@infosys.ca",
                    UserName="noah.seltzer@infosys.ca",
                    FirstName="Noah",
                    LastName="Seltzer",
                    Title=Employee.INTERMEDIATE_SOFTWARE_DEVELOPER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 4).First().Id
                },
                new Employee
                {
                    Email="felix.lin@infosys.ca",
                    UserName="felix.lin@infosys.ca",
                    FirstName="Felix",
                    LastName="Lin",
                    Title=Employee.UI_DESIGNER,
                    CreatedTime=new DateTime(2019, 1, 2),
                    FlexTime=0,
                    VacationTime=24,
                    Status=Employee.CURRENTLY_EMPLOYEED,
                    SupervisorId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    ApproverId=context.Employees.Where(e => e.EmployeeId == 3).First().Id
                }
            };

            return employees;
        }

        public static List<PayGrade> GetPayGrades()
        {
            List<PayGrade> payGrades = new List<PayGrade>
            {
                new PayGrade
                {
                    PayLevel=PayGrade.P1,
                    Cost=10,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.P2,
                    Cost=20,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.P3,
                    Cost=30,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.P4,
                    Cost=40,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.P5,
                    Cost=50,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.P6,
                    Cost=60,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.SS,
                    Cost=80,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.DS,
                    Cost=90,
                    Year=DateTime.Now.Year
                },
                new PayGrade
                {
                    PayLevel=PayGrade.JS,
                    Cost=70,
                    Year=DateTime.Now.Year
                }
            };
            return payGrades;
        }

        public static List<EmployeePay> GetEmployeePays(ApplicationDbContext context)
        {

            List<EmployeePay> employeePays = new List<EmployeePay>
            {
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                    PayGradeId=context.PayGrades.Find(6).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    PayGradeId=context.PayGrades.Find(6).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    PayGradeId=context.PayGrades.Find(5).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    PayGradeId=context.PayGrades.Find(5).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    PayGradeId=context.PayGrades.Find(3).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    PayGradeId=context.PayGrades.Find(3).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    PayGradeId=context.PayGrades.Find(2).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    PayGradeId=context.PayGrades.Find(1).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                },
                new EmployeePay
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    PayGradeId=context.PayGrades.Find(2).PayGradeId,
                    AssignedDate=DateTime.Now.AddDays(-31),
                    Status=EmployeePay.VALID
                }
            };
            return employeePays;
        }

        public static List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>
            {
                new Project
                {
                    ProjectCode="010",
                    Name="HR Reserved",
                    Description="I need descriptions.",
                    CostingProposal=0,
                    OriginalBudget=0,
                    MarkupRate = 0,
                    Status=Project.INTERNAL
                }
            };
            return projects;
        }

        public static List<ProjectEmployee> GetProjectEmployees(ApplicationDbContext context)
        {
            List<ProjectEmployee> projectEmployees = new List<ProjectEmployee>
            {
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_MANAGER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_ASSISTANT
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.RESPONSIBLE_ENGINEER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },

                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.RESPONSIBLE_ENGINEER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },

                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.RESPONSIBLE_ENGINEER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },

                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.RESPONSIBLE_ENGINEER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.RESPONSIBLE_ENGINEER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.EMPLOYEE
                }
            };
            return projectEmployees;
        }

        public static List<WorkPackage> GetWorkPackages(ApplicationDbContext context)
        {
            List<WorkPackage> workPackages = new List<WorkPackage>
            {
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageCode="SICK",
                    Name="Sick Leave",
                    Description="Sick Leave itself is description",
                    Contractor="Contractor",
                    Purpose="Purpose",
                    Input="Input",
                    Output="Output",
                    Activity="Activity"
                },
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageCode="VACN",
                    Name="Vacation",
                    Description="Vacation is good",
                    Contractor="Contractor",
                    Purpose="Purpose",
                    Input="Input",
                    Output="Output",
                    Activity="Activity"
                },
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageCode="SHOL",
                    Name="Static Holidays",
                    Description="Yay!",
                    Contractor="Contractor",
                    Purpose="Purpose",
                    Input="Input",
                    Output="Output",
                    Activity="Activity"
                },
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageCode="FLEX",
                    Name="Flex Hour",
                    Description="Flex hour",
                    Contractor="Contractor",
                    Purpose="Purpose",
                    Input="Input",
                    Output="Output",
                    Activity="Activity"
                }
            };
            return workPackages;
        }

        public static List<WorkPackage> GetManagedWorkPackages(ApplicationDbContext context)
        {
            List<WorkPackage> workPackages = new List<WorkPackage>
            {
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(1).ProjectId,
                    WorkPackageCode="00000",
                    Name="Management",
                    Description="This is for management of the project",
                    Contractor="Contractor",
                    Purpose="Purpose",
                    Input="Input",
                    Output="Output",
                    Activity="Activity"
                }
            };
            return workPackages;
        }
    }
}