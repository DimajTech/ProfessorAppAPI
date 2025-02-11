using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return await _context.Users.Select(userItem => new User()
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

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

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // PUT: api/User/PutUser
        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var userToUpdate = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Picture = user.Picture,
                Description = user.Description,
                LinkedIn = user.LinkedIn,
                ProfessionalBackground = user.ProfessionalBackground,
                Password = user.Password,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                RegistrationStatus = user.RegistrationStatus,
                Role = user.Role
            };

            _context.Entry(userToUpdate).State = EntityState.Modified;

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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.IsActive == false)
            {
                return BadRequest("El usuario ya está inactivo.");
            }

            user.IsActive = false;

            _context.Entry(user).Property(u => u.IsActive).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id.Equals(id));
        }
    }
}
