namespace ProfessorAPI.DTO
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; }
        public string RegistrationStatus { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LinkedIn { get; set; }
        public string Picture { get; set; }
    }
}
