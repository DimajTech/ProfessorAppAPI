using Microsoft.AspNetCore.Mvc;

namespace ProfessorAPI.DTO
{
    public class CommentNewsDTO
    {
        public string PieceOfNewsId { get; set; }
        public string AuthorId { get; set; }
        public string Text { get; set; }
    }
}
