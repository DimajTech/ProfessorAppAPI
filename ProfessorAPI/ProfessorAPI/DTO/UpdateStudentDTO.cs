namespace ProfessorAPI.DTO
{
    public class UpdateStudentDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string description { get; set; }
        public string linkedin { get; set; }
        public string picture { get; set; }
        public string? professionalBackground { get; set; }
    }
}
