using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Students;

public class CreateModel : PageModel
{
    private readonly SchoolContext _db;
    public CreateModel(SchoolContext db) => _db = db;

    [BindProperty]
    public StudentInputModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Server-side validation failure; redisplay form
            return Page();
        }

        var entity = new Student
        {
            FirstMidName = Input.FirstName,
            LastName = Input.LastName,
            EnrollmentDate = Input.EnrollmentDate
        };

        _db.Students.Add(entity);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = $"Student “{entity.LastName}, {entity.FirstMidName}” created.";
        // PRG: avoid double-post, redirect to Index
        return RedirectToPage("./Index");
    }
}
