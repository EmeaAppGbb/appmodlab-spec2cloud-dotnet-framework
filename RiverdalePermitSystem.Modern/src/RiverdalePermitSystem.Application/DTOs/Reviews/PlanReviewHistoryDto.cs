using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Reviews;

public record PlanReviewHistoryDto(
    DateTime ReviewDate,
    ReviewType ReviewType,
    string ReviewerName,
    ReviewStatus Status,
    string? Comments);
