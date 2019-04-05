using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using System.Security.Claims;
using COMP4911Timesheets.ViewModels;

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles = "LM,AD")]
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
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            List<Employee> employees;
            if (User.IsInRole(role: "AD"))
            {
                employees = await _context.Employees
                    .Include(e => e.Approver)
                    .Include(e => e.Supervisor)
                    .Include(e => e.ProjectEmployees)
                    .OrderBy(s => s.EmployeeId)
                    .ToListAsync();
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
            var timesheet = await _context.Timesheets.FirstOrDefaultAsync(m => m.TimesheetId == id);
            timesheet.Status = Timesheet.SUBMITTED_APPROVED;
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
            var timesheet = await _context.Timesheets.FirstOrDefaultAsync(m => m.TimesheetId == id);
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

        public async Task<IActionResult> ProjectIndex()
        {
            var projects = await _context.Projects
                .Include(p => p.WorkPackages)
                .ToListAsync();
            var lineManagerManagements = new List<LineManagerManagement>();
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
                lineManagerManagements.Add
                (
                    new LineManagerManagement
                    {
                        Project = project
                    }
                );
            }
            return View(lineManagerManagements);
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
                    .FirstOrDefaultAsync();
                if (employeePay.Employee.SupervisorId == currentUser.Id && projectEmployee == null)
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
            var projectEmployees = await _context.ProjectEmployees
                .Where(pe => pe.ProjectId == id)
                .Where(pe => pe.Employee.SupervisorId == _userManager.GetUserId(User))
                .Include(pe => pe.Employee)
                .Include(pe => pe.Project)
                .ToListAsync();
            var project = await _context.Projects.FindAsync(id);
            LineManagerManagement lineManagerManagement = new LineManagerManagement
            {
                ProjectEmployees = projectEmployees,
                Project = project
            };
            return View(lineManagerManagement);
        }

        public async Task<IActionResult> RemoveEmployee(int id, LineManagerManagement lineManagerManagement)
        {
            var projectEmployee = await _context.ProjectEmployees
                .Include(pe => pe.Employee)
                .FirstOrDefaultAsync(pe => pe.ProjectEmployeeId == id);
            if (ModelState.IsValid)
            {
                if (projectEmployee.Role == ProjectEmployee.PROJECT_MANAGER)
                {
                    await _userManager.RemoveFromRoleAsync(projectEmployee.Employee, ApplicationRole.PM);
                }
                if (projectEmployee.Role == ProjectEmployee.PROJECT_ASSISTANT)
                {
                    await _userManager.RemoveFromRoleAsync(projectEmployee.Employee, ApplicationRole.PA);
                }
                _context.Remove(projectEmployee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(RemoveEmployees), new { id = projectEmployee.ProjectId });
        }

    }
}
