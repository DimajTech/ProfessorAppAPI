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
        
        // GET: api/Advisement/GetAdvisements
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<Advisement>>> GetAdvisements()
        {
            return await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.Student)
                .Select(advisement => new Advisement()
                {
                    Id = advisement.Id,
                    Content = advisement.Content,
                    Status = advisement.Status,
                    IsPublic = advisement.IsPublic,
                    CreatedAt = advisement.CreatedAt,
                    Course = advisement.Course,
                    Student = advisement.Student
                })
                .ToListAsync();
        }
        
        
        // GET: api/Advisement/GetAdvisement
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<Advisement>> GetAdvisement(string id)
        {
            var advisement = await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (advisement == null)
            {
                return NotFound(new { Message = "Advisement not found" });
            }

            return advisement;
        }

        // PUT: api/Advisement/PutAdvisement
        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<IActionResult> PutAdvisement(string id, Advisement updatedAdvisement)
        {
            if (id != updatedAdvisement.Id)
            {
                return BadRequest(new { Message = "Advisement ID mismatch" });
            }

            var advisement = new Advisement
            {
                Id = updatedAdvisement.Id,
                Content = updatedAdvisement.Content,
                Status = updatedAdvisement.Status,
                IsPublic = updatedAdvisement.IsPublic,
                CreatedAt = updatedAdvisement.CreatedAt,
                CourseId = updatedAdvisement.CourseId,
                StudentId = updatedAdvisement.StudentId
            };

            _context.Entry(advisement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisementExists(id))
                {
                    return NotFound(new { Message = "Advisement not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Advisement/PostAdvisement
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Advisement>> PostAdvisement(Advisement newAdvisement)
        {
            var advisement = new Advisement
            {
                Id = Guid.NewGuid().ToString(),
                Content = newAdvisement.Content,
                Status = newAdvisement.Status,
                IsPublic = newAdvisement.IsPublic,
                CreatedAt = DateTime.UtcNow,
                CourseId = newAdvisement.CourseId,
                StudentId = newAdvisement.StudentId
            };

            _context.Advisements.Add(advisement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdvisement", new { id = advisement.Id }, advisement);
        }

        // DELETE: api/Advisement/DeleteAdvisement
        [HttpDelete]
        [Route("[action]/{id}")]
        public async Task<IActionResult> DeleteAdvisement(string id)
        {
            var advisement = await _context.Advisements.FindAsync(id);
            if (advisement == null)
            {
                return NotFound(new { Message = "Advisement not found" });
            }

            _context.Advisements.Remove(advisement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvisementExists(string id)
        {
            return _context.Advisements.Any(a => a.Id == id);
        }
    }
}
