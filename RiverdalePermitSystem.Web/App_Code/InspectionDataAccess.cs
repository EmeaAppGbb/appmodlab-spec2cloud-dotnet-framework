using System;
using System.Data;
using System.Configuration;

public static class InspectionDataAccess
{
    private static string ConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["PermitDB"].ConnectionString; }
    }

    public static DataTable GetUpcomingInspections()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("InspectionId", typeof(string));
        dt.Columns.Add("PermitId", typeof(string));
        dt.Columns.Add("InspectionType", typeof(string));
        dt.Columns.Add("ScheduledDate", typeof(DateTime));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("InspectorName", typeof(string));

        // In real app: EXEC sp_GetUpcomingInspections
        string[] types = { "Foundation", "Framing", "Electrical", "Plumbing", "Final" };
        string[] inspectors = { "Inspector Davis", "Inspector Johnson", "Inspector Martinez" };

        for (int i = 0; i < 10; i++)
        {
            dt.Rows.Add(
                $"INSP-2024-{4000 + i}",
                $"PERM-2024-{2000 + i}",
                types[i % types.Length],
                DateTime.Now.AddDays(i),
                i % 3 == 0 ? "Scheduled" : "Pending",
                inspectors[i % inspectors.Length]
            );
        }

        return dt;
    }

    public static string ScheduleInspection(string permitId, string inspectionType, DateTime requestedDate, string notes)
    {
        string inspectionId = $"INSP-2024-{DateTime.Now.Ticks % 10000}";
        
        // In real app: EXEC sp_ScheduleInspection @PermitId, @InspectionType, @RequestedDate, @Notes, @InspectionId OUTPUT
        // This stored procedure would:
        // 1. Validate permit exists and is in correct status
        // 2. Check inspector availability
        // 3. Calculate inspection fees
        // 4. Send email notifications
        // 5. Update permit status
        
        return inspectionId;
    }

    public static void CompleteInspection(string inspectionId, string result)
    {
        // In real app: EXEC sp_CompleteInspection @InspectionId, @Result, @Comments, @Photos
        // This stored procedure would:
        // 1. Update inspection status and result
        // 2. Update permit status based on inspection result
        // 3. Generate inspection report
        // 4. Send notification emails
        // 5. Trigger next workflow step if inspection passed
    }

    public static void CancelInspection(string inspectionId)
    {
        // In real app: EXEC sp_CancelInspection @InspectionId
        // This stored procedure would:
        // 1. Update inspection status to cancelled
        // 2. Free up inspector's schedule
        // 3. Send notification to applicant
    }

    public static DataTable GetInspectionHistory(string permitId)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("InspectionDate", typeof(DateTime));
        dt.Columns.Add("InspectionType", typeof(string));
        dt.Columns.Add("Result", typeof(string));
        dt.Columns.Add("InspectorName", typeof(string));
        dt.Columns.Add("Comments", typeof(string));

        // In real app: EXEC sp_GetInspectionHistory @PermitId
        dt.Rows.Add(DateTime.Now.AddDays(-10), "Foundation", "Passed", "Inspector Davis", "Foundation meets code requirements");
        dt.Rows.Add(DateTime.Now.AddDays(-5), "Framing", "Passed", "Inspector Johnson", "Framing approved");

        return dt;
    }

    public static DataTable GetInspectorSchedule(string inspectorId, DateTime startDate, DateTime endDate)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("InspectionId", typeof(string));
        dt.Columns.Add("ScheduledTime", typeof(DateTime));
        dt.Columns.Add("PermitId", typeof(string));
        dt.Columns.Add("PropertyAddress", typeof(string));
        dt.Columns.Add("InspectionType", typeof(string));
        dt.Columns.Add("Status", typeof(string));

        // In real app: EXEC sp_GetInspectorSchedule @InspectorId, @StartDate, @EndDate
        
        return dt;
    }
}
