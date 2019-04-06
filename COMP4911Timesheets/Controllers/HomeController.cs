using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Authorization;
using COMP4911Timesheets.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace COMP4911Timesheets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        public HomeController (
            ApplicationDbContext context,
            UserManager<Employee> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(this.User);
            var rejectedSheets = await _context.Timesheets.Where(t => t.Status == Timesheet.REJECTED_NEED_RESUBMISSION).Where(t => t.EmployeeId == currentUserId).CountAsync();

            var employeeApprovalNeeded = await _context.Employees.Where(e => e.ApproverId == currentUserId).ToListAsync();

            int timesheetsNeeded = 0;
            foreach (var employee in employeeApprovalNeeded) {
                timesheetsNeeded += await _context.Timesheets.Where(t => t.EmployeeId == employee.Id).Where(ts => ts.Status == Timesheet.SUBMITTED_NOT_APPROVED).CountAsync();
            }
            ViewData["TimesheetsNeeded"] = timesheetsNeeded;
            ViewData["RejectedSheets"] = rejectedSheets;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
            //this is a change
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
