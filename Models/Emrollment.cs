using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models;

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(0, 100)]
    public int? Grade { get; set; }
}
