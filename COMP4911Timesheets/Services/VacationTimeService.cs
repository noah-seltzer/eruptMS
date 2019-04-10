using System.Linq;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;

namespace COMP4911Timesheets.Services
{
    public class VacationTimeService : IService
    {
        private readonly ApplicationDbContext _context;

        public VacationTimeService(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public void updateVacationTimes()
        {
            var employees = _context.Employees.ToList();
            foreach (Employee employee in employees)
            {
                employee.VacationTime += 8;
            }
            _context.Employees.UpdateRange(employees);
            _context.SaveChanges();
        }
    }
    public interface IService
    {
        void updateVacationTimes();
    }
}