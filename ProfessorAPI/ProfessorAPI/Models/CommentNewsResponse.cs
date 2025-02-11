using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class CommentNewsResponse
{
    public string Id { get; set; } = null!;

    public string? CommentNewsId { get; set; }

    public string? AuthorId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual User? Author { get; set; }

    public virtual CommentNews? CommentNews { get; set; }
}
