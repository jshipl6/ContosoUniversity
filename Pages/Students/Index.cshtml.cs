using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(SchoolContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public PaginatedList<Student> Students { get; set; } = null!;

        public string NameSort { get; set; } = string.Empty;
        public string DateSort { get; set; } = string.Empty;
        public string CurrentSort { get; set; } = string.Empty;
        public string CurrentFilter { get; set; } = string.Empty;

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString ?? string.Empty;

            // Structured log with named properties
            _logger.LogInformation(
                "Students index requested. Sort={Sort} Search='{Search}' Page={Page}",
                CurrentSort,
                CurrentFilter,
                pageIndex ?? 1
            );

            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;

            if (!string.IsNullOrWhiteSpace(CurrentFilter))
            {
                studentsIQ = studentsIQ.Where(s =>
                    s.LastName.Contains(CurrentFilter) ||
                    s.FirstMidName.Contains(CurrentFilter));
            }

            studentsIQ = CurrentSort switch
            {
                "name_desc" => studentsIQ.OrderByDescending(s => s.LastName),
                "Date" => studentsIQ.OrderBy(s => s.EnrollmentDate),
                "date_desc" => studentsIQ.OrderByDescending(s => s.EnrollmentDate),
                _ => studentsIQ.OrderBy(s => s.LastName),
            };

            const int pageSize = 3;
            Students = await PaginatedList<Student>.CreateAsync(
                studentsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

            _logger.LogInformation(
                "Students page generated. ResultCount={Count} HasNext={HasNext} HasPrev={HasPrev}",
                Students.Count,
                Students.HasNextPage,
                Students.HasPreviousPage
            );
        }
    }
}
