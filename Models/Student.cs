using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models;

public class Student
{
    public int ID { get; set; }

    [Required, StringLength(50), Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(50), Display(Name = "First Name")]
    public string FirstMidName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [NotMapped]
    public string FullName => $"{LastName}, {FirstMidName}";
}
