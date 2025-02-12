using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DimajProfessorsDbContext _context;

        public UserController(DimajProfessorsDbContext context)
        {
            _context = context;
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

        // PATCH: api/User/PatchUser
        [HttpPatch]
        [Route("[action]/{id}")]
        public async Task<IActionResult> PatchUser(string id, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var userToUpdate = await _context.User.FindAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(userToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
            catch (Exception ex) when (ex is DbUpdateConcurrencyException || ex is DbUpdateException)
            {
                return StatusCode(500, ex is DbUpdateConcurrencyException
                    ? "Conflicto de concurrencia. Otro usuario podría haber modificado este registro."
                    : "Error al actualizar el usuario en la base de datos.");
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

        [HttpGet]
        [Route("[action]/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            var user = await _context.User
                .Where(u => u.Email == email)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Password = u.Password,
                    IsActive = u.IsActive ?? false,
                    RegistrationStatus = u.RegistrationStatus ?? "",
                    Role = u.Role ?? "",
                    Name = u.Name ?? "",
                    Description = u.Description ?? "",
                    LinkedIn = u.LinkedIn ?? "",
                    Picture = string.IsNullOrEmpty(u.Picture) ? "/images/user.png" : u.Picture
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
