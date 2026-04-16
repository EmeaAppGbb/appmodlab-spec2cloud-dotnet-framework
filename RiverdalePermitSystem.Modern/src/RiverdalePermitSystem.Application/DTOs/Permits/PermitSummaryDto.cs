using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Permits;

public record PermitSummaryDto(
    string PermitId,
    DateTime ApplicationDate,
    string PropertyAddress,
    PermitType PermitType,
    PermitStatus Status,
    decimal EstimatedCost);
