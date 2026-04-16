namespace RiverdalePermitSystem.Application.Interfaces;

public interface IEmailNotificationService
{
    Task SendPermitConfirmationAsync(string recipientEmail, string applicantName, string permitId);
    Task SendInspectionConfirmationAsync(string recipientEmail, string inspectionId, DateTime scheduledDate, string inspectionType);
    Task SendReviewCompletedAsync(string recipientEmail, string permitId, string reviewStatus, string comments);
    Task SendPermitIssuedAsync(string recipientEmail, string permitId, DateTime expirationDate);
}
