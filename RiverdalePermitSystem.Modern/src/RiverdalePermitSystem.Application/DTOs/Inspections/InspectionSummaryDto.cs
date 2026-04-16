using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Inspections;

public record InspectionSummaryDto(
    string InspectionId,
    string PermitId,
    InspectionType InspectionType,
    DateTime ScheduledDate,
    InspectionStatus Status,
    string InspectorName,
    InspectionResult? Result,
    string? Comments);
