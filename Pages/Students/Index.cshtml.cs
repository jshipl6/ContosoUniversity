using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Students;

public class IndexModel : PageModel
{
    private readonly SchoolContext _db;
    public IndexModel(SchoolContext db) => _db = db;

    public List<Student> Students { get; private set; } = new();
    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<Student> q = _db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Query))
        {
            var term = Query.Trim().ToLower();
            q = q.Where(s => s.FirstName.ToLower().Contains(term) ||
                             s.LastName.ToLower().Contains(term));
        }

        Students = await q.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToListAsync();
    }
}
