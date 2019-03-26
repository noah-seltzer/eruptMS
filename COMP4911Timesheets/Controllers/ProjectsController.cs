using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using COMP4911Timesheets.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace COMP4911Timesheets
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "Email");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,ProjectCode,Name,Description,ProjectManager")] NewProject input)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project
                {
                    ProjectId = input.ProjectId,
                    ProjectCode = input.ProjectCode,
                    Name = input.Name,
                    Description = input.Description,
                    Status = Project.ONGOING,
                };
                _context.Add(project);

                ProjectEmployee manager = new ProjectEmployee
                {
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    Role = ProjectEmployee.PROJECT_MANAGER,
                    ProjectId = input.ProjectId,
                    Project = project,
                    EmployeeId = input.ProjectManager,
                    Employee = _context.Employees.Find(input.ProjectManager),
                };
                _context.Add(manager);

                WorkPackage mgmt = new WorkPackage
                {
                    ProjectId = input.ProjectId,
                    ParentWorkPackageId = null,
                    Name = "Management",
                    Description = ""
                };
                _context.Add(mgmt);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emps = _context.ProjectEmployees.Include(e => e.Employee).Where(e => e.ProjectId == id).ToList();

            var project = _context.Projects
                .Include(w => w.WorkPackages)
                .First(f => f.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            project.ProjectEmployees = emps;

            var g = _context.PayGrades.ToList();

            ManageProject model = new ManageProject();
            model.project = project;
            model.requests = new List<Tuple<string, int>>();

            foreach (var grade in g)
                model.requests.Add(Tuple.Create(grade.PayLevel, 0));

            ViewData["Status"] = new SelectList(Project.Statuses.ToList(), "Key", "Value", project.Status);

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ManageProject model)
        {
            if (id != model.project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(model.project.ProjectId))
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
            return View(model.project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}