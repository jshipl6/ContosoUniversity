namespace ContosoUniversity.Models;

public class Department
{
    public int DepartmentID { get; set; }
    public string Name { get; set; } = "";
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }

    public int? AdministratorID { get; set; }
    public Instructor? Administrator { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
