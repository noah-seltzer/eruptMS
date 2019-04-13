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
        public async Task<IActionResult> Index(string searchString)
        {
            var timesheets = new List<Timesheet>();

            if (!String.IsNullOrEmpty(searchString))
            {
                timesheets = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .Include(t => t.Signature)
                .Where(t => t.Employee.Id == _userManager.GetUserId(HttpContext.User))
                .Where(t => t.WeekEnding.ToString("yyyy/MM/dd").Contains(searchString))
                .OrderByDescending(t => t.WeekNumber)
                .ToListAsync();
            }
            else
            {
                timesheets = await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.EmployeePay)
                .Include(t => t.Signature)
                .Where(t => t.Employee.Id == _userManager.GetUserId(HttpContext.User))
                .OrderByDescending(t => t.WeekNumber)
                .ToListAsync();
            }

            return View(timesheets);
        }

        // GET: Timesheets/Details/5(timesheetid)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets.Include(t=>t.Employee).FirstOrDefaultAsync(t=>t.TimesheetId==id);
            var timesheetrows = _context.TimesheetRows.Where(t => t.TimesheetId == id);
            foreach (TimesheetRow tr in timesheetrows)
            {
                var package = _context.WorkPackages.Find(tr.WorkPackageId);
                var project = _context.Projects.Find(package.ProjectId);
            }

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
        public async Task<IActionResult> Create()
        {
            //get timesheets
            var timesheets = await _context.Timesheets.Where(t => t.Employee.Id == _userManager.GetUserId(HttpContext.User)).ToListAsync();
            //get this Friday
            DateTime friday = Utility.GetNextWeekday(DateTime.Today, DayOfWeek.Friday);
            Timesheet model = new Timesheet()
            {
                //default this Friday
                WeekEnding = friday
            };
            //Friday list
            List<DateTime> fridays = new List<DateTime>();

            //Add Fridays in 2 months
            for (int i = 0; i < 9; i++)
            {
                var newfriday = friday.AddDays(i * 7);
                //Check if timesheet for this week exist
                bool exist = false;
                foreach (Timesheet t in timesheets)
                {
                    if (t.WeekEnding == newfriday) { exist = true; }
                }
                if (!exist) { fridays.Add(newfriday); }
            }

            //Add Fridays in previous month
            for (int i = 4; i > 0; i--)
            {
                var oldfriday = friday.AddDays(-i * 7);
                //Check if timesheet for this week exist
                bool exist = false;
                foreach (Timesheet t in timesheets)
                {
                    if (t.WeekEnding == oldfriday) { exist = true; }
                }
                if (!exist) { fridays.Add(oldfriday); }
            }

            //Create selectlist using Friday list
            var fridayslist = fridays.Select(s => new SelectListItem
            {
                Value = s.Date.ToString(),
                Text = s.Date.ToString("yyyy/MM/dd")
            });
            ViewData["fridays"] = new SelectList(fridayslist, "Value", "Text", friday);
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
                //default status is not submitted and not approved
                timesheet.Status = Timesheet.NOT_SUBMITTED_NOT_APPROVED;

                //calculate week number
                timesheet.WeekNumber = Utility.GetWeekNumberByDate(timesheet.WeekEnding);

                //get user's employee id
                timesheet.EmployeeId = _userManager.GetUserId(HttpContext.User);


                //select the valid employee pay
                var emppay = await _context.EmployeePays.FirstOrDefaultAsync(ep => ep.EmployeeId == timesheet.EmployeeId && ep.Status == EmployeePay.VALID);
                timesheet.EmployeePay = emppay;
                timesheet.EmployeePayId = emppay.EmployeePayId;

                //select valid
                var sign = await _context.Signatures.FirstOrDefaultAsync(s => s.EmployeeId == timesheet.EmployeeId && s.Status == Signature.VALID);

                if (sign == null)
                {
                    TempData["Link"] = "Signature";
                    TempData["begin"] = "Fill in a ";
                    TempData["end"] = " pass phrase in your profile first";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                timesheet.Signature = sign;
                timesheet.SignatureId = sign.SignatureId;

                timesheet.FlexTime = 0;

                _context.Add(timesheet);

                //add HR reserved rows
                var wp1 = _context.WorkPackages.FirstOrDefault(wp => wp.WorkPackageCode == "SICK");
                TimesheetRow tr1 = new TimesheetRow() { Timesheet = timesheet, WorkPackage = wp1 };
                _context.Add(tr1);
                var wp2 = _context.WorkPackages.FirstOrDefault(wp => wp.WorkPackageCode == "VACN");
                TimesheetRow tr2 = new TimesheetRow() { Timesheet = timesheet, WorkPackage = wp2 };
                _context.Add(tr2);
                var wp3 = _context.WorkPackages.FirstOrDefault(wp => wp.WorkPackageCode == "SHOL");
                TimesheetRow tr3 = new TimesheetRow() { Timesheet = timesheet, WorkPackage = wp3 };
                _context.Add(tr3);
                var wp4 = _context.WorkPackages.FirstOrDefault(wp => wp.WorkPackageCode == "FLEX");
                TimesheetRow tr4 = new TimesheetRow() { Timesheet = timesheet, WorkPackage = wp4 };
                _context.Add(tr4);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = timesheet.TimesheetId});
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


            var timesheet = await _context.Timesheets.Include(t => t.Employee).FirstOrDefaultAsync(t => t.TimesheetId == id);
            var timesheetrows = _context.TimesheetRows.Where(t => t.TimesheetId == id);
            foreach (TimesheetRow tr in timesheetrows)
            {
                var package = _context.WorkPackages.Find(tr.WorkPackageId);
                var project = _context.Projects.Find(package.ProjectId);
            }

            string uid = (await _userManager.GetUserAsync(User)).Id;

            var hr_work_pkgs = _context.ProjectEmployees
                    .Where(p => p.EmployeeId == uid
                        && p.ProjectId == 1)
                    .Join(_context.WorkPackages,
                    w => w.WorkPackageId,
                    p => p.WorkPackageId,
                    (p, w) => w)
                    .Include(wp => wp.Project)
                    .ToList();

            if(hr_work_pkgs.Count < 4)
            {//missing hr work packages
                var newPEs = new List<ProjectEmployee>();
                newPEs.Add(new ProjectEmployee
                {
                    EmployeeId = uid,
                    ProjectId = 1,
                    Role = ProjectEmployee.EMPLOYEE,
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    WorkPackageId = 1
                });
                newPEs.Add(new ProjectEmployee
                {
                    EmployeeId = uid,
                    ProjectId = 1,
                    Role = ProjectEmployee.EMPLOYEE,
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    WorkPackageId = 2
                });
                newPEs.Add(new ProjectEmployee
                {
                    EmployeeId = uid,
                    ProjectId = 1,
                    Role = ProjectEmployee.EMPLOYEE,
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    WorkPackageId = 3
                });
                newPEs.Add(new ProjectEmployee
                {
                    EmployeeId = uid,
                    ProjectId = 1,
                    Role = ProjectEmployee.EMPLOYEE,
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    WorkPackageId = 4
                });
                await _context.SaveChangesAsync();
            }

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
            timesheet.FlexTime = 0;
            if (timesheet.TimesheetRows != null)
            {
                double sat = 0, sun = 0, mon = 0, tue = 0, wed = 0, thu = 0, fri = 0, flexused = 0;
                foreach (TimesheetRow tr in timesheet.TimesheetRows)
                {
                    if (tr.WorkPackage.WorkPackageCode == "FLEX")
                    {
                        flexused += tr.SatHour + tr.SunHour + tr.MonHour + tr.TueHour + tr.WedHour + tr.ThuHour + tr.FriHour;
                    }
                    sat += tr.SatHour;
                    sun += tr.SunHour;
                    mon += tr.MonHour;
                    tue += tr.TueHour;
                    wed += tr.WedHour;
                    thu += tr.ThuHour;
                    fri += tr.FriHour;
                }
                if ((sat + sun + mon + tue + wed + thu + fri) > 40) {
                    timesheet.FlexTime += sat + sun + mon + tue + wed + thu + fri - 40;
                } else {
                    if (sat > 8) timesheet.FlexTime += sat - 8;
                    if (sun > 8) timesheet.FlexTime += sun - 8;
                    if (mon > 8) timesheet.FlexTime += mon - 8;
                    if (tue > 8) timesheet.FlexTime += tue - 8;
                    if (wed > 8) timesheet.FlexTime += wed - 8;
                    if (thu > 8) timesheet.FlexTime += thu - 8;
                    if (fri > 8) timesheet.FlexTime += fri - 8;
                }
                timesheet.FlexTime -= flexused;
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

        // GET: Timesheets/Delete/5(timesheetid)
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = _context.Timesheets.Find(id);
            var timesheetrows = _context.TimesheetRows.Where(t => t.TimesheetId == id);
            foreach (TimesheetRow tr in timesheetrows)
            {
                var package = _context.WorkPackages.Find(tr.WorkPackageId);
                var project = _context.Projects.Find(package.ProjectId);
            }

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

        // POST: Timesheets/Submit/5(timesheetid)

        public async Task<IActionResult> Submit(int id, string pass)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            var oldSignature = _context.Signatures.Where(s => s.EmployeeId == _userManager.GetUserId(HttpContext.User)).Where(s => s.Status == Signature.VALID).FirstOrDefault();
            var decryptedOldSignature = Utility.HashDecrypt(oldSignature.HashedSignature);
            if (pass + oldSignature.CreatedTime != decryptedOldSignature)
            {
                TempData["info"] = "PassPhrase not correct";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            timesheet.Status = Timesheet.SUBMITTED_NOT_APPROVED;
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());

        }

        // POST: Timesheets/Retract/5(timesheetid)
        public async Task<IActionResult> Retract(int id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            timesheet.Status = Timesheet.NOT_SUBMITTED_NOT_APPROVED;
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private bool TimesheetExists(int id)
        {
            return _context.Timesheets.Any(e => e.TimesheetId == id);
        }




    }
}
