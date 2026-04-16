using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Domain.Services;

public class PermitStatusMachine
{
    private static readonly Dictionary<PermitStatus, HashSet<PermitStatus>> _validTransitions = new()
    {
        [PermitStatus.Submitted] = new() { PermitStatus.UnderReview },
        [PermitStatus.UnderReview] = new() { PermitStatus.Approved, PermitStatus.ReviewRejectedResubmitRequired },
        [PermitStatus.Approved] = new() { PermitStatus.Issued },
        [PermitStatus.ReviewRejectedResubmitRequired] = new() { PermitStatus.UnderReview },
        [PermitStatus.Issued] = new() { PermitStatus.UnderInspection, PermitStatus.Expired },
        [PermitStatus.UnderInspection] = new() { PermitStatus.CertificateOfOccupancyIssued, PermitStatus.InspectionFailedCorrectionsRequired, PermitStatus.Issued },
        [PermitStatus.InspectionFailedCorrectionsRequired] = new() { PermitStatus.UnderInspection },
        [PermitStatus.CertificateOfOccupancyIssued] = new(),
        [PermitStatus.Expired] = new()
    };

    public bool CanTransition(PermitStatus current, PermitStatus target)
    {
        return _validTransitions.TryGetValue(current, out var allowed) && allowed.Contains(target);
    }

    public PermitStatus Transition(PermitStatus current, PermitStatus target)
    {
        if (!CanTransition(current, target))
            throw new InvalidOperationException($"Cannot transition from {current} to {target}.");

        return target;
    }
}
