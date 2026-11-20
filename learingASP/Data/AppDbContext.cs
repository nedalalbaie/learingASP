using learingASP.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace learingASP.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        // Or this ->  public DbSet<Character> Characters { get; set; }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(e =>
            {
                e.HasIndex(emp => emp.Email)
                    .IsUnique();

                e.Property(emp => emp.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                e.Property(emp => emp.PasswordHash)
                    .IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.HasIndex(t => t.TokenHash).IsUnique();

                e.Property(t => t.TokenHash)
                    .IsRequired()
                    .HasMaxLength(64);

                e.HasOne(t => t.Employee)
                    .WithMany(emp => emp.RefreshTokens)
                    .HasForeignKey(t => t.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LeaveRequest>(e =>
            {
                e.Property(r => r.Days).HasPrecision(5, 2);

                e.HasOne(r => r.Employee)
                 .WithMany(emp => emp.LeaveRequests)
                 .HasForeignKey(r => r.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.CreatedBy)
                 .WithMany()
                 .HasForeignKey(r => r.CreatedById)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.ReviewedBy)
                 .WithMany()
                 .HasForeignKey(r => r.ReviewedById)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
