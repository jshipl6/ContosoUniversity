using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Departments;

public class EditModel : PageModel
{
    private readonly SchoolContext _context;
    public EditModel(SchoolContext context) => _context = context;

    [BindProperty] public Department Department { get; set; } = default!;
    public SelectList Instructors { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Department = await _context.Departments
            .Include(d => d.Administrator)
            .FirstOrDefaultAsync(d => d.DepartmentID == id)
            ?? (Department)null!;
        if (Department == null) return NotFound();

        Instructors = new SelectList(await _context.Instructors
            .OrderBy(i => i.LastName).ToListAsync(), "ID", "LastName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var deptToUpdate = await _context.Departments.FindAsync(id);
        if (deptToUpdate == null) return NotFound();

        if (await TryUpdateModelAsync(deptToUpdate, "Department",
            d => d.Name, d => d.Budget, d => d.StartDate, d => d.AdministratorID))
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        Instructors = new SelectList(await _context.Instructors.OrderBy(i => i.LastName).ToListAsync(), "ID", "LastName");
        return Page();
    }
}
