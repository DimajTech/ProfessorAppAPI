using Microsoft.AspNetCore.Http.HttpResults;
using ProfessorAPI.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ProfessorAPI.DTO
{
    public class UpdateAppointmentDTO
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string ProfessorComment { get; set; }

    }

}
