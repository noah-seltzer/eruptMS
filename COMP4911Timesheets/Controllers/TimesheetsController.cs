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
    public class TimesheetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public TimesheetsController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Timesheets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Timesheets.Include(t => t.Employee).Include(t => t.EmployeePay).Include(t => t.Signature);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Timesheets/Details/5(timesheetid)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var workpackages = await _context.WorkPackages.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .Include(t => t.EmployeePay.PayGrade)
                .Include(t => t.TimesheetRows)
                .Include(t => t.Employee.WorkPackageEmployees)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);

            //ViewData["wpstatus"] = timesheet.Employee.;
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // GET: Timesheets/Create
        public IActionResult Create()
        {
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id");
            //ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays, "EmployeePayId", "EmployeePayId");
            //ViewData["SignatureId"] = new SelectList(_context.Signatures, "SignatureId", "SignatureId");
            return View();
        }

        // POST: Timesheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimesheetId,WeekEnding,WeekNumber,FlexTime,Status,EmployeeId,EmployeePayId,SignatureId")] Timesheet timesheet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timesheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", timesheet.EmployeeId);
            ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays, "EmployeePayId", "EmployeePayId", timesheet.EmployeePayId);
            ViewData["SignatureId"] = new SelectList(_context.Signatures, "SignatureId", "SignatureId", timesheet.SignatureId);
            return View(timesheet);
        }

        // GET: Timesheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", timesheet.EmployeeId);
            ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays, "EmployeePayId", "EmployeePayId", timesheet.EmployeePayId);
            ViewData["SignatureId"] = new SelectList(_context.Signatures, "SignatureId", "SignatureId", timesheet.SignatureId);
            return View(timesheet);
        }

        // POST: Timesheets/Edit/5(timesheetid)
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimesheetId,WeekEnding,WeekNumber,ESignature,FlexTime,Status,EmployeePayId")] Timesheet timesheet)
        {
            if (id != timesheet.TimesheetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timesheet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimesheetExists(timesheet.TimesheetId))
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
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", timesheet.EmployeeId);
            //ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays.Where(e => e.EmployeeId == employeeid), "EmployeePayId", "EmployeePayId", timesheet.EmployeePayId);
            return View(timesheet);
        }

        // GET: Timesheets/Delete/5(timesheetid)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var workpackages = await _context.WorkPackages.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .Include(t => t.EmployeePay.PayGrade)
                .Include(t => t.TimesheetRows)
                .Include(t => t.Employee.WorkPackageEmployees)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // POST: Timesheets/Delete/5(timesheetid)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            var timesheetrows = timesheet.TimesheetRows;
            foreach (TimesheetRow tr in timesheetrows)
            {
                _context.TimesheetRows.Remove(tr);
            }
            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Timesheets/DeleteRow/5(timesheetrowid)
        public async Task<IActionResult> DeleteRow(int id)
        {
            var timesheetRow = await _context.TimesheetRows.FindAsync(id);
            _context.TimesheetRows.Remove(timesheetRow);
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }


        private bool TimesheetExists(int id)
        {
            return _context.Timesheets.Any(e => e.TimesheetId == id);
        }




    }
}
