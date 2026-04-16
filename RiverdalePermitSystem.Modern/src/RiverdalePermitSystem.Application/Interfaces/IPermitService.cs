using RiverdalePermitSystem.Application.DTOs.Permits;
using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.Interfaces;

public interface IPermitService
{
    Task<IReadOnlyList<PermitSummaryDto>> GetRecentPermitsAsync(int count);
    Task<IReadOnlyList<PermitSummaryDto>> SearchPermitsAsync(PermitSearchCriteria criteria);
    Task<PermitDetailDto?> GetPermitByIdAsync(string permitId);
    Task<string> SubmitPermitApplicationAsync(PermitApplicationDto dto);
    Task<decimal> CalculatePermitFeeAsync(PermitType type, decimal estimatedCost, int? squareFootage, ZoningDistrict? zoning);
    Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
    Task<IReadOnlyList<ActivityLogDto>> GetRecentActivityAsync(int count);
    Task<IReadOnlyList<StatusSummaryDto>> GetPermitsByStatusAsync();
}
