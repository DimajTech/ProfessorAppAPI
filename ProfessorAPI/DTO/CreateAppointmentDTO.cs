namespace ProfessorAPI.DTO
{
    public class CreateAppointmentDTO
    {
        public string? Id { get; set; }
        public DateTime Date { get; set; }
        public string Mode { get; set; }
        public string? Status { get; set; }
        public string CourseId { get; set; }
        public string StudentId { get; set; }
    }
}
