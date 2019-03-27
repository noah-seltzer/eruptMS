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
    public class ApproveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApproveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Approve
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.Approver).Include(e => e.Supervisor).Include(e => e.Timesheets);
            return View(await applicationDbContext.ToListAsync());
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

        // GET: Approve/Create
        public IActionResult Create()
        {
            ViewData["ApproverId"] = new SelectList(_context.Employees, "Id", "Id");
            ViewData["SupervisorId"] = new SelectList(_context.Employees, "Id", "Id");
            return View();
        }

        // POST: Approve/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Email,FirstName,LastName,Title,CreatedTime,FlexTime,VacationTime,Status,ApproverId,SupervisorId,Id,UserName,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApproverId"] = new SelectList(_context.Employees, "Id", "Id", employee.ApproverId);
            ViewData["SupervisorId"] = new SelectList(_context.Employees, "Id", "Id", employee.SupervisorId);
            return View(employee);
        }

        // GET: Approve/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["ApproverId"] = new SelectList(_context.Employees, "Id", "Id", employee.ApproverId);
            ViewData["SupervisorId"] = new SelectList(_context.Employees, "Id", "Id", employee.SupervisorId);
            return View(employee);
        }

        // POST: Approve/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmployeeId,Email,FirstName,LastName,Title,CreatedTime,FlexTime,VacationTime,Status,ApproverId,SupervisorId,Id,UserName,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["ApproverId"] = new SelectList(_context.Employees, "Id", "Id", employee.ApproverId);
            ViewData["SupervisorId"] = new SelectList(_context.Employees, "Id", "Id", employee.SupervisorId);
            return View(employee);
        }
            */



        // GET: Approve/Delete/5
        /**
        public async Task<IActionResult> Approve(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int x = Int32.Parse(id);

            var timesheet = await _context.Timesheets
                .Include(e => e.TimesheetRows)
                .FirstOrDefaultAsync(m => m.TimesheetId == x);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }
        */
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
            timesheet.Status = 3;
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
            timesheet.Status = 3;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id, [Bind("EmployeeId,Email,FirstName,LastName,Title,CreatedTime,FlexTime,VacationTime,Status,ApproverId,SupervisorId,Id,UserName,NormalizedUserName,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] Employee employee)
        {
            int x = 0;
            Int32.TryParse(id, out x);
            var timesheet = await _context.Timesheets.FindAsync(x);
            timesheet.Status = 4;
            _context.SaveChanges();

            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {   /**
                    if (!TimesheetExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }*/
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApproverId"] = new SelectList(_context.Employees, "Id", "Id", employee.ApproverId);
            ViewData["SupervisorId"] = new SelectList(_context.Employees, "Id", "Id", employee.SupervisorId);
            return View(employee);
        }
    }
}
