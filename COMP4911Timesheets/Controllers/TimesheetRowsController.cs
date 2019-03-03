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
    public class TimesheetRowsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static int CurrentTimesheetId = -1;
        public TimesheetRowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TimesheetRows
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TimesheetRows.Include(t => t.Timesheet).Include(t => t.WorkPackage);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TimesheetRows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheetRow = await _context.TimesheetRows
                .Include(t => t.Timesheet)
                .Include(t => t.WorkPackage)
                .FirstOrDefaultAsync(m => m.TimesheetRowId == id);
            if (timesheetRow == null)
            {
                return NotFound();
            }

            return View(timesheetRow);
        }

        // GET: TimesheetRows/Create
        public IActionResult Create(int id)
        {

            CurrentTimesheetId = id;
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId");
            return View();
        }

        // POST: TimesheetRows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimesheetRowId,SatHour,SunHour,MonHour,TueHour,WedHour,ThuHour,FriHour,Notes,TimesheetId,WorkPackageId")] TimesheetRow timesheetRow)
        {
            timesheetRow.TimesheetId = CurrentTimesheetId;
            if (ModelState.IsValid)
            {
                _context.Add(timesheetRow);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Timesheets", new { id = CurrentTimesheetId });
            }
            //ViewData["TimesheetId"] = new SelectList(_context.Timesheets, "TimesheetId", "TimesheetId", timesheetRow.TimesheetId);
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", timesheetRow.WorkPackageId);
            return RedirectToAction("Edit", "Timesheets", new { id = CurrentTimesheetId });
        }

        // GET: TimesheetRows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheetRow = await _context.TimesheetRows.FindAsync(id);
            if (timesheetRow == null)
            {
                return NotFound();
            }
            CurrentTimesheetId = timesheetRow.TimesheetId;
            //ViewData["TimesheetId"] = new SelectList(_context.Timesheets, "TimesheetId", "TimesheetId", timesheetRow.TimesheetId);
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", timesheetRow.WorkPackageId);
            return View(timesheetRow);
        }

        // POST: TimesheetRows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimesheetRowId,SatHour,SunHour,MonHour,TueHour,WedHour,ThuHour,FriHour,Notes,TimesheetId,WorkPackageId")] TimesheetRow timesheetRow)
        {
            timesheetRow.TimesheetId = CurrentTimesheetId;
            if (id != timesheetRow.TimesheetRowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timesheetRow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimesheetRowExists(timesheetRow.TimesheetRowId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Timesheets", new { id = CurrentTimesheetId });
            }
            //ViewData["TimesheetId"] = new SelectList(_context.Timesheets, "TimesheetId", "TimesheetId", timesheetRow.TimesheetId);
            ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "WorkPackageId", timesheetRow.WorkPackageId);
            return RedirectToAction("Edit", "Timesheets", new { id = CurrentTimesheetId });
        }

        // GET: TimesheetRows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheetRow = await _context.TimesheetRows
                .Include(t => t.Timesheet)
                .Include(t => t.WorkPackage)
                .FirstOrDefaultAsync(m => m.TimesheetRowId == id);
            if (timesheetRow == null)
            {
                return NotFound();
            }

            return View(timesheetRow);
        }

        // POST: TimesheetRows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timesheetRow = await _context.TimesheetRows.FindAsync(id);
            _context.TimesheetRows.Remove(timesheetRow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimesheetRowExists(int id)
        {
            return _context.TimesheetRows.Any(e => e.TimesheetRowId == id);
        }
    }
}
