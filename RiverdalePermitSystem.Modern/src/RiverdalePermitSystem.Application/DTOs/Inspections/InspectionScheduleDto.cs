using System.ComponentModel.DataAnnotations;
using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Inspections;

public class InspectionScheduleDto
{
    [Required(ErrorMessage = "Permit ID is required")]
    public string PermitId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Inspection type is required")]
    public InspectionType InspectionType { get; set; }

    [Required(ErrorMessage = "Requested date is required")]
    public DateTime RequestedDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
