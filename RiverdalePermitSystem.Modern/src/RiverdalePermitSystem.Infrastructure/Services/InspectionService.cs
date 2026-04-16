using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverdalePermitSystem.Application.DTOs.Inspections;
using RiverdalePermitSystem.Application.Interfaces;
using RiverdalePermitSystem.Domain.Entities;
using RiverdalePermitSystem.Domain.Enums;
using RiverdalePermitSystem.Domain.Services;
using RiverdalePermitSystem.Infrastructure.Data;

namespace RiverdalePermitSystem.Infrastructure.Services;

public class InspectionService : IInspectionService
{
    private readonly PermitDbContext _db;
    private readonly PermitStatusMachine _statusMachine;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<InspectionService> _logger;

    public InspectionService(PermitDbContext db, PermitStatusMachine statusMachine,
        ICurrentUserService currentUser, ILogger<InspectionService> logger)
    {
        _db = db;
        _statusMachine = statusMachine;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IReadOnlyList<InspectionSummaryDto>> GetUpcomingInspectionsAsync()
    {
        return await _db.Inspections
            .Where(i => i.Status == InspectionStatus.Scheduled && i.ScheduledDate >= DateTime.UtcNow.Date)
            .OrderBy(i => i.ScheduledDate)
            .Select(i => new InspectionSummaryDto(
                i.InspectionId, i.PermitId, i.InspectionType,
                i.ScheduledDate, i.Status, i.InspectorId, i.Result, i.Comments))
            .ToListAsync();
    }

    public async Task<string> ScheduleInspectionAsync(InspectionScheduleDto dto)
    {
        // No weekend scheduling
        if (dto.RequestedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException("Inspections cannot be scheduled on weekends.");

        var permit = await _db.Permits.FindAsync(dto.PermitId)
            ?? throw new Application.Exceptions.PermitNotFoundException(dto.PermitId);

        var userName = await _currentUser.GetCurrentUserNameAsync();
        var inspectionId = $"INSP-{DateTime.UtcNow:yyyy}-{DateTime.UtcNow.Ticks % 10000:D4}";

        // Auto-assign inspector (least busy)
        var inspectorId = await _db.Inspections
            .Where(i => i.Status == InspectionStatus.Scheduled)
            .GroupBy(i => i.InspectorId)
            .OrderBy(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync() ?? userName;

        var inspection = new Inspection
        {
            InspectionId = inspectionId,
            PermitId = dto.PermitId,
            InspectorId = inspectorId,
            InspectionType = dto.InspectionType,
            ScheduledDate = dto.RequestedDate,
            Status = InspectionStatus.Scheduled,
            Comments = dto.Notes,
            CreatedDate = DateTime.UtcNow
        };

        _db.Inspections.Add(inspection);

        if (_statusMachine.CanTransition(permit.Status, PermitStatus.UnderInspection))
            permit.Status = _statusMachine.Transition(permit.Status, PermitStatus.UnderInspection);

        permit.ModifiedDate = DateTime.UtcNow;

        _db.ActivityLog.Add(new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            ActivityType = ActivityType.InspectionScheduled,
            PermitId = dto.PermitId,
            Description = $"Inspection ({dto.InspectionType}) scheduled for {dto.RequestedDate:MM/dd/yyyy}",
            UserName = userName
        });

        await _db.SaveChangesAsync();

        _logger.LogInformation("Inspection scheduled: {InspectionId} for permit {PermitId}", inspectionId, dto.PermitId);

        return inspectionId;
    }

    public async Task CompleteInspectionAsync(string inspectionId, InspectionResult result, string? comments)
    {
        var inspection = await _db.Inspections.FindAsync(inspectionId)
            ?? throw new InvalidOperationException($"Inspection '{inspectionId}' not found.");

        var permit = await _db.Permits.FindAsync(inspection.PermitId)
            ?? throw new Application.Exceptions.PermitNotFoundException(inspection.PermitId);

        var userName = await _currentUser.GetCurrentUserNameAsync();

        inspection.Result = result;
        inspection.Status = InspectionStatus.Completed;
        inspection.CompletedDate = DateTime.UtcNow;
        inspection.Comments = comments;

        // Update permit status based on result
        if (result == InspectionResult.Passed)
        {
            if (inspection.InspectionType == InspectionType.Final)
            {
                if (_statusMachine.CanTransition(permit.Status, PermitStatus.CertificateOfOccupancyIssued))
                    permit.Status = _statusMachine.Transition(permit.Status, PermitStatus.CertificateOfOccupancyIssued);
            }
            else
            {
                if (_statusMachine.CanTransition(permit.Status, PermitStatus.Issued))
                    permit.Status = _statusMachine.Transition(permit.Status, PermitStatus.Issued);
            }
        }
        else
        {
            if (_statusMachine.CanTransition(permit.Status, PermitStatus.InspectionFailedCorrectionsRequired))
                permit.Status = _statusMachine.Transition(permit.Status, PermitStatus.InspectionFailedCorrectionsRequired);
        }

        permit.ModifiedDate = DateTime.UtcNow;

        _db.ActivityLog.Add(new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            ActivityType = ActivityType.InspectionCompleted,
            PermitId = inspection.PermitId,
            Description = $"Inspection ({inspection.InspectionType}) completed: {result}",
            UserName = userName
        });

        await _db.SaveChangesAsync();

        _logger.LogInformation("Inspection completed: {InspectionId} result {Result}", inspectionId, result);
    }

    public async Task CancelInspectionAsync(string inspectionId)
    {
        var inspection = await _db.Inspections.FindAsync(inspectionId)
            ?? throw new InvalidOperationException($"Inspection '{inspectionId}' not found.");

        var userName = await _currentUser.GetCurrentUserNameAsync();

        inspection.Status = InspectionStatus.Cancelled;

        _db.ActivityLog.Add(new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            ActivityType = ActivityType.InspectionCancelled,
            PermitId = inspection.PermitId,
            Description = $"Inspection ({inspection.InspectionType}) cancelled",
            UserName = userName
        });

        await _db.SaveChangesAsync();

        _logger.LogInformation("Inspection cancelled: {InspectionId}", inspectionId);
    }

    public async Task<IReadOnlyList<InspectionSummaryDto>> GetInspectionHistoryAsync(string permitId)
    {
        return await _db.Inspections
            .Where(i => i.PermitId == permitId)
            .OrderByDescending(i => i.ScheduledDate)
            .Select(i => new InspectionSummaryDto(
                i.InspectionId, i.PermitId, i.InspectionType,
                i.ScheduledDate, i.Status, i.InspectorId, i.Result, i.Comments))
            .ToListAsync();
    }
}
