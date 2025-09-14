using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Students;

public class CreateModel : PageModel
{
    private readonly SchoolContext _db;
    public CreateModel(SchoolContext db) => _db = db;

    [BindProperty]
    public Student Student { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        _db.Students.Add(Student);
        await _db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
