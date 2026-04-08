-- Stored Procedures for Riverdale Building Permit System
-- These demonstrate the legacy pattern of business logic in stored procedures

USE RiverdalePermitDB;
GO

-- =============================================
-- Procedure: sp_SubmitPermitApplication
-- Description: Submits a new permit application with business rule validation
-- =============================================
CREATE OR ALTER PROCEDURE sp_SubmitPermitApplication
    @PermitId NVARCHAR(50),
    @PropertyAddress NVARCHAR(200),
    @ParcelNumber NVARCHAR(50),
    @PermitType NVARCHAR(50),
    @EstimatedCost DECIMAL(18,2),
    @ZoningDistrict NVARCHAR(10),
    @ProjectDescription NVARCHAR(MAX),
    @ApplicantName NVARCHAR(100),
    @ApplicantEmail NVARCHAR(100),
    @ApplicantPhone NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Business Rule: Validate estimated cost is within zoning limits
        IF @PermitType = 'NewConstruction' AND @EstimatedCost < 10000
        BEGIN
            RAISERROR('New construction permits must have estimated cost of at least $10,000', 16, 1);
            RETURN -1;
        END
        
        -- Insert or update applicant
        DECLARE @ApplicantId INT;
        
        SELECT @ApplicantId = ApplicantId 
        FROM Applicants 
        WHERE Email = @ApplicantEmail;
        
        IF @ApplicantId IS NULL
        BEGIN
            INSERT INTO Applicants (Name, Email, Phone)
            VALUES (@ApplicantName, @ApplicantEmail, @ApplicantPhone);
            
            SET @ApplicantId = SCOPE_IDENTITY();
        END
        
        -- Insert permit
        INSERT INTO Permits (
            PermitId, PropertyAddress, ParcelNumber, PermitType, 
            Status, ApplicantId, EstimatedCost, ZoningDistrict, ProjectDescription
        )
        VALUES (
            @PermitId, @PropertyAddress, @ParcelNumber, @PermitType,
            'Submitted', @ApplicantId, @EstimatedCost, @ZoningDistrict, @ProjectDescription
        );
        
        -- Business Rule: Calculate and insert fees based on permit type and cost
        DECLARE @BaseFee DECIMAL(18,2);
        DECLARE @PercentageFee DECIMAL(5,4);
        
        SELECT 
            @BaseFee = CASE @PermitType
                WHEN 'NewConstruction' THEN 500.00
                WHEN 'Addition' THEN 300.00
                WHEN 'Electrical' THEN 150.00
                WHEN 'Plumbing' THEN 150.00
                WHEN 'Mechanical' THEN 150.00
                WHEN 'Demolition' THEN 250.00
                ELSE 100.00
            END,
            @PercentageFee = CASE @PermitType
                WHEN 'NewConstruction' THEN 0.03
                WHEN 'Addition' THEN 0.025
                ELSE 0.015
            END;
        
        DECLARE @TotalFee DECIMAL(18,2) = @BaseFee + (@EstimatedCost * @PercentageFee);
        
        INSERT INTO Fees (PermitId, FeeType, Amount)
        VALUES (@PermitId, 'Permit Fee', @TotalFee);
        
        -- Log activity
        INSERT INTO ActivityLog (ActivityType, PermitId, Description, UserName)
        VALUES ('Application Submitted', @PermitId, 
                'New ' + @PermitType + ' permit application submitted', 
                SYSTEM_USER);
        
        COMMIT TRANSACTION;
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

-- =============================================
-- Procedure: sp_CalculatePermitFee
-- Description: Complex fee calculation with multiple business rules
-- =============================================
CREATE OR ALTER PROCEDURE sp_CalculatePermitFee
    @PermitType NVARCHAR(50),
    @EstimatedCost DECIMAL(18,2),
    @SquareFootage INT = NULL,
    @ZoningDistrict NVARCHAR(10) = NULL,
    @CalculatedFee DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @BaseFee DECIMAL(18,2);
    DECLARE @PercentageFee DECIMAL(5,4);
    DECLARE @SquareFootageFee DECIMAL(18,2) = 0;
    
    -- Base fee by permit type
    SET @BaseFee = CASE @PermitType
        WHEN 'NewConstruction' THEN 500.00
        WHEN 'Addition' THEN 300.00
        WHEN 'Electrical' THEN 150.00
        WHEN 'Plumbing' THEN 150.00
        WHEN 'Mechanical' THEN 150.00
        WHEN 'Demolition' THEN 250.00
        ELSE 100.00
    END;
    
    -- Percentage fee by permit type
    SET @PercentageFee = CASE @PermitType
        WHEN 'NewConstruction' THEN 0.03
        WHEN 'Addition' THEN 0.025
        WHEN 'Electrical' THEN 0.015
        WHEN 'Plumbing' THEN 0.015
        WHEN 'Mechanical' THEN 0.015
        WHEN 'Demolition' THEN 0.01
        ELSE 0.02
    END;
    
    -- Business Rule: Additional square footage fee for construction
    IF @SquareFootage IS NOT NULL AND @PermitType IN ('NewConstruction', 'Addition')
    BEGIN
        SET @SquareFootageFee = @SquareFootage * 0.50;
    END
    
    -- Business Rule: Zoning surcharge for commercial districts
    DECLARE @ZoningSurcharge DECIMAL(18,2) = 0;
    IF @ZoningDistrict IN ('C1', 'I1')
    BEGIN
        SET @ZoningSurcharge = 200.00;
    END
    
    -- Calculate total
    SET @CalculatedFee = @BaseFee + (@EstimatedCost * @PercentageFee) + @SquareFootageFee + @ZoningSurcharge;
    
    RETURN 0;
END
GO

-- =============================================
-- Procedure: sp_ScheduleInspection
-- Description: Schedule inspection with availability checking
-- =============================================
CREATE OR ALTER PROCEDURE sp_ScheduleInspection
    @PermitId NVARCHAR(50),
    @InspectionType NVARCHAR(50),
    @RequestedDate DATETIME,
    @InspectorId NVARCHAR(100) = NULL,
    @Notes NVARCHAR(MAX) = NULL,
    @InspectionId NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Business Rule: Validate permit exists and is in correct status
        DECLARE @PermitStatus NVARCHAR(50);
        SELECT @PermitStatus = Status FROM Permits WHERE PermitId = @PermitId;
        
        IF @PermitStatus IS NULL
        BEGIN
            RAISERROR('Permit not found', 16, 1);
            RETURN -1;
        END
        
        IF @PermitStatus NOT IN ('Issued', 'Under Inspection')
        BEGIN
            RAISERROR('Permit must be issued before scheduling inspections', 16, 1);
            RETURN -1;
        END
        
        -- Business Rule: No inspections on weekends
        IF DATEPART(WEEKDAY, @RequestedDate) IN (1, 7)
        BEGIN
            RAISERROR('Inspections cannot be scheduled on weekends', 16, 1);
            RETURN -1;
        END
        
        -- Auto-assign inspector if not provided
        IF @InspectorId IS NULL
        BEGIN
            -- Business Logic: Find inspector with least inspections on requested date
            SELECT TOP 1 @InspectorId = InspectorId
            FROM (
                SELECT 'Inspector1' AS InspectorId, COUNT(*) AS InspectionCount
                FROM Inspections
                WHERE CAST(ScheduledDate AS DATE) = CAST(@RequestedDate AS DATE)
                    AND Status <> 'Cancelled'
                GROUP BY InspectorId
                UNION ALL
                SELECT 'Inspector2', 0
            ) InspectorLoad
            ORDER BY InspectionCount;
        END
        
        -- Generate inspection ID
        SET @InspectionId = 'INSP-' + FORMAT(GETDATE(), 'yyyy') + '-' + 
                           CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR(10));
        
        -- Insert inspection
        INSERT INTO Inspections (
            InspectionId, PermitId, InspectorId, InspectionType, 
            ScheduledDate, Status, Comments
        )
        VALUES (
            @InspectionId, @PermitId, @InspectorId, @InspectionType,
            @RequestedDate, 'Scheduled', @Notes
        );
        
        -- Update permit status
        UPDATE Permits 
        SET Status = 'Under Inspection', ModifiedDate = GETDATE()
        WHERE PermitId = @PermitId;
        
        -- Log activity
        INSERT INTO ActivityLog (ActivityType, PermitId, Description, UserName)
        VALUES ('Inspection Scheduled', @PermitId, 
                @InspectionType + ' inspection scheduled for ' + CONVERT(NVARCHAR, @RequestedDate, 101),
                SYSTEM_USER);
        
        COMMIT TRANSACTION;
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

-- =============================================
-- Procedure: sp_CompleteInspection
-- Description: Complete inspection and update permit status
-- =============================================
CREATE OR ALTER PROCEDURE sp_CompleteInspection
    @InspectionId NVARCHAR(50),
    @Result NVARCHAR(50),
    @Comments NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @PermitId NVARCHAR(50);
        DECLARE @InspectionType NVARCHAR(50);
        
        -- Update inspection
        UPDATE Inspections
        SET Result = @Result,
            Status = 'Completed',
            CompletedDate = GETDATE(),
            Comments = ISNULL(Comments, '') + CHAR(13) + CHAR(10) + ISNULL(@Comments, '')
        OUTPUT INSERTED.PermitId, INSERTED.InspectionType
        INTO @TempInspection
        WHERE InspectionId = @InspectionId;
        
        SELECT @PermitId = PermitId, @InspectionType = InspectionType FROM @TempInspection;
        
        -- Business Rule: Update permit status based on inspection result
        IF @Result = 'Passed'
        BEGIN
            -- Check if this was a final inspection
            IF @InspectionType = 'Final'
            BEGIN
                UPDATE Permits
                SET Status = 'Certificate of Occupancy Issued',
                    CompletedDate = GETDATE(),
                    ModifiedDate = GETDATE()
                WHERE PermitId = @PermitId;
            END
            ELSE
            BEGIN
                UPDATE Permits
                SET Status = 'Issued', ModifiedDate = GETDATE()
                WHERE PermitId = @PermitId;
            END
        END
        ELSE IF @Result = 'Failed'
        BEGIN
            UPDATE Permits
            SET Status = 'Inspection Failed - Corrections Required',
                ModifiedDate = GETDATE()
            WHERE PermitId = @PermitId;
        END
        
        -- Log activity
        INSERT INTO ActivityLog (ActivityType, PermitId, Description, UserName)
        VALUES ('Inspection Completed', @PermitId,
                @InspectionType + ' inspection ' + @Result,
                SYSTEM_USER);
        
        COMMIT TRANSACTION;
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

-- =============================================
-- Procedure: sp_SubmitPlanReview
-- Description: Submit plan review with deficiency tracking
-- =============================================
CREATE OR ALTER PROCEDURE sp_SubmitPlanReview
    @PermitId NVARCHAR(50),
    @ReviewType NVARCHAR(50),
    @ReviewerId NVARCHAR(100),
    @Status NVARCHAR(50),
    @Comments NVARCHAR(MAX) = NULL,
    @Deficiencies NVARCHAR(MAX) = NULL,
    @ReviewId NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Generate review ID
        SET @ReviewId = 'REV-' + FORMAT(GETDATE(), 'yyyy') + '-' + 
                       CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR(10));
        
        -- Insert review
        INSERT INTO PlanReviews (
            ReviewId, PermitId, ReviewerId, ReviewType, 
            Status, Comments, Deficiencies, ReviewDate
        )
        VALUES (
            @ReviewId, @PermitId, @ReviewerId, @ReviewType,
            @Status, @Comments, @Deficiencies, GETDATE()
        );
        
        -- Business Rule: Update permit status based on all reviews
        IF @Status = 'Approved'
        BEGIN
            -- Check if all required reviews are approved
            DECLARE @PendingReviews INT;
            SELECT @PendingReviews = COUNT(*)
            FROM PlanReviews
            WHERE PermitId = @PermitId 
                AND Status IN ('Pending', 'InProgress', 'Rejected');
            
            IF @PendingReviews = 0
            BEGIN
                UPDATE Permits
                SET Status = 'Approved', ModifiedDate = GETDATE()
                WHERE PermitId = @PermitId;
            END
        END
        ELSE IF @Status = 'Rejected'
        BEGIN
            UPDATE Permits
            SET Status = 'Review Rejected - Resubmit Required',
                ModifiedDate = GETDATE()
            WHERE PermitId = @PermitId;
        END
        
        -- Log activity
        INSERT INTO ActivityLog (ActivityType, PermitId, Description, UserName)
        VALUES ('Plan Review Submitted', @PermitId,
                @ReviewType + ' review ' + @Status,
                @ReviewerId);
        
        COMMIT TRANSACTION;
        
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

-- Helper table variable declaration for sp_CompleteInspection
IF OBJECT_ID('tempdb..@TempInspection') IS NOT NULL
    DROP TABLE @TempInspection;
CREATE TABLE #TempInspection (PermitId NVARCHAR(50), InspectionType NVARCHAR(50));
GO

PRINT 'Stored procedures created successfully';
GO
