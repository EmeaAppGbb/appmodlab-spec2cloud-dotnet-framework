using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.Exceptions;

public class InvalidStatusTransitionException : Exception
{
    public PermitStatus CurrentStatus { get; }
    public PermitStatus TargetStatus { get; }

    public InvalidStatusTransitionException(PermitStatus current, PermitStatus target)
        : base($"Cannot transition permit from '{current}' to '{target}'.")
    {
        CurrentStatus = current;
        TargetStatus = target;
    }
}
