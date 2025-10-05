using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Instructors
{
    public class EditModel : PageModel
    {
        private readonly SchoolContext _context;

        public EditModel(SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instructor Instructor { get; set; } = default!;

        // Supplies checkbox rows to the page
        public List<AssignedCourseData> AssignedCourseDataList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
                return NotFound();

            // Include office and current course assignments
            Instructor? instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ID == id.Value);

            if (instructor is null)
                return NotFound();

            Instructor = instructor;

            await PopulateAssignedCourseDataAsync(instructor);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCourses)
        {
            if (id is null)
                return NotFound();

            // Load the instructor to update, including their current course assignments
            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                .FirstOrDefaultAsync(i => i.ID == id.Value);

            if (instructorToUpdate is null)
                return NotFound();

            // Try update simple scalar fields and the nested OfficeAssignment.Location
            bool updated = await TryUpdateModelAsync(
                instructorToUpdate,
                "Instructor",
                i => i.LastName,
                i => i.FirstMidName,
                i => i.HireDate,
                i => i.OfficeAssignment
            );

            // Normalize empty location to null (matches tutorial)
            if (updated && instructorToUpdate.OfficeAssignment is not null &&
                string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
            {
                instructorToUpdate.OfficeAssignment = null;
            }

            // Update the join table records from the posted checkbox list
            UpdateInstructorCourses(selectedCourses, instructorToUpdate);

            if (!ModelState.IsValid)
            {
                // If something failed, rebuild the checkbox list and show page again
                await PopulateAssignedCourseDataAsync(instructorToUpdate);
                return Page();
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private async Task PopulateAssignedCourseDataAsync(Instructor instructor)
        {
            var allCourses = await _context.Courses
                .Include(c => c.Department)
                .OrderBy(c => c.Title)
                .ToListAsync();

            var assignedCourseIds = new HashSet<int>(
                instructor.CourseAssignments.Select(ca => ca.CourseID));

            AssignedCourseDataList = allCourses.Select(c => new AssignedCourseData
            {
                CourseID = c.CourseID,
                Title = c.Title,
                Department = c.Department?.Name,
                Assigned = assignedCourseIds.Contains(c.CourseID)
            }).ToList();
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            // If user selected nothing, clear any existing assignments
            if (selectedCourses is null || selectedCourses.Length == 0)
            {
                instructorToUpdate.CourseAssignments.Clear();
                return;
            }

            var selectedIds = new HashSet<int>(selectedCourses.Select(int.Parse));
            var currentIds = new HashSet<int>(instructorToUpdate.CourseAssignments.Select(ca => ca.CourseID));

            // Add newly checked
            foreach (int courseId in selectedIds)
            {
                if (!currentIds.Contains(courseId))
                {
                    instructorToUpdate.CourseAssignments.Add(new CourseAssignment
                    {
                        CourseID = courseId,
                        InstructorID = instructorToUpdate.ID
                    });
                }
            }

            // Remove unchecked
            foreach (var ca in instructorToUpdate.CourseAssignments.ToList())
            {
                if (!selectedIds.Contains(ca.CourseID))
                {
                    instructorToUpdate.CourseAssignments.Remove(ca);
                }
            }
        }
    }
}
