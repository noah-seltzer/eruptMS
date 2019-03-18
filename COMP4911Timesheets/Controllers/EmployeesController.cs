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

namespace COMP4911Timesheets.Controllers
{
    [Authorize(Roles = "AD,HR")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly string defaultPassword = "Password123!";

        public EmployeesController(
            ApplicationDbContext context,
            UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees
                .Include(e => e.Approver)
                .Include(e => e.Supervisor)
                .OrderBy(s => s.EmployeeId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Approver)
                .Include(e => e.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            var employees = _context.Employees.ToList();
            var jobTitles = Employee.JobTitles.ToList();

            ViewData["ApproverId"] = new SelectList(employees, "Id", "Email", _userManager.GetUserId(this.User));
            ViewData["SupervisorId"] = new SelectList(employees, "Id", "Email", _userManager.GetUserId(this.User));
            ViewData["Title"] = new SelectList(jobTitles, "Key", "Value");

            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FirstName,LastName,Title,CreatedTime,FlexTime,VacationTime,Status,ApproverId,SupervisorId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.Status = Employee.CURRENTLY_EMPLOYEED;
                employee.CreatedTime = DateTime.Now;
                employee.UserName = employee.Email;
                employee.SupervisorId = employee.SupervisorId;
                employee.ApproverId = employee.ApproverId;
                await _userManager.CreateAsync(employee, defaultPassword);

                if (employee.Title == Employee.HR_MANAGER)
                {
                    await _userManager.AddToRoleAsync(employee, ApplicationRole.HR);
                }
                if (employee.Title == Employee.ADMIN)
                {
                    await _userManager.AddToRoleAsync(employee, ApplicationRole.AD);
                }

                var supervisor = _context.Employees.Find(employee.SupervisorId);
                await _userManager.AddToRoleAsync(supervisor, ApplicationRole.LM);
                var approver = _context.Employees.Find(employee.ApproverId);
                await _userManager.AddToRoleAsync(approver, ApplicationRole.TA);

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var employees = _context.Employees.ToList();
            var jobTitles = Employee.JobTitles.ToList();
            var statuses = Employee.Statuses.ToList();

            ViewData["ApproverId"] = new SelectList(employees, "Id", "Email", employee.ApproverId);
            ViewData["SupervisorId"] = new SelectList(employees, "Id", "Email", employee.SupervisorId);
            ViewData["Title"] = new SelectList(jobTitles, "Key", "Value", employee.Title);
            ViewData["Status"] = new SelectList(statuses, "Key", "Value", employee.Status);

            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmployeeId,FirstName,LastName,Title,CreatedTime,FlexTime,VacationTime,Status,ApproverId,SupervisorId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (employee.SupervisorId == id || employee.ApproverId == id)
            {
                return BadRequest("Supervisor and Approver can't be themselves");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldEmployee = await _context.Employees.FindAsync(id);
                    if (oldEmployee != null)
                    {
                        var oldApprover = await _context.Employees.FindAsync(oldEmployee.ApproverId);
                        if (oldApprover != null)
                        {
                            await _userManager.RemoveFromRoleAsync(oldApprover, ApplicationRole.TA);
                        }
                        var oldSupervisor = await _context.Employees.FindAsync(oldEmployee.SupervisorId);
                        if (oldSupervisor != null)
                        {
                            await _userManager.RemoveFromRoleAsync(oldSupervisor, ApplicationRole.LM);
                        }
                        if (oldEmployee.Title == Employee.HR_MANAGER)
                        {
                            await _userManager.RemoveFromRoleAsync(oldEmployee, ApplicationRole.HR);
                        }
                        if (oldEmployee.Title == Employee.ADMIN)
                        {
                            await _userManager.RemoveFromRoleAsync(oldEmployee, ApplicationRole.AD);
                        }
                    }

                    var employeeToBeEdited = await _userManager.FindByIdAsync(id);
                    employeeToBeEdited.FirstName = employee.FirstName;
                    employeeToBeEdited.LastName = employee.LastName;
                    employeeToBeEdited.Title = employee.Title;
                    employeeToBeEdited.FlexTime = employee.FlexTime;
                    employeeToBeEdited.VacationTime = employee.VacationTime;
                    employeeToBeEdited.Status = employee.Status;
                    employeeToBeEdited.ApproverId = employee.ApproverId;
                    employeeToBeEdited.SupervisorId = employee.SupervisorId;
                    employeeToBeEdited.UserName = employee.Email;
                    employeeToBeEdited.Email = employee.Email;
                    employeeToBeEdited.PhoneNumber = employee.PhoneNumber;
                    await _userManager.UpdateAsync(employeeToBeEdited);

                    var approver = _context.Employees.Find(employee.ApproverId);
                    await _userManager.AddToRoleAsync(approver, ApplicationRole.TA);
                    var supervisor = _context.Employees.Find(employee.SupervisorId);
                    await _userManager.AddToRoleAsync(supervisor, ApplicationRole.LM);
                    if (employee.Title == Employee.HR_MANAGER)
                    {
                        await _userManager.AddToRoleAsync(employeeToBeEdited, ApplicationRole.HR);
                    }
                    if (employee.Title == Employee.ADMIN)
                    {
                        await _userManager.AddToRoleAsync(employeeToBeEdited, ApplicationRole.AD);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Approver)
                .Include(e => e.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_userManager.GetUserId(this.User) == id)
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            var approver = _context.Employees.Find(employee.ApproverId);
            await _userManager.RemoveFromRoleAsync(approver, ApplicationRole.TA);
            var supervisor = _context.Employees.Find(employee.SupervisorId);
            await _userManager.RemoveFromRoleAsync(supervisor, ApplicationRole.LM);

            if (employee.Title == Employee.HR_MANAGER)
            {
                await _userManager.RemoveFromRoleAsync(employee, ApplicationRole.HR);
            }
            if (employee.Title == Employee.ADMIN)
            {
                await _userManager.RemoveFromRoleAsync(employee, ApplicationRole.AD);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
