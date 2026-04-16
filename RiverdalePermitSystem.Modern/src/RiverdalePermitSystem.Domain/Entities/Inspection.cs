using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Domain.Entities;

public class Inspection
{
    public string InspectionId { get; set; } = string.Empty;
    public string PermitId { get; set; } = string.Empty;
    public string InspectorId { get; set; } = string.Empty;
    public InspectionType InspectionType { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public InspectionResult? Result { get; set; }
    public InspectionStatus Status { get; set; }
    public string? Comments { get; set; }
    public string? Photos { get; set; }
    public DateTime CreatedDate { get; set; }

    public Permit Permit { get; set; } = null!;
}
