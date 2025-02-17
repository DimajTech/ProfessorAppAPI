using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;
using ProfessorAPI.Service.StudentsAPP;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DimajProfessorsDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(DimajProfessorsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        [HttpGet]
        [Route("[action]/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (user == null)
            {
                return NotFound();
            }

            user.Picture = string.IsNullOrEmpty(user.Picture) ? "/images/user.png" : user.Picture;

            return user;
        }


        // GET: api/User/GetUsers
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.Select(userItem => new User()
            {
                Id = userItem.Id,
                Name = userItem.Name,
                Email = userItem.Email,
                Picture = userItem.Picture,
                Description = userItem.Description,
                LinkedIn = userItem.LinkedIn,
                ProfessionalBackground = userItem.ProfessionalBackground,
                IsActive = userItem.IsActive,
                CreatedAt = userItem.CreatedAt,
                RegistrationStatus = userItem.RegistrationStatus,
                Role = userItem.Role
            }).ToListAsync();
        }

        // GET: api/User/GetUser/5
        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/User/PostUser/
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var userId = Guid.NewGuid().ToString();
            user.Id = userId;

            DateTime createdAt = DateTime.UtcNow; //creo que agarra la hora mal
            user.CreatedAt = createdAt;

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // PUT: api/User/PutUser/5
        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] User newValues)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newValues.Id) || id != newValues.Id)
            {
                return BadRequest();
            }

            var originalUser = await _context.User.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (originalUser == null)
            {
                return NotFound();
            }

            originalUser.Name = newValues.Name;
            originalUser.Picture = newValues.Picture;
            originalUser.Description = newValues.Description;
            originalUser.LinkedIn = newValues.LinkedIn;
            originalUser.ProfessionalBackground = newValues.ProfessionalBackground;
            originalUser.Password = newValues.Password;

            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.SaveChangesAsync();

                    /*
                    var professor = new UpdateProfessorRequestDTO
                    {
                        id = originalUser.Id,
                        name = originalUser.Name,
                        picture = originalUser.Picture,
                        description = originalUser.Description,
                        linkedin = originalUser.LinkedIn,
                        professionalBackground = originalUser.ProfessionalBackground,
                        password = originalUser.Password
                    };
                    */

                    var studentUserService = new StudentUserService(_configuration);

                    await studentUserService.UpdateProfessor(originalUser);

                    await transaction.CommitAsync();

                    return Ok(originalUser);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }
            }
        }

        // DELETE: api/User/DeleteUser/5
        [HttpDelete]
        [Route("[action]/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                if (user.IsActive == false)
                {
                    return Ok("El usuario ya está inactivo.");
                }

                user.IsActive = false;
                _context.Entry(user).Property(u => u.IsActive).IsModified = true;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al actualizar el usuario en la base de datos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error inesperado: {ex.Message}");
            }
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id.Equals(id));
        }
    }
}
