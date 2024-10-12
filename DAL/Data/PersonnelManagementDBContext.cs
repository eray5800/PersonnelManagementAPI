using DAL.Configuration;
using DAL.Models;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core
{
    public class PersonnelManagementDBContext : IdentityDbContext<Employee, IdentityRole<Guid>,Guid>
    {
        public PersonnelManagementDBContext(DbContextOptions<PersonnelManagementDBContext> options) : base(options)
        {



        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDetail> EmployeeDetails { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<Expense> Expenses {  get; set; }
        public DbSet<ExpenseRequest> ExpenseRequests { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyHoliday> CompanyHolidays { get; set; }

        public DbSet<CompanyRequest> CompanyRequests { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Education> Educations { get; set; }

        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Leave> Leaves { get; set; }

        




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeDetailConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveRequestConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseRequestConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyHolidayConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyRequestConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());

            modelBuilder.ApplyConfiguration(new CertificationConfiguration());
            modelBuilder.ApplyConfiguration(new EducationConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveConfiguration());
            
            
            base.OnModelCreating(modelBuilder);
        
        }

    }

}
