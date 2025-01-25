using System;
using System.Collections.Generic;

namespace StudentAppAPI.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Picture { get; set; }

    public string? Description { get; set; }

    public string? LinkedIn { get; set; }

    public string? ProfessionalBackground { get; set; }

    public string? Password { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? RegistrationStatus { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Advisement> Advisements { get; set; } = new List<Advisement>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<CommentNews> CommentNews { get; set; } = new List<CommentNews>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<PieceOfNews> PieceOfNews { get; set; } = new List<PieceOfNews>();

    public virtual ICollection<ResponseAdvisement> ResponseAdvisements { get; set; } = new List<ResponseAdvisement>();
}
