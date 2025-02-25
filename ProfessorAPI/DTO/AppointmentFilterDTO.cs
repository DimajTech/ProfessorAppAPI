namespace ProfessorAPI.DTO
{
    public class AppointmentFilterDTO
    {
        public DateOnly? Date { get; set; }
        public string ProfessorId { get; set; }
        public string State { get; set; }
    }
}