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
            // Buscar el ID del profesor a partir del email
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
                .Where(a => a.IsPublic == true && a.Course.ProfessorId != professor) // Excluir consultas del profesor
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
                .ToListAsync();

            if (!advisements.Any())
            {
                return NotFound("No hay consultas públicas disponibles.");
            }

            return Ok(advisements);
        }

        // GET: api/Advisement/GetMyAdvisements/{ProfessorEmail}
        [HttpGet]
        [Route("[action]/{ProfessorEmail}")] // Parámetro en la URL
        public async Task<ActionResult<IEnumerable<Advisement>>> GetMyAdvisements(string ProfessorEmail)
        {
            // Buscar el ID del profesor a partir del email
            var professor = await _context.Users
                .Where(u => u.Email == ProfessorEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (professor == null)
            {
                return NotFound($"No se encontró un profesor con el correo {ProfessorEmail}.");
            }

            var advisements = await _context.Advisements
                .Where(a => a.Course.ProfessorId == professor) // Todas las consultas del profesor
                .Select(a => new Advisement
                {
                    Id = a.Id,
                    CourseId = a.CourseId,
                    Content = a.Content,
                    Status = a.Status,
                    IsPublic = a.IsPublic, // Indica si la consulta es pública o privada
                    StudentId = a.StudentId,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            if (!advisements.Any()) // Si la lista está vacía
            {
                return NotFound("No hay consultas disponibles.");
            }

            return Ok(advisements);
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
