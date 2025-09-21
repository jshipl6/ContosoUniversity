using ContosoUniversity.Data;
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Students;

public class IndexModel : PageModel
{
    private readonly SchoolContext _db;
    public IndexModel(SchoolContext db) => _db = db;

    // Query string bound inputs
    [BindProperty(SupportsGet = true)] public string? Q { get; set; }           // filter text
    [BindProperty(SupportsGet = true)] public string? Sort { get; set; }        // "name" | "name_desc" | "date" | "date_desc"
    [BindProperty(SupportsGet = true)] public int? Page { get; set; }           // page index (1-based)

    // header link helpers
    public string NameSortParam => Sort == "name" ? "name_desc" : "name";
    public string DateSortParam => Sort == "date" ? "date_desc" : "date";

    // Data
    public PaginatedList<Student> Students { get; private set; } = default!;

    public async Task OnGetAsync()
    {
        var query = _db.Students.AsNoTracking();

        // FILTER
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var term = Q.Trim().ToLower();
            query = query.Where(s => s.FirstName.ToLower().Contains(term) ||
                                     s.LastName.ToLower().Contains(term));
        }

        // SORT
        query = Sort switch
        {
            "name" => query.OrderBy(s => s.LastName).ThenBy(s => s.FirstName),
            "name_desc" => query.OrderByDescending(s => s.LastName).ThenByDescending(s => s.FirstName),
            "date" => query.OrderBy(s => s.EnrollmentDate),
            "date_desc" => query.OrderByDescending(s => s.EnrollmentDate),
            _ => query.OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
        };

        // PAGING
        int pageIndex = Page.GetValueOrDefault(1);
        const int pageSize = 5;

        Students = await PaginatedList<Student>.CreateAsync(query, pageIndex, pageSize);
    }
}
