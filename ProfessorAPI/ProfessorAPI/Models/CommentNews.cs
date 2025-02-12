using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class CommentNews
{
    public string Id { get; set; } = null!;

    public string? PieceOfNewsId { get; set; }

    public string? AuthorId { get; set; }

    public string? Text { get; set; }

    public DateTime? Date { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<CommentNewsResponse> CommentNewsResponses { get; set; } = new List<CommentNewsResponse>();

    public virtual PieceOfNews? PieceOfNews { get; set; }
}
