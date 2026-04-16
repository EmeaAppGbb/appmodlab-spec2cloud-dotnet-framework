using Microsoft.Extensions.Logging;
using RiverdalePermitSystem.Application.Interfaces;

namespace RiverdalePermitSystem.Infrastructure.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendPermitConfirmationAsync(string recipientEmail, string applicantName, string permitId)
    {
        _logger.LogInformation("Email: Permit confirmation sent to {Email} for permit {PermitId}", recipientEmail, permitId);
        return Task.CompletedTask;
    }

    public Task SendInspectionConfirmationAsync(string recipientEmail, string inspectionId, DateTime scheduledDate, string inspectionType)
    {
        _logger.LogInformation("Email: Inspection confirmation sent to {Email} for {InspectionType} on {Date}", recipientEmail, inspectionType, scheduledDate);
        return Task.CompletedTask;
    }

    public Task SendReviewCompletedAsync(string recipientEmail, string permitId, string reviewStatus, string comments)
    {
        _logger.LogInformation("Email: Review completed notification sent to {Email} for permit {PermitId}", recipientEmail, permitId);
        return Task.CompletedTask;
    }

    public Task SendPermitIssuedAsync(string recipientEmail, string permitId, DateTime expirationDate)
    {
        _logger.LogInformation("Email: Permit issued notification sent to {Email} for permit {PermitId}", recipientEmail, permitId);
        return Task.CompletedTask;
    }
}
