using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Domain.Entities;

public class PlanReview
{
    public string ReviewId { get; set; } = string.Empty;
    public string PermitId { get; set; } = string.Empty;
    public string ReviewerId { get; set; } = string.Empty;
    public ReviewType ReviewType { get; set; }
    public ReviewStatus Status { get; set; }
    public string? Comments { get; set; }
    public List<string> Deficiencies { get; set; } = new();
    public DateTime ReviewDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public Permit Permit { get; set; } = null!;
}
