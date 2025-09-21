using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data;

public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; } = default!;
    public DbSet<Enrollment> Enrollments { get; set; } = default!;
    public DbSet<Course> Courses { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed a few rows so the UI isn’t empty on first run
        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, FirstName = "Ada", LastName = "Lovelace", EnrollmentDate = new DateTime(2024, 9, 1) },
            new Student { Id = 2, FirstName = "Alan", LastName = "Turing", EnrollmentDate = new DateTime(2024, 9, 1) },
            new Student { Id = 3, FirstName = "Grace", LastName = "Hopper", EnrollmentDate = new DateTime(2024, 9, 1) }
        );
    }
}
