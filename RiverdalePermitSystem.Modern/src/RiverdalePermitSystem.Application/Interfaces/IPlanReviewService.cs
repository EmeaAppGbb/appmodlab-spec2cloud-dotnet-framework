using RiverdalePermitSystem.Application.DTOs.Reviews;

namespace RiverdalePermitSystem.Application.Interfaces;

public interface IPlanReviewService
{
    Task<IReadOnlyList<PlanReviewHistoryDto>> GetPlanReviewHistoryAsync(string permitId);
    Task<string> SubmitPlanReviewAsync(PlanReviewSubmissionDto dto);
}
