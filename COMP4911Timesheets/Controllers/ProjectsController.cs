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
using Microsoft.AspNetCore.Identity;

namespace COMP4911Timesheets
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _usermgr;

        public ProjectsController(ApplicationDbContext context,
                                  UserManager<Employee> userman)
        {
            _context = context;
            _usermgr = userman;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string searchString)
        {
            var uid = (await _usermgr.GetUserAsync(User)).Id;
            ProjectListingModel model = new ProjectListingModel();

            if (User.IsInRole("AD"))
            {
                model.managedProjects = await _context.Projects.ToListAsync();
                model.assignedProjects = new List<Project>();
                return View(model);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                model.managedProjects = await _context.ProjectEmployees
                .Where(pe => pe.EmployeeId == uid 
                          && (pe.Role == ProjectEmployee.PROJECT_MANAGER
                              || pe.Role == ProjectEmployee.PROJECT_ASSISTANT))
                .Join(_context.Projects,
                        p => p.ProjectId,
                        pe => pe.ProjectId,
                        (pe, p) => p)
                        .Where(p => p.Name.Contains(searchString)
                                    || p.ProjectCode.Contains(searchString))
                .ToListAsync();
            }
            else
            {
                model.managedProjects = await _context.ProjectEmployees
                .Where(pe => pe.EmployeeId == uid
                          && (pe.Role == ProjectEmployee.PROJECT_MANAGER
                              || pe.Role == ProjectEmployee.PROJECT_ASSISTANT))
                .Join(_context.Projects,
                        p => p.ProjectId,
                        pe => pe.ProjectId,
                        (pe, p) => p)
                .ToListAsync();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                model.assignedProjects = await _context.ProjectEmployees
                .Where(pe => pe.Role != ProjectEmployee.PROJECT_MANAGER
                          && pe.Role != ProjectEmployee.PROJECT_ASSISTANT
                          && pe.EmployeeId == uid)
                .Join(_context.Projects,
                        p => p.ProjectId,
                        pe => pe.ProjectId,
                        (pe, p) => p)
                        .Where(p => p.Name.Contains(searchString)
                                 || p.ProjectCode.Contains(searchString))
                .ToListAsync();
            }
            else
            {
                model.assignedProjects = await _context.ProjectEmployees
                .Where(pe => pe.Role != ProjectEmployee.PROJECT_MANAGER
                          && pe.Role != ProjectEmployee.PROJECT_ASSISTANT
                          && pe.EmployeeId == uid)
                .Join(_context.Projects,
                      p => p.ProjectId,
                      pe => pe.ProjectId,
                      (pe, p) => p)
                .ToListAsync();
            }

            return View(model);
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
        [Authorize (Roles = "AD")]
        public IActionResult Create()
        {
            ViewBag.Employees = new SelectList(_context.Employees, "Id", "Email");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AD")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectCode,Name,Description,ProjectManager")] NewProject input)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project
                {
                    ProjectCode = input.ProjectCode,
                    Name = input.Name,
                    Description = input.Description,
                    Status = Project.ONGOING,
                };
                _context.Add(project);

                _context.SaveChanges();

                int pId = (_context.Projects.Where(p => p.ProjectCode == input.ProjectCode).First()).ProjectId;

                ProjectEmployee manager = new ProjectEmployee
                {
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    Role = ProjectEmployee.PROJECT_MANAGER,
                    ProjectId = pId,
                    EmployeeId = input.ProjectManager,
                };
                _context.Add(manager);

                Employee mgr = _context.Employees.Find(manager.EmployeeId);
                await _usermgr.AddToRoleAsync(mgr, ApplicationRole.PM);

                WorkPackage mgmt = new WorkPackage
                {
                    ProjectId = pId,
                    WorkPackageCode = "00000",
                    ParentWorkPackageId = null,
                    Name = "Management",
                    Description = "",
                };
                _context.Add(mgmt);

                var pGrades = _context.PayGrades.ToList();
                foreach (var g in pGrades)
                    _context.Add(new ProjectRequest
                    {
                        ProjectId = pId,
                        PayGradeId = g.PayGradeId,
                        AmountRequested = 0,
                        Status = ProjectRequest.VALID
                    });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "PM,PA")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = _context.Projects
                .Include(w => w.WorkPackages)
                .First(f => f.ProjectId == id);

            if (project == null) return NotFound();

            //We get the ProjectEmployees separately so we can Include the Employee of each 
            var emps = _context.ProjectEmployees
                .Include(e => e.Employee)
                .Where(e => e.ProjectId == id)
                .ToList();

            var manager = _context.ProjectEmployees
                .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_MANAGER)
                .FirstOrDefault();

            var assistant = _context.ProjectEmployees
                .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_ASSISTANT)
                .FirstOrDefault();

            var reqs = _context.ProjectRequests
                .Include(r => r.PayGrade)
                .Where(r => r.ProjectId == id)
                .ToList();

            //This is for when new pay grades have been added that the project does not yet have
            #region Not Enough Pay Grades
            var grds = _context.PayGrades.ToList();
            if(reqs.Count < grds.Count)
            {
                bool exists = false;
                foreach(var grade in grds)
                {
                    foreach (var req in reqs)
                    {
                        if (grade.PayGradeId == req.PayGradeId)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        ProjectRequest request = new ProjectRequest
                        {
                            PayGradeId = grade.PayGradeId,
                            AmountRequested = 0,
                            ProjectId = id,
                            Status = ProjectRequest.VALID
                        };
                        reqs.Add(request);
                        _context.Add(request);
                    }
                    else
                    {
                        exists = false;
                    }
                }
                await _context.SaveChangesAsync();
            }
            #endregion

            ManageProject model = new ManageProject();
            model.project = project;
            model.project.ProjectEmployees = emps;
            model.requests = reqs;
            model.projectManager = manager.EmployeeId;
            if (assistant != null)
                model.managersAssistant = assistant.EmployeeId;

            List<Employee> employees = _context.Employees.Where(e => e.Id != model.projectManager).ToList();

            List<SelectListItem> empItemsNoManager = new List<SelectListItem>();
            empItemsNoManager.AddRange(new SelectList(employees, "Id", "Email"));
            empItemsNoManager.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.EmployeesA = empItemsNoManager;

            List<SelectListItem> empItemsAll = new List<SelectListItem>();
            empItemsAll.AddRange(new SelectList(employees, "Id", "Email"));
            var managerObj = _context.Employees.Find(model.projectManager);
            empItemsAll.Insert(0, new SelectListItem { Text = managerObj.Email, Value = managerObj.Id});
            ViewBag.EmployeesM = empItemsAll;

            ViewBag.Status = new SelectList(Project.Statuses.ToList(), "Key", "Value", project.Status);

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "PM,PA")]
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

                    foreach (var req in model.requests)
                    { 
                        var updateReq = _context.ProjectRequests.FirstOrDefault(r =>
                            r.ProjectId == req.ProjectId && r.PayGradeId == req.PayGradeId);

                        updateReq.AmountRequested = req.AmountRequested;
                    }

                    var managerPE = _context.ProjectEmployees
                        .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_MANAGER)
                        .FirstOrDefault();

                    if (model.projectManager != managerPE.EmployeeId)
                    {
                        Employee oldmgr = _context.Employees.Find(managerPE.EmployeeId);
                        await _usermgr.RemoveFromRoleAsync(oldmgr, ApplicationRole.PM);

                        Employee mgr = _context.Employees.Find(managerPE.EmployeeId);
                        await _usermgr.AddToRoleAsync(mgr, ApplicationRole.PM);

                        managerPE.EmployeeId = model.projectManager;
                        _context.Update(managerPE);
                    }

                    var assistantPE = _context.ProjectEmployees
                        .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_ASSISTANT)
                        .FirstOrDefault();

                    if (!String.IsNullOrEmpty(model.managersAssistant)) //if we are setting an assistant
                    {
                        if (assistantPE == null)                        //if the project doesnt have an assitant already
                        {
                            assistantPE = _context.ProjectEmployees     //get the new assistants PE entry
                                .Where(e => e.ProjectId == id && e.EmployeeId == model.managersAssistant)
                                .FirstOrDefault();
                            
                            if(assistantPE == null)                     //if the new PA wasnt already on the project (no PE entry)
                            {
                                assistantPE = new ProjectEmployee       //add them to the project as the PA 
                                {
                                    EmployeeId = model.managersAssistant,
                                    ProjectId = id,
                                    Role = ProjectEmployee.PROJECT_ASSISTANT,
                                    Status = ProjectEmployee.CURRENTLY_WORKING
                                };
                                _context.Add(assistantPE);
                            }
                            else                                        //if they were on the project, just update PE to have PA
                            {
                                assistantPE.Role = ProjectEmployee.PROJECT_ASSISTANT;
                                _context.Update(assistantPE);
                            }

                        }
                        else if (model.managersAssistant != assistantPE.EmployeeId) //if the project already had a PA and this one is NEW
                        {
                            assistantPE.Role = ProjectEmployee.EMPLOYEE;
                            var emp = _context.ProjectEmployees
                                .Where(e => e.ProjectId == id && e.EmployeeId == model.managersAssistant)
                                .FirstOrDefault();
                            emp.Role = ProjectEmployee.PROJECT_ASSISTANT;
                            _context.Update(assistantPE);
                            _context.Update(emp);
                            Employee oldAssist = _context.Employees.Find(assistantPE.EmployeeId);
                            await _usermgr.RemoveFromRoleAsync(oldAssist, ApplicationRole.PA);
                        }

                        Employee assistant = _context.Employees.Find(assistantPE.EmployeeId);
                        await _usermgr.AddToRoleAsync(assistant, ApplicationRole.PA);

                    }
                    else
                    {                                   // if we are clearing the PA
                        if (assistantPE != null)
                        {
                            assistantPE.Role = ProjectEmployee.EMPLOYEE;
                            _context.Update(assistantPE);
                            Employee oldAssist = _context.Employees.Find(assistantPE.EmployeeId);
                            await _usermgr.RemoveFromRoleAsync(oldAssist, ApplicationRole.PA);
                        }
                    }

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
        [Authorize(Roles = "AD")]
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
        [Authorize(Roles = "AD")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            project.Status = Project.CLOSED;
            _context.Update(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}