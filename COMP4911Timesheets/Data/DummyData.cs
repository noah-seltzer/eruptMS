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
                            "HR", "PM", "RE"
                        });
                    }
                    else if (employee.EmployeeId == 3)
                    {
                        await userManager.AddToRolesAsync(employee, new List<string>() {
                            "PM", "PA"
                        });
                    }
                    else if (employee.EmployeeId == 4)
                    {
                        await userManager.AddToRolesAsync(employee, new List<string>() {
                            "PM", "PA", "RE"
                        });
                    }
                    else if (employee.EmployeeId == 7)
                    {
                        await userManager.AddToRoleAsync(employee, "PA");
                    }
                    else if (employee.EmployeeId == 8)
                    {
                        await userManager.AddToRoleAsync(employee, "RE");
                    }
                    else if (employee.EmployeeId == 9)
                    {
                        await userManager.AddToRoleAsync(employee, "RE");
                    }
                    await userManager.AddToRoleAsync(employee, "EM");
                }

                var payGrades = GetPayGrades().ToArray();
                context.PayGrades.AddRange(payGrades);
                context.SaveChanges();

                var employeePays = GetEmployeePays(context).ToArray();
                context.EmployeePays.AddRange(employeePays);
                context.SaveChanges();

                var signatures = GetSignatures(context).ToArray();
                context.Signatures.AddRange(signatures);
                context.SaveChanges();

                var projects = GetProjects().ToArray();
                context.Projects.AddRange(projects);
                context.SaveChanges();

                var projectReports = GetProjectReports(context).ToArray();
                context.ProjectReports.AddRange(projectReports);
                context.SaveChanges();

                var projectEmployees = GetProjectEmployees(context).ToArray();
                context.ProjectEmployees.AddRange(projectEmployees);
                context.SaveChanges();

                var workPackages = GetWorkPackages(context).ToArray();
                context.WorkPackages.AddRange(workPackages);
                context.SaveChanges();

                var childWorkPackages = GetChildWorkPackages(context).ToArray();
                context.WorkPackages.AddRange(childWorkPackages);
                context.SaveChanges();

                var managedWorkPackages = GetManagedWorkPackages(context).ToArray();
                context.WorkPackages.AddRange(managedWorkPackages);
                context.SaveChanges();

                var budgets = GetBudgets(context).ToArray();
                context.Budgets.AddRange(budgets);
                context.SaveChanges();

                var workPackageReports = GetWorkPacakgeReports(context).ToArray();
                context.WorkPackageReports.AddRange(workPackageReports);
                context.SaveChanges();

                var workPackageEmployees = GetWorkPackageEmployees(context).ToArray();
                context.WorkPackageEmployees.AddRange(workPackageEmployees);
                context.SaveChanges();

                var timesheets = GetTimesheets(context).ToArray();
                context.Timesheets.AddRange(timesheets);
                context.SaveChanges();

                var timesheetRows = GetTimesheetRows(context).ToArray();
                context.TimesheetRows.AddRange(timesheetRows);
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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
                    VacationTime=0,
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

        // public static List<Credential> GetCredentials(ApplicationDbContext context)
        // {
        //     List<Credential> credentials = new List<Credential>
        //     {

        //     };
        //     return credentials;
        // }

        public static List<Signature> GetSignatures(ApplicationDbContext context)
        {
            List<Signature> signatures = new List<Signature>
            {
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    PassPhrase="Employee Two",
                    HashedSignature=Utility.HashEncrypt("Employee Two"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    PassPhrase="Employee Three",
                    HashedSignature=Utility.HashEncrypt("Employee Three"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    PassPhrase="Employee Four",
                    HashedSignature=Utility.HashEncrypt("Employee Four"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    PassPhrase="Employee Five",
                    HashedSignature=Utility.HashEncrypt("Employee Five"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    PassPhrase="Employee Six",
                    HashedSignature=Utility.HashEncrypt("Employee Six"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    PassPhrase="Employee Seven",
                    HashedSignature=Utility.HashEncrypt("Employee Seven"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    PassPhrase="Employee Eight",
                    HashedSignature=Utility.HashEncrypt("Employee Eight"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    PassPhrase="Employee Nine",
                    HashedSignature=Utility.HashEncrypt("Employee Nine"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    PassPhrase="Employee Ten",
                    HashedSignature=Utility.HashEncrypt("Employee Ten"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    PassPhrase="Employee Eleven",
                    HashedSignature=Utility.HashEncrypt("Employee Eleven"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    PassPhrase="Employee Twelve",
                    HashedSignature=Utility.HashEncrypt("Employee Twelve"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                },
                new Signature
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    PassPhrase="Employee Thirteen",
                    HashedSignature=Utility.HashEncrypt("Employee Thirteen"),
                    CreatedTime=DateTime.Now.AddDays(-31),
                    Status=Signature.VALID
                }
            };
            return signatures;
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
                    Status=Project.INTERNAL
                },
                new Project
                {
                    ProjectCode="1205",
                    Name="BCIT Construction",
                    Description="Another description?",
                    CostingProposal=50000.00,
                    OriginalBudget=60000.00,
                    Status=Project.ONGOING
                }
            };
            return projects;
        }

        public static List<ProjectReport> GetProjectReports(ApplicationDbContext context)
        {
            List<ProjectReport> projectReports = new List<ProjectReport>
            {

            };
            return projectReports;
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
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_ASSISTANT
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_ASSISTANT
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_MANAGER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_MANAGER
                },
                new ProjectEmployee {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Status=ProjectEmployee.CURRENTLY_WORKING,
                    Role=ProjectEmployee.PROJECT_ASSISTANT
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
                    Name="SHOL, I forgot what it is",
                    Description="I can't explain",
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
                },
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    WorkPackageCode="AB",
                    Name="Buying the Land",
                    Description="We should buy the land first to build BCIT",
                    Contractor="BCIT",
                    Purpose="To build BCIT",
                    Input="Nothing",
                    Output="Nothing",
                    Activity="Something"
                }
            };
            return workPackages;
        }

        public static List<WorkPackage> GetChildWorkPackages(ApplicationDbContext context)
        {
            List<WorkPackage> workPackages = new List<WorkPackage>
            {
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    WorkPackageCode="CD",
                    Name="Borrow money from the banks",
                    Description="We should have the money to buy the land",
                    Contractor="BCIT",
                    Purpose="To build BCIT",
                    Input="Input can be anything",
                    Output="Output could be anything",
                    Activity="Something may be activity",
                    ParentWorkPackageId=context.WorkPackages.Where(wp => wp.WorkPackageCode == "AB").First().WorkPackageId
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
                },
                new WorkPackage
                {
                    ProjectId=context.Projects.Find(2).ProjectId,
                    WorkPackageCode="00000",
                    Name="Management",
                    Description="This is for management of the project",
                    Contractor="Contractor",
                    Purpose="Management",
                    Input="Input",
                    Output="Output",
                    Activity="Activity"
                }
            };
            return workPackages;
        }

        public static List<Budget> GetBudgets(ApplicationDbContext context)
        {
            List<Budget> budgets = new List<Budget>
            {
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(1).PayGradeId,
                    Hour=100,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(2).PayGradeId,
                    Hour=50,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    Hour=40,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(3).PayGradeId,
                    Hour=50,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    Hour=40,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(5).PayGradeId,
                    Hour=800,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(2).PayGradeId,
                    Hour=50,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(3).PayGradeId,
                    Hour=50,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    Hour=20,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ESTIMATE
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(2).PayGradeId,
                    Hour=12,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(2).PayGradeId,
                    Hour=12,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(3).PayGradeId,
                    Hour=57,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(1).PayGradeId,
                    Hour=40,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(3).PayGradeId,
                    Hour=23,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(4).PayGradeId,
                    Hour=54,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                },
                new Budget
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    PayGradeId=context.PayGrades.Find(5).PayGradeId,
                    Hour=48,
                    Status=Budget.VALID,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-21), DayOfWeek.Friday)),
                    Type=Budget.ACTUAL
                }
            };
            return budgets;
        }

        public static List<WorkPackageReport> GetWorkPacakgeReports(ApplicationDbContext context)
        {
            List<WorkPackageReport> workPackageReports = new List<WorkPackageReport>
            {
                new WorkPackageReport
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    Status=WorkPackageReport.VALID,
                    Comments="This is comment part",
                    StartingPercentage=0.0,
                    CompletedPercentage=(52.0/190),
                    CostStarted=0.0,
                    CostFinished=640.0,
                    WorkAccomplished="Maybe something",
                    WorkAccomplishedNP="HMM",
                    Problem="None",
                    ProblemAnticipated="None"
                },
                new WorkPackageReport
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    Status=WorkPackageReport.VALID,
                    Comments="This is comment part",
                    StartingPercentage=0.0,
                    CompletedPercentage=(69.0/120),
                    CostStarted=0.0,
                    CostFinished=1950.0,
                    WorkAccomplished="nope",
                    WorkAccomplishedNP="nope",
                    Problem="noooo",
                    ProblemAnticipated="nah"
                },
                new WorkPackageReport
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    Status=WorkPackageReport.VALID,
                    Comments="This is comment part",
                    StartingPercentage=0.0,
                    CompletedPercentage=(125.0/890),
                    CostStarted=0.0,
                    CostFinished=5770.0,
                    WorkAccomplished="nope",
                    WorkAccomplishedNP="nope",
                    Problem="noooo",
                    ProblemAnticipated="nah"
                }
            };
            return workPackageReports;
        }

        public static List<WorkPackageEmployee> GetWorkPackageEmployees(ApplicationDbContext context)
        {
            List<WorkPackageEmployee> workPackageEmployees = new List<WorkPackageEmployee>
            {
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                // new WorkPackageEmployee
                // {
                //     WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                //     EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                //     Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                //     Status=WorkPackageEmployee.CURRENTLY_WORKING
                // },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                // new WorkPackageEmployee
                // {
                //     WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                //     EmployeeId=context.Employees.Where(e => e.EmployeeId == 1).First().Id,
                //     Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                //     Status=WorkPackageEmployee.CURRENTLY_WORKING
                // },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                // new WorkPackageEmployee
                // {
                //     WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                //     EmployeeId=context.Employees.Find(1).EmployeeId,
                //     Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                //     Status=WorkPackageEmployee.CURRENTLY_WORKING
                // },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                // new WorkPackageEmployee
                // {
                //     WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                //     EmployeeId=context.Employees.Find(1).EmployeeId,
                //     Role=WorkPackageEmployee.RESPONSIBLE_ENGINEER,
                //     Status=WorkPackageEmployee.CURRENTLY_WORKING
                // },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 5).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 6).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 11).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                },
                new WorkPackageEmployee
                {
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    Role=WorkPackageEmployee.ASSIGNED_EMPLOYEE,
                    Status=WorkPackageEmployee.CURRENTLY_WORKING
                }
            };
            return workPackageEmployees;
        }

        public static List<Timesheet> GetTimesheets(ApplicationDbContext context)
        {
            List<Timesheet> timesheets = new List<Timesheet>
            {
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    EmployeePayId=context.EmployeePays.Find(1).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 2).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    EmployeePayId=context.EmployeePays.Find(2).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 3).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    EmployeePayId=context.EmployeePays.Find(3).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 4).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    EmployeePayId=context.EmployeePays.Find(6).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 7).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    EmployeePayId=context.EmployeePays.Find(7).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 8).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    EmployeePayId=context.EmployeePays.Find(8).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 9).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    EmployeePayId=context.EmployeePays.Find(9).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 10).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    EmployeePayId=context.EmployeePays.Find(11).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 12).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    EmployeePayId=context.EmployeePays.Find(12).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-14), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 13).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 2).First().Id,
                    EmployeePayId=context.EmployeePays.Find(1).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 2).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 3).First().Id,
                    EmployeePayId=context.EmployeePays.Find(2).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 3).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 4).First().Id,
                    EmployeePayId=context.EmployeePays.Find(3).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 4).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 7).First().Id,
                    EmployeePayId=context.EmployeePays.Find(6).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 7).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 8).First().Id,
                    EmployeePayId=context.EmployeePays.Find(7).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 8).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 9).First().Id,
                    EmployeePayId=context.EmployeePays.Find(8).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 9).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 10).First().Id,
                    EmployeePayId=context.EmployeePays.Find(9).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 10).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 12).First().Id,
                    EmployeePayId=context.EmployeePays.Find(11).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 12).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                },
                new Timesheet
                {
                    EmployeeId=context.Employees.Where(e => e.EmployeeId == 13).First().Id,
                    EmployeePayId=context.EmployeePays.Find(12).EmployeePayId,
                    WeekEnding=Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday),
                    WeekNumber=Utility.GetWeekNumberByDate(Utility.GetPreviousWeekday(DateTime.Today.AddDays(-7), DayOfWeek.Friday)),
                    SignatureId=context.Signatures.Where(s => s.EmployeeId == context.Employees.Where(e => e.EmployeeId == 13).First().Id).First().SignatureId,
                    Status=Timesheet.SUBMITTED_APPROVED
                }
            };
            return timesheets;
        }

        public static List<TimesheetRow> GetTimesheetRows(ApplicationDbContext context)
        {
            List<TimesheetRow> timesheetRows = new List<TimesheetRow>
            {
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(1).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(1).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(1).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(1).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(1).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(2).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(2).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(2).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(2).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(2).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=3,
                    TueHour=4,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=1,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(2).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=5,
                    TueHour=4,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=7,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(3).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(3).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes="Vacation to Germany"
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(3).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(3).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(3).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=4,
                    WedHour=2,
                    ThuHour=7,
                    FriHour=3,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(3).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=4,
                    WedHour=6,
                    ThuHour=1,
                    FriHour=5,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(4).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(4).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(4).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(4).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(4).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(5).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(5).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(5).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(5).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(5).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=1,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(5).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=7,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=4,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=2,
                    TueHour=3,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=5,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=5,
                    TueHour=5,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=2,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(6).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=1,
                    TueHour=0,
                    WedHour=4,
                    ThuHour=8,
                    FriHour=1,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(7).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(7).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(7).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(7).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(7).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(8).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(8).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(8).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(8).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(8).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(9).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(9).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(9).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(9).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(9).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=0,
                    WedHour=4,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(9).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=8,
                    WedHour=4,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(10).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(10).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(10).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(10).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(10).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(11).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(11).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(11).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(11).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(11).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=3,
                    TueHour=4,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=1,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(11).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=5,
                    TueHour=4,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=7,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(12).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(12).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes="Vacation to Germany"
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(12).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(12).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(12).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(7).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=4,
                    WedHour=2,
                    ThuHour=7,
                    FriHour=3,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(12).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=4,
                    WedHour=6,
                    ThuHour=1,
                    FriHour=5,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(13).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(13).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(13).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(13).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(13).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(14).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(14).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(14).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(14).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(14).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=1,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(14).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=7,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=4,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=2,
                    TueHour=3,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=5,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=5,
                    TueHour=5,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=2,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(15).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(8).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=1,
                    TueHour=0,
                    WedHour=4,
                    ThuHour=8,
                    FriHour=1,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(16).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(16).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(16).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(16).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(16).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(17).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(17).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(17).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(17).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(17).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=8,
                    WedHour=8,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(18).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(1).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(18).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(2).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=8,
                    FriHour=8,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(18).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(3).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(18).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(4).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=0,
                    WedHour=0,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(18).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(5).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=8,
                    TueHour=0,
                    WedHour=4,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                },
                new TimesheetRow
                {
                    TimesheetId=context.Timesheets.Find(18).TimesheetId,
                    WorkPackageId=context.WorkPackages.Find(6).WorkPackageId,
                    SatHour=0,
                    SunHour=0,
                    MonHour=0,
                    TueHour=8,
                    WedHour=4,
                    ThuHour=0,
                    FriHour=0,
                    Notes=""
                }
            };
            return timesheetRows;
        }
    }
}