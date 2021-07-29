using Microsoft.EntityFrameworkCore;

namespace APIOkta
{
    public class EmployeeDbContext:DbContext
    {
        public DbSet<Employee> Employees { get; set; }

    }
}