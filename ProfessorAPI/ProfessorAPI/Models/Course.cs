using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class Course
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ProfessorId { get; set; }

    public string? Semester { get; set; }

    public int? Year { get; set; }

    public bool? IsActive { get; set; }
}
