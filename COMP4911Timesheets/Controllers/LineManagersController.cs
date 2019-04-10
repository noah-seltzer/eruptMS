using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using COMP4911Timesheets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles = "LM,AD,TA")]
    public class LineManagersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public LineManagersController(
            ApplicationDbContext context,
            UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString, string approverSearch)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            List<Employee> employees;
            List<Employee> approvalList;

            if (User.IsInRole(role: "AD"))
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    employees = await _context.Employees
                        .Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString))
                        .Include(e => e.Approver)
                        .Include(e => e.Supervisor)
                        .Include(e => e.ProjectEmployees)
                        .OrderBy(s => s.EmployeeId).ToListAsync();
                }
                else
                {
                    employees = await _context.Employees
                        .Include(e => e.Approver)
                        .Include(e => e.Supervisor)
                        .Include(e => e.ProjectEmployees)
                        .OrderBy(s => s.EmployeeId)
                        .ToListAsync();
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    employees = await _context.Employees
                        .Where(e => e.SupervisorId == currentUser.Id)
                        .Where(s => s.LastName.Contains(searchString)
                                           || s.FirstName.Contains(searchString))
                        .Include(e => e.Approver)
                        .Include(e => e.Supervisor)
                        .Include(e => e.ProjectEmployees)
                        .OrderBy(s => s.EmployeeId).ToListAsync();
                }
                else
                {
                    employees = await _context.Employees
                        .Include(e => e.Approver)
                        .Include(e => e.Supervisor)
                        .Include(e => e.ProjectEmployees)
                        .OrderBy(s => s.EmployeeId)
                        .Where(e => e.SupervisorId == currentUser.Id)
                        .ToListAsync();
                }
            }

            var lineManagerManagements = new List<LineManagerManagement>();
            foreach (var employee in employees)
            {
                var timesheets = await _context.Timesheets
                    .Where(t => t.EmployeeId == employee.Id)
                    .Where(t => t.Status == Timesheet.SUBMITTED_NOT_APPROVED)
                    .ToListAsync();
                employee.Timesheets = timesheets;
                lineManagerManagements.Add
                (
                    new LineManagerManagement
                    {
                        Employee = employee
                    }
                );
            }


            if (!String.IsNullOrEmpty(approverSearch))
            {
                approvalList = await _context.Employees
                    .Where(e => e.SupervisorId != currentUser.Id && e.ApproverId == currentUser.Id)
                    .Where(s => s.LastName.Contains(approverSearch)
                                       || s.FirstName.Contains(approverSearch))
                    .Include(e => e.Approver)
                    .Include(e => e.Supervisor)
                    .Include(e => e.ProjectEmployees)
                    .OrderBy(s => s.EmployeeId)
                    .ToListAsync();
            }
            else
            {
                approvalList = await _context.Employees
                    .Include(e => e.Approver)
                    .Include(e => e.Supervisor)
                    .Include(e => e.ProjectEmployees)
                    .OrderBy(s => s.EmployeeId)
                    .Where(e => e.SupervisorId != currentUser.Id && e.ApproverId == currentUser.Id)
                    .ToListAsync();
            }


            var approverManagements = new List<LineManagerManagement>();
            foreach (var employee in approvalList)
            {
                var timesheets = await _context.Timesheets
                    .Where(t => t.EmployeeId == employee.Id)
                    .Where(t => t.Status == Timesheet.SUBMITTED_NOT_APPROVED)
                    .ToListAsync();
                var employeePay = await _context.EmployeePays
                    .Where(ep => ep.EmployeeId == employee.Id)
                    .Where(ep => ep.Status == EmployeePay.VALID)
                    .Include(ep => ep.PayGrade)
                    .FirstOrDefaultAsync();
                employee.Timesheets = timesheets;
                approverManagements.Add
                (
                    new LineManagerManagement
                    {
                        Employee = employee,
                        EmployeePay = employeePay
                    }
                );
            }
            ViewData["ApproverList"] = approverManagements;

            var requestManagements = new List<LineManagerManagement>();
            var projects = await _context.Projects
                .Include(p => p.WorkPackages)
                .ToListAsync();
            foreach (var project in projects)
            {
                var projectRequests = await _context.ProjectRequests
                .Where(pr => pr.ProjectId == project.ProjectId)
                .Where(pr => pr.Status == ProjectRequest.VALID)
                .Where(pr => pr.AmountRequested != 0)
                .Include(pr => pr.PayGrade)
                .Include(pr => pr.Project)
                .ToListAsync();
                project.ProjectRequests = projectRequests;
                requestManagements.Add
                (
                    new LineManagerManagement
                    {
                        Project = project
                    }
                );
            }
            ViewData["ProjectRequest"] = requestManagements;

            return View(lineManagerManagements);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Timesheets)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            var timesheets = await _context.Timesheets
                .Where(t => t.Status == Timesheet.SUBMITTED_NOT_APPROVED)
                .Where(t => t.EmployeeId == employee.Id)
                .ToListAsync();
            employee.Timesheets = timesheets;
            var lineManagerManagement = new LineManagerManagement
            {
                Employee = employee
            };

            return View(lineManagerManagement);
        }

        public async Task<IActionResult> TimesheetView(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            try
            {
                ViewBag.ErrorMessage = TempData["TimesheetMessage"].ToString();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.ToString());
            }
            var projects = await _context.Projects.ToListAsync();
            var workpackages = await _context.WorkPackages.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.TimesheetRows)
                .FirstOrDefaultAsync(t => t.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }
            var lineManagerManagement = new LineManagerManagement
            {
                Timesheet = timesheet
            };

            return View(lineManagerManagement);
        }

        public async Task<IActionResult> Approval(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var timesheet = await _context.Timesheets.Where(m => m.TimesheetId == id).FirstOrDefaultAsync();
            var user = await _context.Employees.Where(e => e.Id == timesheet.EmployeeId).FirstOrDefaultAsync();
            var employeePay = await _context.EmployeePays.Where(ep => ep.EmployeeId == user.Id).Where(ep => ep.Status == EmployeePay.VALID).FirstOrDefaultAsync();
            var rows = await _context.TimesheetRows.Where(r => r.TimesheetId == id).ToListAsync();
            timesheet.Status = Timesheet.SUBMITTED_APPROVED;
            user.FlexTime = timesheet.FlexTime;

            foreach (var row in rows)
            {
                Budget budget = new Budget
                {
                    WorkPackageId = row.WorkPackageId,
                    WeekNumber = timesheet.WeekNumber,
                    Hour = row.SatHour + row.SunHour + row.MonHour + row.TueHour + row.WedHour + row.ThuHour + row.FriHour,
                    PayGradeId = employeePay.PayGradeId,
                    PayGrade = employeePay.PayGrade,
                    Status = Budget.VALID,
                    Type = Budget.ACTUAL,
                    WorkPackage = row.WorkPackage
                };
                await _context.AddAsync(budget);
            }

            var vacTimesheetRow = await _context.TimesheetRows
                .Include(tr => tr.WorkPackage)
                .Where(tr => tr.TimesheetId == id)
                .Where(tr => tr.WorkPackage.WorkPackageCode == "VACN")
                .FirstOrDefaultAsync();
            if (vacTimesheetRow != null)
            {
                var vacCounter = 0.0;
                vacCounter += vacTimesheetRow.MonHour;
                vacCounter += vacTimesheetRow.TueHour;
                vacCounter += vacTimesheetRow.WedHour;
                vacCounter += vacTimesheetRow.ThuHour;
                vacCounter += vacTimesheetRow.FriHour;
                if (vacCounter > timesheet.Employee.VacationTime)
                {
                    TempData["TimesheetMessage"] = "Vacation time on the timesheet exceeds the time that the employee has.";
                    return RedirectToAction(nameof(TimesheetView), new { id = id });
                }
                timesheet.Employee.VacationTime -= vacCounter;
            }

            var sickTimesheetRow = await _context.TimesheetRows
                .Include(tr => tr.WorkPackage)
                .Where(tr => tr.TimesheetId == id)
                .Where(tr => tr.WorkPackage.WorkPackageCode == "SICK")
                .FirstOrDefaultAsync();
            if (sickTimesheetRow != null)
            {
                var sickCounter = 0.0;
                sickCounter += sickTimesheetRow.MonHour;
                sickCounter += sickTimesheetRow.TueHour;
                sickCounter += sickTimesheetRow.WedHour;
                sickCounter += sickTimesheetRow.ThuHour;
                sickCounter += sickTimesheetRow.FriHour;
                timesheet.Employee.SickLeave += sickCounter;
            }

            _context.Update(timesheet);
            await _userManager.UpdateAsync(user);

            _context.SaveChanges();
            await ApprovalConfirmed(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Approval")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovalConfirmed(int id)
        {
            //int x = 0;
            //Int32.TryParse(id, out x);
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var timesheet = await _context.Timesheets.Where(m => m.TimesheetId == id).FirstOrDefaultAsync();

            timesheet.Status = Timesheet.SUBMITTED_APPROVED;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(LineManagerManagement lineManagerManagement)
        {
            var timesheetToBeChanged = await _context.Timesheets.FindAsync(lineManagerManagement.Timesheet.TimesheetId);
            timesheetToBeChanged.Status = Timesheet.REJECTED_NEED_RESUBMISSION;
            timesheetToBeChanged.Comments = lineManagerManagement.Timesheet.Comments;
            _context.Update(timesheetToBeChanged);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewRequests(int id)
        {
            var project = await _context.Projects
                .Where(p => p.ProjectId == id)
                .FirstOrDefaultAsync();
            var projectRequests = await _context.ProjectRequests
                .Where(pr => pr.ProjectId == id)
                .Where(pr => pr.Status == ProjectRequest.VALID)
                .Where(pr => pr.AmountRequested != 0)
                .Include(pr => pr.PayGrade)
                .Include(pr => pr.Project)
                .ToListAsync();

            var employeeNum = new Dictionary<int, int>();
            // paygradeid & number of employees
            var currentUser = await _userManager.GetUserAsync(User);

            foreach (ProjectRequest projectRequest in projectRequests)
            {
                var employeePays = await _context.EmployeePays
                .Include(e => e.Employee)
                .Include(e => e.PayGrade)
                .Where(ep => ep.PayGradeId == projectRequest.PayGradeId)
                .Where(ep => ep.Status == EmployeePay.VALID)
                .ToListAsync();
                foreach (var employeePay in employeePays)
                {
                    var projectEmployee = await _context.ProjectEmployees
                        .Where(pe => pe.EmployeeId == employeePay.EmployeeId)
                        .Where(pe => pe.ProjectId == projectRequest.ProjectId)
                        .Where(pe => pe.Status == ProjectEmployee.CURRENTLY_WORKING)
                        .FirstOrDefaultAsync();
                    if ((employeePay.Employee.SupervisorId == currentUser.Id || User.IsInRole("AD")) && projectEmployee == null)
                    {
                        if (employeeNum.ContainsKey(employeePay.PayGradeId.GetValueOrDefault()))
                        {
                            employeeNum[employeePay.PayGradeId.GetValueOrDefault()] += 1;
                        }
                        else
                        {
                            employeeNum[employeePay.PayGradeId.GetValueOrDefault()] = 1;
                        }
                    }
                }
            }

            ViewData["num"] = employeeNum;

            project.ProjectRequests = projectRequests;
            LineManagerManagement lineManagerManagement = new LineManagerManagement
            {
                Project = project,
            };

            return View(lineManagerManagement);
        }

        public async Task<IActionResult> AssignEmployees(int id)
        {
            var projectRequest = await _context.ProjectRequests
                .Include(pr => pr.PayGrade)
                .Include(pr => pr.Project)
                .Where(pr => pr.ProjectRequestId == id)
                .FirstOrDefaultAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            var employeePays = await _context.EmployeePays
                .Include(e => e.Employee)
                .Include(e => e.PayGrade)
                .Where(ep => ep.PayGradeId == projectRequest.PayGradeId)
                .Where(ep => ep.Status == EmployeePay.VALID)
                .ToListAsync();
            var employees = new List<Employee>();
            foreach (var employeePay in employeePays)
            {
                var projectEmployee = await _context.ProjectEmployees
                    .Where(pe => pe.EmployeeId == employeePay.EmployeeId)
                    .Where(pe => pe.ProjectId == projectRequest.ProjectId)
                    .Where(pe => pe.Status == ProjectEmployee.CURRENTLY_WORKING)
                    .FirstOrDefaultAsync();
                if ((employeePay.Employee.SupervisorId == currentUser.Id || User.IsInRole("AD")) && projectEmployee == null)
                {
                    employees.Add(employeePay.Employee);
                }
            }
            ViewData["Employees"] = new SelectList(employees, "Id", "Email");
            LineManagerManagement lineManagerManagement = new LineManagerManagement
            {
                ProjectRequest = projectRequest,
                Employees = new List<Employee>
                {
                    new Employee
                    {
                    }
                }
            };
            return View(lineManagerManagement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEmployees(LineManagerManagement lineManagerManagement)
        {
            if (ModelState.IsValid)
            {
                var projectRequest = await _context.ProjectRequests.FindAsync(lineManagerManagement.ProjectRequest.ProjectRequestId);
                var currentUser = await _userManager.GetUserAsync(User);
                var employeePays = await _context.EmployeePays
                    .Include(e => e.Employee)
                    .Include(e => e.PayGrade)
                    .Where(ep => ep.PayGradeId == projectRequest.PayGradeId)
                    .Where(ep => ep.Status == EmployeePay.VALID)
                    .ToListAsync();
                var employees = new List<Employee>();
                foreach (var employeePay in employeePays)
                {
                    var projectEmployee = await _context.ProjectEmployees
                        .Where(pe => pe.EmployeeId == employeePay.EmployeeId)
                        .Where(pe => pe.ProjectId == projectRequest.ProjectId)
                        .Where(pe => pe.Status == ProjectEmployee.CURRENTLY_WORKING)
                        .FirstOrDefaultAsync();
                    if (employeePay.Employee.SupervisorId == currentUser.Id && projectEmployee == null)
                    {
                        employees.Add(employeePay.Employee);
                    }
                }
                ViewData["Employees"] = new SelectList(employees, "Id", "Email");
                if (projectRequest == null)
                {
                    return View(lineManagerManagement);
                }
                if (projectRequest.AmountRequested < lineManagerManagement.EmployeeIds.Count)
                {
                    ViewBag.ErrorMessage = "You cannot assign more employees than reqeusted amount.";
                    return View(lineManagerManagement);
                }
                // end of validation
                var projectEmployees = new List<ProjectEmployee>();
                foreach (var id in lineManagerManagement.EmployeeIds)
                {
                    projectEmployees.Add(new ProjectEmployee
                    {
                        EmployeeId = id,
                        ProjectId = projectRequest.ProjectId,
                        Status = ProjectEmployee.CURRENTLY_WORKING,
                        Role = ProjectEmployee.NOT_ASSIGNED
                    });
                }
                await _context.ProjectEmployees.AddRangeAsync(projectEmployees);
                await _context.SaveChangesAsync();
                projectRequest.AmountRequested -= lineManagerManagement.EmployeeIds.Count;
                _context.ProjectRequests.Update(projectRequest);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ViewRequests), new { id = lineManagerManagement.ProjectRequest.ProjectId });
        }

        public async Task<IActionResult> ChangeTA(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            LineManagerManagement lineManagerManagement = new LineManagerManagement
            {
                Employee = employee
            };
            var employees = await _context.Employees.ToListAsync();
            ViewData["Approvers"] = new SelectList(employees, "Id", "Email", employee.ApproverId);
            return View(lineManagerManagement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTA(LineManagerManagement lineManagerManagement)
        {
            if (ModelState.IsValid)
            {
                var employees = await _context.Employees.ToListAsync();
                var oldEmployee = _context.Employees.Find(lineManagerManagement.Employee.Id);

                if (lineManagerManagement.Employee.Id == lineManagerManagement.Employee.ApproverId)
                {
                    ViewData["Approvers"] = new SelectList(employees, "Id", "Email", oldEmployee.ApproverId);
                    ViewBag.ErrorMessage = "Timesheet Approver should not be the person self";
                    return View(lineManagerManagement);
                }
                var oldApprover = _context.Employees.Find(oldEmployee.ApproverId);
                await _userManager.RemoveFromRoleAsync(oldApprover, ApplicationRole.TA);
                var employeeToBeEdited = await _userManager.FindByIdAsync(lineManagerManagement.Employee.Id);
                employeeToBeEdited.ApproverId = lineManagerManagement.Employee.ApproverId;
                await _userManager.UpdateAsync(employeeToBeEdited);
                var newApprover = _context.Employees.Find(employeeToBeEdited.ApproverId);
                await _userManager.AddToRoleAsync(newApprover, ApplicationRole.TA);
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> RemoveEmployees(int id)
        {
            bool isAdmin = await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), "AD");

            var managerId = (await _context.ProjectEmployees
                .Where(pe => pe.ProjectId == id)
                .Where(pe => pe.Role == ProjectEmployee.PROJECT_MANAGER)
                .FirstOrDefaultAsync())
                .EmployeeId;

            var employeesPositions = await _context.ProjectEmployees
                .Where(pe => pe.ProjectId == id)
                .Where(pe => isAdmin || pe.Employee.SupervisorId == _userManager.GetUserId(User))
                .Where(pe => pe.Status == ProjectEmployee.CURRENTLY_WORKING)
                .GroupBy(pe => pe.EmployeeId)
                .Join(_context.Employees, g => g.Key, e => e.Id, (g, e) => new { E = e, PEs = g.ToList() })
                .ToListAsync();

            List<ProjectEmployee> PEs = new List<ProjectEmployee>();
            foreach (var g in employeesPositions)
            {
                g.PEs.Sort((a, b) => { return a.Role - b.Role; }); //so we get the most senior role
                var pe = g.PEs.First();
                pe.Employee = g.E;
                PEs.Add(pe);
            }

            PEs.Sort((a, b) => { return a.Role - b.Role; });

            LineManagerManagement lineManagerManagement = new LineManagerManagement
            {
                ProjectEmployees = PEs,
                Project = await _context.Projects.FindAsync(id)
            };
            return View(lineManagerManagement);
        }

        public async Task<IActionResult> RemoveEmployee(string id, int projectId, LineManagerManagement lineManagerManagement)
        {
            var projectEmployees = await _context.ProjectEmployees
                .Include(pe => pe.Employee)
                .Where(pe => pe.EmployeeId == id)
                .Where(pe => pe.ProjectId == projectId)
                .ToListAsync();
            lineManagerManagement.ProjectEmployee = new ProjectEmployee();
            if (ModelState.IsValid)
            {
                foreach (ProjectEmployee projectEmployee in projectEmployees)
                {
                    if (projectEmployee.Role == ProjectEmployee.PROJECT_MANAGER)
                    {
                        await _userManager.RemoveFromRoleAsync(projectEmployee.Employee, ApplicationRole.PM);
                    }
                    if (projectEmployee.Role == ProjectEmployee.PROJECT_ASSISTANT)
                    {
                        await _userManager.RemoveFromRoleAsync(projectEmployee.Employee, ApplicationRole.PA);
                    }
                    projectEmployee.Role = ProjectEmployee.NOT_ASSIGNED;
                    projectEmployee.Status = ProjectEmployee.NOT_WORKING;
                    lineManagerManagement.ProjectEmployee.ProjectId = projectEmployee.ProjectId;
                    _context.Update(projectEmployee);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(RemoveEmployees), new { id = lineManagerManagement.ProjectEmployee.ProjectId });
        }
    }
}