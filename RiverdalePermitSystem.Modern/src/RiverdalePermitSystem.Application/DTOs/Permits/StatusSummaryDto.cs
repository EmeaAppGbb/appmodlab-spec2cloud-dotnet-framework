using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Permits;

public record StatusSummaryDto(
    PermitStatus Status,
    int Count,
    decimal TotalValue);
