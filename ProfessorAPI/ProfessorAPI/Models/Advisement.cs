using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class Advisement
{
    public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? Content { get; set; }

    public string? Status { get; set; }

    public bool? IsPublic { get; set; }

    public string? StudentId { get; set; }

    public DateTime? CreatedAt { get; set; }
}
