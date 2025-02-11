using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class Appointment
{
    public string Id { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string? Mode { get; set; }

    public string? Status { get; set; }

    public string? CourseId { get; set; }

    public string? StudentId { get; set; }

    public string? ProfessorComment { get; set; }
}
