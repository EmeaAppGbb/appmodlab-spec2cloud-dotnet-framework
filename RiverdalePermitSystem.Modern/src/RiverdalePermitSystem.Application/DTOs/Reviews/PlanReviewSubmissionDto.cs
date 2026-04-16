using System.ComponentModel.DataAnnotations;
using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Reviews;

public class PlanReviewSubmissionDto
{
    [Required(ErrorMessage = "Permit ID is required")]
    public string PermitId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Review type is required")]
    public ReviewType ReviewType { get; set; }

    [Required(ErrorMessage = "Review status is required")]
    public ReviewStatus Status { get; set; }

    [StringLength(4000)]
    public string? Comments { get; set; }

    public List<string> Deficiencies { get; set; } = new();
}
