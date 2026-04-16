using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverdalePermitSystem.Application.DTOs.Permits;
using RiverdalePermitSystem.Application.Exceptions;
using RiverdalePermitSystem.Application.Interfaces;
using RiverdalePermitSystem.Domain.Entities;
using RiverdalePermitSystem.Domain.Enums;
using RiverdalePermitSystem.Domain.Services;
using RiverdalePermitSystem.Infrastructure.Data;

namespace RiverdalePermitSystem.Infrastructure.Services;

public class PermitService : IPermitService
{
    private readonly PermitDbContext _db;
    private readonly PermitFeeCalculator _feeCalculator;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<PermitService> _logger;

    public PermitService(PermitDbContext db, PermitFeeCalculator feeCalculator,
        ICurrentUserService currentUser, ILogger<PermitService> logger)
    {
        _db = db;
        _feeCalculator = feeCalculator;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PermitSummaryDto>> GetRecentPermitsAsync(int count)
    {
        return await _db.Permits
            .OrderByDescending(p => p.ApplicationDate)
            .Take(count)
            .Select(p => new PermitSummaryDto(
                p.PermitId, p.ApplicationDate, p.PropertyAddress,
                p.PermitType, p.Status, p.EstimatedCost))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PermitSummaryDto>> SearchPermitsAsync(PermitSearchCriteria criteria)
    {
        var query = _db.Permits.AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.PermitId))
            query = query.Where(p => p.PermitId.Contains(criteria.PermitId));
        if (!string.IsNullOrWhiteSpace(criteria.Address))
            query = query.Where(p => p.PropertyAddress.Contains(criteria.Address));
        if (criteria.PermitType.HasValue)
            query = query.Where(p => p.PermitType == criteria.PermitType.Value);
        if (criteria.Status.HasValue)
            query = query.Where(p => p.Status == criteria.Status.Value);

        return await query
            .OrderByDescending(p => p.ApplicationDate)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Select(p => new PermitSummaryDto(
                p.PermitId, p.ApplicationDate, p.PropertyAddress,
                p.PermitType, p.Status, p.EstimatedCost))
            .ToListAsync();
    }

    public async Task<PermitDetailDto?> GetPermitByIdAsync(string permitId)
    {
        return await _db.Permits
            .Include(p => p.Applicant)
            .Where(p => p.PermitId == permitId)
            .Select(p => new PermitDetailDto(
                p.PermitId, p.ApplicationDate, p.PropertyAddress, p.ParcelNumber,
                p.PermitType, p.Status, p.EstimatedCost, p.SquareFootage,
                p.ZoningDistrict, p.ProjectDescription,
                p.Applicant != null ? p.Applicant.Name : null,
                p.Applicant != null ? p.Applicant.Email : null,
                p.IssuedDate, p.ExpirationDate))
            .FirstOrDefaultAsync();
    }

    public async Task<string> SubmitPermitApplicationAsync(PermitApplicationDto dto)
    {
        var userName = await _currentUser.GetCurrentUserNameAsync();

        // Upsert applicant by email
        var applicant = await _db.Applicants.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (applicant == null)
        {
            applicant = new Applicant
            {
                Name = dto.ApplicantName,
                Email = dto.Email,
                Phone = dto.Phone,
                Company = dto.Company,
                LicenseNumber = dto.LicenseNumber,
                CreatedDate = DateTime.UtcNow
            };
            _db.Applicants.Add(applicant);
        }
        else
        {
            applicant.Name = dto.ApplicantName;
            applicant.Phone = dto.Phone;
            applicant.Company = dto.Company;
            applicant.LicenseNumber = dto.LicenseNumber;
        }

        var permit = Permit.Create(
            dto.PermitType, dto.PropertyAddress, dto.ParcelNumber,
            dto.ProjectDescription, dto.EstimatedCost, dto.SquareFootage,
            dto.ZoningDistrict, userName);

        permit.Applicant = applicant;

        // Calculate and record fee
        var feeAmount = _feeCalculator.Calculate(dto.PermitType, dto.EstimatedCost, dto.SquareFootage, dto.ZoningDistrict);
        permit.Fees.Add(new Fee
        {
            PermitId = permit.PermitId,
            FeeType = "Application Fee",
            Amount = feeAmount,
            CreatedDate = DateTime.UtcNow
        });

        _db.Permits.Add(permit);

        // Activity log
        _db.ActivityLog.Add(new ActivityLogEntry
        {
            Timestamp = DateTime.UtcNow,
            ActivityType = ActivityType.ApplicationSubmitted,
            PermitId = permit.PermitId,
            Description = $"Permit application submitted for {dto.PropertyAddress}",
            UserName = userName
        });

        await _db.SaveChangesAsync();

        _logger.LogInformation("Permit submitted: {PermitId} by {ApplicantEmail} for {PermitType}",
            permit.PermitId, dto.Email, dto.PermitType);

        return permit.PermitId;
    }

    public Task<decimal> CalculatePermitFeeAsync(PermitType type, decimal estimatedCost, int? squareFootage, ZoningDistrict? zoning)
    {
        return Task.FromResult(_feeCalculator.Calculate(type, estimatedCost, squareFootage, zoning));
    }

    public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
    {
        var totalPermits = await _db.Permits.CountAsync();
        var pendingReview = await _db.Permits.CountAsync(p => p.Status == PermitStatus.UnderReview);
        var inspectionsToday = await _db.Inspections.CountAsync(i => i.ScheduledDate.Date == DateTime.UtcNow.Date);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var monthlyRevenue = await _db.Fees
            .Where(f => f.PaidDate >= monthStart)
            .SumAsync(f => (decimal?)f.Amount) ?? 0m;

        return new DashboardStatisticsDto(totalPermits, pendingReview, inspectionsToday, monthlyRevenue);
    }

    public async Task<IReadOnlyList<ActivityLogDto>> GetRecentActivityAsync(int count)
    {
        return await _db.ActivityLog
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .Select(a => new ActivityLogDto(a.Timestamp, a.ActivityType, a.PermitId, a.Description, a.UserName))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StatusSummaryDto>> GetPermitsByStatusAsync()
    {
        return await _db.Permits
            .GroupBy(p => p.Status)
            .Select(g => new StatusSummaryDto(g.Key, g.Count(), g.Sum(p => p.EstimatedCost)))
            .ToListAsync();
    }
}
