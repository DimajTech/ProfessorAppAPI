using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StudentAppAPI.Models;

public partial class Advisement
{
    public string? Id { get; set; }

    public string? CourseId { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public bool? IsPublic { get; set; }

    public string? StudentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    [JsonIgnore]
    public virtual ICollection<ResponseAdvisement> ResponseAdvisements { get; set; } = new List<ResponseAdvisement>();

    public virtual User? Student { get; set; }
}
