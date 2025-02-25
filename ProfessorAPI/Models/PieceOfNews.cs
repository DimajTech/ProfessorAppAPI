using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class PieceOfNews
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? File { get; set; }

    public string? AuthorId { get; set; }

    public string? Description { get; set; }

    public DateOnly? Date { get; set; }

    public byte[]? Picture { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<CommentNews> CommentNews { get; set; } = new List<CommentNews>();
}
