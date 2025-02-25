namespace ProfessorAPI.DTO
{
    public class InsertUserDTO
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IsActive { get; set; }
        public string? CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        
    }
}
