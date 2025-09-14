using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Students;

public class DeleteModel : PageModel
{
    private readonly SchoolContext _db;
    public DeleteModel(SchoolContext db) => _db = db;

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
        var s = await _db.Students.FindAsync(Student.Id);
        if (s is null) return NotFound();
        _db.Students.Remove(s);
        await _db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
