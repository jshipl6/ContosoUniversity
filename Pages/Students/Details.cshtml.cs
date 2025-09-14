using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Students;

public class DetailsModel : PageModel
{
    private readonly SchoolContext _db;
    public DetailsModel(SchoolContext db) => _db = db;

    public Student Student { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s is null) return NotFound();
        Student = s;
        return Page();
    }
}
