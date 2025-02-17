using ProfessorAPI.Models;
using System.Data;

namespace ProfessorAPI.DTO
{
    public class CreatePieceOfNewsDTO
    {
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string UserId { get; set; }
        public string UserRole { get; set; }

    }
}
