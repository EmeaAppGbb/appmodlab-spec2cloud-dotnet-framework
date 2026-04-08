-- Riverdale Building Permit System Database Schema
-- SQL Server 2019+

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RiverdalePermitDB')
BEGIN
    ALTER DATABASE RiverdalePermitDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RiverdalePermitDB;
END
GO

CREATE DATABASE RiverdalePermitDB;
GO

USE RiverdalePermitDB;
GO

-- Applicants Table
CREATE TABLE Applicants (
    ApplicantId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Company NVARCHAR(100) NULL,
    LicenseNumber NVARCHAR(50) NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    INDEX IX_Applicants_Email (Email)
);

-- Contractors Table
CREATE TABLE Contractors (
    ContractorId INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName NVARCHAR(200) NOT NULL,
    LicenseNumber NVARCHAR(50) NOT NULL UNIQUE,
    InsuranceExpiry DATE NULL,
    Rating DECIMAL(3,2) NULL,
    ContactEmail NVARCHAR(100) NOT NULL,
    ContactPhone NVARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1,
    INDEX IX_Contractors_LicenseNumber (LicenseNumber)
);

-- Permits Table
CREATE TABLE Permits (
    PermitId NVARCHAR(50) PRIMARY KEY,
    ApplicationDate DATETIME NOT NULL DEFAULT GETDATE(),
    PropertyAddress NVARCHAR(200) NOT NULL,
    ParcelNumber NVARCHAR(50) NOT NULL,
    PermitType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Submitted',
    ApplicantId INT NOT NULL,
    ContractorId INT NULL,
    EstimatedCost DECIMAL(18,2) NOT NULL,
    SquareFootage INT NULL,
    ZoningDistrict NVARCHAR(10) NULL,
    ProjectDescription NVARCHAR(MAX) NULL,
    IssuedDate DATETIME NULL,
    ExpirationDate DATETIME NULL,
    CompletedDate DATETIME NULL,
    CreatedBy NVARCHAR(100) DEFAULT SYSTEM_USER,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Permits_Applicants FOREIGN KEY (ApplicantId) REFERENCES Applicants(ApplicantId),
    CONSTRAINT FK_Permits_Contractors FOREIGN KEY (ContractorId) REFERENCES Contractors(ContractorId),
    INDEX IX_Permits_Status (Status),
    INDEX IX_Permits_ApplicationDate (ApplicationDate),
    INDEX IX_Permits_PropertyAddress (PropertyAddress)
);

-- Plan Reviews Table
CREATE TABLE PlanReviews (
    ReviewId NVARCHAR(50) PRIMARY KEY,
    PermitId NVARCHAR(50) NOT NULL,
    ReviewerId NVARCHAR(100) NOT NULL,
    ReviewType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    Comments NVARCHAR(MAX) NULL,
    Deficiencies NVARCHAR(MAX) NULL,
    ReviewDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME NULL,
    CompletedDate DATETIME NULL,
    CONSTRAINT FK_PlanReviews_Permits FOREIGN KEY (PermitId) REFERENCES Permits(PermitId),
    INDEX IX_PlanReviews_PermitId (PermitId),
    INDEX IX_PlanReviews_Status (Status)
);

-- Inspections Table
CREATE TABLE Inspections (
    InspectionId NVARCHAR(50) PRIMARY KEY,
    PermitId NVARCHAR(50) NOT NULL,
    InspectorId NVARCHAR(100) NOT NULL,
    InspectionType NVARCHAR(50) NOT NULL,
    ScheduledDate DATETIME NOT NULL,
    CompletedDate DATETIME NULL,
    Result NVARCHAR(50) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Scheduled',
    Comments NVARCHAR(MAX) NULL,
    Photos NVARCHAR(MAX) NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Inspections_Permits FOREIGN KEY (PermitId) REFERENCES Permits(PermitId),
    INDEX IX_Inspections_PermitId (PermitId),
    INDEX IX_Inspections_ScheduledDate (ScheduledDate),
    INDEX IX_Inspections_InspectorId (InspectorId)
);

-- Fees Table
CREATE TABLE Fees (
    FeeId INT IDENTITY(1,1) PRIMARY KEY,
    PermitId NVARCHAR(50) NOT NULL,
    FeeType NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaidDate DATETIME NULL,
    PaymentMethod NVARCHAR(50) NULL,
    TransactionId NVARCHAR(100) NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Fees_Permits FOREIGN KEY (PermitId) REFERENCES Permits(PermitId),
    INDEX IX_Fees_PermitId (PermitId)
);

-- Activity Log Table
CREATE TABLE ActivityLog (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    Timestamp DATETIME DEFAULT GETDATE(),
    ActivityType NVARCHAR(100) NOT NULL,
    PermitId NVARCHAR(50) NULL,
    Description NVARCHAR(MAX) NULL,
    UserName NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_ActivityLog_Permits FOREIGN KEY (PermitId) REFERENCES Permits(PermitId),
    INDEX IX_ActivityLog_Timestamp (Timestamp),
    INDEX IX_ActivityLog_PermitId (PermitId)
);

-- Insert Sample Data
INSERT INTO Applicants (Name, Email, Phone, Company, LicenseNumber)
VALUES 
    ('John Smith', 'jsmith@email.com', '555-0101', 'Smith Construction', 'LIC-2024-001'),
    ('Mary Johnson', 'mjohnson@email.com', '555-0102', NULL, NULL),
    ('Bob Williams', 'bwilliams@email.com', '555-0103', 'Williams Builders', 'LIC-2024-002'),
    ('Sarah Davis', 'sdavis@email.com', '555-0104', NULL, NULL);

INSERT INTO Contractors (CompanyName, LicenseNumber, InsuranceExpiry, Rating, ContactEmail, ContactPhone)
VALUES
    ('Premier Builders LLC', 'CONT-2024-001', '2025-12-31', 4.8, 'info@premierbuilders.com', '555-0201'),
    ('Riverside Construction', 'CONT-2024-002', '2025-06-30', 4.5, 'contact@riversideconst.com', '555-0202'),
    ('Elite Electrical Services', 'CONT-2024-003', '2025-09-30', 4.9, 'service@eliteelectric.com', '555-0203');

INSERT INTO Permits (PermitId, PropertyAddress, ParcelNumber, PermitType, Status, ApplicantId, EstimatedCost, ZoningDistrict, ProjectDescription)
VALUES
    ('PERM-2024-1001', '123 Main Street, Riverdale', 'PAR-2024-001', 'NewConstruction', 'Under Review', 1, 250000, 'R1', 'New single family residence'),
    ('PERM-2024-1002', '456 Oak Avenue, Riverdale', 'PAR-2024-002', 'Addition', 'Approved', 2, 75000, 'R1', 'Second story addition'),
    ('PERM-2024-1003', '789 Elm Drive, Riverdale', 'PAR-2024-003', 'Electrical', 'Issued', 3, 15000, 'C1', 'Commercial electrical upgrade'),
    ('PERM-2024-1004', '321 Maple Lane, Riverdale', 'PAR-2024-004', 'Plumbing', 'Submitted', 4, 12000, 'R2', 'Bathroom remodel with new fixtures'),
    ('PERM-2024-1005', '654 Pine Road, Riverdale', 'PAR-2024-005', 'NewConstruction', 'Under Review', 1, 450000, 'R1', 'Custom home with basement');

GO
