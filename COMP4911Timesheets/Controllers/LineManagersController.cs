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
    [Authorize(Roles = "LM")]
    public class LineManagersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly string defaultPassword = "Password123!";

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
            var employees = await _context.Employees
                .Include(e => e.Approver)
                .Include(e => e.Supervisor)
                .Include(e => e.ProjectEmployees)
                .OrderBy(s => s.EmployeeId)
                .Where(e => e.SupervisorId == currentUser.Id)
                .ToListAsync();
            var lineManagerManagements = new List<LineManagerManagement>();
            foreach (var employee in employees)
            {
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

        public async Task<IActionResult> ProjectIndex()
        {
            var projects = await _context.Projects
                .ToListAsync();
            var lineManagerManagements = new List<LineManagerManagement>();
            foreach (var project in projects)
            {
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
    }
}
