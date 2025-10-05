using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Xml.Linq;
using CourseAssignmentEntity = ContosoUniversity.Models.CourseAssignment;
using CourseEntity = ContosoUniversity.Models.Course;
using DepartmentEntity = ContosoUniversity.Models.Department;
using EnrollmentEntity = ContosoUniversity.Models.Enrollment;
using InstructorEntity = ContosoUniversity.Models.Instructor;
using OfficeAssignmentEntity = ContosoUniversity.Models.OfficeAssignment;
// Aliases so the compiler always uses your EF entities
using StudentEntity = ContosoUniversity.Models.Student;

namespace ContosoUniversity.Data
{
    public static class SeedXml
    {
        public static async Task EnsureSeededAsync(IServiceProvider services, string contentRoot)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SchoolContext>();

            await context.Database.MigrateAsync();

            if (await context.Students.AnyAsync()
             || await context.Instructors.AnyAsync()
             || await context.Departments.AnyAsync()
             || await context.Courses.AnyAsync())
            {
                return;
            }

            var xmlPath = Path.Combine(contentRoot, "Data", "seed.xml");
            var doc = XDocument.Load(xmlPath);
            var root = doc.Root!;

            var studentMap = new Dictionary<string, int>();
            var instructorMap = new Dictionary<string, int>();
            var deptMap = new Dictionary<string, int>();

            // Students
            foreach (var s in root.Element("Students")!.Elements("Student"))
            {
                var xmlId = (string)s.Attribute("id")!;
                var first = (string)s.Element("FirstName")!;
                var last = (string)s.Element("LastName")!;
                var enroll = DateTime.Parse((string)s.Element("EnrollmentDate")!, CultureInfo.InvariantCulture);

                var student = new StudentEntity
                {
                    FirstMidName = first,
                    LastName = last,
                    EnrollmentDate = enroll
                };

                context.Students.Add(student);
                await context.SaveChangesAsync();

                studentMap[xmlId] = student.ID;
            }

            // Instructors
            foreach (var i in root.Element("Instructors")!.Elements("Instructor"))
            {
                var xmlId = (string)i.Attribute("id")!;
                var first = (string)i.Element("FirstName")!;
                var last = (string)i.Element("LastName")!;
                var hire = DateTime.Parse((string)i.Element("HireDate")!, CultureInfo.InvariantCulture);

                var instructor = new InstructorEntity
                {
                    FirstMidName = first,
                    LastName = last,
                    HireDate = hire
                };

                context.Instructors.Add(instructor);
                await context.SaveChangesAsync();

                instructorMap[xmlId] = instructor.ID;
            }

            // OfficeAssignments
            foreach (var o in root.Element("OfficeAssignments")!.Elements("OfficeAssignment"))
            {
                var instrRef = (string)o.Element("InstructorRef")!;
                var location = (string)o.Element("Location")!;

                if (!instructorMap.TryGetValue(instrRef, out var instrId)) continue;

                context.OfficeAssignments.Add(new OfficeAssignmentEntity
                {
                    InstructorID = instrId,
                    Location = location
                });
            }
            await context.SaveChangesAsync();

            // Departments
            foreach (var d in root.Element("Departments")!.Elements("Department"))
            {
                var xmlId = (string)d.Attribute("id")!;
                var name = (string)d.Element("Name")!;
                var budget = decimal.Parse((string)d.Element("Budget")!, CultureInfo.InvariantCulture);
                var start = DateTime.Parse((string)d.Element("StartDate")!, CultureInfo.InvariantCulture);
                var adminRef = (string?)d.Element("AdministratorRef");

                var dept = new DepartmentEntity
                {
                    Name = name,
                    Budget = budget,
                    StartDate = start,
                    AdministratorID = adminRef != null && instructorMap.TryGetValue(adminRef, out var aid) ? aid : null
                };

                context.Departments.Add(dept);
                await context.SaveChangesAsync();

                deptMap[xmlId] = dept.DepartmentID;
            }

            // Courses (create rows, remember instructor refs)
            var courseTemp = new List<(CourseEntity course, IEnumerable<string> instrRefs)>();
            foreach (var c in root.Element("Courses")!.Elements("Course"))
            {
                var courseId = int.Parse((string)c.Element("CourseID")!, CultureInfo.InvariantCulture);
                var title = (string)c.Element("Title")!;
                var credits = int.Parse((string)c.Element("Credits")!, CultureInfo.InvariantCulture);
                var deptRef = (string)c.Element("DepartmentRef")!;

                var course = new CourseEntity
                {
                    CourseID = courseId,
                    Title = title,
                    Credits = credits,
                    DepartmentID = deptMap[deptRef]
                };

                context.Courses.Add(course);

                var refs = c.Element("InstructorRefs")!.Elements("InstructorRef").Select(x => (string)x);
                courseTemp.Add((course, refs));
            }
            await context.SaveChangesAsync();

            // CourseAssignments
            foreach (var (course, refs) in courseTemp)
            {
                foreach (var r in refs)
                {
                    if (instructorMap.TryGetValue(r, out var instrId))
                    {
                        context.CourseAssignments.Add(new CourseAssignmentEntity
                        {
                            CourseID = course.CourseID,
                            InstructorID = instrId
                        });
                    }
                }
            }
            await context.SaveChangesAsync();

            // Enrollments
            foreach (var e in root.Element("Enrollments")!.Elements("Enrollment"))
            {
                var sRef = (string)e.Element("StudentRef")!;
                var cRef = int.Parse((string)e.Element("CourseRef")!, CultureInfo.InvariantCulture);
                var gradeText = (string?)e.Element("Grade");

                Grade? grade = gradeText switch
                {
                    "A" => Grade.A,
                    "B" => Grade.B,
                    "C" => Grade.C,
                    "D" => Grade.D,
                    "F" => Grade.F,
                    _ => null
                };

                context.Enrollments.Add(new EnrollmentEntity
                {
                    StudentID = studentMap[sRef],
                    CourseID = cRef,
                    Grade = grade
                });
            }
            await context.SaveChangesAsync();
        }
    }
}
