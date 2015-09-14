using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using OI.Entities.Models;
using Repository.Pattern.Ef6;

namespace OI.Entities
{
    public class OIDataContext : DataContext
    {
        static OIDataContext()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<EResultDataContext, Migrations.Configuration>("EResultDataContext"));
        }

        public OIDataContext()
            : base("Name=OIDataContext")
        {
        }
        
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }

        /*

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // account
            modelBuilder.Entity<Account>().HasRequired(r => r.Role).WithMany().HasForeignKey(r => r.RoleId);
            modelBuilder.Entity<Account>().HasRequired(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId);
            modelBuilder.Entity<Account>().Property(t => t.Username).HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute("IX_Username", 1) { IsUnique = true }));

            // document
            modelBuilder.Entity<Document>().HasRequired(e => e.Employee).WithMany().HasForeignKey(e => e.EmployeeId);

            // employee
            modelBuilder.Entity<Employee>().HasRequired(c => c.Company).WithMany().HasForeignKey(c => c.CompanyId);
            

            base.OnModelCreating(modelBuilder);
        }*/
    }
}
