namespace ContosoUniversity.Models;

public class Course
{
    public int CourseID { get; set; }
    public string Title { get; set; } = "";
    public int Credits { get; set; }

    public int DepartmentID { get; set; }
    public Department? Department { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
}
