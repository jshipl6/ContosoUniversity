using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Data
{
    public class ExportModel : PageModel
    {
        private readonly SchoolContext _context;
        public ExportModel(SchoolContext context) => _context = context;

        public int StudentCount { get; private set; }
        public int InstructorCount { get; private set; }
        public int DepartmentCount { get; private set; }
        public int CourseCount { get; private set; }
        public int EnrollmentCount { get; private set; }

        public async Task OnGetAsync()
        {
            StudentCount = await _context.Students.CountAsync();
            InstructorCount = await _context.Instructors.CountAsync();
            DepartmentCount = await _context.Departments.CountAsync();
            CourseCount = await _context.Courses.CountAsync();
            EnrollmentCount = await _context.Enrollments.CountAsync();
        }

        public async Task<FileContentResult> OnPostDownloadAsync()
        {
            // Pull everything we need with minimal round-trips
            var students = await _context.Students
                .AsNoTracking()
                .OrderBy(s => s.ID)
                .ToListAsync();

            var instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .AsNoTracking()
                .OrderBy(i => i.ID)
                .ToListAsync();

            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.DepartmentID)
                .ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.CourseAssignments)
                    .ThenInclude(ca => ca.Instructor)
                .AsNoTracking()
                .OrderBy(c => c.CourseID)
                .ToListAsync();

            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .OrderBy(e => e.EnrollmentID)
                .ToListAsync();

            // Stable, compact ids for refs in XML
            string StudentRef(int studentId) => $"s{studentId}";
            string InstructorRef(int instructorId) => $"i{instructorId}";
            string DepartmentRef(int deptId) => $"d{deptId}";

            var doc =
                new XDocument(
                    new XElement("Seed",
                        new XElement("Students",
                            students.Select(s =>
                                new XElement("Student",
                                    new XAttribute("id", StudentRef(s.ID)),
                                    new XElement("FirstName", s.FirstMidName),
                                    new XElement("LastName", s.LastName),
                                    new XElement("EnrollmentDate", s.EnrollmentDate.ToString("yyyy-MM-dd"))
                                ))
                        ),
                        new XElement("Instructors",
                            instructors.Select(i =>
                                new XElement("Instructor",
                                    new XAttribute("id", InstructorRef(i.ID)),
                                    new XElement("FirstName", i.FirstMidName),
                                    new XElement("LastName", i.LastName),
                                    new XElement("HireDate", i.HireDate.ToString("yyyy-MM-dd"))
                                ))
                        ),
                        new XElement("OfficeAssignments",
                            instructors
                                .Where(i => i.OfficeAssignment != null)
                                .Select(i =>
                                    new XElement("OfficeAssignment",
                                        new XElement("InstructorRef", InstructorRef(i.ID)),
                                        new XElement("Location", i.OfficeAssignment!.Location)
                                    )))
                        ,
                        new XElement("Departments",
                            departments.Select(d =>
                                new XElement("Department",
                                    new XAttribute("id", DepartmentRef(d.DepartmentID)),
                                    new XElement("Name", d.Name),
                                    new XElement("Budget", d.Budget),
                                    new XElement("StartDate", d.StartDate.ToString("yyyy-MM-dd")),
                                    d.AdministratorID.HasValue
                                        ? new XElement("AdministratorRef", InstructorRef(d.AdministratorID.Value))
                                        : null
                                )))
                        ,
                        new XElement("Courses",
                            courses.Select(c =>
                                new XElement("Course",
                                    new XElement("CourseID", c.CourseID),
                                    new XElement("Title", c.Title),
                                    new XElement("Credits", c.Credits),
                                    new XElement("DepartmentRef", DepartmentRef(c.DepartmentID)),
                                    new XElement("InstructorRefs",
                                        c.CourseAssignments
                                         .OrderBy(ca => ca.InstructorID)
                                         .Select(ca =>
                                            new XElement("InstructorRef", InstructorRef(ca.InstructorID))))
                                )))
                        ,
                        new XElement("Enrollments",
                            enrollments.Select(e =>
                                new XElement("Enrollment",
                                    new XElement("StudentRef", StudentRef(e.StudentID)),
                                    new XElement("CourseRef", e.CourseID),
                                    e.Grade.HasValue ? new XElement("Grade", e.Grade.Value.ToString()) : null
                                )))
                    )
                );

            var bytes = Encoding.UTF8.GetBytes(doc.Declaration + doc.ToString(SaveOptions.DisableFormatting));
            return File(bytes, "application/xml", "seed-export.xml");
        }
    }
}
