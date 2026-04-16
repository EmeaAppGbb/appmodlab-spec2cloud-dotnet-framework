namespace RiverdalePermitSystem.Application.Exceptions;

public class PermitNotFoundException : Exception
{
    public string PermitId { get; }

    public PermitNotFoundException(string permitId)
        : base($"Permit '{permitId}' was not found.")
    {
        PermitId = permitId;
    }
}
