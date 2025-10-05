using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;
        public IndexModel(SchoolContext context) => _context = context;

        public IList<Department> Departments { get; private set; } = new List<Department>();

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments
                .Include(d => d.Administrator)
                .OrderBy(d => d.Name)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
