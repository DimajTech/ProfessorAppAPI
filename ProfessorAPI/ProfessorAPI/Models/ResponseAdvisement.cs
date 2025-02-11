using System;
using System.Collections.Generic;

namespace ProfessorAPI.Models;

public partial class ResponseAdvisement
{
    public string Id { get; set; } = null!;

    public string AdvisementId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Text { get; set; }

    public DateTime? Date { get; set; }
}
