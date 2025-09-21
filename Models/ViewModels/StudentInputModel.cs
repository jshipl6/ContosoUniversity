using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models.ViewModels;

public class StudentInputModel
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(40)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Enrollment date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow.Date;
}
