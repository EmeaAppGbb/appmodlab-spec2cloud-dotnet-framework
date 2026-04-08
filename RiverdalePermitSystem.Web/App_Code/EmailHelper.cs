using System;
using System.Net.Mail;
using System.Net;
using System.Configuration;

public static class EmailHelper
{
    private static string SmtpServer
    {
        get { return ConfigurationManager.AppSettings["SmtpServer"]; }
    }

    private static int SmtpPort
    {
        get { return int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "25"); }
    }

    private static string EmailFrom
    {
        get { return ConfigurationManager.AppSettings["EmailFrom"]; }
    }

    public static void SendPermitConfirmation(string recipientEmail, string applicantName, string permitId)
    {
        try
        {
            // In a real application with System.Net.Mail:
            // MailMessage message = new MailMessage();
            // message.From = new MailAddress(EmailFrom);
            // message.To.Add(recipientEmail);
            // message.Subject = $"Permit Application Confirmation - {permitId}";
            // message.Body = GetPermitConfirmationBody(applicantName, permitId);
            // message.IsBodyHtml = true;
            //
            // SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort);
            // smtp.Credentials = new NetworkCredential("username", "password");
            // smtp.EnableSsl = false;
            // smtp.Send(message);

            // Simulated email send
            System.Diagnostics.Debug.WriteLine($"Email sent to {recipientEmail}: Permit {permitId} confirmation");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Email send failed: {ex.Message}");
            // In production: log to database or file
        }
    }

    private static string GetPermitConfirmationBody(string applicantName, string permitId)
    {
        return $@"
            <html>
            <body>
                <h2>Permit Application Received</h2>
                <p>Dear {applicantName},</p>
                <p>Your building permit application has been successfully submitted.</p>
                <p><strong>Permit ID:</strong> {permitId}</p>
                <p><strong>Expected Review Time:</strong> 5-10 business days</p>
                <p>You will receive email updates as your permit moves through the review process.</p>
                <p>Thank you,<br/>Riverdale City Building Department</p>
            </body>
            </html>
        ";
    }

    public static void SendInspectionConfirmation(string recipientEmail, string inspectionId, DateTime scheduledDate, string inspectionType)
    {
        // In real app: Similar pattern to SendPermitConfirmation
        System.Diagnostics.Debug.WriteLine($"Inspection confirmation sent to {recipientEmail}: {inspectionType} on {scheduledDate:MM/dd/yyyy}");
    }

    public static void SendReviewCompletedNotification(string recipientEmail, string permitId, string reviewStatus, string comments)
    {
        // In real app: Similar pattern with status-specific templates
        System.Diagnostics.Debug.WriteLine($"Review notification sent to {recipientEmail}: Permit {permitId} {reviewStatus}");
    }

    public static void SendPermitIssuedNotification(string recipientEmail, string permitId, DateTime expirationDate)
    {
        // In real app: Include permit document attachment
        System.Diagnostics.Debug.WriteLine($"Permit issued notification sent to {recipientEmail}: {permitId}");
    }
}
