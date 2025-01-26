using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StudentAppAPI.Models;

public partial class Course
{
    public string Id { get; set; } = null!;

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ProfessorId { get; set; }

    public string? Semester { get; set; }

    public int? Year { get; set; }

    public bool? IsActive { get; set; }

    [JsonIgnore]
    public virtual ICollection<Advisement> Advisements { get; set; } = new List<Advisement>();

    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User? Professor { get; set; }
}
