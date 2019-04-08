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
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = new List<Employee>();

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = await _context.Employees
                    .Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString))
                    .Include(e => e.Approver)
                    .Include(e => e.Supervisor)
                    .OrderBy(s => s.EmployeeId).ToListAsync();
            }
            else
            {
                employees = await _context.Employees
                    .Include(e => e.Approver)
                    .Include(e => e.Supervisor)
                    .OrderBy(s => s.EmployeeId).ToListAsync();
            }

            var employeeManagements = new List<EmployeeManagement>();
            foreach (var employee in employees)
            {
                employeeManagements.Add
                (
                    new EmployeeManagement
                    {
                        Employee = employee,
                        EmployeePay = await _context.EmployeePays.Where(ep => ep.EmployeeId == employee.Id).Where(ep => ep.Status == EmployeePay.VALID).Include(ep => ep.PayGrade).FirstOrDefaultAsync()
                    }
                );
            }
            return View(employeeManagements);
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
            var employeeManagement = new EmployeeManagement
            {
                Employee = employee,
                EmployeePay = await _context.EmployeePays.Where(ep => ep.EmployeeId == id).Where(ep => ep.Status == EmployeePay.VALID).Include(ep => ep.PayGrade).FirstOrDefaultAsync()
            };
            return View(employeeManagement);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            var employees = await _context.Employees.ToListAsync();
            var jobTitles = Employee.JobTitles.ToList();
            var payLevels = await _context.PayGrades.Where(pg => pg.Year == DateTime.Now.Year).OrderBy(pg => pg.PayLevel).ToListAsync();

            ViewData["ApproverId"] = new SelectList(employees, "Id", "Email", _userManager.GetUserId(this.User));
            ViewData["SupervisorId"] = new SelectList(employees, "Id", "Email", _userManager.GetUserId(this.User));
            ViewData["Title"] = new SelectList(jobTitles, "Key", "Value");
            ViewData["PayLevel"] = new SelectList(payLevels, "PayGradeId", "PayLevel");

            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeManagement employeeManagement)
        {
            if (ModelState.IsValid)
            {
                if (_context.Employees.Where(e => e.Email == employeeManagement.Employee.Email).FirstOrDefault() != null)
                {
                    ViewBag.ErrorMessage = "There is an employee already with the email you entered.";
                    return await Create();
                }
                employeeManagement.Employee.Status = Employee.CURRENTLY_EMPLOYEED;
                employeeManagement.Employee.CreatedTime = DateTime.Now;
                employeeManagement.Employee.UserName = employeeManagement.Employee.Email;
                await _userManager.CreateAsync(employeeManagement.Employee, defaultPassword);

                employeeManagement.EmployeePay.EmployeeId = employeeManagement.Employee.Id;
                employeeManagement.EmployeePay.AssignedDate = DateTime.Now;
                employeeManagement.EmployeePay.Status = EmployeePay.VALID;
                await _context.AddAsync(employeeManagement.EmployeePay);
                await _context.SaveChangesAsync();

                if (employeeManagement.Employee.Title == Employee.HR_MANAGER)
                {
                    await _userManager.AddToRoleAsync(employeeManagement.Employee, ApplicationRole.HR);
                }
                if (employeeManagement.Employee.Title == Employee.ADMIN)
                {
                    await _userManager.AddToRoleAsync(employeeManagement.Employee, ApplicationRole.AD);
                }

                var internalProjects = await _context.Projects
                    .Where(p => p.Status == Project.INTERNAL)
                    .Include(p => p.WorkPackages)
                    .ToListAsync();
                foreach (Project internalProject in internalProjects)
                {
                    var workPackages = internalProject.WorkPackages;
                    foreach (WorkPackage workPackage in workPackages)
                    {
                        if (workPackage.WorkPackageCode == "00000")
                        {
                            continue;
                        }
                        var projectEmployee = new ProjectEmployee
                        {
                            EmployeeId = employeeManagement.Employee.Id,
                            ProjectId = internalProject.ProjectId,
                            WorkPackageId = workPackage.WorkPackageId,
                            Status = ProjectEmployee.CURRENTLY_WORKING,
                            Role = ProjectEmployee.EMPLOYEE
                        };
                        _context.Add(projectEmployee);
                        await _context.SaveChangesAsync();
                        await _userManager.AddToRoleAsync(employeeManagement.Employee, ApplicationRole.EM);
                    }
                }

                var supervisor = _context.Employees.Find(employeeManagement.Employee.SupervisorId);
                await _userManager.AddToRoleAsync(supervisor, ApplicationRole.LM);
                var approver = _context.Employees.Find(employeeManagement.Employee.ApproverId);
                await _userManager.AddToRoleAsync(approver, ApplicationRole.TA);

                return RedirectToAction(nameof(Index));
            }
            return View(employeeManagement);
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

            var employees = await _context.Employees.ToListAsync();
            var jobTitles = Employee.JobTitles.ToList();
            var statuses = Employee.Statuses.ToList();
            var payLevels = await _context.PayGrades.Where(pg => pg.Year == DateTime.Now.Year).OrderBy(pg => pg.PayLevel).ToListAsync();
            var employeePay = await _context.EmployeePays.Where(ep => ep.EmployeeId == id).Where(ep => ep.Status == EmployeePay.VALID).FirstOrDefaultAsync();

            ViewData["ApproverId"] = new SelectList(employees, "Id", "Email", employee.ApproverId);
            ViewData["SupervisorId"] = new SelectList(employees, "Id", "Email", employee.SupervisorId);
            ViewData["Title"] = new SelectList(jobTitles, "Key", "Value", employee.Title);
            ViewData["Status"] = new SelectList(statuses, "Key", "Value", employee.Status);
            ViewData["PayLevel"] = new SelectList(payLevels, "PayGradeId", "PayLevel", employeePay.PayGradeId);

            var employeeManagement = new EmployeeManagement
            {
                Employee = employee,
                EmployeePay = employeePay,
            };

            return View(employeeManagement);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EmployeeManagement employeeManagement)
        {
            if (id != employeeManagement.Employee.Id)
            {
                return NotFound();
            }

            if (employeeManagement.Employee.SupervisorId == id || employeeManagement.Employee.ApproverId == id)
            {
                ViewBag.ErrorMessage = "Supervisor and Approver can't be themselves";
                return await Edit(id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var something = _userManager.GetUserId(this.User);
                    if (id == _userManager.GetUserId(this.User) && employeeManagement.Employee.Title == Employee.ADMIN)
                    {
                        ViewBag.ErrorMessage = "You cannot assign yourself to admin";
                        return await Edit(id);
                    }

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
                    employeeToBeEdited.FirstName = employeeManagement.Employee.FirstName;
                    employeeToBeEdited.LastName = employeeManagement.Employee.LastName;
                    employeeToBeEdited.Title = employeeManagement.Employee.Title;
                    employeeToBeEdited.FlexTime = employeeManagement.Employee.FlexTime;
                    employeeToBeEdited.VacationTime = employeeManagement.Employee.VacationTime;
                    employeeToBeEdited.Status = employeeManagement.Employee.Status;
                    employeeToBeEdited.ApproverId = employeeManagement.Employee.ApproverId;
                    employeeToBeEdited.SupervisorId = employeeManagement.Employee.SupervisorId;
                    employeeToBeEdited.UserName = employeeManagement.Employee.Email;
                    employeeToBeEdited.Email = employeeManagement.Employee.Email;
                    employeeToBeEdited.PhoneNumber = employeeManagement.Employee.PhoneNumber;
                    if (!string.IsNullOrEmpty(employeeManagement.passPhrase))
                    {
                        var datetime = DateTime.Now;
                        var newSignature = new Signature
                        {
                            HashedSignature = Utility.HashEncrypt(employeeManagement.passPhrase + datetime),
                            CreatedTime = datetime,
                            Status = Signature.VALID,
                            EmployeeId = id
                        };

                        var oldSig = _context.Signatures
                            .Where(s => s.EmployeeId == id)
                            .FirstOrDefault();

                        if (oldSig == null)
                            _context.Signatures.Add(newSignature);
                        else
                        {
                            oldSig.CreatedTime = newSignature.CreatedTime;
                            oldSig.HashedSignature = newSignature.HashedSignature;
                            _context.Signatures.Update(oldSig);
                        }

                        await _context.SaveChangesAsync();
                    }
                    await _userManager.UpdateAsync(employeeToBeEdited);

                    var employeePayToBeDisabled = _context.EmployeePays.Find(employeeManagement.EmployeePay.EmployeePayId);
                    employeePayToBeDisabled.Status = EmployeePay.INVALID;
                    _context.Update(employeePayToBeDisabled);
                    await _context.SaveChangesAsync();

                    var newEmployeePay = new EmployeePay
                    {
                        AssignedDate = DateTime.Now,
                        Status = EmployeePay.VALID,
                        EmployeeId = employeeToBeEdited.Id,
                        PayGradeId = employeeManagement.EmployeePay.PayGradeId
                    };
                    _context.Add(newEmployeePay);
                    await _context.SaveChangesAsync();

                    var approver = _context.Employees.Find(employeeManagement.Employee.ApproverId);
                    await _userManager.AddToRoleAsync(approver, ApplicationRole.TA);
                    var supervisor = _context.Employees.Find(employeeManagement.Employee.SupervisorId);
                    await _userManager.AddToRoleAsync(supervisor, ApplicationRole.LM);
                    if (employeeManagement.Employee.Title == Employee.HR_MANAGER)
                    {
                        await _userManager.AddToRoleAsync(employeeToBeEdited, ApplicationRole.HR);
                    }
                    if (employeeManagement.Employee.Title == Employee.ADMIN)
                    {
                        await _userManager.AddToRoleAsync(employeeToBeEdited, ApplicationRole.AD);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeManagement.Employee.Id))
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

            return View(employeeManagement);
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

            var employeePay = await _context.EmployeePays.Where(ep => ep.EmployeeId == id).Where(ep => ep.Status == EmployeePay.VALID).Include(ep => ep.PayGrade).FirstOrDefaultAsync();

            var employeeManagement = new EmployeeManagement
            {
                Employee = employee,
                EmployeePay = employeePay
            };

            return View(employeeManagement);
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
