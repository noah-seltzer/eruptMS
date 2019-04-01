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

namespace COMP4911Timesheets.Controllers
{
    public class ApproveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private static int INVALID = 0;
        private static int NOT_SUBMITTED_NOT_APPROVED = 1;
        private static int SUBMITTED_NOT_APPROVED = 2;
        private static int SUBMITTED_APPROVED = 3;
        private static int REJECTED_NEED_RESUBMISSION = 4;

        public ApproveController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Approve
        public async Task<IActionResult> Index()
        {
            var Employees = _context.Employees.Include(e => e.Approver).Include(e => e.Supervisor).Include(e => e.Timesheets);
            List<Employee> temp = await Employees.ToListAsync();
            List<Employee> supervisedEmployee = new List<Employee>();
            //if (_context.Employees.Find(_userManager.GetUserAsync(this.User)) == Employee.ADMIN)
            if(_context.Employees.Find(_userManager.GetUserId(this.User)).Title == Employee.ADMIN)
            {
                return View(await Employees.ToListAsync());
            }
            foreach (Employee e in temp)
            {
                if (e.ApproverId == _userManager.GetUserId(this.User))
                {
                    supervisedEmployee.Add(e);
                }
            }
            return View(supervisedEmployee);
        }

        // GET: Approve/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Approver)
                .Include(e => e.Supervisor)
                .Include(e => e.Timesheets)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        
        
        // POST: Approve/Delete/5
        public async Task<IActionResult> TimesheetView(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            /**
            int x = 0;
            Int32.TryParse(id, out x);
            */
            var timesheet = await _context.Timesheets
                .Include(e => e.TimesheetRows)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            var projects = await _context.Projects.ToListAsync();
            var packages = await _context.WorkPackages.ToListAsync();
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        
        public async Task<IActionResult> Approval(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var timesheet = await _context.Timesheets.FirstOrDefaultAsync(m => m.TimesheetId == id);
            timesheet.Status = SUBMITTED_APPROVED;
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
            timesheet.Status = SUBMITTED_APPROVED;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int TimesheetId)
        {
            if (TimesheetId == 0)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets.FindAsync(TimesheetId);
            if (timesheet == null)
            {
                return NotFound();
            }
            await Reject(TimesheetId, timesheet);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int TimesheetId, [Bind("Comments")] Timesheet timesheet)
        {
            var timesheetToBeChanged = await _context.Timesheets.FindAsync(TimesheetId);
            timesheetToBeChanged.Status = REJECTED_NEED_RESUBMISSION;
            timesheetToBeChanged.Comments = timesheet.Comments;
            _context.Update(timesheetToBeChanged);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
