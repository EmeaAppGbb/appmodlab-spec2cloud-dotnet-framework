namespace RiverdalePermitSystem.Application.DTOs.Permits;

public record DashboardStatisticsDto(
    int TotalPermits,
    int PendingReview,
    int InspectionsToday,
    decimal MonthlyRevenue);
