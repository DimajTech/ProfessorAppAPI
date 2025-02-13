﻿using Microsoft.AspNetCore.Mvc;
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

    }
}
