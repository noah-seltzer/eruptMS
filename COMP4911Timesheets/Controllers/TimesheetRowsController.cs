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

        public TimesheetRowsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: TimesheetRows/Create
        public async Task<IActionResult> Create(int id)
        {
            //if (System.Security.Principal.WindowsIdentity.GetCurrent().) { }
            TimesheetRow model = new TimesheetRow()
            {
                TimesheetId = id
            };
            var projects = await _context.Projects.ToListAsync();
            var timesheet = await _context.Timesheets.Include(t => t.Employee.WorkPackageEmployees).FirstOrDefaultAsync(t => t.TimesheetId == id);
            var packages = await _context.WorkPackages.ToListAsync();
            var wpes = timesheet.Employee.WorkPackageEmployees.OrderBy(wpee => wpee.WorkPackageId)
                  .Select(s => new SelectListItem
                  {
                      Value = s.WorkPackageId.ToString(),
                      Text = s.WorkPackage.Project.Name+" --- "+s.WorkPackage.Name
                  });
            ViewData["WorkPackageId"] = new SelectList(wpes, "Value", "Text");
            //ViewData["WorkPackageId"] = new SelectList(_context.WorkPackages, "WorkPackageId", "Project.Name" );
            return View(model);
        }

        // POST: TimesheetRows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimesheetRowId,SatHour,SunHour,MonHour,TueHour,WedHour,ThuHour,FriHour,Notes,TimesheetId,WorkPackageId")] TimesheetRow timesheetRow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timesheetRow);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Timesheets", new { id = timesheetRow.TimesheetId });
            }
            var projects = await _context.Projects.ToListAsync();
            var timesheet = await _context.Timesheets.Include(t => t.Employee.WorkPackageEmployees).FirstOrDefaultAsync(t => t.TimesheetId == timesheetRow.TimesheetId);
            var packages = await _context.WorkPackages.ToListAsync();
            var wpes = timesheet.Employee.WorkPackageEmployees.OrderBy(wpee => wpee.WorkPackageId)
                  .Select(s => new SelectListItem
                  {
                      Value = s.WorkPackageId.ToString(),
                      Text = s.WorkPackage.Project.Name + " --- " + s.WorkPackage.Name
                  });
            ViewData["WorkPackageId"] = new SelectList(wpes, "Value", "Text");
            return View(timesheetRow);
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
            var projects = await _context.Projects.ToListAsync();
            var timesheet = await _context.Timesheets.Include(t => t.Employee.WorkPackageEmployees).FirstOrDefaultAsync(t => t.TimesheetId == id);
            var packages = await _context.WorkPackages.ToListAsync();
            var wpes = timesheet.Employee.WorkPackageEmployees.OrderBy(wpee => wpee.WorkPackageId)
                  .Select(s => new SelectListItem
                  {
                      Value = s.WorkPackageId.ToString(),
                      Text = s.WorkPackage.Project.Name + " --- " + s.WorkPackage.Name
                  });
            ViewData["WorkPackageId"] = new SelectList(wpes, "Value", "Text");
            return View(timesheetRow);
        }

        // POST: TimesheetRows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimesheetRowId,SatHour,SunHour,MonHour,TueHour,WedHour,ThuHour,FriHour,Notes,TimesheetId,WorkPackageId")] TimesheetRow timesheetRow)
        {
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
                return RedirectToAction("Edit", "Timesheets", new { id = timesheetRow.TimesheetId });
            }
            var projects = await _context.Projects.ToListAsync();
            var timesheet = await _context.Timesheets.Include(t => t.Employee.WorkPackageEmployees).FirstOrDefaultAsync(t => t.TimesheetId == id);
            var packages = await _context.WorkPackages.ToListAsync();
            var wpes = timesheet.Employee.WorkPackageEmployees.OrderBy(wpee => wpee.WorkPackageId)
                  .Select(s => new SelectListItem
                  {
                      Value = s.WorkPackageId.ToString(),
                      Text = s.WorkPackage.Project.Name + " --- " + s.WorkPackage.Name
                  });
            ViewData["WorkPackageId"] = new SelectList(wpes, "Value", "Text");
            return View(timesheetRow);
        }


        private bool TimesheetRowExists(int id)
        {
            return _context.TimesheetRows.Any(e => e.TimesheetRowId == id);
        }




    }
}
