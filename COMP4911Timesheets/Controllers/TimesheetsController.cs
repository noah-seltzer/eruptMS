using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.Controllers
{
    public class TimesheetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private TimesheetRowsController timesheetRowsController;
        private static int employeeid = 2;
        public TimesheetsController(ApplicationDbContext context)
        {
            _context = context;
            timesheetRowsController = new TimesheetRowsController(context);
        }

        // GET: Timesheets
        public async Task<IActionResult> Index(int id)
        {
            if(id > 0){
                employeeid = id;
            }
            var timesheets = _context.Timesheets.Include(t => t.Employee).Include(t => t.EmployeePay).Where(t => t.EmployeeId == employeeid);
            var employees = await _context.Employees.ToListAsync();
            var payments = await _context.PayGrades.ToListAsync();
            var timesheetrows = await _context.TimesheetRows.ToListAsync();
            var employeepays = await _context.EmployeePays.ToListAsync();
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeid);
            ViewData["EmployeeName"] = employee.LastName;
            return View(await timesheets.ToListAsync());
        }

        // GET: Timesheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var employees = await _context.Employees.ToListAsync();
            var payments = await _context.PayGrades.ToListAsync();
            var employeepays = await _context.EmployeePays.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            var timesheetrows = await _context.TimesheetRows.ToListAsync();
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // GET: Timesheets/Create
        public IActionResult Create()
        {
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays.Where(e=>e.EmployeeId == employeeid), "EmployeePayId", "EmployeePayId");
            return View();
        }

        // POST: Timesheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimesheetId,WeekEnding,WeekNumber,ESignature,FlexTime,Status,EmployeePayId")] Timesheet timesheet)
        {
            var employees = await _context.Employees.ToListAsync();
            var payments = await _context.PayGrades.ToListAsync();
            var timesheetrows = await _context.TimesheetRows.ToListAsync();
            var employeepays = await _context.EmployeePays.ToListAsync();
            timesheet.EmployeeId = employeeid;
            if (ModelState.IsValid)
            {
                _context.Add(timesheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", timesheet.EmployeeId);
            ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays.Where(e => e.EmployeeId == employeeid), "EmployeePayId", "EmployeePayId", timesheet.EmployeePayId);
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
            var employees = await _context.Employees.ToListAsync();
            var payments = await _context.PayGrades.ToListAsync();
            var timesheetrows = await _context.TimesheetRows.ToListAsync();
            var employeepays = await _context.EmployeePays.ToListAsync();


            if (timesheet == null)
            {
                return NotFound();
            }
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", timesheet.EmployeeId);
            ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays.Where(e => e.EmployeeId == employeeid), "EmployeePayId", "EmployeePayId", timesheet.EmployeePayId);
            return View(timesheet);
        }

        // POST: Timesheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimesheetId,WeekEnding,WeekNumber,ESignature,FlexTime,Status,EmployeePayId")] Timesheet timesheet)
        {
            timesheet.EmployeeId = employeeid;
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
            ViewData["EmployeePayId"] = new SelectList(_context.EmployeePays.Where(e => e.EmployeeId == employeeid), "EmployeePayId", "EmployeePayId", timesheet.EmployeePayId);
            return View(timesheet);
        }

        // GET: Timesheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var employees = await _context.Employees.ToListAsync();
            var payments = await _context.PayGrades.ToListAsync();
            var timesheetrows = await _context.TimesheetRows.ToListAsync();
            var employeepays = await _context.EmployeePays.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // POST: Timesheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employees = await _context.Employees.ToListAsync();
            var payments = await _context.PayGrades.ToListAsync();
            var timesheetrows = await _context.TimesheetRows.ToListAsync();
            var employeepays = await _context.EmployeePays.ToListAsync();
            var timesheet = await _context.Timesheets.FindAsync(id);
            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //[HttpPost, ActionName("DeleteRow")]
        //[ValidateAntiForgeryToken]
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
