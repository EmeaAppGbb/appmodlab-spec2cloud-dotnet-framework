# Step 5: Modern Architecture Design — Riverdale City Building Permit System

> **Generated:** 2026-04-16 | **Methodology:** Spec2Cloud Modern Architecture Design
> **Application:** Riverdale City Building Permit System
> **Current Platform:** ASP.NET Web Forms / .NET Framework 4.8
> **Target Platform:** .NET 9 / Blazor Server / EF Core / Azure App Service / Azure SQL
> **Input:** Step 3 — Architecture Specification, Step 4 — Component Specifications

---

## Table of Contents

1. [Target Platform](#1-target-platform)
2. [Clean Architecture Layers](#2-clean-architecture-layers)
3. [Migration Map — Pages to Components](#3-migration-map--pages-to-components)
4. [Data Access Modernization](#4-data-access-modernization)
5. [Project Structure](#5-project-structure)
6. [Azure Services Integration](#6-azure-services-integration)
7. [Cross-Cutting Concerns](#7-cross-cutting-concerns)

---

## 1. Target Platform

### 1.1 Technology Stack

| Layer | Legacy Technology | Modern Technology | Justification |
|---|---|---|---|
| **Runtime** | .NET Framework 4.8 | **.NET 9** | LTS-adjacent (current), cross-platform, performance, modern C# features (records, pattern matching, primary constructors) |
| **UI Framework** | ASP.NET Web Forms (ViewState, PostBack, UpdatePanel) | **Blazor Server** | Server-side rendering via SignalR; eliminates ViewState overhead; component-based architecture; real-time UI updates; maintains server-affinity (suitable for intranet/municipal app) |
| **ORM / Data Access** | ADO.NET static classes returning `DataTable` | **Entity Framework Core 9** | Strongly-typed entities, LINQ queries, migrations, change tracking, connection pooling, compiled queries |
| **Database** | SQL Server LocalDB (file-attached `.mdf`) | **Azure SQL Database** | Managed PaaS, automatic backups, geo-replication, elastic scaling, built-in threat detection |
| **Hosting** | IIS Express / IIS on Windows | **Azure App Service (Linux)** | Managed PaaS, auto-scaling, deployment slots, custom domains, managed certificates, integrated CI/CD |
| **Authentication** | Simulated (Session GUID) | **Microsoft Entra ID + ASP.NET Core Identity** | Enterprise SSO, MFA, conditional access, RBAC, claims-based authorization |
| **Email** | `System.Net.Mail.SmtpClient` (simulated) | **Azure Communication Services** | Managed email delivery, tracking, templates, high deliverability, no SMTP infrastructure |
| **Caching** | None | **Azure Cache for Redis** | Distributed session/data cache for horizontal scaling |
| **Logging** | `Debug.WriteLine` | **Application Insights + Serilog** | Structured logging, distributed tracing, alerting, dashboards, log analytics |
| **Secrets** | `Web.config` plaintext | **Azure Key Vault** | Centralized secrets management, rotation, audit trail, managed identity access |
| **CI/CD** | None (manual IIS deployment) | **GitHub Actions** | Automated build, test, deploy to Azure App Service slots |

### 1.2 Target Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Azure Cloud                                         │
│                                                                                  │
│  ┌─── Azure App Service (Linux) ─────────────────────────────────────────────┐  │
│  │                                                                            │  │
│  │  ┌────────────────────────────────────────────────────────────────────┐    │  │
│  │  │            RiverdalePermitSystem.Web (.NET 9 / Blazor Server)     │    │  │
│  │  │                                                                    │    │  │
│  │  │  ┌─────────────┐  ┌──────────────┐  ┌───────────────────────┐    │    │  │
│  │  │  │ Blazor       │  │ Application  │  │ Infrastructure        │    │    │  │
│  │  │  │ Components   │→ │ Services     │→ │ (EF Core, Email,      │    │    │  │
│  │  │  │ (.razor)     │  │ (Interfaces) │  │  External Services)   │    │    │  │
│  │  │  └─────────────┘  └──────────────┘  └───────────────────────┘    │    │  │
│  │  │         ↕ SignalR          ↑                    │                  │    │  │
│  │  └────────────────────────────┼────────────────────┼──────────────────┘    │  │
│  │                               │                    │                       │  │
│  │  Domain Layer (Entities, Value Objects, Enums)     │                       │  │
│  └───────────────────────────────────────────────────┼───────────────────────┘  │
│                                                       │                          │
│       ┌──────────────────────┬──────────────────┬────┴───────────┐              │
│       ▼                      ▼                  ▼                ▼              │
│  ┌──────────┐  ┌───────────────────┐  ┌──────────────┐  ┌─────────────┐       │
│  │Azure SQL │  │Azure Communication│  │ Azure Cache  │  │ Azure Key   │       │
│  │ Database │  │    Services       │  │ for Redis    │  │   Vault     │       │
│  └──────────┘  └───────────────────┘  └──────────────┘  └─────────────┘       │
│                                                                                  │
│  ┌───────────────────┐  ┌───────────────────┐                                   │
│  │ Microsoft Entra ID │  │ Application       │                                   │
│  │ (Authentication)   │  │ Insights          │                                   │
│  └───────────────────┘  └───────────────────┘                                   │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 1.3 Key Platform Decisions

| Decision | Rationale |
|---|---|
| **Blazor Server** over Blazor WebAssembly | Municipal intranet app with sensitive data; server-side keeps business logic secure; no WASM download latency; real-time SignalR updates replace UpdatePanel pattern; simpler auth integration |
| **.NET 9** over .NET 8 LTS | Latest stable release with performance improvements; Blazor Server enhancements; EF Core 9 improvements; can upgrade to .NET 10 LTS when available |
| **EF Core** over Dapper | Complex domain model with relationships (Permits→Inspections, Permits→PlanReviews, etc.); migrations for schema evolution; change tracking for audit; compiled queries for performance-critical paths |
| **Azure App Service** over AKS/Containers | Right-sized for a municipal department app; simpler operations; built-in scaling; deployment slots for zero-downtime deploys; no container orchestration overhead |
| **Single deployable** over microservices | Current scope (6 pages, 7 tables, ~20 operations) does not justify distributed architecture; Clean Architecture enables future extraction if needed |

---

## 2. Clean Architecture Layers

### 2.1 Layer Overview

```
┌──────────────────────────────────────────────────────────────────────┐
│                    Presentation Layer (Blazor Server)                  │
│  Razor components, layouts, pages, shared UI components               │
│  Depends on: Application Layer                                        │
├──────────────────────────────────────────────────────────────────────┤
│                    Application Layer (Services)                       │
│  Service interfaces, DTOs, validators, MediatR commands/queries       │
│  Depends on: Domain Layer                                             │
├──────────────────────────────────────────────────────────────────────┤
│                    Domain Layer (Core)                                 │
│  Entities, value objects, enums, domain events, business rules        │
│  Depends on: Nothing (innermost layer)                                │
├──────────────────────────────────────────────────────────────────────┤
│                    Infrastructure Layer (EF Core + External)          │
│  DbContext, repositories, email service, Azure integrations           │
│  Depends on: Application Layer (implements interfaces)                │
└──────────────────────────────────────────────────────────────────────┘
```

### 2.2 Domain Layer — `RiverdalePermitSystem.Domain`

The innermost layer contains enterprise-wide business rules and entities with **zero** external dependencies.

#### Entities

| Entity | Properties | Business Rules |
|---|---|---|
| **Permit** | `PermitId` (string, PK), `ApplicationDate`, `PropertyAddress`, `ParcelNumber`, `PermitType` (enum), `Status` (enum), `EstimatedCost`, `SquareFootage?`, `ZoningDistrict` (enum?), `ProjectDescription`, `IssuedDate?`, `ExpirationDate?`, `CompletedDate?`, `CreatedBy`, `CreatedDate`, `ModifiedDate` | Status transitions validated via state machine; fee calculation as domain method |
| **Applicant** | `ApplicantId` (int, PK), `Name`, `Email`, `Phone`, `Company?`, `LicenseNumber?`, `CreatedDate` | Email uniqueness for upsert logic |
| **Contractor** | `ContractorId` (int, PK), `CompanyName`, `LicenseNumber`, `InsuranceExpiry?`, `Rating?`, `ContactEmail`, `ContactPhone`, `IsActive` | License number uniqueness |
| **PlanReview** | `ReviewId` (string, PK), `PermitId` (FK), `ReviewerId`, `ReviewType` (enum), `Status` (enum), `Comments?`, `Deficiencies` (List\<string\>), `ReviewDate`, `DueDate?`, `CompletedDate?` | Status cascading to parent permit |
| **Inspection** | `InspectionId` (string, PK), `PermitId` (FK), `InspectorId`, `InspectionType` (enum), `ScheduledDate`, `CompletedDate?`, `Result` (enum?), `Status` (enum), `Comments?`, `Photos?`, `CreatedDate` | No weekend scheduling; auto-assignment; final inspection triggers CO |
| **Fee** | `FeeId` (int, PK), `PermitId` (FK), `FeeType`, `Amount`, `PaidDate?`, `PaymentMethod?`, `TransactionId?`, `CreatedDate` | Calculated from permit type, cost, sqft, zoning |
| **ActivityLogEntry** | `LogId` (int, PK), `Timestamp`, `ActivityType` (enum), `PermitId?` (FK), `Description`, `UserName` | Immutable audit record |

#### Enumerations

```csharp
public enum PermitType
{
    NewConstruction, Addition, Electrical, Plumbing, Mechanical, Demolition
}

public enum PermitStatus
{
    Submitted, UnderReview, Approved, ReviewRejectedResubmitRequired,
    Issued, UnderInspection, InspectionFailedCorrectionsRequired,
    CertificateOfOccupancyIssued, Expired
}

public enum InspectionType
{
    Foundation, Framing, Electrical, Plumbing, Mechanical, Final
}

public enum InspectionResult { Passed, Failed }
public enum InspectionStatus { Scheduled, Completed, Cancelled }

public enum ReviewType
{
    Structural, Electrical, Plumbing, Mechanical, FireSafety, ZoningCompliance
}

public enum ReviewStatus { Pending, InProgress, Approved, ApprovedWithConditions, Rejected }

public enum ZoningDistrict { R1, R2, C1, I1 }

public enum ActivityType
{
    ApplicationSubmitted, InspectionScheduled, InspectionCompleted,
    InspectionCancelled, PlanReviewSubmitted, PermitIssued, PermitExpired
}
```

#### Domain Services

| Service | Responsibility | Replaces |
|---|---|---|
| **PermitFeeCalculator** | Single authoritative fee calculation: base fee + percentage + sqft surcharge + zoning surcharge | Triple-duplicated logic in `PermitDataAccess.CalculatePermitFee()`, `sp_CalculatePermitFee`, and `sp_SubmitPermitApplication` |
| **PermitStatusMachine** | Validates and enforces permit status transitions per the lifecycle diagram | Status transition logic scattered across 3 stored procedures |

**Fee Schedule (consolidated from SP + C# implementations):**

| Permit Type | Base Fee | Percentage | Sqft Surcharge | Zoning Surcharge (C1/I1) |
|---|---|---|---|---|
| NewConstruction | $500.00 | 3.0% | $0.50/sqft | $200.00 |
| Addition | $300.00 | 2.5% | $0.50/sqft | $200.00 |
| Electrical | $150.00 | 1.5% | — | $200.00 |
| Plumbing | $150.00 | 1.5% | — | $200.00 |
| Mechanical | $150.00 | 1.5% | — | $200.00 |
| Demolition | $250.00 | 1.0% | — | $200.00 |

### 2.3 Application Layer — `RiverdalePermitSystem.Application`

Contains application-specific business logic, use case orchestration, and interface definitions. References only the Domain layer.

#### Service Interfaces

| Interface | Methods | Replaces |
|---|---|---|
| **IPermitService** | `GetRecentPermitsAsync(int count)`, `SearchPermitsAsync(PermitSearchCriteria criteria)`, `GetPermitByIdAsync(string permitId)`, `SubmitPermitApplicationAsync(PermitApplicationDto dto)`, `CalculatePermitFeeAsync(PermitType type, decimal cost, int? sqft, ZoningDistrict? zoning)`, `GetDashboardStatisticsAsync()`, `GetRecentActivityAsync(int count)`, `GetPermitsByStatusAsync()` | `PermitDataAccess` (11 static methods) |
| **IPlanReviewService** | `GetPlanReviewHistoryAsync(string permitId)`, `SubmitPlanReviewAsync(PlanReviewSubmissionDto dto)` | `PermitDataAccess.GetPlanReviewHistory()`, `SubmitPlanReview()` |
| **IInspectionService** | `GetUpcomingInspectionsAsync()`, `ScheduleInspectionAsync(InspectionScheduleDto dto)`, `CompleteInspectionAsync(string inspectionId, InspectionResult result, string? comments)`, `CancelInspectionAsync(string inspectionId)`, `GetInspectionHistoryAsync(string permitId)`, `GetInspectorScheduleAsync(string inspectorId, DateRange range)` | `InspectionDataAccess` (6 static methods) |
| **IEmailNotificationService** | `SendPermitConfirmationAsync(...)`, `SendInspectionConfirmationAsync(...)`, `SendReviewCompletedAsync(...)`, `SendPermitIssuedAsync(...)` | `EmailHelper` (4 static methods) |
| **ICurrentUserService** | `GetCurrentUserIdAsync()`, `GetCurrentUserNameAsync()`, `GetCurrentUserRolesAsync()` | `Session["UserId"]`, `Session["UserRole"]` |

#### DTOs (Data Transfer Objects)

| DTO | Purpose | Replaces |
|---|---|---|
| `PermitApplicationDto` | Strongly-typed input for permit submission (with `[Required]`, `[EmailAddress]`, `[Range]` annotations) | 7 loose parameters + `DataTable applicantData` |
| `PermitSearchCriteria` | Search filters + pagination parameters | 6 loose parameters on `SearchPermits()` |
| `PermitSummaryDto` | Lightweight permit record for grids/lists | `DataTable` rows |
| `PermitDetailDto` | Full permit details with applicant info | `DataTable` from `GetPermitById()` |
| `DashboardStatisticsDto` | Dashboard aggregate data (record type) | `DataTable` single-row |
| `ActivityLogDto` | Activity feed entry | `DataTable` rows |
| `StatusSummaryDto` | Status group with count and total value | `DataTable` rows |
| `PlanReviewSubmissionDto` | Plan review input with `List<string>` deficiencies | 6 string parameters + semicolon-delimited deficiencies |
| `PlanReviewHistoryDto` | Review history entry | `DataTable` rows |
| `InspectionScheduleDto` | Inspection scheduling input | 4 loose parameters |
| `InspectionSummaryDto` | Inspection grid entry | `DataTable` rows |

#### Validation

| Validator | Framework | Replaces |
|---|---|---|
| `PermitApplicationValidator` | FluentValidation / Data Annotations | ASP.NET `RequiredFieldValidator`, `RegularExpressionValidator`, `RangeValidator` across 3 wizard steps |
| `InspectionScheduleValidator` | FluentValidation / Data Annotations | `RequiredFieldValidator` (×3) + SP-level permit/weekend validation |
| `PlanReviewSubmissionValidator` | FluentValidation / Data Annotations | No validation (anti-pattern in legacy) |

### 2.4 Infrastructure Layer — `RiverdalePermitSystem.Infrastructure`

Implements interfaces defined in the Application layer. Contains all external dependencies.

#### Components

| Component | Responsibility | Dependencies |
|---|---|---|
| **PermitDbContext** | EF Core DbContext with 7 DbSets, Fluent API configuration, seed data | EF Core 9, Azure SQL |
| **PermitService** | Implements `IPermitService`; EF Core queries + domain service orchestration | `PermitDbContext`, `PermitFeeCalculator` |
| **PlanReviewService** | Implements `IPlanReviewService`; manages reviews with status cascading | `PermitDbContext`, `PermitStatusMachine` |
| **InspectionService** | Implements `IInspectionService`; scheduling with validation + auto-assignment | `PermitDbContext`, `PermitStatusMachine` |
| **AzureEmailNotificationService** | Implements `IEmailNotificationService` via Azure Communication Services | Azure Communication Services SDK |
| **EntraIdCurrentUserService** | Implements `ICurrentUserService` from `AuthenticationStateProvider` claims | Microsoft.Identity.Web |
| **EF Core Configurations** | `IEntityTypeConfiguration<T>` per entity for Fluent API mappings | EF Core 9 |
| **EF Core Migrations** | Database schema versioning and evolution | EF Core Migrations tooling |

### 2.5 Presentation Layer — `RiverdalePermitSystem.Web`

The Blazor Server application host. Contains Razor components, layouts, and the ASP.NET Core startup pipeline.

*(Detailed component mapping in Section 3 below.)*

---

## 3. Migration Map — Pages to Components

### 3.1 Page-to-Component Migration Summary

| # | Legacy Page | Legacy LOC | Blazor Component | Route | Auth Role | Key Changes |
|---|---|---|---|---|---|---|
| 1 | `Default.aspx` / `.cs` | 55 + 42 | `Pages/Home.razor` | `/` | All authenticated | GridView → `QuickGrid`; UpdatePanel → Blazor diffing; `Response.Redirect` → `NavigationManager` |
| 2 | `Pages/PermitApplication.aspx` / `.cs` | 187 + 182 | `Pages/Permits/Apply.razor` | `/permits/apply` | Applicant | MultiView wizard → Stepper component with step state; ViewState → component fields; validators → `EditForm` + `DataAnnotationsValidator`; fee calc → injected `IPermitService` |
| 3 | `Pages/PermitSearch.aspx` / `.cs` | 138 + 65 | `Pages/Permits/Search.razor` | `/permits/search` `/permits/search/{PermitId}` | All authenticated | ObjectDataSource → `@inject IPermitService`; GridView → `QuickGrid` with server-side paging; Session detail → URL parameter; FormView → inline component |
| 4 | `Pages/InspectionSchedule.aspx` / `.cs` | 76 + 78 | `Pages/Inspections/Schedule.razor` | `/inspections` | Inspector, Admin | Add pass/fail selection; `DateTime.Parse` → `DateOnly` binding; add confirmation dialogs; add permit existence validation |
| 5 | `Pages/PlanReview.aspx` / `.cs` | 82 + 86 | `Pages/Reviews/PlanReview.razor` | `/reviews` `/reviews/{PermitId}` | Reviewer, Admin | CheckBoxList → `InputCheckbox` loop with `List<string>`; GUID reviewer → real user identity; add validation group |
| 6 | `Pages/Dashboard.aspx` / `.cs` | 63 + 44 | `Pages/Admin/Dashboard.razor` | `/dashboard` | Admin | Add `[Authorize(Roles = "Admin")]`; add auto-refresh timer; add chart components |

### 3.2 Master Page → Layout Migration

| Legacy Component | Blazor Replacement | File |
|---|---|---|
| `MasterPages/Site.Master` | `Shared/MainLayout.razor` | Defines `<header>`, `<nav>`, `@Body`, `<footer>` |
| Navigation links (hardcoded 6 items) | `Shared/NavMenu.razor` | Role-based `AuthorizeView` visibility; active link highlighting via `NavLink` |
| `ScriptManager` (global AJAX) | *Removed* — Blazor Server SignalR handles UI diffing automatically | N/A |
| `lblUserName` / `lblUserRole` display | `Shared/UserInfo.razor` | Reads from `AuthenticationStateProvider`; shows real name + role |
| `lnkLogout` link button | `Shared/UserInfo.razor` | Redirects to `/MicrosoftIdentity/Account/SignOut` |

### 3.3 User Control → Component Migration

| Legacy Control | Blazor Replacement | File | Changes |
|---|---|---|---|
| `UserControls/AddressLookup.ascx` | `Shared/AddressLookup.razor` | Shared component | Replace hardcoded addresses with Azure Maps API; `EventCallback<string>` replaces property polling; debounced client-side search |
| `UserControls/PermitHeader.ascx` | `Shared/PermitHeader.razor` | Shared component | `[Parameter] Permit permit` replaces 3 string properties; status color coding; CSS class instead of inline styles |

### 3.4 Detailed Migration — Permit Application Wizard

The most complex page (`PermitApplication.aspx`, 369 total LOC) migrates as follows:

**Legacy Pattern → Modern Pattern:**

| Concern | Legacy (Web Forms) | Modern (Blazor) |
|---|---|---|
| **Wizard navigation** | `MultiView` with 4 `View` panels, `ActiveViewIndex` manipulation | `WizardStep` enum + `switch` rendering; or `MudStepper`/custom stepper |
| **Form state** | 13 `ViewState` entries round-tripped per postback | Private C# fields in component (`private PermitApplicationModel _model = new()`) |
| **Validation** | 9 ASP.NET validators across 3 `ValidationGroup`s | `EditForm` + `DataAnnotationsValidator` + `FluentValidation`; validate per step via model subsets |
| **Fee calculation** | `UpdatePanel` + `btnCalculateFee_Click` → static method | `@onclick` → `await PermitService.CalculatePermitFeeAsync()` → bind to field |
| **Address lookup** | `AddressLookup.ascx` with postback | `AddressLookup.razor` with `EventCallback<string>` |
| **Submit** | `Response.Write("<script>alert(...)") `on error | `try/catch` → component error state → rendered error UI / toast notification |
| **Post-submit** | `Session["SubmittedPermitData"]`; show confirmation panel | Navigate to `/permits/search/{permitId}` with success toast; or show inline confirmation |
| **Anti-forgery** | None | Built-in Blazor anti-forgery (automatic in .NET 9) |
| **Idempotency** | None (double-click risk) | Disable submit button on click; idempotency key in service layer |

### 3.5 Detailed Migration — Permit Search

| Concern | Legacy (Web Forms) | Modern (Blazor) |
|---|---|---|
| **Data binding** | `ObjectDataSource` with reflection-based `TypeName` | `@inject IPermitService` + manual `await` calls |
| **Pagination** | GridView `AllowPaging` + `PageIndexChanging` event | `QuickGrid` with `ItemsProvider` for server-side virtual scrolling |
| **Detail view** | `Session["SelectedPermitId"]` → `FormView` + `SessionParameter` | Route parameter `/permits/search/{PermitId}` or dialog/drawer component |
| **Query string** | `Request.QueryString["permitId"]` → manual population | `[SupplyParameterFromQuery]` attribute on component parameter |
| **Report** | Crystal Reports placeholder (JS alert) | PDF export via QuestPDF or server-side report generation |

---

## 4. Data Access Modernization

### 4.1 ADO.NET to EF Core Migration

| Aspect | Legacy (ADO.NET) | Modern (EF Core 9) |
|---|---|---|
| **Connection management** | `ConfigurationManager.ConnectionStrings["PermitDB"]` (never used; simulated data) | `builder.Services.AddDbContext<PermitDbContext>(o => o.UseSqlServer(...))` with connection pooling |
| **Query pattern** | Static methods returning `DataTable` | LINQ queries returning strongly-typed entities/DTOs |
| **Stored procedures** | 5 SPs with embedded business logic | Business logic moved to domain services; SPs replaced by EF Core operations + domain validation |
| **Schema management** | Manual SQL scripts (`Schema.sql`) | EF Core Migrations with `dotnet ef migrations add/update` |
| **Transaction management** | SP-level `BEGIN TRAN` / `COMMIT` / `ROLLBACK` | `IDbContextTransaction` or implicit SaveChanges transactions |
| **Mapping** | None (DataTable magic strings) | Fluent API `IEntityTypeConfiguration<T>` per entity |
| **Concurrency** | None | Optimistic concurrency via `[ConcurrencyCheck]` / `RowVersion` on `Permit` and `Inspection` |

### 4.2 Stored Procedure Migration Strategy

Each stored procedure's business logic is extracted into the appropriate Clean Architecture layer:

#### `sp_SubmitPermitApplication` → Domain + Application Layer

| SP Logic | Target Layer | Implementation |
|---|---|---|
| Applicant upsert by email | **Infrastructure** (`PermitService`) | EF Core `FirstOrDefaultAsync` + `Add`/`Update` |
| Min cost validation ($10K for NewConstruction) | **Application** (`PermitApplicationValidator`) | FluentValidation rule |
| Fee calculation (base + % + sqft + zoning) | **Domain** (`PermitFeeCalculator`) | Pure method; single source of truth |
| Activity log entry | **Infrastructure** (`PermitService`) | EF Core `ActivityLog.Add()` |
| Permit ID generation (`PERM-YYYY-NNNN`) | **Domain** (`Permit` factory method) | `Permit.Create()` or `IPermitIdGenerator` |

#### `sp_CalculatePermitFee` → Domain Layer

| SP Logic | Target Layer | Implementation |
|---|---|---|
| Base fee by permit type | **Domain** (`PermitFeeCalculator`) | Lookup table / switch expression |
| Percentage fee by type | **Domain** (`PermitFeeCalculator`) | Multiplier table |
| Square footage surcharge | **Domain** (`PermitFeeCalculator`) | Conditional surcharge |
| Zoning district surcharge | **Domain** (`PermitFeeCalculator`) | Conditional surcharge |

#### `sp_ScheduleInspection` → Application + Domain Layer

| SP Logic | Target Layer | Implementation |
|---|---|---|
| Permit exists + status check | **Application** (`InspectionScheduleValidator`) | FluentValidation async rule with DB lookup |
| No weekend scheduling | **Domain** (`Inspection` entity / validator) | `DayOfWeek` check |
| Auto-assign inspector (least busy) | **Infrastructure** (`InspectionService`) | EF Core query: `GROUP BY InspectorId, COUNT(*)` |
| Update permit status to "Under Inspection" | **Domain** (`PermitStatusMachine`) | State transition method |
| Activity log | **Infrastructure** (`InspectionService`) | EF Core `ActivityLog.Add()` |

#### `sp_CompleteInspection` → Domain + Application Layer

| SP Logic | Target Layer | Implementation |
|---|---|---|
| Final + Passed → CO Issued | **Domain** (`PermitStatusMachine`) | State machine transition |
| Passed + non-Final → Issued | **Domain** (`PermitStatusMachine`) | State machine transition |
| Failed → Corrections Required | **Domain** (`PermitStatusMachine`) | State machine transition |
| Activity log | **Infrastructure** (`InspectionService`) | EF Core `ActivityLog.Add()` |

#### `sp_SubmitPlanReview` → Domain + Application Layer

| SP Logic | Target Layer | Implementation |
|---|---|---|
| All reviews approved → Permit Approved | **Domain** (`PermitStatusMachine`) | State transition check all related reviews |
| Any rejected → Review Rejected | **Domain** (`PermitStatusMachine`) | State transition |
| Activity log | **Infrastructure** (`PlanReviewService`) | EF Core `ActivityLog.Add()` |

### 4.3 EF Core Entity Configuration Examples

```csharp
// Permit entity configuration
public class PermitConfiguration : IEntityTypeConfiguration<Permit>
{
    public void Configure(EntityTypeBuilder<Permit> builder)
    {
        builder.HasKey(p => p.PermitId);
        builder.Property(p => p.PermitId).HasMaxLength(50);
        builder.Property(p => p.PropertyAddress).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ParcelNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PermitType).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.EstimatedCost).HasColumnType("decimal(18,2)");
        builder.Property(p => p.ZoningDistrict).HasConversion<string>().HasMaxLength(10);
        builder.Property(p => p.ProjectDescription).HasMaxLength(4000);

        builder.HasOne(p => p.Applicant).WithMany(a => a.Permits).HasForeignKey(p => p.ApplicantId);
        builder.HasOne(p => p.Contractor).WithMany(c => c.Permits).HasForeignKey(p => p.ContractorId);

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ApplicationDate);
        builder.HasIndex(p => p.PropertyAddress);

        builder.Property(p => p.RowVersion).IsRowVersion();
    }
}
```

### 4.4 DataTable to DTO Migration Map

| Legacy Return Type | Legacy Columns | Modern DTO | Modern Properties |
|---|---|---|---|
| `DataTable` from `GetRecentPermits` | PermitId, ApplicationDate, PropertyAddress, PermitType, Status, EstimatedCost | `PermitSummaryDto` | `string PermitId`, `DateTime ApplicationDate`, `string PropertyAddress`, `PermitType Type`, `PermitStatus Status`, `decimal EstimatedCost` |
| `DataTable` from `GetPermitById` | + ParcelNumber, ApplicantName | `PermitDetailDto` | All summary fields + `string ParcelNumber`, `string ApplicantName`, `string? ApplicantEmail`, navigation properties |
| `DataTable` from `GetDashboardStatistics` | TotalPermits, PendingReview, InspectionsToday, MonthlyRevenue | `DashboardStatisticsDto` | `int TotalPermits`, `int PendingReview`, `int InspectionsToday`, `decimal MonthlyRevenue` |
| `DataTable` from `GetRecentActivity` | Timestamp, ActivityType, PermitId, Description, UserName | `ActivityLogDto` | `DateTime Timestamp`, `ActivityType Type`, `string? PermitId`, `string Description`, `string UserName` |
| `DataTable` from `GetPermitsByStatus` | Status, Count, TotalValue | `StatusSummaryDto` | `PermitStatus Status`, `int Count`, `decimal TotalValue` |
| `DataTable` from `GetUpcomingInspections` | InspectionId, PermitId, InspectionType, ScheduledDate, Status, InspectorName | `InspectionSummaryDto` | Strongly-typed with enums |
| `DataTable` from `GetPlanReviewHistory` | ReviewDate, ReviewType, ReviewerName, Status, Comments | `PlanReviewHistoryDto` | Strongly-typed with enums |

---

## 5. Project Structure

### 5.1 Solution Layout

```
RiverdalePermitSystem.sln
│
├── src/
│   ├── RiverdalePermitSystem.Domain/              # Domain Layer (innermost)
│   │   ├── Entities/
│   │   │   ├── Permit.cs
│   │   │   ├── Applicant.cs
│   │   │   ├── Contractor.cs
│   │   │   ├── PlanReview.cs
│   │   │   ├── Inspection.cs
│   │   │   ├── Fee.cs
│   │   │   └── ActivityLogEntry.cs
│   │   ├── Enums/
│   │   │   ├── PermitType.cs
│   │   │   ├── PermitStatus.cs
│   │   │   ├── InspectionType.cs
│   │   │   ├── InspectionResult.cs
│   │   │   ├── InspectionStatus.cs
│   │   │   ├── ReviewType.cs
│   │   │   ├── ReviewStatus.cs
│   │   │   ├── ZoningDistrict.cs
│   │   │   └── ActivityType.cs
│   │   ├── Services/
│   │   │   ├── PermitFeeCalculator.cs
│   │   │   └── PermitStatusMachine.cs
│   │   ├── ValueObjects/
│   │   │   └── DateRange.cs
│   │   └── RiverdalePermitSystem.Domain.csproj
│   │
│   ├── RiverdalePermitSystem.Application/         # Application Layer
│   │   ├── Interfaces/
│   │   │   ├── IPermitService.cs
│   │   │   ├── IPlanReviewService.cs
│   │   │   ├── IInspectionService.cs
│   │   │   ├── IEmailNotificationService.cs
│   │   │   └── ICurrentUserService.cs
│   │   ├── DTOs/
│   │   │   ├── Permits/
│   │   │   │   ├── PermitApplicationDto.cs
│   │   │   │   ├── PermitSearchCriteria.cs
│   │   │   │   ├── PermitSummaryDto.cs
│   │   │   │   ├── PermitDetailDto.cs
│   │   │   │   ├── DashboardStatisticsDto.cs
│   │   │   │   ├── ActivityLogDto.cs
│   │   │   │   └── StatusSummaryDto.cs
│   │   │   ├── Reviews/
│   │   │   │   ├── PlanReviewSubmissionDto.cs
│   │   │   │   └── PlanReviewHistoryDto.cs
│   │   │   └── Inspections/
│   │   │       ├── InspectionScheduleDto.cs
│   │   │       └── InspectionSummaryDto.cs
│   │   ├── Validators/
│   │   │   ├── PermitApplicationValidator.cs
│   │   │   ├── InspectionScheduleValidator.cs
│   │   │   └── PlanReviewSubmissionValidator.cs
│   │   ├── Exceptions/
│   │   │   ├── PermitNotFoundException.cs
│   │   │   ├── InvalidStatusTransitionException.cs
│   │   │   └── DuplicateReviewException.cs
│   │   └── RiverdalePermitSystem.Application.csproj
│   │
│   ├── RiverdalePermitSystem.Infrastructure/      # Infrastructure Layer
│   │   ├── Data/
│   │   │   ├── PermitDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── PermitConfiguration.cs
│   │   │   │   ├── ApplicantConfiguration.cs
│   │   │   │   ├── ContractorConfiguration.cs
│   │   │   │   ├── PlanReviewConfiguration.cs
│   │   │   │   ├── InspectionConfiguration.cs
│   │   │   │   ├── FeeConfiguration.cs
│   │   │   │   └── ActivityLogEntryConfiguration.cs
│   │   │   └── Migrations/
│   │   │       └── (EF Core auto-generated migrations)
│   │   ├── Services/
│   │   │   ├── PermitService.cs
│   │   │   ├── PlanReviewService.cs
│   │   │   ├── InspectionService.cs
│   │   │   ├── AzureEmailNotificationService.cs
│   │   │   └── EntraIdCurrentUserService.cs
│   │   ├── DependencyInjection.cs                 # IServiceCollection extensions
│   │   └── RiverdalePermitSystem.Infrastructure.csproj
│   │
│   └── RiverdalePermitSystem.Web/                 # Presentation Layer (Blazor Server)
│       ├── Components/
│       │   ├── Pages/
│       │   │   ├── Home.razor                     # ← Default.aspx
│       │   │   ├── Permits/
│       │   │   │   ├── Apply.razor                # ← PermitApplication.aspx
│       │   │   │   └── Search.razor               # ← PermitSearch.aspx
│       │   │   ├── Inspections/
│       │   │   │   └── Schedule.razor             # ← InspectionSchedule.aspx
│       │   │   ├── Reviews/
│       │   │   │   └── PlanReview.razor           # ← PlanReview.aspx
│       │   │   └── Admin/
│       │   │       └── Dashboard.razor            # ← Dashboard.aspx
│       │   ├── Layout/
│       │   │   ├── MainLayout.razor               # ← Site.Master
│       │   │   └── NavMenu.razor                  # ← Navigation from Site.Master
│       │   └── Shared/
│       │       ├── AddressLookup.razor            # ← AddressLookup.ascx
│       │       ├── PermitHeader.razor             # ← PermitHeader.ascx
│       │       ├── UserInfo.razor                 # User display + logout
│       │       ├── ConfirmDialog.razor            # Confirmation modal
│       │       ├── LoadingSpinner.razor            # Loading indicator
│       │       └── StatusBadge.razor              # Color-coded status display
│       ├── wwwroot/
│       │   ├── css/
│       │   │   └── site.css                       # ← Styles/Site.css (modernized)
│       │   └── favicon.ico
│       ├── Program.cs                             # App startup, DI, middleware
│       ├── appsettings.json                       # Non-secret configuration
│       ├── appsettings.Development.json           # Dev overrides
│       └── RiverdalePermitSystem.Web.csproj
│
├── tests/
│   ├── RiverdalePermitSystem.Domain.Tests/        # Domain unit tests
│   │   ├── PermitFeeCalculatorTests.cs
│   │   ├── PermitStatusMachineTests.cs
│   │   └── EntityTests/
│   ├── RiverdalePermitSystem.Application.Tests/   # Application layer tests
│   │   ├── Validators/
│   │   └── DTOMappingTests.cs
│   ├── RiverdalePermitSystem.Infrastructure.Tests/ # Integration tests
│   │   ├── Services/
│   │   └── Data/
│   └── RiverdalePermitSystem.Web.Tests/           # Blazor component tests (bUnit)
│       ├── Pages/
│       └── Shared/
│
├── infra/                                          # Infrastructure-as-Code
│   ├── main.bicep                                 # Azure resource definitions
│   ├── parameters.dev.json
│   ├── parameters.staging.json
│   └── parameters.prod.json
│
└── .github/
    └── workflows/
        ├── ci.yml                                 # Build + test on PR
        └── cd.yml                                 # Deploy to Azure App Service
```

### 5.2 Project Dependencies (Dependency Rule)

```
RiverdalePermitSystem.Web
  ├── references → RiverdalePermitSystem.Application
  ├── references → RiverdalePermitSystem.Infrastructure (for DI registration only)
  └── references → RiverdalePermitSystem.Domain (transitively)

RiverdalePermitSystem.Infrastructure
  ├── references → RiverdalePermitSystem.Application
  └── references → RiverdalePermitSystem.Domain (transitively)

RiverdalePermitSystem.Application
  └── references → RiverdalePermitSystem.Domain

RiverdalePermitSystem.Domain
  └── references → (nothing — zero dependencies)
```

### 5.3 NuGet Packages

| Project | Package | Purpose |
|---|---|---|
| **Domain** | *(none)* | Pure C# — no external dependencies |
| **Application** | `FluentValidation` | DTO validation with rich rules |
| **Infrastructure** | `Microsoft.EntityFrameworkCore.SqlServer` | Azure SQL provider |
| | `Microsoft.EntityFrameworkCore.Tools` | Migrations tooling |
| | `Azure.Communication.Email` | Azure Communication Services |
| | `Microsoft.Identity.Web` | Entra ID authentication |
| **Web** | `Microsoft.AspNetCore.Components.QuickGrid` | High-perf Blazor data grid |
| | `Microsoft.Identity.Web.UI` | Auth UI components |
| | `Serilog.AspNetCore` | Structured logging |
| | `Serilog.Sinks.ApplicationInsights` | Log sink to App Insights |
| **Tests** | `xunit`, `FluentAssertions`, `Moq` | Unit testing |
| | `bunit` | Blazor component testing |
| | `Microsoft.EntityFrameworkCore.InMemory` | In-memory DB for tests |
| | `Microsoft.Playwright` | End-to-end browser testing |

---

## 6. Azure Services Integration

### 6.1 Azure Architecture Diagram

```
                           ┌─────────────────────────┐
                           │   Microsoft Entra ID     │
                           │   (Authentication)       │
                           └────────────┬────────────┘
                                        │ OpenID Connect
                                        ▼
┌──────────────────┐      ┌──────────────────────────────────────┐
│   Users           │─────►│        Azure App Service              │
│   (Browsers)      │ HTTPS│   Plan: Standard S1 (or Premium P1)  │
└──────────────────┘      │                                       │
                          │  ┌─────────────────────────────────┐  │
                          │  │  Blazor Server App (.NET 9)     │  │
                          │  │  + Managed Identity (System)    │  │
                          │  └──┬──────┬──────┬──────┬────────┘  │
                          │     │      │      │      │            │
                          └─────┼──────┼──────┼──────┼────────────┘
                                │      │      │      │
            ┌───────────────────┘      │      │      └──────────────────┐
            ▼                          ▼      ▼                         ▼
   ┌─────────────────┐   ┌──────────────┐ ┌──────────────┐   ┌──────────────┐
   │  Azure SQL       │   │ Azure Cache  │ │ Azure Key    │   │ Azure Comm.  │
   │  Database        │   │ for Redis    │ │ Vault        │   │ Services     │
   │                  │   │              │ │              │   │ (Email)      │
   │  Tier: S1 (20DTU)│   │ Basic C0     │ │ Standard     │   │              │
   │  + Geo-Replication│  │              │ │              │   │              │
   └─────────────────┘   └──────────────┘ └──────────────┘   └──────────────┘
            │
            ▼
   ┌─────────────────┐   ┌──────────────────────────┐
   │ Application      │   │ GitHub Actions            │
   │ Insights         │   │ (CI/CD Pipeline)          │
   │ + Log Analytics  │   │                           │
   │ Workspace        │   │ Build → Test → Deploy     │
   └─────────────────┘   └──────────────────────────┘
```

### 6.2 Azure Resource Inventory

| Azure Service | SKU/Tier | Purpose | Replaces |
|---|---|---|---|
| **Azure App Service** | Standard S1 (scale to P1v3 for production) | Host Blazor Server app; deployment slots (staging/production); managed TLS certificates | IIS Express / IIS on-premises |
| **Azure SQL Database** | Standard S1 (20 DTU) | Relational database; automatic backups (7-day retention); geo-replication for DR | SQL Server LocalDB (file-attached `.mdf`) |
| **Azure Cache for Redis** | Basic C0 | Distributed session cache; output caching; data cache for dashboard stats | SQL Server session state (`sessionState mode="SQLServer"`) |
| **Azure Key Vault** | Standard | Store connection strings, email credentials, API keys; accessed via Managed Identity | `Web.config` plaintext settings |
| **Microsoft Entra ID** | Included with M365 | Enterprise SSO, MFA, RBAC, conditional access policies | Simulated auth (`Session["UserId"] = Guid.NewGuid()`) |
| **Azure Communication Services** | Pay-as-you-go | Transactional email delivery with tracking and templates | `System.Net.Mail.SmtpClient` (simulated) |
| **Application Insights** | Pay-as-you-go | APM, distributed tracing, live metrics, alerting, availability tests | `Debug.WriteLine` + no logging |
| **Log Analytics Workspace** | Pay-as-you-go | Centralized log storage; KQL queries; dashboard integration | No persistent logging |
| **Azure Monitor** | Included | Alerts on App Service metrics (CPU, memory, response time, 5xx errors) | No monitoring |

### 6.3 Managed Identity Integration

All Azure service connections use **System-assigned Managed Identity** — zero credentials in code or config:

| Service | Connection Method | Configuration |
|---|---|---|
| Azure SQL Database | `Authentication=Active Directory Managed Identity` in connection string | Grant `db_datareader`, `db_datawriter` roles to App Service identity |
| Azure Key Vault | `Azure.Identity.DefaultAzureCredential` | Grant `Key Vault Secrets User` role to App Service identity |
| Azure Cache for Redis | Microsoft Entra ID authentication | Grant `Redis Cache Contributor` role |
| Azure Communication Services | Managed Identity via `Azure.Communication.Email` SDK | Connection string from Key Vault |
| Application Insights | Connection string via environment variable | `APPLICATIONINSIGHTS_CONNECTION_STRING` in App Service config |

### 6.4 Deployment Pipeline (GitHub Actions)

```yaml
# .github/workflows/cd.yml (simplified)
name: Build and Deploy

on:
  push:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal

  deploy-staging:
    needs: build-and-test
    runs-on: ubuntu-latest
    environment: staging
    steps:
      - run: dotnet publish src/RiverdalePermitSystem.Web -c Release -o publish
      - uses: azure/webapps-deploy@v3
        with:
          app-name: riverdale-permits
          slot-name: staging
          package: publish

  deploy-production:
    needs: deploy-staging
    runs-on: ubuntu-latest
    environment: production  # requires manual approval
    steps:
      - uses: azure/cli@v2
        with:
          inlineScript: |
            az webapp deployment slot swap \
              --name riverdale-permits \
              --resource-group rg-riverdale \
              --slot staging --target-slot production
```

---

## 7. Cross-Cutting Concerns

### 7.1 Authentication & Authorization

#### Authentication Architecture

| Aspect | Legacy | Modern |
|---|---|---|
| **Provider** | Windows Auth (unenforced) + random GUID session | **Microsoft Entra ID** via OpenID Connect |
| **Protocol** | Session cookies with GUID | OAuth 2.0 / OIDC tokens + cookie auth for Blazor Server |
| **Identity** | `Session["UserId"] = Guid.NewGuid()` | Entra ID claims: `oid`, `preferred_username`, `name`, `roles` |
| **MFA** | None | Entra ID Conditional Access policies |
| **Session** | SQL Server session state | Blazor Server circuit + Redis distributed cache |

#### Role-Based Access Control (RBAC)

| Role | Entra ID Group | Accessible Pages | Legacy Equivalent |
|---|---|---|---|
| **Applicant** | `sg-permits-applicants` | Home, Apply for Permit, Search Permits | All users (default `"Applicant"`) |
| **Inspector** | `sg-permits-inspectors` | Home, Search, Inspection Schedule, Plan Review | No distinction |
| **Admin** | `sg-permits-admins` | All pages including Dashboard | No distinction |

**Blazor Authorization Implementation:**

```csharp
// Program.cs — configure auth
builder.Services.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("InspectorOrAdmin", policy => policy.RequireRole("Inspector", "Admin"));
});

// Dashboard.razor — page-level authorization
@page "/dashboard"
@attribute [Authorize(Policy = "AdminOnly")]

// NavMenu.razor — conditional navigation
<AuthorizeView Policy="AdminOnly">
    <NavLink href="/dashboard">Dashboard</NavLink>
</AuthorizeView>
```

### 7.2 Logging & Observability

#### Logging Architecture

| Aspect | Legacy | Modern |
|---|---|---|
| **Framework** | `Debug.WriteLine` (C# layer); `INSERT INTO ActivityLog` (SQL layer) | **Serilog** with structured logging |
| **Sinks** | Debug output window (non-persistent) | Application Insights, Console (dev), Azure Log Analytics |
| **Audit trail** | `ActivityLog` table via stored procedures | `ActivityLog` table via EF Core (preserved) + Application Insights custom events |
| **Error tracking** | `Application_Error` → `Debug.WriteLine` | Global exception handler middleware + Application Insights exceptions |
| **Performance** | None | Application Insights request telemetry, dependency tracking, custom metrics |

**Logging Configuration:**

```csharp
// Program.cs
builder.Host.UseSerilog((context, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "RiverdalePermitSystem")
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(TelemetryConverter.Traces));
```

**Structured Log Examples:**

```csharp
// In PermitService
_logger.LogInformation("Permit submitted: {PermitId} by {ApplicantEmail} for {PermitType}",
    permit.PermitId, dto.Email, dto.PermitType);

// In InspectionService
_logger.LogWarning("Weekend inspection scheduling attempted for {PermitId} on {Date}",
    dto.PermitId, dto.RequestedDate);
```

### 7.3 Dependency Injection

#### DI Registration Strategy

All services are registered in `Infrastructure/DependencyInjection.cs` and called from `Program.cs`:

```csharp
// Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core
        services.AddDbContext<PermitDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PermitDB")));

        // Application services (scoped — per-request/circuit)
        services.AddScoped<IPermitService, PermitService>();
        services.AddScoped<IPlanReviewService, PlanReviewService>();
        services.AddScoped<IInspectionService, InspectionService>();
        services.AddScoped<ICurrentUserService, EntraIdCurrentUserService>();

        // Email (singleton — thread-safe SDK client)
        services.AddSingleton<IEmailNotificationService, AzureEmailNotificationService>();

        // Domain services (transient — stateless)
        services.AddTransient<PermitFeeCalculator>();
        services.AddTransient<PermitStatusMachine>();

        // Redis distributed cache
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        return services;
    }
}

// Program.cs
builder.Services.AddInfrastructure(builder.Configuration);
```

#### DI Lifetime Strategy

| Lifetime | Services | Rationale |
|---|---|---|
| **Scoped** | `PermitService`, `PlanReviewService`, `InspectionService`, `CurrentUserService`, `PermitDbContext` | One instance per Blazor circuit/request; matches EF Core DbContext lifetime |
| **Singleton** | `AzureEmailNotificationService` | Thread-safe SDK client; one connection pool |
| **Transient** | `PermitFeeCalculator`, `PermitStatusMachine` | Stateless domain services; lightweight creation |

### 7.4 Configuration Management

#### Configuration Sources (Precedence Order)

| Source | Environment | Contents |
|---|---|---|
| `appsettings.json` | All | Non-secret defaults, feature flags, logging levels |
| `appsettings.{Environment}.json` | Per-environment | Environment-specific overrides (connection timeout, cache TTL) |
| **Azure Key Vault** | Staging/Production | Connection strings, API keys, email credentials |
| **Environment Variables** | Azure App Service | App Insights connection string, ASPNETCORE_ENVIRONMENT |
| **User Secrets** | Development only | Local dev connection strings |

**Configuration Structure:**

```json
// appsettings.json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "<from-key-vault>",
    "ClientId": "<from-key-vault>",
    "CallbackPath": "/signin-oidc"
  },
  "ConnectionStrings": {
    "PermitDB": "<from-key-vault-or-env>"
  },
  "Email": {
    "SenderAddress": "permits@riverdalecity.gov",
    "SenderDisplayName": "Riverdale Permits"
  },
  "Dashboard": {
    "ActivityFeedCount": 20,
    "AutoRefreshIntervalSeconds": 60
  },
  "Permits": {
    "RecentPermitsCount": 10,
    "SearchPageSize": 20
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

**Key Vault Integration:**

```csharp
// Program.cs
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri(builder.Configuration["KeyVault:Uri"]!);
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}
```

### 7.5 Error Handling

#### Error Handling Strategy

| Layer | Legacy Pattern | Modern Pattern |
|---|---|---|
| **Global** | `Application_Error` → `Debug.WriteLine` | ASP.NET Core exception middleware + Application Insights |
| **Page-level** | `Response.Write("<script>alert('{ex.Message}')</script>")` (XSS) | Blazor `ErrorBoundary` component with custom fallback UI |
| **Form-level** | Label with CSS class change | Component state + rendered error messages / toast notifications |
| **Data access** | No try/catch (unhandled) | Service-level try/catch → custom exceptions → component error state |
| **Validation** | ASP.NET validators (client-side) | `EditForm` + `DataAnnotationsValidator` + `ValidationSummary` (server-side) |

**Blazor Error Boundary Pattern:**

```razor
@* MainLayout.razor *@
<ErrorBoundary @ref="_errorBoundary">
    <ChildContent>
        @Body
    </ChildContent>
    <ErrorContent Context="ex">
        <div class="alert alert-danger">
            <h4>An error occurred</h4>
            <p>We're sorry, something went wrong. Please try again.</p>
            <button @onclick="() => _errorBoundary?.Recover()">Try Again</button>
        </div>
    </ErrorContent>
</ErrorBoundary>
```

### 7.6 Anti-Forgery & Security

| Concern | Legacy | Modern |
|---|---|---|
| **CSRF** | None | Blazor Server built-in anti-forgery (automatic in .NET 9) |
| **XSS** | 3 `Response.Write` vectors with `ex.Message` | Blazor auto-encodes all rendered output; no raw HTML injection |
| **Input sanitization** | None | Model binding + validation; EF Core parameterized queries |
| **HTTPS** | HTTP on port 58745 | Azure App Service enforced HTTPS + HSTS |
| **Headers** | None | CSP, X-Frame-Options, X-Content-Type-Options via middleware |
| **Secrets** | Plaintext in `Web.config` | Azure Key Vault with Managed Identity |

**Security Middleware Configuration:**

```csharp
// Program.cs
app.UseHsts();
app.UseHttpsRedirection();
app.UseSecurityHeaders(policies =>
{
    policies.AddContentSecurityPolicy(csp =>
    {
        csp.AddDefaultSrc().Self();
        csp.AddScriptSrc().Self().UnsafeInline(); // Required for Blazor
        csp.AddStyleSrc().Self().UnsafeInline();
        csp.AddConnectSrc().Self(); // SignalR WebSocket
    });
    policies.AddFrameOptionsDeny();
    policies.AddXContentTypeOptionsNoSniff();
    policies.AddReferrerPolicyStrictOriginWhenCrossOrigin();
});
```

### 7.7 Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("PermitDB")!, name: "azure-sql")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis")
    .AddAzureKeyVault(new Uri(builder.Configuration["KeyVault:Uri"]!),
        new DefaultAzureCredential(), name: "key-vault");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

### 7.8 Testing Strategy

| Test Type | Framework | Scope | Target Coverage |
|---|---|---|---|
| **Domain Unit Tests** | xUnit + FluentAssertions | Fee calculation, status machine, entity validation | 95%+ |
| **Application Unit Tests** | xUnit + Moq | Validators, DTO mapping, service orchestration | 90%+ |
| **Infrastructure Integration Tests** | xUnit + EF Core InMemory/TestContainers | Service implementations, DB queries, email integration | 80%+ |
| **Blazor Component Tests** | bUnit | Component rendering, user interaction, service injection | 80%+ |
| **End-to-End Tests** | Playwright | Full user workflows (apply, search, review, inspect) | Critical paths |

---

*This modern architecture design was generated as part of the Spec2Cloud modernization methodology. It provides the target architecture blueprint for migrating the Riverdale City Building Permit System from ASP.NET Web Forms / .NET Framework 4.8 to .NET 9 / Blazor Server / EF Core / Azure. Proceed to Step 6: Implementation.*
