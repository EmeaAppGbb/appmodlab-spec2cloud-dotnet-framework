namespace RiverdalePermitSystem.Domain.Enums;

public enum PermitStatus
{
    Submitted,
    UnderReview,
    Approved,
    ReviewRejectedResubmitRequired,
    Issued,
    UnderInspection,
    InspectionFailedCorrectionsRequired,
    CertificateOfOccupancyIssued,
    Expired
}
