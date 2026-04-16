using RiverdalePermitSystem.Application.DTOs.Inspections;
using RiverdalePermitSystem.Domain.Enums;
using RiverdalePermitSystem.Domain.ValueObjects;

namespace RiverdalePermitSystem.Application.Interfaces;

public interface IInspectionService
{
    Task<IReadOnlyList<InspectionSummaryDto>> GetUpcomingInspectionsAsync();
    Task<string> ScheduleInspectionAsync(InspectionScheduleDto dto);
    Task CompleteInspectionAsync(string inspectionId, InspectionResult result, string? comments);
    Task CancelInspectionAsync(string inspectionId);
    Task<IReadOnlyList<InspectionSummaryDto>> GetInspectionHistoryAsync(string permitId);
}
