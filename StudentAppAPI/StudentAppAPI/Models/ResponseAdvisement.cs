using System;
using System.Collections.Generic;

namespace StudentAppAPI.Models;

public partial class ResponseAdvisement
{
    public string Id { get; set; } = null!;

    public string? AdvisementId { get; set; }

    public string? UserId { get; set; }

    public string? Text { get; set; }

    public DateTime? Date { get; set; }

    public virtual Advisement? Advisement { get; set; }

    public virtual User? User { get; set; }
}
