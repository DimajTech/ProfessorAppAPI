using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly DimajProfessorsDbContext _context;

        public AppointmentController(DimajProfessorsDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetAppointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments([FromBody] AppointmentFilterDTO data)
        {
            var query = _context.Appointments
                .Include(a => a.Course)
                .Include(a => a.User) // Información del estudiante
                .Where(a => a.Course.ProfessorId == data.ProfessorId)
                .Where(a => a.Status == data.State)
                .AsQueryable();

            if (data.Date.HasValue)
            {
                query = query.Where(a => a.Date.HasValue && a.Date.Value.Date == data.Date.Value.ToDateTime(TimeOnly.MinValue).Date);
            }

            var appointments = await query
                .Select(a => new Appointment
                {
                    Id = a.Id,
                    Date = a.Date,
                    Mode = a.Mode,
                    Status = a.Status,
                    ProfessorComment = a.ProfessorComment,
                    Course = a.Course != null ? new Course
                    {
                        Id = a.Course.Id,
                        Code = a.Course.Code,
                        Name = a.Course.Name,
                        Year = 0,
                        IsActive = true
                    } : null,
                    User = a.User != null ? new User
                    {
                        Id = a.User.Id,
                        Name = a.User.Name,
                        Email = a.User.Email,

                        IsActive = true
                    } : null
                })
                .ToListAsync();

            if (!appointments.Any())
            {
                return NotFound("No Appointments found.");
            }

            return Ok(appointments);
        }

        [HttpGet]
        [Route("GetReviewedAppointments/{professorId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetReviewedAppointments(string professorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Course)
                .Include(a => a.User)
                .Where(a => a.Course.ProfessorId == professorId)
                .Where(a => a.Status == "accepted" || a.Status == "denied")
                .Select(a => new Appointment
                {
                    Id = a.Id,
                    Date = a.Date,
                    Mode = a.Mode,
                    Status = a.Status,
                    ProfessorComment = a.ProfessorComment,
                    Course = a.Course != null ? new Course
                    {
                        Id = a.Course.Id,
                        Code = a.Course.Code,
                        Name = a.Course.Name,
                        Year = 0,
                        IsActive = true
                    } : null,
                    User = a.User != null ? new User
                    {
                        Id = a.User.Id,
                        Name = a.User.Name,
                        Email = a.User.Email,
                        IsActive = true
                    } : null
                })
                .ToListAsync();

            if (!appointments.Any())
            {
                return NotFound("No reviewed appointments found.");
            }

            return Ok(appointments);
        }

        [HttpGet]
        [Route("GetAppointmentById/{appointmentId}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(string appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Course)
                .Include(a => a.User)
                .Where(a => a.Id == appointmentId)
                .Select(a => new Appointment
                {
                    Id = a.Id,
                    Date = a.Date,
                    Mode = a.Mode,
                    Status = a.Status,
                    ProfessorComment = a.ProfessorComment,
                    Course = a.Course != null ? new Course
                    {
                        Id = a.Course.Id,
                        Code = a.Course.Code,
                        Name = a.Course.Name,
                        Year = 0,
                        IsActive = true
                    } : null,
                    User = a.User != null ? new User
                    {
                        Id = a.User.Id,
                        Name = a.User.Name,
                        Email = a.User.Email,
                        IsActive = true
                    } : null
                })
                .FirstOrDefaultAsync();

            return Ok(appointment);
        }

        [HttpPut]
        [Route("PutAppointment")]
        public async Task<IActionResult> PutAppointment([FromBody] Appointment updatedAppointment)
        {
            if (updatedAppointment == null || string.IsNullOrEmpty(updatedAppointment.Id))
            {
                return BadRequest("Datos inválidos o ID faltante");
            }

            var existingAppointment = await _context.Appointments.FindAsync(updatedAppointment.Id);

            if (existingAppointment == null)
            {
                return NotFound("cita no encontrada");
            }

            //actualizar los campos que vienen de request
            if (updatedAppointment.Date != default)
                existingAppointment.Date = updatedAppointment.Date;

            if (!string.IsNullOrEmpty(updatedAppointment.Mode))
                existingAppointment.Mode = updatedAppointment.Mode;

            if (!string.IsNullOrEmpty(updatedAppointment.Status))
                existingAppointment.Status = updatedAppointment.Status;

            if (!string.IsNullOrEmpty(updatedAppointment.ProfessorComment))
                existingAppointment.ProfessorComment = updatedAppointment.ProfessorComment;

            if (updatedAppointment.Course != null)
                existingAppointment.Course = updatedAppointment.Course;

            if (updatedAppointment.User != null)
                existingAppointment.User = updatedAppointment.User;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, appointment = existingAppointment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al actualizar la cita.", details = ex.Message });
            }
        }

    }
}