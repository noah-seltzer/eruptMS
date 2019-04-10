using System.Linq;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.Services
{
    public class SickLeaveService : ISickLeaveService
    {
        private readonly ApplicationDbContext _context;

        public SickLeaveService(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public void updateSickLeaves()
        {
            var employees = _context.Employees.ToList();
            foreach (Employee employee in employees)
            {
                employee.SickLeave = 0;
            }
            _context.Employees.UpdateRange(employees);
            _context.SaveChanges();
        }
    }
    public interface ISickLeaveService
    {
        void updateSickLeaves();
    }
}