using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.DTOs.Permits;

public class PermitSearchCriteria
{
    public string? PermitId { get; set; }
    public string? Address { get; set; }
    public PermitType? PermitType { get; set; }
    public PermitStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
