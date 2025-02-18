using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisementController : ControllerBase
    {
        private readonly DimajProfessorsDbContext _context;
        private readonly string STUDENT_APP_URL;
        private readonly IConfiguration _configuration;

        public AdvisementController(DimajProfessorsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            STUDENT_APP_URL = _configuration["EnvironmentVariables:STUDENT_APP_URL"];

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
                .Include(a => a.User)
                .Select(advisement => new Advisement()
                {
                    Id = advisement.Id,
                    Content = advisement.Content,
                    Status = advisement.Status,
                    IsPublic = advisement.IsPublic,
                    CreatedAt = advisement.CreatedAt,
                    Course = advisement.Course,
                    User = advisement.User
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
                .Include(a => a.User)
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
        public async Task<ActionResult<Advisement>> AddAdvisement(Advisement newAdvisement)
        {
            var advisement = new Advisement
            {
                Id = newAdvisement.Id,
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

        // GET: api/Advisement/GetPublicAdvisements/{ProfessorEmail}
        [HttpGet]
        [Route("[action]/{ProfessorEmail}")]
        public async Task<ActionResult<IEnumerable<Advisement>>> GetPublicAdvisements(string ProfessorEmail)
        {

            var professor = await _context.User
                .Where(u => u.Email == ProfessorEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (professor == null)
            {
                return NotFound($"No se encontró un profesor con el correo {ProfessorEmail}.");
            }

            var advisements = await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.User)
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


                User = a.User != null ? new User
                {
                    Id = a.User.Id,
                    Name = a.User.Name,
                    Email = a.  User.Email
                } : null

            }).ToList();

            return Ok(filteredAdvisements);
        }

        // GET: api/Advisement/GetMyAdvisements/{ProfessorEmail}
        [HttpGet]
        [Route("[action]/{ProfessorEmail}")] // Parámetro en la URL
        public async Task<ActionResult<IEnumerable<Advisement>>> GetMyAdvisements(string ProfessorEmail)
        {
            var professor = await _context.User
                .Where(u => u.Email == ProfessorEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (professor == null)
            {
                return NotFound($"No se encontró un profesor con el correo {ProfessorEmail}.");
            }

            var advisements = await _context.Advisements
                .Include(a => a.Course)
                .Include(a => a.User)
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


                User = a.User != null ? new User
                {
                    Id = a.User.Id,
                    Name = a.User.Name
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
                .Include(a => a.User)
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
                    User = a.User       
                })
                .FirstOrDefaultAsync();

            if (advisement == null)
            {
                return NotFound($"No se encontró la consulta con ID {AdvisementId}.");
            }

            return Ok(advisement);
        }

        //TODO:


        [HttpGet]
        [Route("GetResponsesByAdvisement/{advisementId}")]
        public async Task<ActionResult<IEnumerable<ResponseAdvisement>>> GetResponsesByAdvisement(string advisementId)
        {
            var responses = await _context.ResponseAdvisements
             .Include(r => r.User)
             .Where(r => r.AdvisementId == advisementId)
             .OrderByDescending(r => r.Date) // Ordena de más reciente a más antigua
             .Select(r => new ResponseAdvisement
             {
                 Id = r.Id,
                 Text = r.Text,
                 Date = r.Date,
                 User = new User
                 {
                     Id = r.User.Id,
                     Name = r.User.Name,
                     Role = r.User.Role,
                     IsActive = true
                 }
             })
             .ToListAsync();

            return Ok(responses);
        }

        [HttpPost("CreateResponseAdvisement")]
        public async Task<IActionResult> CreateResponseAdvisement([FromBody] CreateResponseAdvisementDTO responseDto)
        {
            try
            {
                if (responseDto == null || string.IsNullOrWhiteSpace(responseDto.AdvisementId) ||
                    string.IsNullOrWhiteSpace(responseDto.UserId) || string.IsNullOrWhiteSpace(responseDto.Text))
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                var response = new ResponseAdvisement
                {
                    Id = responseDto.Id is null ? Guid.NewGuid().ToString() : responseDto.Id,
                    AdvisementId = responseDto.AdvisementId,
                    UserId = responseDto.UserId,
                    Text = responseDto.Text,
                    Date = DateTime.Now
                };
                responseDto.Id = response.Id;

                SendAdvisementResponse(responseDto);
                // Agregar a la base de datos con Entity Framework
                _context.ResponseAdvisements.Add(response);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Response added successfully", response });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred", error = e.Message });
            }
        }

        [HttpPost("CreateResponseAdvisementFromStudent")]
        public async Task<IActionResult> CreateResponseAdvisementFromStudent([FromBody] CreateResponseAdvisementDTO responseDto)
        {
            try
            {
                if (responseDto == null || string.IsNullOrWhiteSpace(responseDto.AdvisementId) ||
                    string.IsNullOrWhiteSpace(responseDto.UserId) || string.IsNullOrWhiteSpace(responseDto.Text))
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                var response = new ResponseAdvisement
                {
                    Id = responseDto.Id is null ? Guid.NewGuid().ToString() : responseDto.Id,
                    AdvisementId = responseDto.AdvisementId,
                    UserId = responseDto.UserId,
                    Text = responseDto.Text,
                    Date = DateTime.Now
                };
                responseDto.Id = response.Id;

                // Agregar a la base de datos con Entity Framework
                _context.ResponseAdvisements.Add(response);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Response added successfully", response });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred", error = e.Message });
            }
        }



        private IActionResult SendAdvisementResponse(CreateResponseAdvisementDTO response)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(STUDENT_APP_URL);

                    var responseFormat = new
                    {
                        Id = response.Id,
                        AdvisementId = response.AdvisementId, // Cambiado de PieceOfNewsId a AdvisementId
                        UserId = response.UserId, // Cambiado de AuthorId a UserId
                        Text = response.Text
                    };


                    var postTask = client.PostAsJsonAsync("Advisement/AddNewResponseFromAPI", responseFormat);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return Ok(new { Message = "Response added successfully" });
                    }
                    else
                    {
                        var errorMessage = result.Content.ReadAsStringAsync().Result;
                        return StatusCode((int)result.StatusCode, new { Message = "Failed to add comment", Error = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Exception occurred", Error = ex.Message });
            }
        }


    }
}
