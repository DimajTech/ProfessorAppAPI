using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAppAPI.Models;

namespace StudentAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly DimajStudentsDbContext _context;

        public CourseController(DimajStudentsDbContext context)
        {
            _context = context;
        }

        // GET: api/User/GetCourses
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            try
            {
                var courses = await _context.Courses
                    .Include(course => course.Professor)
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ha ocurrido un error al obtener los cursos.", Details = ex.Message });
            }
        }

        // GET: api/Course/GetCourse/5
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<Course>> GetCourse(string id)
        {
            var course = await _context.Courses.Include(course => course.Professor)
            .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // POST: api/Course/PostCourse
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // PUT: api/Course/PutCourse/5
        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<IActionResult> PutCourse(string id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            var courseToUpdate = new Course
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                ProfessorId = course.ProfessorId,
                Semester = course.Semester,
                Year = course.Year,
                IsActive = course.IsActive,
            };

            _context.Entry(courseToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Student/DeleteCourse/5
        [HttpDelete]
        [Route("[action]/{id}")]
        public async Task<IActionResult> DeleteCorse(string id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(string id)
        {
            return _context.Courses.Any(e => e.Id.Equals(id));
        }

    }
}
