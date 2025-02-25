using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfessorAPI.DTO;
using ProfessorAPI.Models;
using System.Net;

namespace ProfessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly DimajProfessorsDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string STUDENT_APP_URL;


        public AppointmentController(DimajProfessorsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            STUDENT_APP_URL = _configuration["EnvironmentVariables:STUDENT_APP_URL"];
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
        public async Task<IActionResult> PutAppointment([FromBody] UpdateAppointmentDTO updatedAppointment)
        {
            if (updatedAppointment == null || string.IsNullOrEmpty(updatedAppointment.Id))
            {
                return BadRequest("Invalid Data");
            }

            var existingAppointment = await _context.Appointments.FindAsync(updatedAppointment.Id);

            if (existingAppointment == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrEmpty(updatedAppointment.Status))
                existingAppointment.Status = updatedAppointment.Status;

            if (!string.IsNullOrEmpty(updatedAppointment.ProfessorComment))
                existingAppointment.ProfessorComment = updatedAppointment.ProfessorComment;

            try
            {
                await _context.SaveChangesAsync();


                SendUpdateAppointment(updatedAppointment);
                return Ok(new { success = true, appointment = existingAppointment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al actualizar la cita.", details = ex.Message });
            }
        }



        //Comunicaciòn MVC
        private IActionResult SendUpdateAppointment(UpdateAppointmentDTO updateAppointment)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(STUDENT_APP_URL);

                    var responseFormat = new
                    {
                        Id = updateAppointment.Id,
                        Status = updateAppointment.Status,
                        ProfessorComment = updateAppointment.ProfessorComment,
                    };

                    
                    var putTask = client.PutAsJsonAsync("/Appointment/UpdateAppointment", responseFormat);
                    putTask.Wait();

                    var result = putTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return Ok(new { Message = "Updated successfully" });
                    }
                    else
                    {
                        var errorMessage = result.Content.ReadAsStringAsync().Result;
                        return StatusCode((int)result.StatusCode, new { Message = "Failed to update", Error = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Exception occurred", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateAppointmentFromMVC([FromBody] CreateAppointmentDTO appointmentDTO)
        {
            try
            {
                var newAppointment = new Appointment
                {
                    Id = appointmentDTO.Id,
                    Date = appointmentDTO.Date,
                    Mode = appointmentDTO.Mode,
                    Status = appointmentDTO.Status,
                    CourseId = appointmentDTO.CourseId,
                    StudentId = appointmentDTO.StudentId,
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Response added successfully..." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred adding...", Error = ex.Message });
            }
        }
    }
}