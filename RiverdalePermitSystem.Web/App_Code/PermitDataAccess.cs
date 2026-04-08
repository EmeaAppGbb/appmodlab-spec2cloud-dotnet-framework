using System;
using System.Data;
using System.Configuration;

public static class PermitDataAccess
{
    private static string ConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["PermitDB"].ConnectionString; }
    }

    public static DataTable GetRecentPermits(int count)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("PermitId", typeof(string));
        dt.Columns.Add("ApplicationDate", typeof(DateTime));
        dt.Columns.Add("PropertyAddress", typeof(string));
        dt.Columns.Add("PermitType", typeof(string));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("EstimatedCost", typeof(decimal));

        for (int i = 1; i <= count; i++)
        {
            dt.Rows.Add(
                $"PERM-2024-{1000 + i}",
                DateTime.Now.AddDays(-i),
                $"{100 + i} Main Street, Riverdale",
                i % 3 == 0 ? "NewConstruction" : (i % 2 == 0 ? "Addition" : "Electrical"),
                i % 4 == 0 ? "Issued" : (i % 3 == 0 ? "Under Review" : "Submitted"),
                15000m + (i * 1000)
            );
        }

        return dt;
    }

    public static decimal CalculatePermitFee(string permitType, decimal estimatedCost)
    {
        decimal baseFee = 100m;
        decimal percentageFee = 0.02m;

        switch (permitType)
        {
            case "NewConstruction":
                baseFee = 500m;
                percentageFee = 0.03m;
                break;
            case "Addition":
                baseFee = 300m;
                percentageFee = 0.025m;
                break;
            case "Electrical":
            case "Plumbing":
            case "Mechanical":
                baseFee = 150m;
                percentageFee = 0.015m;
                break;
            case "Demolition":
                baseFee = 250m;
                percentageFee = 0.01m;
                break;
        }

        return baseFee + (estimatedCost * percentageFee);
    }

    public static string SubmitPermitApplication(string propertyAddress, string parcelNumber, string zoning,
        string permitType, string description, decimal estimatedCost, DataTable applicantData)
    {
        string permitId = $"PERM-2024-{DateTime.Now.Ticks % 100000}";
        
        // In a real application, this would execute stored procedures:
        // EXEC sp_InsertPermit @PermitId, @PropertyAddress, @ParcelNumber, @PermitType, @EstimatedCost, @Status
        // EXEC sp_InsertApplicant @ApplicantData
        // EXEC sp_CalculateFees @PermitId
        
        return permitId;
    }

    public static DataTable SearchPermits(string permitId, string address, string permitType, string status, int startRow, int pageSize)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("PermitId", typeof(string));
        dt.Columns.Add("ApplicationDate", typeof(DateTime));
        dt.Columns.Add("PropertyAddress", typeof(string));
        dt.Columns.Add("PermitType", typeof(string));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("EstimatedCost", typeof(decimal));

        for (int i = 1; i <= pageSize; i++)
        {
            dt.Rows.Add(
                $"PERM-2024-{2000 + startRow + i}",
                DateTime.Now.AddDays(-(startRow + i)),
                $"{200 + i} Oak Avenue, Riverdale",
                string.IsNullOrEmpty(permitType) ? "NewConstruction" : permitType,
                string.IsNullOrEmpty(status) ? "Submitted" : status,
                25000m + (i * 2000)
            );
        }

        return dt;
    }

    public static DataTable GetPermitById(string permitId)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("PermitId", typeof(string));
        dt.Columns.Add("ApplicationDate", typeof(DateTime));
        dt.Columns.Add("PropertyAddress", typeof(string));
        dt.Columns.Add("ParcelNumber", typeof(string));
        dt.Columns.Add("PermitType", typeof(string));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("EstimatedCost", typeof(decimal));
        dt.Columns.Add("ApplicantName", typeof(string));

        dt.Rows.Add(
            permitId,
            DateTime.Now.AddDays(-5),
            "123 Main Street, Riverdale",
            "PAR-2024-001",
            "NewConstruction",
            "Under Review",
            45000m,
            "John Smith"
        );

        return dt;
    }

    public static DataTable GetDashboardStatistics()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("TotalPermits", typeof(int));
        dt.Columns.Add("PendingReview", typeof(int));
        dt.Columns.Add("InspectionsToday", typeof(int));
        dt.Columns.Add("MonthlyRevenue", typeof(decimal));

        // In real app: EXEC sp_GetDashboardStats
        dt.Rows.Add(1247, 38, 12, 125430.50m);

        return dt;
    }

    public static DataTable GetRecentActivity(int count)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Timestamp", typeof(DateTime));
        dt.Columns.Add("ActivityType", typeof(string));
        dt.Columns.Add("PermitId", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        dt.Columns.Add("UserName", typeof(string));

        string[] activities = { "Application Submitted", "Review Completed", "Inspection Scheduled", "Permit Issued" };
        string[] users = { "jsmith", "mjones", "bwilliams", "sdavis" };

        for (int i = 0; i < count; i++)
        {
            dt.Rows.Add(
                DateTime.Now.AddMinutes(-i * 15),
                activities[i % activities.Length],
                $"PERM-2024-{3000 + i}",
                $"{activities[i % activities.Length]} for permit",
                users[i % users.Length]
            );
        }

        return dt;
    }

    public static DataTable GetPermitsByStatus()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("Count", typeof(int));
        dt.Columns.Add("TotalValue", typeof(decimal));

        // In real app: EXEC sp_GetPermitsByStatus
        dt.Rows.Add("Submitted", 38, 847500m);
        dt.Rows.Add("Under Review", 25, 625000m);
        dt.Rows.Add("Approved", 42, 1150000m);
        dt.Rows.Add("Issued", 156, 4275000m);
        dt.Rows.Add("Expired", 8, 175000m);

        return dt;
    }

    public static DataTable GetPlanReviewHistory(string permitId)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ReviewDate", typeof(DateTime));
        dt.Columns.Add("ReviewType", typeof(string));
        dt.Columns.Add("ReviewerName", typeof(string));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("Comments", typeof(string));

        // In real app: EXEC sp_GetPlanReviewHistory @PermitId
        dt.Rows.Add(DateTime.Now.AddDays(-3), "Structural", "M. Anderson", "Approved", "Plans meet structural requirements");
        dt.Rows.Add(DateTime.Now.AddDays(-1), "Electrical", "R. Thompson", "Pending", "Under review");

        return dt;
    }

    public static string SubmitPlanReview(string permitId, string reviewType, string reviewer, 
        string status, string comments, string deficiencies)
    {
        string reviewId = $"REV-2024-{DateTime.Now.Ticks % 10000}";
        
        // In real app: EXEC sp_SubmitPlanReview @PermitId, @ReviewType, @Reviewer, @Status, @Comments, @Deficiencies
        
        return reviewId;
    }
}
