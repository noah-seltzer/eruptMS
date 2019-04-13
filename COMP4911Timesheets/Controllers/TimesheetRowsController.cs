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
    public class TimesheetRowsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public TimesheetRowsController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: TimesheetRows/Create/timesheetId
        public async Task<IActionResult> Create(int id)
        {

            TimesheetRow model = new TimesheetRow()
            {
                TimesheetId = id
            };

            var timesheet = await _context.Timesheets.Include(t => t.Employee).FirstOrDefaultAsync(t => t.TimesheetId == id);
            var timesheetrows = _context.TimesheetRows.Where(t => t.TimesheetId == id);
            var pes = _context.ProjectEmployees.Where(pe => pe.EmployeeId == timesheet.EmployeeId);
            List<WorkPackage> wpl = new List<WorkPackage>();
            foreach (ProjectEmployee pe in pes)
            {
                if (pe.ProjectId != null && pe.WorkPackageId != null && pe.Status == ProjectEmployee.CURRENTLY_WORKING)
                {
                    bool exist = false;
                    foreach (TimesheetRow tr in timesheetrows)
                    {
                        if (tr.WorkPackageId == pe.WorkPackageId)
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        var wp = await _context.WorkPackages.Include(wpp => wpp.Project).FirstOrDefaultAsync(wpp => wpp.WorkPackageId == pe.WorkPackageId);
                        wpl.Add(wp);
                    }
                }
            }

            //authorization
            if (timesheet.Employee.Id != _userManager.GetUserId(User))
            {
                return NotFound();
            }


            //check any available wp
            if (wpl.Count == 0)
            {
                TempData["info2"] = "No available work package";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var wpes = wpl
                  .Select(s => new SelectListItem
                  {
                      Value = s.WorkPackageId.ToString(),
                      Text = s.Project.Name + " --- " + s.Name
                  });
            ViewData["WorkPackageId"] = new SelectList(wpes, "Value", "Text");

            //            string uid = (await _userManager.GetUserAsync(User)).Id;

            //            var listInfo = _context.ProjectEmployees
            //                .Where(pe => pe.EmployeeId == uid 
            //                          && pe.Status == ProjectEmployee.CURRENTLY_WORKING)
            //                .Join(_context.WorkPackages,
            //                pe => pe.ProjectId,
            //                wp => wp.ProjectId,
            //                (pe, wp) => new { PE = pe, WP = wp })
            //                .Join(_context.Projects,
            //                i => i.PE.ProjectId,
            //                p => p.ProjectId,
            //                (i, p) => new { WorkPackgeInfo = i.WP, ProjectInfo = p })
            //                .Distinct()
            //                .ToList();

            //            var list = new List<SelectListItem>();
            //            foreach(var info in listInfo)
            //            {
            //                list.Add(new SelectListItem
            //                {
            //                    Value = info.WorkPackgeInfo.WorkPackageId.ToString(),
            //                    Text = info.ProjectInfo.Name + "---" + info.WorkPackgeInfo.Name
            //                });   
            //            }


            //            ViewData["WorkPackageId"] = new SelectList(list, "Value", "Text");
            //>>>>>>> origin/TEST
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

            var timesheet = await _context.Timesheets.Include(t => t.Employee).FirstOrDefaultAsync(t => t.TimesheetId == timesheetRow.TimesheetId);
            var timesheetrows = _context.TimesheetRows.Where(t => t.TimesheetId == timesheetRow.TimesheetId);
            var pes = _context.ProjectEmployees.Where(pe => pe.EmployeeId == timesheet.EmployeeId);
            List<WorkPackage> wpl = new List<WorkPackage>();
            foreach (ProjectEmployee pe in pes)
            {
                bool exist = false;
                foreach (TimesheetRow tr in timesheetrows)
                {
                    if (tr.WorkPackageId == pe.WorkPackageId)
                    {
                        exist = true;
                    }
                }
                if (!exist)
                {
                    var wp = await _context.WorkPackages.Include(wpp => wpp.Project).FirstOrDefaultAsync(wpp => wpp.WorkPackageId == pe.WorkPackageId);
                    wpl.Add(wp);
                }
            }

            //authorization
            if (timesheet.Employee.Id != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }

            var wpes = wpl
                  .Select(s => new SelectListItem
                  {
                      Value = s.WorkPackageId.ToString(),
                      Text = s.Project.Name + " --- " + s.Name
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

            var timesheet = await _context.Timesheets.Include(t => t.Employee).FirstOrDefaultAsync(t => t.TimesheetId == timesheetRow.TimesheetId);
            var package = _context.WorkPackages.Find(timesheetRow.WorkPackageId);
            var project = _context.Projects.Find(package.ProjectId);


            //authorization
            if (timesheet.Employee.Id != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }

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

            var timesheet = await _context.Timesheets.Include(t => t.Employee).FirstOrDefaultAsync(t => t.TimesheetId == timesheetRow.TimesheetId);
            var package = _context.WorkPackages.Find(timesheetRow.WorkPackageId);
            var project = _context.Projects.Find(package.ProjectId);


            //authorization
            if (timesheet.Employee.Id != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }


            return View(timesheetRow);
        }


        private bool TimesheetRowExists(int id)
        {
            return _context.TimesheetRows.Any(e => e.TimesheetRowId == id);
        }




    }
}
