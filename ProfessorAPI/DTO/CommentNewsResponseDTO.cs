﻿using Microsoft.AspNetCore.Mvc;

namespace ProfessorAPI.DTO
{
    public class CommentNewsResponseDTO
    {
        public string? Id { get; set; }
        public string CommentNewsId { get; set; }
        public string AuthorId { get; set; }
        public string Text { get; set; }
    }
}
