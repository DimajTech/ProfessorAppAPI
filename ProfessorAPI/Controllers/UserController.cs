﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;
using ProfessorAPI.Service.StudentsAPP;
using Microsoft.EntityFrameworkCore.Storage;
using ProfessorAPI.Service.AdminApp;

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
        public async Task<ActionResult<User>> PostUser([FromBody]User user)
        {
            
            user.IsActive = true;           

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
           
            try
            {
                await _context.SaveChangesAsync();

                var studentUserService = new StudentUserService(_configuration);

                await studentUserService.UpdateProfessor(originalUser);

                var adminUserService = new AdminUserService(_configuration);

                await adminUserService.UpdateProfessor(originalUser);

                return Ok(originalUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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

        //----------------------- STUDENT-TO-PROFESSOR METHODS -----------------------
        [HttpPut]
        [Route("[action]/{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDTO newValues)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newValues.id) || id != newValues.id)
            {
                return BadRequest();
            }

            var originalUser = await _context.User.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (originalUser == null)
            {
                return NotFound();
            }

            originalUser.Name = newValues.name;
            originalUser.Picture = newValues.picture;
            originalUser.Description = newValues.description;
            originalUser.LinkedIn = newValues.linkedin;
            originalUser.Password = newValues.password;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok(originalUser);
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<User>> InsertStudentUser(User user)
        {
            
            DateTime createdAt = DateTime.UtcNow; //creo que agarra la hora mal
            user.CreatedAt = createdAt;

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        //----------------------- ADMIN-TO-PROFESSOR METHODS -----------------------
        [HttpPatch]
        [Route("[action]/{id}")]
        public async Task<IActionResult> ChangeUserStatus(string id, [FromBody] StatusUpdateDTO status)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id.Equals(id));

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.RegistrationStatus = status.status;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok();
        }
    }
}
