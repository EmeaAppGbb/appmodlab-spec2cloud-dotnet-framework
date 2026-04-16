using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Domain.Entities;

public class ActivityLogEntry
{
    public int LogId { get; set; }
    public DateTime Timestamp { get; set; }
    public ActivityType ActivityType { get; set; }
    public string? PermitId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
