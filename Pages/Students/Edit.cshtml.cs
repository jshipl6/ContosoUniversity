using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Students;

public class EditModel : PageModel
{
    private readonly SchoolContext _db;
    public EditModel(SchoolContext db) => _db = db;

    [BindProperty]
    public StudentInputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var s = await _db.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (s is null) return NotFound();

        Input = new StudentInputModel
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            EnrollmentDate = s.EnrollmentDate
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var s = await _db.Students.FindAsync(Input.Id);
        if (s is null) return NotFound();

        s.FirstName = Input.FirstName;
        s.LastName = Input.LastName;
        s.EnrollmentDate = Input.EnrollmentDate;
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = $"Student “{s.LastName}, {s.FirstName}” updated.";
        return RedirectToPage("./Index");
    }
}
