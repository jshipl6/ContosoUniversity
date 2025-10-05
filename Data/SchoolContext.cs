using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data;

public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<OfficeAssignment> OfficeAssignments => Set<OfficeAssignment>();
    public DbSet<CourseAssignment> CourseAssignments => Set<CourseAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CourseAssignment>()
            .HasKey(ca => new { ca.CourseID, ca.InstructorID });

        modelBuilder.Entity<OfficeAssignment>()
            .HasOne(o => o.Instructor)
            .WithOne(i => i.OfficeAssignment!)
            .HasForeignKey<OfficeAssignment>(o => o.InstructorID);
    }
}
