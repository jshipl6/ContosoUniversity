using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models;

public class Course
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 10)]
    public int Credits { get; set; }
}
