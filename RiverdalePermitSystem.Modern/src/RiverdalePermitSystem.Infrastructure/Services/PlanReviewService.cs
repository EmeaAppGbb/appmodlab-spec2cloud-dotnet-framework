using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverdalePermitSystem.Application.DTOs.Reviews;
using RiverdalePermitSystem.Application.Interfaces;
using RiverdalePermitSystem.Domain.Entities;
using RiverdalePermitSystem.Domain.Enums;
using RiverdalePermitSystem.Domain.Services;
using RiverdalePermitSystem.Infrastructure.Data;

namespace RiverdalePermitSystem.Infrastructure.Services;

public class PlanReviewService : IPlanReviewService
{
    private readonly PermitDbContext _db;
    private readonly PermitStatusMachine _statusMachine;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<PlanReviewService> _logger;

    public PlanReviewService(PermitDbContext db, PermitStatusMachine statusMachine,
        ICurrentUserService currentUser, ILogger<PlanReviewService> logger)
    {
        _db = db;
        _statusMachine = statusMachine;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlanReviewHistoryDto>> GetPlanReviewHistoryAsync(string permitId)
    {
        return await _db.PlanReviews
            .Where(r => r.PermitId == permitId)
            .OrderByDescending(r => r.ReviewDate)
            .Select(r => new PlanReviewHistoryDto(
                r.ReviewDate, r.ReviewType, r.ReviewerId, r.Status, r.Comments))
            .ToListAsync();
    }

    public async Task<string> SubmitPlanReviewAsync(PlanReviewSubmissionDto dto)
    {
        var userName = await _currentUser.GetCurrentUserNameAsync();
        var reviewId = $"REV-{DateTime.UtcNow:yyyy}-{DateTime.UtcNow.Ticks % 10000:D4}";

        var review = new PlanReview
        {
            ReviewId = reviewId,
            PermitId = dto.PermitId,
            ReviewerId = userName,
            ReviewType = dto.ReviewType,
            Status = dto.Status,
            Comments = dto.Comments,
            Deficiencies = dto.Deficiencies,
            ReviewDate = DateTime.UtcNow,
            CompletedDate = dto.Status is ReviewStatus.Approved or ReviewStatus.Rejected or ReviewStatus.ApprovedWithConditions
                ? DateTime.UtcNow : null
        };

        _db.PlanReviews.Add(review);

        // Update permit status based on all reviews
        var permit = await _db.Permits.FindAsync(dto.PermitId);
        if (permit != null)
        {
            var allReviews = await _db.PlanReviews.Where(r => r.PermitId == dto.PermitId).ToListAsync();
            allReviews.Add(review);

            if (allReviews.Any(r => r.Status == ReviewStatus.Rejected))
            {
                if (_statusMachine.CanTransition(permit.Status, PermitStatus.ReviewRejectedResubmitRequired))
                    permit.Status = _statusMachine.Transition(permit.Status, PermitStatus.ReviewRejectedResubmitRequired);
            }
            else if (allReviews.All(r => r.Status is ReviewStatus.Approved or ReviewStatus.ApprovedWithConditions) && allReviews.Count > 0)
            {
                if (_statusMachine.CanTransition(permit.Status, PermitStatus.Approved))
                    permit.Status = _statusMachine.Transition(permit.Status, PermitStatus.Approved);
            }

            permit.ModifiedDate = DateTime.UtcNow;
        }

        _db.ActivityLog.Add(new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            ActivityType = ActivityType.PlanReviewSubmitted,
            PermitId = dto.PermitId,
            Description = $"Plan review ({dto.ReviewType}) submitted: {dto.Status}",
            UserName = userName
        });

        await _db.SaveChangesAsync();

        _logger.LogInformation("Plan review submitted: {ReviewId} for permit {PermitId}", reviewId, dto.PermitId);

        return reviewId;
    }
}
