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
            try
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.ToString());
            }
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
                          && pe.WorkPackageId == null
                          && pe.Role != ProjectEmployee.NOT_ASSIGNED) // null WP is marker for mgmt roles
                .Join(_context.Projects,
                        p => p.ProjectId,
                        pe => pe.ProjectId,
                        (pe, p) => p)
                        .Where(p => p.Name.Contains(searchString) || p.ProjectCode.Contains(searchString))
                .ToListAsync();
            }
            else
            {
                model.managedProjects = await _context.ProjectEmployees
                .Where(pe => pe.EmployeeId == uid
                          && pe.WorkPackageId == null
                          && pe.Role != ProjectEmployee.NOT_ASSIGNED) // null WP is marker for mgmt roles
                .Join(_context.Projects,
                        p => p.ProjectId,
                        pe => pe.ProjectId,
                        (pe, p) => p)
                .ToListAsync();
            }
            /*
             * THIS IS NOT DUPLICATE CODE, THEYRE TWO DIFFERENT LISTS!!!
             */
            if (!String.IsNullOrEmpty(searchString))
            {
                model.assignedProjects = await _context.ProjectEmployees
                .Where(pe => pe.EmployeeId == uid
                          && pe.WorkPackageId == null
                          && pe.Role != ProjectEmployee.PROJECT_MANAGER
                          && pe.Role != ProjectEmployee.PROJECT_ASSISTANT
                          && pe.Status == ProjectEmployee.CURRENTLY_WORKING) // null WP is marker for mgmt roles
                .Join(_context.Projects,
                        p => p.ProjectId,
                        pe => pe.ProjectId,
                        (pe, p) => p)
                        .Where(p => p.Name.Contains(searchString) || p.ProjectCode.Contains(searchString))
                .ToListAsync();
            }
            else
            {
                model.assignedProjects = await _context.ProjectEmployees
                .Where(pe => pe.EmployeeId == uid
                          && pe.WorkPackageId == null
                          && pe.Role != ProjectEmployee.PROJECT_MANAGER
                          && pe.Role != ProjectEmployee.PROJECT_ASSISTANT
                          && pe.Status == ProjectEmployee.CURRENTLY_WORKING) // null WP is marker for mgmt roles
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
        [Authorize(Roles = "AD")]
        public IActionResult Create()
        {
            ViewBag.MEmployees = new SelectList(_context.Employees, "Id", "Email");
            List<SelectListItem> assItems = new List<SelectListItem>();
            assItems.AddRange(new SelectList(_context.Employees, "Id", "Email"));
            assItems.Insert(0, new SelectListItem { Text = "None", Value = "" });
            ViewBag.AEmployees = assItems;
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AD")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectCode,Name,Description,ProjectManager,ManagersAssistant,MarkupRate")] NewProject input)
        {
            if (ModelState.IsValid)
            {
                if (input.ProjectCode.ToString().Length != 4)
                {
                    ViewBag.MEmployees = new SelectList(_context.Employees, "Id", "Email");
                    List<SelectListItem> assItems = new List<SelectListItem>();
                    assItems.AddRange(new SelectList(_context.Employees, "Id", "Email"));
                    assItems.Insert(0, new SelectListItem { Text = "None", Value = "" });
                    ViewBag.AEmployees = assItems;
                    ViewBag.CodeError = "Project code must be four digits long.";
                    return View(input);
                }

                bool projectCodeExists = _context.Projects
                                                 .Where(p => p.ProjectCode == input.ProjectCode)
                                                 .FirstOrDefault() != null;
                if (projectCodeExists)
                {
                    ViewBag.Employees = new SelectList(_context.Employees, "Id", "Email");
                    ViewBag.CodeError = "Project with code " + input.ProjectCode + " aleady exists!";
                    return View(input);
                }

                if (input.ProjectManager == input.ManagersAssistant)
                {
                    ViewBag.Employees = new SelectList(_context.Employees, "Id", "Email");
                    ViewBag.MgrIsAssist = "Manager and Assistant cannot be the same person!";
                    return View(input);
                }

                Project project = new Project
                {
                    ProjectCode = input.ProjectCode,
                    Name = input.Name,
                    Description = input.Description,
                    Status = Project.ONGOING,
                    MarkupRate = input.MarkupRate
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
                    WorkPackageId = null
                };
                _context.Add(manager);

                Employee mgr = _context.Employees.Find(manager.EmployeeId);
                await _usermgr.AddToRoleAsync(mgr, ApplicationRole.PM);

                if (!String.IsNullOrEmpty(input.ManagersAssistant))
                {
                    ProjectEmployee assist = new ProjectEmployee
                    {
                        Status = ProjectEmployee.CURRENTLY_WORKING,
                        Role = ProjectEmployee.PROJECT_ASSISTANT,
                        ProjectId = pId,
                        EmployeeId = input.ManagersAssistant,
                        WorkPackageId = null
                    };
                    _context.Add(assist);

                    Employee ast = _context.Employees.Find(assist.EmployeeId);
                    await _usermgr.AddToRoleAsync(ast, ApplicationRole.PA);
                }

                WorkPackage mgmt = new WorkPackage
                {
                    ProjectId = pId,
                    WorkPackageCode = "00000",
                    ParentWorkPackageId = null,
                    Name = "Management",
                    Description = "",
                    Status = WorkPackage.ARCHIVED
                };
                _context.Add(mgmt);
                _context.SaveChanges();

                mgmt = _context.WorkPackages
                    .Where(w => w.ProjectId == pId
                             && w.WorkPackageCode == "00000")
                    .FirstOrDefault();

                ProjectEmployee managerAsRE = new ProjectEmployee
                {
                    Status = ProjectEmployee.CURRENTLY_WORKING,
                    Role = ProjectEmployee.RESPONSIBLE_ENGINEER,
                    ProjectId = pId,
                    EmployeeId = input.ProjectManager,
                    WorkPackageId = mgmt.WorkPackageId
                };
                _context.Add(managerAsRE);

                await _usermgr.AddToRoleAsync(mgr, ApplicationRole.RE);

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
        [Authorize(Roles = "PM,PA,AD")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = _context.Projects
                .Include(w => w.WorkPackages)
                .First(f => f.ProjectId == id);

            if (project == null) return NotFound();

            var manager = _context.ProjectEmployees
                .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_MANAGER)
                .FirstOrDefault();

            var assistant = _context.ProjectEmployees
                .Include(a => a.Employee)
                .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_ASSISTANT)
                .FirstOrDefault();

            //Check authorization to edit
            var uid = (await _usermgr.GetUserAsync(User)).Id;
            if (!User.IsInRole("AD") && manager.EmployeeId != uid && assistant != null && assistant.EmployeeId != uid)
                return RedirectToAction(nameof(Index));

            //We get the ProjectEmployees separately so we can Include the Employee of each 
            var mgmtAndUnasigned = _context.ProjectEmployees
                .Include(e => e.Employee)
                .Where(e => e.ProjectId == id
                    && e.Status == ProjectEmployee.CURRENTLY_WORKING)
                .GroupBy(pe => new { pe.EmployeeId, pe.Role })
                .Select(pe => pe.FirstOrDefault())
                .OrderBy(pe => pe.Role)
                .Distinct()
                .ToList();

            var reqs = _context.ProjectRequests
                .Include(r => r.PayGrade)
                .Where(r => r.ProjectId == id)
                .ToList();

            //This is for when new pay grades have been added that the project does not yet have
            #region Not Enough Pay Grades
            var grds = _context.PayGrades.ToList();
            if (reqs.Count < grds.Count)
            {
                bool exists = false;
                foreach (var grade in grds)
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
            model.project.ProjectEmployees = mgmtAndUnasigned;
            model.requests = reqs;
            model.projectManager = manager.EmployeeId;
            if (assistant != null)
                model.managersAssistant = assistant.EmployeeId;

            List<Employee> mgrList = new List<Employee>();
            List<Employee> assList = new List<Employee>();

            ViewBag.Assistant = false;

            if (User.IsInRole("AD"))    //Admin
            {
                mgrList = _context.Employees
                .ToList();

                assList = mgrList;
            }
            else if (manager.EmployeeId == uid) // Project Manager
            {
                mgrList = _context.ProjectEmployees
                    .Where(pe => pe.ProjectId == id)
                    .Join(_context.Employees,
                        pe => pe.EmployeeId,
                        em => em.Id,
                        (pe, em) => em)
                    .Distinct()
                    .ToList();

                assList = mgrList;
            }
            else // Assistant
            {
                mgrList.Add(_context.Employees.Find(manager.EmployeeId));
                assList.Add(_context.Employees.Find(assistant.EmployeeId));
                ViewBag.Assistant = true;
            }

            List<SelectListItem> mgrItems = new List<SelectListItem>();
            mgrItems.AddRange(new SelectList(mgrList, "Id", "Email"));


            List<SelectListItem> assItems = new List<SelectListItem>();

            if (ViewBag.Assistant)
            {
                assItems.Insert(0, new SelectListItem { Text = assistant.Employee.Email, Value = assistant.EmployeeId });
            }
            else
            {
                assItems.AddRange(new SelectList(mgrList, "Id", "Email"));
                assItems.Insert(0, new SelectListItem { Text = "None", Value = "" });
            }


            ViewBag.EmployeesM = mgrItems;
            ViewBag.EmployeesA = assItems;

            ViewBag.Status = new SelectList(Project.Statuses.ToList(), "Key", "Value", project.Status);
            ViewBag.WPs = new SelectList(_context.WorkPackages.Where(w => w.ProjectId == project.ProjectId).ToList()
                , "Name", "Name", project.WorkPackages.First());

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "PM,PA,AD")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ManageProject model)
        {

            if (id != model.project.ProjectId)
            {
                return NotFound();
            }

            var uid = (await _usermgr.GetUserAsync(User)).Id;
            var assist = _context.ProjectEmployees
                    .Where(pe => pe.ProjectId == id && pe.Role == ProjectEmployee.PROJECT_ASSISTANT)
                    .FirstOrDefault();

            bool isAssist = assist != null && assist.EmployeeId.Equals(uid);

            if (isAssist)//the assistants model will have two nulls because they cannot change PM or PMA
            {
                model.projectManager = _context.ProjectEmployees
                    .Where(pe => pe.ProjectId == id && pe.Role == ProjectEmployee.PROJECT_MANAGER)
                    .FirstOrDefault()
                    .EmployeeId;
                model.managersAssistant = uid;
            }
            else
            {
                if (model.projectManager == model.managersAssistant)
                {
                    ViewBag.MgrIsAssist = "Manager and Assistant cannot be the same person!";
                    return await Edit(id);
                }
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

                    ProjectEmployee managerPE = _context.ProjectEmployees
                        .Where(e => e.ProjectId == id && e.Role == ProjectEmployee.PROJECT_MANAGER)
                        .FirstOrDefault();

                    if (model.projectManager != managerPE.EmployeeId) //we are setting a new PM
                    {
                        ProjectEmployee managerRE = _context.ProjectEmployees
                            .Where(e => e.ProjectId == id
                                     && e.Role == ProjectEmployee.RESPONSIBLE_ENGINEER)
                            .Join(_context.WorkPackages,
                                wp => wp.WorkPackageId,
                                pe => pe.WorkPackageId,
                                (pe, wp) => new { wp, pe })
                            .Where(join => join.wp.WorkPackageCode == "00000")
                            .Select(join => join.pe)
                            .FirstOrDefault();

                        Employee oldmgr = _context.Employees.Find(managerPE.EmployeeId);
                        await _usermgr.RemoveFromRoleAsync(oldmgr, ApplicationRole.PM);
                        await _usermgr.RemoveFromRoleAsync(oldmgr, ApplicationRole.RE);

                        Employee newmgr = _context.Employees.Find(model.projectManager);
                        await _usermgr.AddToRoleAsync(newmgr, ApplicationRole.PM);
                        await _usermgr.AddToRoleAsync(newmgr, ApplicationRole.RE);

                        ProjectEmployee unassigned = _context.ProjectEmployees
                            .Where(pe => pe.ProjectId == id
                                && pe.EmployeeId == newmgr.Id
                                && pe.Role == ProjectEmployee.NOT_ASSIGNED)
                            .FirstOrDefault();

                        if (unassigned != null)
                        {
                            unassigned.Role = ProjectEmployee.RESPONSIBLE_ENGINEER;
                            unassigned.Status = ProjectEmployee.CURRENTLY_WORKING;
                            _context.Update(unassigned);
                            _context.Remove(managerRE);
                        }
                        else
                        {
                            managerRE.EmployeeId = model.projectManager;
                            _context.Update(managerRE);
                        }

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
                            assistantPE = new ProjectEmployee       //add them to the project as the PA 
                            {
                                EmployeeId = model.managersAssistant,
                                ProjectId = id,
                                Role = ProjectEmployee.PROJECT_ASSISTANT,
                                Status = ProjectEmployee.CURRENTLY_WORKING,
                                WorkPackageId = null
                            };
                            _context.Add(assistantPE);
                        }
                        else if (model.managersAssistant != assistantPE.EmployeeId) //if the project already had a PA and this one is NEW
                        {
                            _context.Remove(assistantPE);
                            _context.Update(new ProjectEmployee
                            {
                                EmployeeId = model.managersAssistant,
                                ProjectId = id,
                                Role = ProjectEmployee.PROJECT_ASSISTANT,
                                Status = ProjectEmployee.CURRENTLY_WORKING,
                                WorkPackageId = null
                            });
                            Employee oldAssist = _context.Employees.Find(assistantPE.EmployeeId);
                            await _usermgr.RemoveFromRoleAsync(oldAssist, ApplicationRole.PA);
                            //The following makes sure that removing an assistant, doesnt remove them from the
                            //project if that was their only assignment in the project
                            var check = _context.ProjectEmployees
                                .Where(w => w.ProjectId == id && w.EmployeeId == assistantPE.EmployeeId)
                                .FirstOrDefault();
                            if (check == null)
                                _context.Add(new ProjectEmployee
                                {
                                    EmployeeId = assistantPE.EmployeeId,
                                    ProjectId = id,
                                    Status = ProjectEmployee.CURRENTLY_WORKING,
                                    Role = ProjectEmployee.NOT_ASSIGNED
                                });
                        }

                        Employee assistant = _context.Employees.Find(assistantPE.EmployeeId);
                        await _usermgr.AddToRoleAsync(assistant, ApplicationRole.PA);

                    }
                    else
                    {                                   // if we are clearing the PA
                        if (assistantPE != null)
                        {
                            Employee oldAssist = _context.Employees.Find(assistantPE.EmployeeId);
                            await _usermgr.RemoveFromRoleAsync(oldAssist, ApplicationRole.PA);
                            _context.Remove(assistantPE);
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

        public async Task<IActionResult> Close(int id)
        {
            var workPackages = await _context.WorkPackages
                .Where(wp => wp.ProjectId == id)
                .ToListAsync();
            foreach (var workPackage in workPackages)
            {
                workPackage.Status = WorkPackage.CLOSED;
            }
            _context.UpdateRange(workPackages);
            var project = await _context.Projects.FindAsync(id);
            project.Status = Project.CLOSED;
            _context.Update(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Archive(int id)
        {
            var workPackages = await _context.WorkPackages
                .Where(wp => wp.ProjectId == id)
                .Where(wp => wp.Status == WorkPackage.OPENED)
                .ToListAsync();
            foreach (var workPackage in workPackages)
            {
                workPackage.Status = WorkPackage.ARCHIVED;
            }
            _context.UpdateRange(workPackages);
            var project = await _context.Projects.FindAsync(id);
            project.Status = Project.ARCHIVED;
            _context.Update(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reopen(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            project.Status = Project.ONGOING;
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