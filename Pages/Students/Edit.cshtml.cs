using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Students;

public class EditModel : PageModel
{
    private readonly SchoolContext _db;
    public EditModel(SchoolContext db) => _db = db;

    [BindProperty]
    public Student Student { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s is null) return NotFound();
        Student = s;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var exists = await _db.Students.FindAsync(Student.Id);
        if (exists is null) return NotFound();

        exists.FirstName = Student.FirstName;
        exists.LastName = Student.LastName;
        exists.EnrollmentDate = Student.EnrollmentDate;

        await _db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
