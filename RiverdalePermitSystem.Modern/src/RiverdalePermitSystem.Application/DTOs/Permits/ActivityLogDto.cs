using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Permits;

public record ActivityLogDto(
    DateTime Timestamp,
    ActivityType ActivityType,
    string? PermitId,
    string Description,
    string UserName);
