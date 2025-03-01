﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProfessorAPI.Models;

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

    public string? RegistrationStatus { get; set; }

    public string? Role { get; set; }

    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<Advisement> Advisements { get; set; } = new List<Advisement>();
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    [JsonIgnore]
    public virtual ICollection<CommentNews> CommentNews { get; set; } = new List<CommentNews>();
    [JsonIgnore]
    public virtual ICollection<CommentNewsResponse> CommentNewsResponses { get; set; } = new List<CommentNewsResponse>();
    [JsonIgnore]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    [JsonIgnore]
    public virtual ICollection<PieceOfNews> PieceOfNews { get; set; } = new List<PieceOfNews>();
    [JsonIgnore]
    public virtual ICollection<ResponseAdvisement> ResponseAdvisements { get; set; } = new List<ResponseAdvisement>();
}
