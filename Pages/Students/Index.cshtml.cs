using ContosoUniversity.Data;
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Students;

public class IndexModel : PageModel
{
    private readonly SchoolContext _context;
    private readonly int _pageSize;

    // IConfiguration is resolved by DI automatically
    public IndexModel(SchoolContext context, IConfiguration configuration)
    {
        _context = context;
        _pageSize = configuration.GetValue<int>("PageSize", 4); // tutorial defaults to 3/4; choose 4 if config is missing
    }

    public string NameSort { get; set; } = "";
    public string DateSort { get; set; } = "";
    public string CurrentFilter { get; set; } = "";
    public string CurrentSort { get; set; } = "";

    public PaginatedList<Student> Students { get; set; } = default!;

    public async Task OnGetAsync(string? sortOrder, string? currentFilter, string? searchString, int? pageIndex)
    {
        CurrentSort = sortOrder ?? string.Empty;
        NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        DateSort = sortOrder == "Date" ? "date_desc" : "Date";

        // New search resets to first page
        if (searchString != null)
        {
            pageIndex = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        CurrentFilter = searchString ?? string.Empty;

        IQueryable<Student> studentsIQ = _context.Students;

        // FILTER
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            studentsIQ = studentsIQ.Where(s =>
                s.LastName.Contains(searchString) ||
                s.FirstMidName.Contains(searchString));
        }

        // SORT
        studentsIQ = sortOrder switch
        {
            "name_desc" => studentsIQ.OrderByDescending(s => s.LastName),
            "Date" => studentsIQ.OrderBy(s => s.EnrollmentDate),
            "date_desc" => studentsIQ.OrderByDescending(s => s.EnrollmentDate),
            _ => studentsIQ.OrderBy(s => s.LastName)
        };

        // PAGING
        var page = pageIndex ?? 1;
        var pageSize = _pageSize; // read from config
        Students = await PaginatedList<Student>.CreateAsync(
            studentsIQ.AsNoTracking(), page, pageSize);
    }
}
