using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.Models;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisementController : ControllerBase
    {
        private readonly DimajProfessorsDbContext _context;

        public AdvisementController(DimajProfessorsDbContext context)
        {
            _context = context;
        }

        // GET: api/<AdvisementController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Advisement/GetPublicAdvisements/{ProfessorEmail}
        [HttpGet]
        [Route("[action]/{ProfessorEmail}")]
        public async Task<ActionResult<IEnumerable<Advisement>>> GetPublicAdvisements(string ProfessorEmail)
        {
         
            var professor = await _context.Users
                .Where(u => u.Email == ProfessorEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (professor == null)
            {
                return NotFound($"No se encontró un profesor con el correo {ProfessorEmail}.");
            }

            var advisements = await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.Student)
                .Where(a => a.IsPublic == true && a.Course.ProfessorId != professor)
                .ToListAsync();

            if (!advisements.Any())
            {
                return NotFound("No hay consultas públicas disponibles.");
            }

            var filteredAdvisements = advisements.Select(a => new Advisement
            {
                Id = a.Id,
                Content = a.Content,
                Status = a.Status,
                IsPublic = a.IsPublic,
                CreatedAt = a.CreatedAt,
                CourseId = a.CourseId,
                StudentId = a.StudentId,

               
                Course = a.Course != null ? new Course
                {
                    Id = a.Course.Id,
                    Code = a.Course.Code,
                    Name = a.Course.Name
                } : null,

             
                Student = a.Student != null ? new User
                {
                    Id = a.Student.Id,
                    Name = a.Student.Name,
                    Email = a.Student.Email
                } : null

            }).ToList();

            return Ok(filteredAdvisements);
        }

        // GET: api/Advisement/GetMyAdvisements/{ProfessorEmail}
        [HttpGet]
        [Route("[action]/{ProfessorEmail}")] // Parámetro en la URL
        public async Task<ActionResult<IEnumerable<Advisement>>> GetMyAdvisements(string ProfessorEmail)
        {
            var professor = await _context.Users
                .Where(u => u.Email == ProfessorEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (professor == null)
            {
                return NotFound($"No se encontró un profesor con el correo {ProfessorEmail}.");
            }

            var advisements = await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.Student)
                .Where(a => a.Course.ProfessorId == professor) // Todas las consultas del profesor
                .ToListAsync();

            if (!advisements.Any()) 
            {
                return NotFound("No hay consultas disponibles.");
            }

           
            var filteredAdvisements = advisements.Select(a => new Advisement
            {
                Id = a.Id,
                Content = a.Content,
                Status = a.Status,
                IsPublic = a.IsPublic,
                CreatedAt = a.CreatedAt,
                CourseId = a.CourseId,
                StudentId = a.StudentId,

                Course = a.Course != null ? new Course
                {
                    Id = a.Course.Id,
                    Code = a.Course.Code,
                    Name = a.Course.Name
                } : null,

          
                Student = a.Student != null ? new User
                {
                    Id = a.Student.Id,
                    Name = a.Student.Name
                } : null

            }).ToList();

            return Ok(filteredAdvisements);
        }

        // GET: api/Advisement/GetAdvisementById/{AdvisementId}
        [HttpGet]
        [Route("[action]/{AdvisementId}")]
        public async Task<ActionResult<Advisement>> GetAdvisementById(string AdvisementId)
        {
            var advisement = await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.Student)
                .Where(a => a.Id == AdvisementId)
                .Select(a => new Advisement
                {
                    Id = a.Id,
                    CourseId = a.CourseId,
                    Content = a.Content,
                    Status = a.Status,
                    IsPublic = a.IsPublic,
                    StudentId = a.StudentId,
                    CreatedAt = a.CreatedAt,
                    Course = a.Course,
                    Student = a.Student
                })
                .FirstOrDefaultAsync();

            if (advisement == null)
            {
                return NotFound($"No se encontró la consulta con ID {AdvisementId}.");
            }

            return Ok(advisement);
        }



    }
}
