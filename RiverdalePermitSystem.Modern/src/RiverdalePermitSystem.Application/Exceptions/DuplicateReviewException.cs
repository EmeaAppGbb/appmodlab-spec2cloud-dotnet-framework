using RiverdalePermitSystem.Domain.Enums;

namespace RiverdalePermitSystem.Application.Exceptions;

public class DuplicateReviewException : Exception
{
    public string PermitId { get; }
    public ReviewType ReviewType { get; }

    public DuplicateReviewException(string permitId, ReviewType reviewType)
        : base($"A '{reviewType}' review already exists for permit '{permitId}'.")
    {
        PermitId = permitId;
        ReviewType = reviewType;
    }
}
