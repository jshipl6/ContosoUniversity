using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages;

public class AboutModel : PageModel
{
    private readonly SchoolContext _context;
    public AboutModel(SchoolContext context) => _context = context;

    public IList<EnrollmentDateGroup> Students { get; set; } = new List<EnrollmentDateGroup>();

    public async Task OnGetAsync()
    {
        IQueryable<EnrollmentDateGroup> data =
            from student in _context.Students
            group student by student.EnrollmentDate into dateGroup
            select new EnrollmentDateGroup
            {
                EnrollmentDate = dateGroup.Key,
                StudentCount = dateGroup.Count()
            };

        Students = await data.AsNoTracking().ToListAsync();
    }
}
