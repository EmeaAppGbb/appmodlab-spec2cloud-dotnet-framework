# Step 3: Architecture Specification — Riverdale City Building Permit System

> **Generated:** 2026-04-16 | **Methodology:** Spec2Cloud Architecture Specification
> **Application:** Riverdale City Building Permit System
> **Current Platform:** ASP.NET Web Forms / .NET Framework 4.8
> **Input:** Step 2 — Spec2Cloud Analysis Report

---

## Table of Contents

1. [System Context](#1-system-context)
2. [Current Architecture](#2-current-architecture)
3. [Architecture Constraints](#3-architecture-constraints)
4. [Quality Attributes](#4-quality-attributes)
5. [Component Inventory](#5-component-inventory)
6. [Data Architecture](#6-data-architecture)
7. [Integration Points](#7-integration-points)
8. [Cross-Cutting Concerns](#8-cross-cutting-concerns)

---

## 1. System Context

### 1.1 System Purpose

The Riverdale City Building Permit System is a **municipal web application** that manages the full lifecycle of building permits for the City of Riverdale. It supports permit application submission, plan review, inspection scheduling, fee calculation, and administrative dashboards.

### 1.2 Users & Roles

| Actor | Role | Key Activities |
|---|---|---|
| **Public Applicants** | Property owners, homeowners, or their agents who submit permit applications | Submit permit applications via 4-step wizard, track permit status, receive email notifications |
| **Plan Review Inspectors** | City staff who review submitted building plans for code compliance | Load permits, submit structural/electrical/mechanical reviews, record deficiencies, approve or reject plans |
| **Building Inspectors** | City staff who perform on-site inspections | Schedule inspections, complete inspections with pass/fail results, cancel inspections |
| **Administrative Users** | City department managers and clerks | View dashboard statistics (total permits, pending reviews, daily inspections, monthly revenue), monitor recent activity, view permit status summaries |

> **Note:** The current system does **not** enforce role-based access control. All users are assigned the `"Applicant"` role via `Session["UserRole"]` at session start (`Global.asax.cs` line 22), and all pages are accessible to all users (`<allow users="*" />` in `Web.config` line 18).

### 1.3 System Context Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           External Actors                               │
│                                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐                 │
│  │   Public      │  │   Plan       │  │  Building    │                 │
│  │  Applicants   │  │  Reviewers   │  │  Inspectors  │                 │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘                 │
│         │                  │                  │                         │
│         └──────────────────┼──────────────────┘                         │
│                            │ HTTP (Port 58745)                          │
│                            ▼                                            │
│              ┌─────────────────────────────┐                            │
│              │  Riverdale Building Permit  │                            │
│              │        System               │                            │
│              │  (ASP.NET Web Forms / IIS)  │                            │
│              └──────┬──────────┬───────────┘                            │
│                     │          │                                         │
│           ┌─────────▼──┐  ┌───▼──────────────┐                         │
│           │ SQL Server  │  │ SMTP Server       │                        │
│           │ (LocalDB)   │  │ smtp.riverdale     │                       │
│           │             │  │   city.gov:25      │                       │
│           └─────────────┘  └──────────────────┘                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 1.4 Key Business Processes

| Process | Flow |
|---|---|
| **Permit Application** | Applicant → 4-step wizard (property → applicant → project → review) → Submit → Fee calculation → Email confirmation |
| **Plan Review** | Reviewer loads permit → Selects review type → Records status/comments/deficiencies → Permit status updated based on all reviews |
| **Inspection Scheduling** | Inspector enters permit ID → Selects type/date → System validates permit status → Auto-assigns inspector → Sends confirmation |
| **Inspection Completion** | Inspector records result → System updates permit status → Final inspection pass → Certificate of Occupancy issued |
| **Permit Search** | User enters search criteria → Paged results displayed → Click for detail view |
| **Dashboard Monitoring** | Admin views aggregate statistics, recent activity feed, and permit status breakdown |

---

## 2. Current Architecture

### 2.1 Architecture Style

The system follows a **monolithic two-tier architecture** implemented as an ASP.NET Web Forms application:

- **Tier 1 — Presentation + Business Logic:** A single IIS-hosted web application containing `.aspx` pages (markup + code-behind), user controls (`.ascx`), static data access classes, and a static email helper — all compiled into a single assembly.
- **Tier 2 — Database:** SQL Server (LocalDB) with stored procedures containing embedded business logic.

There is **no explicit service layer, domain model, or dependency injection**. Business logic is split between C# code-behind, static `App_Code` classes, and T-SQL stored procedures.

### 2.2 Architecture Diagram

```
┌────────────────────────────────────────────────────────────────────────┐
│                        IIS Express / IIS                               │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │              ASP.NET Web Forms Application (.NET 4.8)            │  │
│  │                                                                  │  │
│  │  ┌─── Presentation Tier ──────────────────────────────────────┐  │  │
│  │  │                                                            │  │  │
│  │  │  Site.Master (layout, navigation, session display)         │  │  │
│  │  │    ├── Default.aspx           (Home / recent permits)      │  │  │
│  │  │    ├── PermitApplication.aspx (4-step wizard, ViewState)   │  │  │
│  │  │    ├── PermitSearch.aspx      (search, ObjectDataSource)   │  │  │
│  │  │    ├── InspectionSchedule.aspx (schedule + grid)           │  │  │
│  │  │    ├── PlanReview.aspx        (review form + history)      │  │  │
│  │  │    └── Dashboard.aspx         (stats + activity feed)      │  │  │
│  │  │                                                            │  │  │
│  │  │  User Controls:                                            │  │  │
│  │  │    ├── AddressLookup.ascx  (simulated address autocomplete)│  │  │
│  │  │    └── PermitHeader.ascx   (read-only permit summary)      │  │  │
│  │  └────────────────────────────────────────────────────────────┘  │  │
│  │                          │                                       │  │
│  │                          │ Static method calls (no DI)           │  │
│  │                          ▼                                       │  │
│  │  ┌─── Business Logic / Data Access (App_Code) ────────────────┐  │  │
│  │  │                                                            │  │  │
│  │  │  PermitDataAccess (static)     — 11 methods                │  │  │
│  │  │    • GetRecentPermits, SearchPermits, GetPermitById        │  │  │
│  │  │    • SubmitPermitApplication, CalculatePermitFee           │  │  │
│  │  │    • GetDashboardStatistics, GetRecentActivity             │  │  │
│  │  │    • GetPermitsByStatus, GetPlanReviewHistory              │  │  │
│  │  │    • SubmitPlanReview                                      │  │  │
│  │  │                                                            │  │  │
│  │  │  InspectionDataAccess (static) — 6 methods                 │  │  │
│  │  │    • GetUpcomingInspections, ScheduleInspection            │  │  │
│  │  │    • CompleteInspection, CancelInspection                  │  │  │
│  │  │    • GetInspectionHistory, GetInspectorSchedule            │  │  │
│  │  │                                                            │  │  │
│  │  │  EmailHelper (static)          — 4 methods                 │  │  │
│  │  │    • SendPermitConfirmation, SendInspectionConfirmation    │  │  │
│  │  │    • SendReviewCompletedNotification                       │  │  │
│  │  │    • SendPermitIssuedNotification                          │  │  │
│  │  └────────────────────────────────────────────────────────────┘  │  │
│  │                          │                                       │  │
│  │                          │ ADO.NET / ConfigurationManager        │  │
│  │                          ▼                                       │  │
│  │  ┌─── Configuration ─────────────────────────────────────────┐  │  │
│  │  │  Web.config                                               │  │  │
│  │  │    • Connection string: PermitDB (LocalDB .mdf)           │  │  │
│  │  │    • SMTP settings: smtp.riverdalecity.gov:25             │  │  │
│  │  │    • Session state: SQL Server mode                       │  │  │
│  │  │    • Auth: Windows Authentication, allow all              │  │  │
│  │  │    • ViewState: Enabled globally, encryption Always       │  │  │
│  │  └──────────────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘
                          │
                          │ TCP / Named Pipes
                          ▼
┌────────────────────────────────────────────────────────────────────────┐
│                    SQL Server (LocalDB)                                 │
│                                                                        │
│  Database: RiverdalePermitDB                                           │
│                                                                        │
│  Tables (7):                                                           │
│    Applicants, Contractors, Permits, PlanReviews,                      │
│    Inspections, Fees, ActivityLog                                      │
│                                                                        │
│  Stored Procedures (5):                                                │
│    sp_SubmitPermitApplication, sp_CalculatePermitFee,                  │
│    sp_ScheduleInspection, sp_CompleteInspection,                       │
│    sp_SubmitPlanReview                                                 │
└────────────────────────────────────────────────────────────────────────┘
```

### 2.3 Presentation Tier

The presentation tier uses classic ASP.NET Web Forms patterns:

| Pattern | Implementation | Files |
|---|---|---|
| **Master Pages** | `Site.Master` provides HTML shell, navigation, header with user info, footer | 1 master page |
| **Content Pages** | Each `.aspx` page uses `ContentPlaceHolder` to inject content into master layout | 6 pages |
| **Code-Behind** | `.aspx.cs` files handle postback events, data binding, ViewState management | 6 code-behind files |
| **User Controls** | Reusable `.ascx` controls with properties exposed to parent pages | 2 controls |
| **MultiView/Wizard** | `PermitApplication.aspx` uses `MultiView` with 4 `View` panels for wizard steps | 1 page |
| **UpdatePanel** | 5 UpdatePanels for partial page updates via AJAX (fee calc, grids, dashboard) | 5 instances |
| **ScriptManager** | Global `ScriptManager` in `Site.Master` with `EnablePartialRendering="true"` | 1 instance |
| **ObjectDataSource** | `PermitSearch.aspx` uses `ObjectDataSource` to bind search results with paging | 2 instances |
| **Validators** | `RequiredFieldValidator`, `RegularExpressionValidator`, `RangeValidator` with `ValidationGroup` | ~12 validators |
| **URL Routing** | `RouteConfig` maps friendly URLs: `/apply`, `/search`, `/dashboard` | 3 routes |

### 2.4 Business Logic / Data Access Tier

All data access is implemented as **static classes** in `App_Code/`:

- **No interfaces** — classes cannot be mocked or substituted.
- **No dependency injection** — all calls are direct static method invocations.
- **DataTable returns** — all methods return `DataTable` objects; zero strongly-typed models.
- **Simulated data** — current implementation uses hardcoded/generated data patterns with comments indicating where stored procedures would be called.
- **Fee calculation duplication** — `PermitDataAccess.CalculatePermitFee()` in C# duplicates logic in `sp_CalculatePermitFee` and `sp_SubmitPermitApplication` in SQL.

### 2.5 Application Lifecycle

The `Global.asax.cs` handles three lifecycle events:

| Event | Behavior |
|---|---|
| `Application_Start` | Registers URL routes via `RouteConfig.RegisterRoutes()` — maps `/apply`, `/search`, `/dashboard` |
| `Application_Error` | Logs exception message to `Debug.WriteLine()` — no persistent logging |
| `Session_Start` | Assigns `Session["UserRole"] = "Applicant"` and `Session["UserId"] = Guid.NewGuid()` — simulated auth |

---

## 3. Architecture Constraints

### 3.1 Platform Constraints

| Constraint | Description | Impact |
|---|---|---|
| **.NET Framework 4.8** | Runtime reached end-of-support in 2022; no path to incremental upgrade | Must rewrite to .NET 9+; cannot lift-and-shift |
| **ASP.NET Web Forms** | No equivalent in .NET Core/.NET 9; fundamentally incompatible | Full UI rewrite required; no migration tooling exists |
| **IIS Dependency** | `System.Web` assembly requires IIS pipeline; not portable to Kestrel | Must remove all `System.Web` references |
| **Windows-only** | `System.Web`, `System.Drawing`, `System.EnterpriseServices` are Windows-only | Cannot containerize on Linux without rewrite |
| **Legacy `.csproj` format** | MSBuild 15.0 / non-SDK-style project file (184 lines of explicit file includes) | Must convert to SDK-style `.csproj` |

### 3.2 State Management Constraints

| Constraint | Location | Coupling Level |
|---|---|---|
| **ViewState for wizard data** | `PermitApplication.aspx.cs` lines 102-116 — stores 12 form fields | 🔴 High — wizard navigation depends on `ViewState` round-trips |
| **ViewState for calculated fee** | `PermitApplication.aspx.cs` line 94 — `ViewState["CalculatedFee"]` | 🟡 Medium — derived value stored client-side |
| **Global ViewState encryption** | `Web.config` line 21 — `viewStateEncryptionMode="Always"` | 🟡 Medium — all pages incur encryption overhead |
| **Session for user identity** | `Global.asax.cs` lines 22-23, `Site.Master.cs` lines 12-13 | 🔴 High — no real authentication; GUID-based identity |
| **Session for submitted data** | `PermitApplication.aspx.cs` line 174 — `Session["SubmittedPermitData"]` | 🟡 Medium — DataTable stored in session post-submit |
| **Session for selected permit** | `PermitSearch.aspx.cs` line 44 — `Session["SelectedPermitId"]` | 🟡 Medium — detail panel uses session-based parameter |
| **SQL Server session provider** | `Web.config` line 20 — `sessionState mode="SQLServer"` | 🟡 Medium — requires SQL Server for session persistence |

### 3.3 Data Access Constraints

| Constraint | Description | Impact |
|---|---|---|
| **Static classes** | `PermitDataAccess`, `InspectionDataAccess`, `EmailHelper` — all `public static class` | Cannot be unit tested, mocked, or replaced via DI |
| **DataTable coupling** | 11 methods return `DataTable`; pages bind directly to `DataTable.DataSource` | No type safety, no IntelliSense, no serialization contracts |
| **Connection string via ConfigurationManager** | `ConfigurationManager.ConnectionStrings["PermitDB"]` in each data access class | Tied to `Web.config`; not injectable or environment-aware |
| **Business logic in stored procedures** | 5 SPs contain validation, fee calculation, status transitions, auto-assignment | Cannot unit test business rules without database; logic is duplicated |
| **No transaction management in C#** | All transactions are within stored procedures; C# layer has no `TransactionScope` | Business rule enforcement depends entirely on SP execution |

### 3.4 Security Constraints

| Constraint | Description | Severity |
|---|---|---|
| **No real authentication** | Windows Auth configured but `<allow users="*" />`; session assigns random GUID | 🔴 Critical |
| **No authorization enforcement** | All pages accessible to all users; no role checks in code-behind | 🔴 Critical |
| **No CSRF protection** | No `AntiForgeryToken` on any form; forms use standard ASP.NET postback | 🔴 High |
| **XSS via Response.Write** | 3 pages inject `ex.Message` into `<script>alert()</script>` via `Response.Write()` | 🔴 Critical |
| **Plaintext SMTP credentials** | `Web.config` stores SMTP server; `EmailHelper.cs` has hardcoded credential comments | 🟡 Medium |

---

## 4. Quality Attributes

### 4.1 Quality Attribute Assessment

| Attribute | Current State | Current Rating | Target State | Target Rating |
|---|---|---|---|---|
| **Performance** | ViewState encryption on all pages; full-page postbacks for most interactions; UpdatePanels serialize entire page for partial updates; simulated data (no actual DB latency) | ⭐⭐ (2/5) | Blazor Server SignalR diffs; EF Core compiled queries; Azure SQL with connection pooling; Redis cache for session | ⭐⭐⭐⭐ (4/5) |
| **Scalability** | Single IIS instance; in-process state + SQL session; no horizontal scaling; LocalDB file-attached database | ⭐ (1/5) | Azure App Service auto-scaling; stateless app tier; Azure SQL elastic; Redis distributed cache | ⭐⭐⭐⭐⭐ (5/5) |
| **Maintainability** | Static classes prevent modular replacement; business logic split between C#, code-behind, and SQL; fee calculation triply duplicated; no separation of concerns | ⭐⭐ (2/5) | Clean architecture with DI; interface-based services; single source of truth for business rules; SDK-style project | ⭐⭐⭐⭐⭐ (5/5) |
| **Testability** | Zero tests; static classes untestable; no interfaces; no DI; ViewState coupling prevents isolated testing | ⭐ (1/5) | xUnit + bUnit + Playwright; interface-based services mockable; EF Core in-memory provider; >80% coverage target | ⭐⭐⭐⭐⭐ (5/5) |
| **Security** | Windows Auth (unenforced); open access; XSS vectors; no CSRF; no input sanitization; plaintext SMTP; `Debug.WriteLine` error exposure | ⭐ (1/5) | ASP.NET Core Identity + Microsoft Entra ID; RBAC; anti-forgery tokens; CSP headers; Azure Key Vault; HTTPS/HSTS | ⭐⭐⭐⭐⭐ (5/5) |
| **Reliability** | Global error handler logs to Debug only; no retry logic; no health checks; exceptions shown to user via `alert()` | ⭐ (1/5) | Application Insights; structured logging; health endpoints; circuit breakers; graceful error pages | ⭐⭐⭐⭐ (4/5) |
| **Deployability** | Manual IIS deployment; no CI/CD; no containerization; no environment-specific configuration management | ⭐ (1/5) | GitHub Actions CI/CD; Azure App Service deployment slots; bicep/ARM infrastructure-as-code | ⭐⭐⭐⭐⭐ (5/5) |

### 4.2 Key Quality Gaps

| Gap | Current | Target | Priority |
|---|---|---|---|
| Test coverage | 0% | >80% | 🔴 P1 |
| Authentication | Simulated | Entra ID + Identity | 🔴 P1 |
| Role-based authorization | None | RBAC (Applicant, Inspector, Admin) | 🔴 P1 |
| Error handling | `Debug.WriteLine` + `Response.Write` alerts | Structured logging + Application Insights | 🟡 P2 |
| CI/CD pipeline | None | GitHub Actions → Azure App Service | 🟡 P2 |
| Horizontal scaling | Impossible | Auto-scale via App Service | 🟢 P3 |

---

## 5. Component Inventory

### 5.1 Pages

| # | Page | File | LOC (Markup) | LOC (Code-Behind) | Responsibility | Dependencies |
|---|---|---|---|---|---|---|
| 1 | **Home** | `Default.aspx` / `.cs` | 55 | 42 | Landing page displaying 10 most recent permits in a GridView; row click navigates to search | `PermitDataAccess.GetRecentPermits()` |
| 2 | **Permit Application** | `Pages/PermitApplication.aspx` / `.cs` | 187 | 182 | 4-step wizard (property → applicant → project → review/submit) using MultiView, ViewState, validators, UpdatePanel for fee calculation | `PermitDataAccess.CalculatePermitFee()`, `PermitDataAccess.SubmitPermitApplication()`, `EmailHelper.SendPermitConfirmation()`, `AddressLookup.ascx` |
| 3 | **Permit Search** | `Pages/PermitSearch.aspx` / `.cs` | 138 | 65 | Search form + paginated GridView via ObjectDataSource; detail panel via FormView; supports query string pre-population | `PermitDataAccess.SearchPermits()`, `PermitDataAccess.GetPermitById()` (via ObjectDataSource) |
| 4 | **Inspection Schedule** | `Pages/InspectionSchedule.aspx` / `.cs` | 76 | 78 | Scheduling form (permit ID, type, date, notes) + upcoming inspections GridView; complete/cancel via row commands | `InspectionDataAccess.GetUpcomingInspections()`, `InspectionDataAccess.ScheduleInspection()`, `InspectionDataAccess.CompleteInspection()`, `InspectionDataAccess.CancelInspection()` |
| 5 | **Plan Review** | `Pages/PlanReview.aspx` / `.cs` | 82 | 86 | Load permit by ID, display header, review type/status selection, comments, deficiency checklist, review history grid | `PermitDataAccess.GetPermitById()`, `PermitDataAccess.GetPlanReviewHistory()`, `PermitDataAccess.SubmitPlanReview()`, `PermitHeader.ascx` |
| 6 | **Dashboard** | `Pages/Dashboard.aspx` / `.cs` | 63 | 44 | Statistics cards (total permits, pending review, inspections today, monthly revenue), recent activity grid, status summary grid | `PermitDataAccess.GetDashboardStatistics()`, `PermitDataAccess.GetRecentActivity()`, `PermitDataAccess.GetPermitsByStatus()` |

### 5.2 Master Page

| Component | File | LOC | Responsibility | Dependencies |
|---|---|---|---|---|
| **Site.Master** | `MasterPages/Site.Master` / `.cs` | 39 + 24 | HTML layout; navigation bar (Home, Apply, Search, Inspections, Plan Review, Dashboard); user info display (truncated GUID + role); logout; ScriptManager; ContentPlaceHolders (`head`, `MainContent`) | `Session["UserId"]`, `Session["UserRole"]` |

### 5.3 User Controls

| # | Control | File | LOC | Responsibility | Properties | Used By |
|---|---|---|---|---|---|---|
| 1 | **AddressLookup** | `UserControls/AddressLookup.ascx` / `.cs` | 7 + 44 | Simulated address autocomplete: text input + "Lookup" button → hardcoded address list → selection populates text box | `Address` (get/set) | `PermitApplication.aspx` |
| 2 | **PermitHeader** | `UserControls/PermitHeader.ascx` / `.cs` | 8 + 30 | Read-only display of permit summary: Permit ID, Property Address, Status | `PermitId`, `PropertyAddress`, `Status` (get/set) | `PlanReview.aspx` |

### 5.4 Data Access Classes

| # | Class | File | LOC | Type | Methods | Responsibility |
|---|---|---|---|---|---|---|
| 1 | **PermitDataAccess** | `App_Code/PermitDataAccess.cs` | 213 | `public static class` | 11 | All permit-related data operations: CRUD, search with paging, fee calculation, dashboard statistics, plan review history, status aggregation. Currently returns simulated `DataTable` data. |
| 2 | **InspectionDataAccess** | `App_Code/InspectionDataAccess.cs` | 106 | `public static class` | 6 | All inspection-related data operations: upcoming inspections, schedule/complete/cancel, history, inspector schedule. Currently returns simulated `DataTable` data. |
| 3 | **EmailHelper** | `App_Code/EmailHelper.cs` | 84 | `public static class` | 4 | Email notifications: permit confirmation (with HTML template), inspection confirmation, review completed, permit issued. Currently simulated via `Debug.WriteLine`. |

### 5.5 Data Access Method Inventory

#### PermitDataAccess (11 methods)

| # | Method | Signature | Returns | Purpose |
|---|---|---|---|---|
| 1 | `GetRecentPermits` | `(int count)` | `DataTable` | Fetch most recent N permits for home page |
| 2 | `CalculatePermitFee` | `(string permitType, decimal estimatedCost)` | `decimal` | Pure computation: base fee + percentage by permit type |
| 3 | `SubmitPermitApplication` | `(string propertyAddress, string parcelNumber, string zoning, string permitType, string description, decimal estimatedCost, DataTable applicantData)` | `string` (permit ID) | Submit new permit; returns generated ID |
| 4 | `SearchPermits` | `(string permitId, string address, string permitType, string status, int startRow, int pageSize)` | `DataTable` | Paginated search with multi-field filter |
| 5 | `GetPermitById` | `(string permitId)` | `DataTable` | Single permit lookup by ID |
| 6 | `GetDashboardStatistics` | `()` | `DataTable` | Aggregate stats: total permits, pending, inspections today, revenue |
| 7 | `GetRecentActivity` | `(int count)` | `DataTable` | Recent N activity log entries |
| 8 | `GetPermitsByStatus` | `()` | `DataTable` | Status group counts with total values |
| 9 | `GetPlanReviewHistory` | `(string permitId)` | `DataTable` | All reviews for a permit |
| 10 | `SubmitPlanReview` | `(string permitId, string reviewType, string reviewer, string status, string comments, string deficiencies)` | `string` (review ID) | Submit a plan review; returns generated ID |

#### InspectionDataAccess (6 methods)

| # | Method | Signature | Returns | Purpose |
|---|---|---|---|---|
| 1 | `GetUpcomingInspections` | `()` | `DataTable` | All upcoming/pending inspections |
| 2 | `ScheduleInspection` | `(string permitId, string inspectionType, DateTime requestedDate, string notes)` | `string` (inspection ID) | Schedule a new inspection |
| 3 | `CompleteInspection` | `(string inspectionId, string result)` | `void` | Mark inspection as completed with result |
| 4 | `CancelInspection` | `(string inspectionId)` | `void` | Cancel a scheduled inspection |
| 5 | `GetInspectionHistory` | `(string permitId)` | `DataTable` | Historical inspections for a permit |
| 6 | `GetInspectorSchedule` | `(string inspectorId, DateTime startDate, DateTime endDate)` | `DataTable` | Inspector's schedule for date range |

#### EmailHelper (4 methods)

| # | Method | Signature | Returns | Purpose |
|---|---|---|---|---|
| 1 | `SendPermitConfirmation` | `(string recipientEmail, string applicantName, string permitId)` | `void` | HTML-formatted permit submission confirmation |
| 2 | `SendInspectionConfirmation` | `(string recipientEmail, string inspectionId, DateTime scheduledDate, string inspectionType)` | `void` | Inspection scheduling confirmation |
| 3 | `SendReviewCompletedNotification` | `(string recipientEmail, string permitId, string reviewStatus, string comments)` | `void` | Plan review status notification |
| 4 | `SendPermitIssuedNotification` | `(string recipientEmail, string permitId, DateTime expirationDate)` | `void` | Permit issuance notification |

### 5.6 Stored Procedures

| # | Procedure | Parameters | Purpose | Business Rules Embedded |
|---|---|---|---|---|
| 1 | **sp_SubmitPermitApplication** | `@PermitId, @PropertyAddress, @ParcelNumber, @PermitType, @EstimatedCost, @ZoningDistrict, @ProjectDescription, @ApplicantName, @ApplicantEmail, @ApplicantPhone` | Insert new permit with applicant upsert, fee calculation, and activity logging | Min cost validation for NewConstruction ($10K); applicant upsert by email; fee calculation (base + percentage by type); activity log entry |
| 2 | **sp_CalculatePermitFee** | `@PermitType, @EstimatedCost, @SquareFootage (optional), @ZoningDistrict (optional), @CalculatedFee OUTPUT` | Calculate total permit fee with surcharges | Base fee by type (6 types); percentage fee by type; square footage surcharge for construction/addition ($0.50/sqft); zoning surcharge for C1/I1 districts ($200) |
| 3 | **sp_ScheduleInspection** | `@PermitId, @InspectionType, @RequestedDate, @InspectorId (optional), @Notes (optional), @InspectionId OUTPUT` | Schedule inspection with validation and auto-assignment | Permit must exist and be in Issued/Under Inspection status; no weekend scheduling; auto-assign inspector with least inspections on date; update permit status to "Under Inspection"; activity log |
| 4 | **sp_CompleteInspection** | `@InspectionId, @Result, @Comments (optional)` | Complete inspection and cascade permit status | If Passed + Final → "Certificate of Occupancy Issued"; if Passed + non-Final → "Issued"; if Failed → "Inspection Failed - Corrections Required"; activity log |
| 5 | **sp_SubmitPlanReview** | `@PermitId, @ReviewType, @ReviewerId, @Status, @Comments (optional), @Deficiencies (optional), @ReviewId OUTPUT` | Submit plan review with status cascading | If Approved and no pending/rejected reviews → permit "Approved"; if Rejected → "Review Rejected - Resubmit Required"; activity log |

### 5.7 Configuration Files

| File | Purpose | Key Settings |
|---|---|---|
| `Web.config` | Application configuration | Connection string (LocalDB), SMTP settings (host/port/from), authentication (Windows), authorization (allow all), session (SQL Server mode, 30 min timeout), ViewState (enabled, encrypted), custom errors, runtime assembly bindings |
| `Web.Debug.config` | Debug transform | Debug-specific overrides |
| `Web.Release.config` | Release transform | Release-specific overrides |
| `Global.asax` / `.cs` | Application lifecycle | Route registration, error logging, session initialization |
| `RiverdalePermitSystem.sln` | Solution file | Single project solution |
| `RiverdalePermitSystem.Web.csproj` | Project file | Non-SDK-style; 184 lines; explicit file includes; 12 assembly references |

---

## 6. Data Architecture

### 6.1 Database Schema

**Database:** `RiverdalePermitDB` on SQL Server 2019+ (currently LocalDB with `.mdf` attach)

#### Entity-Relationship Diagram

```
┌─────────────┐     ┌──────────────────┐     ┌──────────────┐
│ Applicants   │     │    Permits        │     │ Contractors   │
│─────────────│     │──────────────────│     │──────────────│
│ ApplicantId  │◄────│ ApplicantId (FK)  │────►│ ContractorId  │
│ Name         │  1:N│ PermitId (PK)     │N:1  │ CompanyName   │
│ Email        │     │ ApplicationDate   │     │ LicenseNumber │
│ Phone        │     │ PropertyAddress   │     │ InsuranceExpiry│
│ Company      │     │ ParcelNumber      │     │ Rating        │
│ LicenseNumber│     │ PermitType        │     │ ContactEmail  │
│ CreatedDate  │     │ Status            │     │ ContactPhone  │
└─────────────┘     │ EstimatedCost     │     │ IsActive      │
                    │ SquareFootage     │     └──────────────┘
                    │ ZoningDistrict    │
                    │ ProjectDescription│
                    │ IssuedDate        │
                    │ ExpirationDate    │
                    │ CompletedDate     │
                    │ CreatedBy         │
                    │ CreatedDate       │
                    │ ModifiedDate      │
                    └────────┬─────────┘
                             │ 1:N
               ┌─────────────┼───────────────┬──────────────┐
               ▼             ▼               ▼              ▼
        ┌─────────────┐ ┌───────────┐ ┌───────────┐ ┌────────────┐
        │ PlanReviews  │ │Inspections│ │   Fees    │ │ActivityLog │
        │─────────────│ │───────────│ │───────────│ │────────────│
        │ ReviewId(PK) │ │InspectionId│ │ FeeId(PK) │ │ LogId(PK)  │
        │ PermitId(FK) │ │PermitId(FK)│ │PermitId(FK)│ │ Timestamp  │
        │ ReviewerId   │ │InspectorId │ │ FeeType   │ │ActivityType│
        │ ReviewType   │ │InspectionType│ │ Amount    │ │ PermitId(FK)│
        │ Status       │ │ScheduledDate│ │ PaidDate  │ │Description │
        │ Comments     │ │CompletedDate│ │PaymentMethod│ │ UserName  │
        │ Deficiencies │ │ Result     │ │TransactionId│ └────────────┘
        │ ReviewDate   │ │ Status     │ │ CreatedDate│
        │ DueDate      │ │ Comments   │ └───────────┘
        │ CompletedDate│ │ Photos     │
        └─────────────┘ │ CreatedDate│
                        └───────────┘
```

### 6.2 Table Specifications

#### Applicants

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `ApplicantId` | `INT IDENTITY` | No | — | Primary key, auto-increment |
| `Name` | `NVARCHAR(100)` | No | — | |
| `Email` | `NVARCHAR(100)` | No | — | Indexed; used for upsert in `sp_SubmitPermitApplication` |
| `Phone` | `NVARCHAR(20)` | No | — | |
| `Company` | `NVARCHAR(100)` | Yes | — | |
| `LicenseNumber` | `NVARCHAR(50)` | Yes | — | |
| `CreatedDate` | `DATETIME` | Yes | `GETDATE()` | |

#### Contractors

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `ContractorId` | `INT IDENTITY` | No | — | Primary key |
| `CompanyName` | `NVARCHAR(200)` | No | — | |
| `LicenseNumber` | `NVARCHAR(50)` | No | — | Unique constraint + index |
| `InsuranceExpiry` | `DATE` | Yes | — | |
| `Rating` | `DECIMAL(3,2)` | Yes | — | |
| `ContactEmail` | `NVARCHAR(100)` | No | — | |
| `ContactPhone` | `NVARCHAR(20)` | No | — | |
| `IsActive` | `BIT` | Yes | `1` | |

#### Permits

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `PermitId` | `NVARCHAR(50)` | No | — | Primary key; format `PERM-YYYY-NNNN` |
| `ApplicationDate` | `DATETIME` | No | `GETDATE()` | |
| `PropertyAddress` | `NVARCHAR(200)` | No | — | Indexed |
| `ParcelNumber` | `NVARCHAR(50)` | No | — | |
| `PermitType` | `NVARCHAR(50)` | No | — | Values: NewConstruction, Addition, Electrical, Plumbing, Mechanical, Demolition |
| `Status` | `NVARCHAR(50)` | No | `'Submitted'` | Indexed; lifecycle values (see 6.4) |
| `ApplicantId` | `INT` | No | — | FK → Applicants |
| `ContractorId` | `INT` | Yes | — | FK → Contractors |
| `EstimatedCost` | `DECIMAL(18,2)` | No | — | |
| `SquareFootage` | `INT` | Yes | — | |
| `ZoningDistrict` | `NVARCHAR(10)` | Yes | — | Values: R1, R2, C1, I1 |
| `ProjectDescription` | `NVARCHAR(MAX)` | Yes | — | |
| `IssuedDate` | `DATETIME` | Yes | — | |
| `ExpirationDate` | `DATETIME` | Yes | — | |
| `CompletedDate` | `DATETIME` | Yes | — | Set when CO issued |
| `CreatedBy` | `NVARCHAR(100)` | Yes | `SYSTEM_USER` | |
| `CreatedDate` | `DATETIME` | Yes | `GETDATE()` | |
| `ModifiedDate` | `DATETIME` | Yes | `GETDATE()` | Updated by stored procedures |

#### PlanReviews

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `ReviewId` | `NVARCHAR(50)` | No | — | PK; format `REV-YYYY-NNNN` |
| `PermitId` | `NVARCHAR(50)` | No | — | FK → Permits; indexed |
| `ReviewerId` | `NVARCHAR(100)` | No | — | |
| `ReviewType` | `NVARCHAR(50)` | No | — | Values: Structural, Electrical, Mechanical, etc. |
| `Status` | `NVARCHAR(50)` | No | `'Pending'` | Indexed; values: Pending, InProgress, Approved, Rejected |
| `Comments` | `NVARCHAR(MAX)` | Yes | — | |
| `Deficiencies` | `NVARCHAR(MAX)` | Yes | — | Semicolon-delimited list |
| `ReviewDate` | `DATETIME` | Yes | `GETDATE()` | |
| `DueDate` | `DATETIME` | Yes | — | |
| `CompletedDate` | `DATETIME` | Yes | — | |

#### Inspections

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `InspectionId` | `NVARCHAR(50)` | No | — | PK; format `INSP-YYYY-NNNN` |
| `PermitId` | `NVARCHAR(50)` | No | — | FK → Permits; indexed |
| `InspectorId` | `NVARCHAR(100)` | No | — | Indexed |
| `InspectionType` | `NVARCHAR(50)` | No | — | Values: Foundation, Framing, Electrical, Plumbing, Final |
| `ScheduledDate` | `DATETIME` | No | — | Indexed |
| `CompletedDate` | `DATETIME` | Yes | — | |
| `Result` | `NVARCHAR(50)` | Yes | — | Values: Passed, Failed |
| `Status` | `NVARCHAR(50)` | No | `'Scheduled'` | Values: Scheduled, Completed, Cancelled |
| `Comments` | `NVARCHAR(MAX)` | Yes | — | |
| `Photos` | `NVARCHAR(MAX)` | Yes | — | |
| `CreatedDate` | `DATETIME` | Yes | `GETDATE()` | |

#### Fees

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `FeeId` | `INT IDENTITY` | No | — | Primary key |
| `PermitId` | `NVARCHAR(50)` | No | — | FK → Permits; indexed |
| `FeeType` | `NVARCHAR(50)` | No | — | e.g., "Permit Fee" |
| `Amount` | `DECIMAL(18,2)` | No | — | |
| `PaidDate` | `DATETIME` | Yes | — | |
| `PaymentMethod` | `NVARCHAR(50)` | Yes | — | |
| `TransactionId` | `NVARCHAR(100)` | Yes | — | |
| `CreatedDate` | `DATETIME` | Yes | `GETDATE()` | |

#### ActivityLog

| Column | Type | Nullable | Default | Notes |
|---|---|---|---|---|
| `LogId` | `INT IDENTITY` | No | — | Primary key |
| `Timestamp` | `DATETIME` | Yes | `GETDATE()` | Indexed |
| `ActivityType` | `NVARCHAR(100)` | No | — | Values: Application Submitted, Inspection Scheduled, Inspection Completed, Plan Review Submitted |
| `PermitId` | `NVARCHAR(50)` | Yes | — | FK → Permits; indexed |
| `Description` | `NVARCHAR(MAX)` | Yes | — | |
| `UserName` | `NVARCHAR(100)` | No | — | `SYSTEM_USER` or reviewer ID |

### 6.3 Index Strategy

| Table | Index | Columns | Type |
|---|---|---|---|
| Applicants | `IX_Applicants_Email` | Email | Non-clustered |
| Contractors | `IX_Contractors_LicenseNumber` | LicenseNumber | Non-clustered, unique |
| Permits | `IX_Permits_Status` | Status | Non-clustered |
| Permits | `IX_Permits_ApplicationDate` | ApplicationDate | Non-clustered |
| Permits | `IX_Permits_PropertyAddress` | PropertyAddress | Non-clustered |
| PlanReviews | `IX_PlanReviews_PermitId` | PermitId | Non-clustered |
| PlanReviews | `IX_PlanReviews_Status` | Status | Non-clustered |
| Inspections | `IX_Inspections_PermitId` | PermitId | Non-clustered |
| Inspections | `IX_Inspections_ScheduledDate` | ScheduledDate | Non-clustered |
| Inspections | `IX_Inspections_InspectorId` | InspectorId | Non-clustered |
| Fees | `IX_Fees_PermitId` | PermitId | Non-clustered |
| ActivityLog | `IX_ActivityLog_Timestamp` | Timestamp | Non-clustered |
| ActivityLog | `IX_ActivityLog_PermitId` | PermitId | Non-clustered |

### 6.4 Permit Status Lifecycle

```
                    ┌──────────┐
                    │ Submitted │
                    └─────┬────┘
                          │ Plan Review begins
                          ▼
                   ┌──────────────┐
              ┌────│ Under Review  │────┐
              │    └──────────────┘    │
              │ All approved           │ Any rejected
              ▼                        ▼
        ┌──────────┐    ┌───────────────────────────────┐
        │ Approved  │    │ Review Rejected - Resubmit    │
        └─────┬────┘    │         Required               │
              │          └───────────────────────────────┘
              │ Permit issued
              ▼
         ┌─────────┐
         │  Issued  │◄──────────────────────────────┐
         └─────┬───┘                                 │
               │ Inspection scheduled                │ Non-final passed
               ▼                                     │
      ┌──────────────────┐                           │
      │ Under Inspection  │───────────┬──────────────┘
      └──────────────────┘            │
               │ Final passed         │ Failed
               ▼                      ▼
┌──────────────────────────┐  ┌─────────────────────────────────────┐
│ Certificate of Occupancy │  │ Inspection Failed - Corrections     │
│         Issued           │  │            Required                  │
└──────────────────────────┘  └─────────────────────────────────────┘
```

### 6.5 Data Flow Patterns

| Operation | Data Flow |
|---|---|
| **Permit Submit** | `PermitApplication.aspx` → ViewState (12 fields) → `PermitDataAccess.SubmitPermitApplication()` → `sp_SubmitPermitApplication` → INSERT Applicants (upsert) + INSERT Permits + INSERT Fees + INSERT ActivityLog |
| **Fee Calculation** | `PermitApplication.aspx` → `btnCalculateFee_Click` → `PermitDataAccess.CalculatePermitFee()` → returns `decimal` → `ViewState["CalculatedFee"]` |
| **Plan Review** | `PlanReview.aspx` → `PermitDataAccess.GetPermitById()` → display → `PermitDataAccess.SubmitPlanReview()` → `sp_SubmitPlanReview` → INSERT PlanReviews + UPDATE Permits + INSERT ActivityLog |
| **Schedule Inspection** | `InspectionSchedule.aspx` → `InspectionDataAccess.ScheduleInspection()` → `sp_ScheduleInspection` → INSERT Inspections + UPDATE Permits + INSERT ActivityLog |
| **Complete Inspection** | `InspectionSchedule.aspx` → `InspectionDataAccess.CompleteInspection()` → `sp_CompleteInspection` → UPDATE Inspections + UPDATE Permits + INSERT ActivityLog |
| **Dashboard** | `Dashboard.aspx` → `PermitDataAccess.GetDashboardStatistics()` + `GetRecentActivity()` + `GetPermitsByStatus()` → bind to GridViews/Labels |
| **Search** | `PermitSearch.aspx` → `ObjectDataSource` → `PermitDataAccess.SearchPermits()` → paginated DataTable → GridView; detail via `Session["SelectedPermitId"]` → `GetPermitById()` → FormView |

### 6.6 Business Logic Duplication

| Business Rule | C# Implementation | SQL Implementation | Risk |
|---|---|---|---|
| Fee calculation (base + percentage) | `PermitDataAccess.CalculatePermitFee()` — switch on 6 permit types | `sp_CalculatePermitFee` — CASE expression on 6 types + sqft + zoning surcharges | 🔴 Triple duplication (also in `sp_SubmitPermitApplication`) |
| Permit type categories | `PermitDataAccess.CalculatePermitFee()` switch cases | Multiple SP CASE expressions | 🟡 Scattered constants |
| Permit status transitions | Not in C# | `sp_CompleteInspection`, `sp_SubmitPlanReview`, `sp_ScheduleInspection` | 🟡 Status machine only in SQL |

> **Note:** The C# `CalculatePermitFee()` method computes only base + percentage fees. The SQL `sp_CalculatePermitFee` additionally applies square footage surcharges ($0.50/sqft for construction/addition) and zoning district surcharges ($200 for C1/I1). This discrepancy means the client-side estimate shown in the wizard may differ from the actual fee stored in the database.

---

## 7. Integration Points

### 7.1 Email Integration (EmailHelper)

| Aspect | Details |
|---|---|
| **Protocol** | SMTP via `System.Net.Mail.SmtpClient` |
| **Server** | `smtp.riverdalecity.gov` (from `Web.config` AppSettings) |
| **Port** | 25 (plaintext, no TLS) |
| **Authentication** | Hardcoded `"username"/"password"` in commented code (`EmailHelper.cs` line 36) |
| **Sender** | `permits@riverdalecity.gov` (from `Web.config` AppSettings) |
| **Current State** | Simulated — all methods log to `Debug.WriteLine()` instead of sending email |
| **Email Templates** | 1 HTML template implemented (`GetPermitConfirmationBody`); 3 others stubbed |
| **Triggered By** | `PermitApplication.aspx.cs` line 163 calls `SendPermitConfirmation` after submit |

**Email Notifications Defined:**

| # | Notification | Trigger | Template |
|---|---|---|---|
| 1 | Permit Application Confirmation | `btnSubmit_Click` in `PermitApplication.aspx.cs` | HTML body with permit ID, review timeline |
| 2 | Inspection Scheduling Confirmation | Planned (stub in `EmailHelper.cs`) | Not implemented |
| 3 | Plan Review Completed | Planned (stub in `EmailHelper.cs`) | Not implemented |
| 4 | Permit Issued | Planned (stub in `EmailHelper.cs`) | Not implemented |

### 7.2 Database Integration

| Aspect | Details |
|---|---|
| **Provider** | SQL Server via ADO.NET (`System.Data.SqlClient`) |
| **Instance** | `(LocalDB)\MSSQLLocalDB` — SQL Server LocalDB |
| **Database** | File-attached `.mdf` via `AttachDbFilename=\|DataDirectory\|\PermitDB.mdf` |
| **Authentication** | Integrated Security (Windows) |
| **Connection String** | Stored in `Web.config` `<connectionStrings>` section |
| **Session Storage** | Separate SQL Server session state: `sessionState mode="SQLServer"` on same LocalDB instance |
| **Current State** | Connection string is defined but data access classes return simulated data; no actual database calls are executed |

### 7.3 Authentication Integration

| Aspect | Details |
|---|---|
| **Configured Mode** | Windows Authentication (`<authentication mode="Windows" />`) |
| **Actual Behavior** | No authentication enforced; `<allow users="*" />` allows anonymous access |
| **Session Identity** | Random GUID generated at session start (`Session["UserId"] = Guid.NewGuid()`) |
| **Role Assignment** | Hardcoded to `"Applicant"` for all sessions (`Session["UserRole"] = "Applicant"`) |
| **Logout** | `Session.Clear()` + `Session.Abandon()` + redirect to home |

### 7.4 External Services Summary

| Service | Protocol | Status | Security | Migration Target |
|---|---|---|---|---|
| SQL Server (LocalDB) | TCP/Named Pipes | Active (simulated data) | Windows Integrated | Azure SQL Database |
| SMTP Server | SMTP (port 25) | Simulated | No TLS, hardcoded credentials | Azure Communication Services |
| IIS Express | HTTP | Active | Windows Auth (unenforced) | Azure App Service |

---

## 8. Cross-Cutting Concerns

### 8.1 Error Handling

The application has **minimal and inconsistent** error handling:

| Pattern | Location | Behavior | Risk |
|---|---|---|---|
| **Global error handler** | `Global.asax.cs` → `Application_Error` | `Debug.WriteLine($"Application Error: {ex?.Message}")` — logs to debug output only; no persistent logging, no error page routing | 🔴 Errors lost in production |
| **Response.Write script injection** | `PermitApplication.aspx.cs:178`, `PermitSearch.aspx.cs:50`, `PlanReview.aspx.cs:35` | `Response.Write($"<script>alert('Error: {ex.Message}');</script>")` — injects unescaped exception message into HTML | 🔴 XSS vector; exposes internal errors to users |
| **try/catch with label** | `InspectionSchedule.aspx.cs:49-53`, `PlanReview.aspx.cs:79-83` | Sets error message to a Label control with warning CSS class | 🟢 Safer pattern but still shows `ex.Message` |
| **Custom errors config** | `Web.config:26` | `<customErrors mode="RemoteOnly" defaultRedirect="~/Error.aspx" />` — but `Error.aspx` does not exist | 🟡 Configured but broken |
| **SP error handling** | All 5 stored procedures | `TRY/CATCH` with `ROLLBACK` on failure; `RAISERROR` for business rule violations; `THROW` for unexpected errors | 🟢 Consistent pattern in SQL layer |

### 8.2 Logging

| Pattern | Location | Mechanism | Persistence |
|---|---|---|---|
| **Debug.WriteLine** | `Global.asax.cs:17`, `EmailHelper.cs:41,46,70,76,82` | `System.Diagnostics.Debug.WriteLine()` | None — only visible in debugger output window |
| **Activity Log (database)** | All 5 stored procedures | `INSERT INTO ActivityLog` with activity type, permit ID, description, username | SQL Server — durable audit trail |
| **No application logging framework** | Entire codebase | No log4net, NLog, Serilog, or similar | N/A |

> **Summary:** The only durable logging is the `ActivityLog` table populated by stored procedures. The C# application layer has zero persistent logging.

### 8.3 Authentication & Authorization Patterns

| Aspect | Implementation | Location |
|---|---|---|
| **Identity source** | Random GUID generated per session | `Global.asax.cs:23` |
| **Role assignment** | Hardcoded `"Applicant"` for all users | `Global.asax.cs:22` |
| **Identity display** | First 8 chars of GUID shown as username | `Site.Master.cs:12` |
| **Role display** | Role from session shown in header | `Site.Master.cs:13` |
| **Logout** | Clear + abandon session, redirect home | `Site.Master.cs:19-21` |
| **Authorization checks** | None — no `if (role == "Admin")` guards on any page | All pages |
| **Page-level authorization** | `Web.config:18` → `<allow users="*" />` — all pages open | Global |
| **Reviewer identity** | Uses `Session["UserId"]` truncated to 8 chars as reviewer name | `PlanReview.aspx.cs:14` |

### 8.4 Validation Patterns

| Pattern | Location | Implementation |
|---|---|---|
| **ASP.NET Validators** | `PermitApplication.aspx` | `RequiredFieldValidator` (7), `RegularExpressionValidator` (1 for email), `RangeValidator` (1 for cost); organized into `ValidationGroup` per wizard step (Step1, Step2, Step3) |
| **Server-side Page.IsValid** | `PermitApplication.aspx.cs:30,46,61` | `if (Page.IsValid)` checks before advancing wizard steps |
| **SP-level validation** | `sp_SubmitPermitApplication` | Min cost check for NewConstruction ($10K); `RAISERROR` on failure |
| **SP-level validation** | `sp_ScheduleInspection` | Permit exists check; status check (Issued/Under Inspection); weekend check; `RAISERROR` on failures |
| **No client-side sanitization** | `PermitSearch.aspx.cs:13` | `Request.QueryString["permitId"]` used directly without encoding |
| **No model validation** | Entire C# codebase | Zero use of `DataAnnotations`, no validation attributes on models (no models exist) |

### 8.5 State Management Patterns

| Pattern | Scope | Usage |
|---|---|---|
| **ViewState** | Per-page, per-postback | Wizard form data (12 fields); calculated fee; transmitted encrypted in every postback via hidden `__VIEWSTATE` field |
| **Session State** | Per-user, server-side (SQL Server mode) | User identity (`UserId`, `UserRole`); submitted permit data (`SubmittedPermitData`); selected permit for detail view (`SelectedPermitId`) |
| **Query String** | Per-request | `permitId` parameter for pre-populating search (`PermitSearch.aspx.cs:13`) |
| **No caching** | N/A | No output caching, no data caching, no distributed cache |

### 8.6 Navigation & Routing

| Pattern | Implementation | Location |
|---|---|---|
| **Friendly URL routes** | `RouteConfig.RegisterRoutes()` maps `/apply`, `/search`, `/dashboard` | `Global.asax.cs:11,31-33` |
| **Direct `.aspx` navigation** | `Site.Master` hyperlinks use `~/Pages/*.aspx` paths | `Site.Master:23-28` |
| **Programmatic redirect** | `Response.Redirect()` for logout and detail navigation | `Site.Master.cs:21`, `Default.aspx.cs:38` |
| **Query string navigation** | Permit ID passed via query string from home to search | `Default.aspx.cs:38` |

### 8.7 UI Patterns

| Pattern | Usage | Instances |
|---|---|---|
| **Master Page layout** | Single `Site.Master` defines header, nav, content area, footer | 1 |
| **GridView data binding** | DataTable bound to `GridView.DataSource` in code-behind | 7 grids across 5 pages |
| **UpdatePanel partial rendering** | Async refresh of specific page sections | 5 instances |
| **FormView** | Single-record display for permit details | 1 (PermitSearch.aspx) |
| **MultiView wizard** | 4-step form wizard with Previous/Next navigation | 1 (PermitApplication.aspx) |
| **CSS styling** | Single `Site.css` (233 lines) — custom styles, no framework (no Bootstrap) | 1 file |
| **No JavaScript frameworks** | Zero client-side JS files; all interactivity via server postbacks | N/A |

### 8.8 Configuration Management

| Setting | Location | Pattern |
|---|---|---|
| Connection strings | `Web.config` `<connectionStrings>` | `ConfigurationManager.ConnectionStrings["PermitDB"]` |
| SMTP settings | `Web.config` `<appSettings>` | `ConfigurationManager.AppSettings["SmtpServer"]`, `["SmtpPort"]`, `["EmailFrom"]` |
| Report path | `Web.config` `<appSettings>` | `ConfigurationManager.AppSettings["ReportPath"]` (referenced but unused) |
| Validation mode | `Web.config` `<appSettings>` | `ValidationSettings:UnobtrusiveValidationMode = "None"` |
| Framework targeting | `Web.config` `<system.web>` | `targetFramework="4.8"` in both `<compilation>` and `<httpRuntime>` |
| Max upload size | `Web.config` `<httpRuntime>` | `maxRequestLength="10240"` (10 MB) |
| Session timeout | `Web.config` `<sessionState>` | `timeout="30"` (30 minutes) |

> **Note:** All configuration is in XML `Web.config` files with no environment-variable support, no secrets management, and no configuration transforms beyond the empty Debug/Release transform files.

---

*This architecture specification was generated as part of the Spec2Cloud modernization methodology. It provides a comprehensive baseline for planning the target architecture and migration strategy. Proceed to Step 4: Target Architecture Design.*
