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
            var applicationDbContext = _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .Include(t => t.Signature)
                .Where(t => t.Employee.Id == _userManager.GetUserId(HttpContext.User))
                .OrderByDescending(t => t.WeekNumber);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Timesheets/Details/5(timesheetid)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var projects = await _context.Projects.ToListAsync();
            var workpackages = await _context.WorkPackages.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.TimesheetRows)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);

            if (timesheet == null)
            {
                return NotFound();
            }

            //authorization
            if (timesheet.EmployeeId != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // GET: Timesheets/Create
        public IActionResult Create()
        {
            DateTime friday = Utility.GetNextWeekday(DateTime.Today, DayOfWeek.Friday);
            Timesheet model = new Timesheet()
            {
                //default this Friday
                WeekEnding = friday
            };
            List<DateTime> fridays = new List<DateTime>();
            for (int i = 0; i < 30; i++)
            {
                fridays.Add(friday.AddDays(i * 7));
            }
            var fridayslist = fridays.Select(s => new SelectListItem
            {
                Value = s.Date.ToString(),
                Text = s.Date.ToString("yyyy/MM/dd")
            });
            ViewData["fridays"] = new SelectList(fridayslist, "Value", "Text");
            return View(model);

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
                //default status
                timesheet.Status = 3;

                //calculate week number
                timesheet.WeekNumber = Utility.GetWeekNumberByDate(timesheet.WeekEnding);

                timesheet.EmployeeId = _userManager.GetUserId(HttpContext.User);

                //////////////////
                ///how to select employee pay automatically?
                //////////////////
                var emppay = await _context.EmployeePays.FirstOrDefaultAsync(ep => ep.EmployeeId == timesheet.EmployeeId);
                timesheet.EmployeePay = emppay;
                timesheet.EmployeePayId = emppay.EmployeePayId;

                //////////////////
                ///how to select Signature automatically?
                //////////////////
                var sign = await _context.Signatures.FirstOrDefaultAsync(s => s.EmployeeId == timesheet.EmployeeId);
                timesheet.Signature = sign;
                timesheet.SignatureId = sign.SignatureId;

                timesheet.FlexTime = 40;

                _context.Add(timesheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timesheet);
        }

        // GET: Timesheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects.ToListAsync();
            var workpackages = await _context.WorkPackages.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.TimesheetRows)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);

            DateTime friday = Utility.GetNextWeekday(DateTime.Today, DayOfWeek.Friday);
            Timesheet model = new Timesheet()
            {
                //default this Friday
                WeekEnding = friday
            };
            List<DateTime> fridays = new List<DateTime>();
            for (int i = 0; i < 30; i++)
            {
                fridays.Add(friday.AddDays(i * 7));
            }
            var fridayslist = fridays.Select(s => new SelectListItem
            {
                Value = s.Date.ToString(),
                Text = s.Date.ToString("yyyy/MM/dd")
            });
            ViewData["fridays"] = new SelectList(fridayslist, "Value", "Text");


            if (timesheet == null)
            {
                return NotFound();
            }


            //authorization
            if (timesheet.EmployeeId != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }

            //calculate flex time
            timesheet.FlexTime = 40;
            if (timesheet.TimesheetRows != null)
            {
                foreach (TimesheetRow tr in timesheet.TimesheetRows)
                {
                    timesheet.FlexTime = timesheet.FlexTime - tr.SatHour - tr.SunHour - tr.MonHour - tr.TueHour - tr.WedHour - tr.ThuHour - tr.FriHour;
                }
            }
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

            return View(timesheet);
        }

        // POST: Timesheets/Edit/5(timesheetid)
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimesheetId,WeekEnding,WeekNumber,SignatureId,FlexTime,Status,EmployeeId,EmployeePayId")] Timesheet timesheet)
        {
            if (id != timesheet.TimesheetId)
            {
                return NotFound();
            }

            //authorization
            if (timesheet.EmployeeId != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }

            //calculate week number
            timesheet.WeekNumber = Utility.GetWeekNumberByDate(timesheet.WeekEnding);

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
            return View(timesheet);
        }

        // GET: Timesheets/Delete/5(timesheetid)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var projects = await _context.Projects.ToListAsync();
            var workpackages = await _context.WorkPackages.ToListAsync();
            var timesheet = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.TimesheetRows)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            //authorization
            if (timesheet.EmployeeId != _userManager.GetUserId(HttpContext.User))
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
            var timesheet = await _context.Timesheets.Include(m => m.TimesheetRows).FirstOrDefaultAsync(m => m.TimesheetId == id);
            //authorization
            if (timesheet.EmployeeId != _userManager.GetUserId(HttpContext.User))
            {
                return NotFound();
            }

            var timesheetrows = timesheet.TimesheetRows;
            if (timesheetrows != null)
            {
                foreach (TimesheetRow tr in timesheetrows)
                {
                    _context.TimesheetRows.Remove(tr);
                }
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
