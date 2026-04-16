# Step 2: Spec2Cloud Analysis — Riverdale City Building Permit System

> **Generated:** 2026-04-16 | **Methodology:** Spec2Cloud Reverse-Engineering Analysis
> **Application:** Riverdale City Building Permit System
> **Current Platform:** ASP.NET Web Forms / .NET Framework 4.8

---

## 1. Executive Summary

The Riverdale City Building Permit System is a **legacy ASP.NET Web Forms application** running on .NET Framework 4.8 — a platform that reached end-of-support in 2022. The application manages the full lifecycle of building permits for a municipal government: application submission, plan review, inspection scheduling, and administrative dashboards.

### Key Findings

| Category | Assessment | Risk |
|---|---|---|
| **Framework Maturity** | End-of-life (.NET Framework 4.8) | 🔴 Critical |
| **Architecture** | Monolithic, tightly coupled, no DI | 🔴 High |
| **Security** | Windows Auth, open access, XSS vectors | 🔴 High |
| **Data Access** | Static classes, DataTables, no ORM | 🟡 Medium |
| **Business Logic** | Split between C# and SQL stored procedures | 🟡 Medium |
| **UI Technology** | Web Forms (ViewState, UpdatePanels, PostBack) | 🔴 High |
| **Testability** | Zero — static classes, no interfaces, no DI | 🔴 Critical |
| **Cloud Readiness** | Not cloud-native; IIS/Windows-dependent | 🔴 High |
| **Code Complexity** | Low-to-moderate; well-structured for its era | 🟢 Low |
| **Database Schema** | Well-normalized, clean schema with proper FKs | 🟢 Low |

**Bottom Line:** The application is well-organized for a Web Forms app but uses fundamentally obsolete patterns that cannot be incrementally upgraded. A full rewrite to .NET 9 / Blazor Server or ASP.NET Core MVC with EF Core is the recommended path, leveraging the existing clean database schema as a solid foundation.

---

## 2. Technology Inventory

### 2.1 Frameworks & Runtime

| Technology | Version | Status | Migration Target |
|---|---|---|---|
| .NET Framework | 4.8 | ⛔ End of support | .NET 9 |
| ASP.NET Web Forms | 4.8 | ⛔ No .NET Core equivalent | Blazor Server / ASP.NET Core MVC |
| C# Language | ~7.x (implicit) | ⚠️ Outdated | C# 13 |
| MSBuild (legacy `.csproj`) | 15.0 | ⚠️ Legacy format | SDK-style `.csproj` |

### 2.2 Framework References (from `.csproj`)

| Assembly | Purpose | Migration Path |
|---|---|---|
| `System.Web` | Core ASP.NET Web Forms | Remove — replaced by ASP.NET Core |
| `System.Web.Extensions` | AJAX / ScriptManager | Remove — use Blazor interactivity |
| `System.Web.DynamicData` | Dynamic Data framework | Remove — unused |
| `System.Web.Entity` | Entity Framework integration | Remove — replace with EF Core |
| `System.Web.ApplicationServices` | Auth services | Remove — use ASP.NET Core Identity |
| `System.Data` | ADO.NET | Replace with EF Core |
| `System.Data.DataSetExtensions` | DataSet LINQ | Remove — use EF Core entities |
| `System.ComponentModel.DataAnnotations` | Validation attributes | Keep — works in .NET 9 |
| `System.Configuration` | ConfigurationManager | Replace with `IConfiguration` |
| `System.Web.Services` | ASMX web services | Remove — use minimal APIs |
| `System.EnterpriseServices` | COM+ integration | Remove |
| `System.Drawing` | Image processing | Replace with `System.Drawing.Common` or SkiaSharp |
| `System.Xml` / `System.Xml.Linq` | XML processing | Keep — available in .NET 9 |

### 2.3 NuGet Packages

**None.** The application relies entirely on .NET Framework BCL assemblies. This simplifies migration since there are no third-party package compatibility concerns.

### 2.4 Architectural Patterns Identified

| Pattern | Usage | Modernization Impact |
|---|---|---|
| Master Pages | `Site.Master` for layout | Replace with Blazor `MainLayout.razor` |
| Code-Behind | All `.aspx.cs` files | Replace with Blazor `@code` blocks or MVC controllers |
| User Controls (`.ascx`) | `AddressLookup`, `PermitHeader` | Replace with Blazor/Razor components |
| MultiView/Wizard | Permit application 4-step wizard | Replace with Blazor component state or Stepper component |
| Static Data Access Classes | `PermitDataAccess`, `InspectionDataAccess` | Replace with DI-injected repository/service classes |
| Global.asax | App lifecycle, routing, session init | Replace with `Program.cs` middleware pipeline |
| URL Routing | `RouteConfig` with `MapPageRoute` | Replace with ASP.NET Core routing |

---

## 3. Code Complexity Metrics

### 3.1 Lines of Code (LOC) by File

| File | LOC | Complexity | Notes |
|---|---|---|---|
| **App_Code/PermitDataAccess.cs** | 213 | Medium | 11 methods, fee calculation logic, DataTable construction |
| **App_Code/InspectionDataAccess.cs** | 106 | Low | 6 methods, simulated data patterns |
| **App_Code/EmailHelper.cs** | 84 | Low | 4 methods, SMTP email templates |
| **Pages/PermitApplication.aspx** | 187 | Medium | 4-step wizard with validators |
| **Pages/PermitApplication.aspx.cs** | 182 | Medium-High | ViewState management, wizard navigation, submit logic |
| **Pages/PermitSearch.aspx** | 138 | Medium | ObjectDataSource, FormView, paging |
| **Pages/PermitSearch.aspx.cs** | 65 | Low | Search, pagination, details panel |
| **Pages/InspectionSchedule.aspx** | 76 | Low | Schedule form + grid |
| **Pages/InspectionSchedule.aspx.cs** | 78 | Low | CRUD operations |
| **Pages/PlanReview.aspx** | 82 | Low | Review form + history grid |
| **Pages/PlanReview.aspx.cs** | 86 | Low-Medium | Review submission, deficiency aggregation |
| **Pages/Dashboard.aspx** | 63 | Low | Stats cards + grids |
| **Pages/Dashboard.aspx.cs** | 44 | Low | Data loading |
| **Default.aspx** | 55 | Low | Landing page with recent permits |
| **Default.aspx.cs** | 42 | Low | Grid data binding |
| **MasterPages/Site.Master** | 39 | Low | Layout template |
| **MasterPages/Site.Master.cs** | 24 | Low | Session-based user display |
| **UserControls/AddressLookup.ascx(.cs)** | 51 | Low | Simulated address lookup |
| **UserControls/PermitHeader.ascx(.cs)** | 39 | Low | Display-only control |
| **Global.asax.cs** | 36 | Low | App start, error handling, session init |
| **Styles/Site.css** | 233 | Low | Full application styling |
| **Web.config** | 56 | Low | Configuration |
| **Database/Schema.sql** | 157 | Medium | 7 tables with FKs and indexes |
| **Database/StoredProcedures/PermitProcedures.sql** | 421 | High | 5 stored procs with business logic |
| **.csproj** | 184 | Low | Project definition |

### 3.2 Summary Metrics

| Metric | Value |
|---|---|
| **Total C# LOC** | ~1,050 |
| **Total ASPX/ASCX markup LOC** | ~690 |
| **Total SQL LOC** | ~578 |
| **Total CSS LOC** | 233 |
| **Total Application LOC** | ~2,551 |
| **Number of Pages** | 6 (.aspx) |
| **Number of User Controls** | 2 (.ascx) |
| **Number of Data Access Classes** | 3 (static) |
| **Number of Stored Procedures** | 5 |
| **Number of Database Tables** | 7 |

### 3.3 Cyclomatic Complexity Estimates

| Component | Est. Cyclomatic Complexity | Risk |
|---|---|---|
| `PermitDataAccess.CalculatePermitFee()` | 8 (switch + conditions) | Medium |
| `PermitApplication.btnSubmit_Click()` | 6 (try/catch, ViewState reads) | Medium |
| `PermitApplication` wizard navigation | 10 (multiple step handlers) | Medium |
| `sp_SubmitPermitApplication` | 12 (validation, upsert, fee calc, logging) | High |
| `sp_ScheduleInspection` | 14 (validation, assignment, status updates) | High |
| `sp_CompleteInspection` | 10 (result branching, status updates) | Medium |
| `sp_SubmitPlanReview` | 10 (status branching, approval checking) | Medium |
| `sp_CalculatePermitFee` | 8 (permit type branching, surcharges) | Medium |
| All other code-behind methods | 2-4 each | Low |

---

## 4. Legacy Pattern Catalog

### 4.1 ViewState Usage 🔴

| Location | Pattern | Impact |
|---|---|---|
| `Web.config` line 21 | `enableViewState="true"` globally, encryption mode `Always` | All pages transmit encrypted ViewState in every postback |
| `PermitApplication.aspx` line 1 | `EnableViewState="true"` page directive | Explicit ViewState dependency |
| `PermitApplication.aspx.cs` lines 102-116 | `SaveToViewState()` — stores 12 form fields in ViewState | Wizard state management via ViewState |
| `PermitApplication.aspx.cs` lines 118-130 | `LoadReviewData()` — reads all ViewState fields | Tight coupling to ViewState |
| `PermitApplication.aspx.cs` line 94 | `ViewState["CalculatedFee"]` storage | Calculated values in ViewState |

**Migration Impact:** All ViewState usage must be replaced with Blazor component state, form models, or cascading parameters. The wizard pattern specifically needs a state container or form model object.

### 4.2 UpdatePanel (Partial Page Rendering) 🟡

| Location | Purpose |
|---|---|
| `Site.Master` line 13 | `ScriptManager` with `EnablePartialRendering="true"` |
| `Default.aspx` line 19 | `upRecentPermits` — refresh recent permits grid |
| `Dashboard.aspx` line 9 | `upDashboard` — refresh statistics |
| `Dashboard.aspx` line 51 | `upStatusChart` — refresh status summary |
| `PermitApplication.aspx` line 134 | `upFeeCalculation` — async fee calculation |
| `InspectionSchedule.aspx` line 49 | `upInspections` — refresh inspection grid |

**Migration Impact:** Replace with Blazor's native component re-rendering. UpdatePanels serialize the entire page; Blazor diffs only changed elements.

### 4.3 DataTable / DataSet Usage 🔴

| Location | Pattern |
|---|---|
| `PermitDataAccess.cs` (all methods) | Returns `DataTable` with manually-defined columns |
| `InspectionDataAccess.cs` (all methods) | Returns `DataTable` with manually-defined columns |
| `PermitApplication.aspx.cs` line 136 | Creates `DataTable` for applicant data transfer |
| All code-behind files | Bind `DataTable` directly to `GridView.DataSource` |

**Count:** 11 methods return `DataTable` objects. Zero strongly-typed models exist.

**Migration Impact:** Define C# record/class models (e.g., `Permit`, `Inspection`, `Applicant`, `PlanReview`, `Fee`, `ActivityLog`). Replace all DataTable usage with `List<T>` or `IEnumerable<T>`.

### 4.4 ObjectDataSource 🟡

| Location | Configuration |
|---|---|
| `PermitSearch.aspx` lines 51-63 | `odsPermits` — paged search via `PermitDataAccess.SearchPermits` |
| `PermitSearch.aspx` lines 127-133 | `odsPermitDetails` — detail lookup via `SessionParameter` |

**Migration Impact:** Replace with service injection and direct data binding in Blazor or MVC controller actions.

### 4.5 Stored Procedures with Business Logic 🟡

| Procedure | Business Rules Embedded |
|---|---|
| `sp_SubmitPermitApplication` | Cost validation, applicant upsert, fee calculation, activity logging |
| `sp_CalculatePermitFee` | Fee structure by type, square footage surcharge, zoning surcharge |
| `sp_ScheduleInspection` | Permit status validation, weekend check, auto-assign inspector, status update |
| `sp_CompleteInspection` | Result-based status transitions, final inspection → CO issuance |
| `sp_SubmitPlanReview` | Approval aggregation, status cascading, deficiency tracking |

**Migration Impact:** Extract all business rules into C# domain service classes. Stored procedures should become simple CRUD operations or be replaced entirely by EF Core. Fee calculation logic is **duplicated** between `PermitDataAccess.CalculatePermitFee()` and `sp_CalculatePermitFee` — a maintenance risk.

### 4.6 Session State Usage 🟡

| Location | Session Key | Purpose |
|---|---|---|
| `Global.asax.cs` line 22 | `UserRole` | Default role assignment on session start |
| `Global.asax.cs` line 23 | `UserId` | Random GUID as user identifier |
| `Site.Master.cs` line 12 | `UserId` (read) | Display username in header |
| `Site.Master.cs` line 13 | `UserRole` (read) | Display role in header |
| `Site.Master.cs` line 19 | `Session.Clear()` / `Abandon()` | Logout |
| `PermitApplication.aspx.cs` line 174 | `SubmittedPermitData` | Stores submitted DataTable in session |
| `PermitSearch.aspx.cs` line 44 | `SelectedPermitId` | Pass selected permit to detail panel |
| `PlanReview.aspx.cs` line 14 | `UserId` (read) | Display reviewer name |
| `Web.config` line 20 | SQL Server session state provider | Server-side session storage |

**Migration Impact:** Replace with ASP.NET Core Identity for authentication and `TempData`/component state for transient data. The SQL Server session provider can be replaced with distributed cache (Redis/Azure Cache).

### 4.7 Inline Script Injection 🔴

| Location | Pattern |
|---|---|
| `PermitApplication.aspx.cs` line 178 | `Response.Write($"<script>alert(...);</script>")` |
| `PermitSearch.aspx.cs` line 50 | `Response.Write($"<script>alert(...);</script>")` |
| `PlanReview.aspx.cs` line 35 | `Response.Write("<script>alert(...);</script>")` |

**Migration Impact:** Replace with proper notification patterns (toast/alert components). These are also XSS vectors since exception messages are injected unescaped into script tags.

---

## 5. Dependency Analysis

### 5.1 NuGet Packages

**None.** The application has zero NuGet dependencies.

### 5.2 Framework Assembly Dependencies

```
System.Web ──────────────────── Core Web Forms runtime (NO .NET Core equivalent)
├── System.Web.Extensions ───── AJAX/ScriptManager (NO .NET Core equivalent)
├── System.Web.DynamicData ──── Dynamic Data (NO .NET Core equivalent)
├── System.Web.Entity ───────── EF integration (replace with EF Core)
├── System.Web.ApplicationServices ── Auth (replace with Identity)
└── System.Web.Services ─────── ASMX services (replace with minimal APIs)

System.Data ─────────────────── ADO.NET (available in .NET 9)
├── System.Data.DataSetExtensions ── DataSet LINQ (available but discouraged)
└── System.Configuration ────── ConfigurationManager (replace with IConfiguration)

System.EnterpriseServices ───── COM+ (remove)
System.Drawing ──────────────── GDI+ (replace with cross-platform alternative)
```

### 5.3 External Service Dependencies

| Service | Protocol | Configuration |
|---|---|---|
| SQL Server (LocalDB) | TCP/Named Pipes | `Web.config` connection string, LocalDB `.mdf` attach |
| SMTP Server | SMTP (port 25) | `smtp.riverdalecity.gov` — no TLS, plaintext credentials |
| IIS Express | HTTP | Port 58745, Windows Auth |

### 5.4 Inter-Component Dependency Map

```
Site.Master (layout)
├── Default.aspx ─────────────→ PermitDataAccess (static)
├── Pages/Dashboard.aspx ─────→ PermitDataAccess (static)
├── Pages/PermitApplication.aspx
│   ├── AddressLookup.ascx (user control)
│   ├── PermitDataAccess (static)
│   └── EmailHelper (static)
├── Pages/PermitSearch.aspx ──→ PermitDataAccess (static, via ObjectDataSource)
├── Pages/InspectionSchedule.aspx → InspectionDataAccess (static)
├── Pages/PlanReview.aspx
│   ├── PermitHeader.ascx (user control)
│   └── PermitDataAccess (static)
└── Global.asax (lifecycle)
```

---

## 6. Data Access Patterns & Database Coupling

### 6.1 Current Data Access Architecture

```
┌──────────────────────────┐
│     ASPX Code-Behind     │
│  (Direct static calls)   │
└──────────┬───────────────┘
           │ Static method calls
           ▼
┌──────────────────────────┐
│   Static Data Access     │
│  PermitDataAccess        │  ← No interfaces, no DI, untestable
│  InspectionDataAccess    │
│  EmailHelper             │
└──────────┬───────────────┘
           │ ADO.NET (simulated)
           ▼
┌──────────────────────────┐
│   SQL Server + Stored    │
│   Procedures             │  ← Business logic embedded in SQL
│   (5 procs, 7 tables)    │
└──────────────────────────┘
```

### 6.2 Data Access Methods Inventory

| Class | Method | Returns | DB Operation |
|---|---|---|---|
| `PermitDataAccess` | `GetRecentPermits(int)` | `DataTable` | Simulated (hardcoded data) |
| `PermitDataAccess` | `CalculatePermitFee(string, decimal)` | `decimal` | Pure computation (no DB) |
| `PermitDataAccess` | `SubmitPermitApplication(...)` | `string` | Simulated INSERT |
| `PermitDataAccess` | `SearchPermits(...)` | `DataTable` | Simulated SELECT with paging |
| `PermitDataAccess` | `GetPermitById(string)` | `DataTable` | Simulated SELECT |
| `PermitDataAccess` | `GetDashboardStatistics()` | `DataTable` | Simulated aggregation |
| `PermitDataAccess` | `GetRecentActivity(int)` | `DataTable` | Simulated SELECT |
| `PermitDataAccess` | `GetPermitsByStatus()` | `DataTable` | Simulated GROUP BY |
| `PermitDataAccess` | `GetPlanReviewHistory(string)` | `DataTable` | Simulated SELECT |
| `PermitDataAccess` | `SubmitPlanReview(...)` | `string` | Simulated INSERT |
| `InspectionDataAccess` | `GetUpcomingInspections()` | `DataTable` | Simulated SELECT |
| `InspectionDataAccess` | `ScheduleInspection(...)` | `string` | Simulated INSERT |
| `InspectionDataAccess` | `CompleteInspection(...)` | `void` | Simulated UPDATE |
| `InspectionDataAccess` | `CancelInspection(...)` | `void` | Simulated UPDATE |
| `InspectionDataAccess` | `GetInspectionHistory(string)` | `DataTable` | Simulated SELECT |
| `InspectionDataAccess` | `GetInspectorSchedule(...)` | `DataTable` | Simulated SELECT |

### 6.3 Database Schema Analysis

**Tables (7):**

| Table | Columns | Relationships | Indexes |
|---|---|---|---|
| `Applicants` | 7 | — | Email |
| `Contractors` | 8 | — | LicenseNumber (unique) |
| `Permits` | 15 | FK → Applicants, Contractors | Status, ApplicationDate, PropertyAddress |
| `PlanReviews` | 10 | FK → Permits | PermitId, Status |
| `Inspections` | 11 | FK → Permits | PermitId, ScheduledDate, InspectorId |
| `Fees` | 7 | FK → Permits | PermitId |
| `ActivityLog` | 5 | FK → Permits | Timestamp, PermitId |

**Schema Quality:** ✅ Well-normalized, proper foreign keys, appropriate indexing. The schema maps cleanly to EF Core entities.

### 6.4 Business Logic Duplication

| Rule | C# Location | SQL Location | Risk |
|---|---|---|---|
| Fee calculation (base + percentage) | `PermitDataAccess.CalculatePermitFee()` | `sp_CalculatePermitFee`, `sp_SubmitPermitApplication` | 🔴 Triple duplication |
| Permit type categories | `PermitDataAccess.CalculatePermitFee()` switch | Multiple stored procedures | 🟡 Scattered constants |

---

## 7. Security Concerns

### 7.1 Critical Issues 🔴

| # | Issue | Location | Severity |
|---|---|---|---|
| 1 | **XSS via Response.Write** | `PermitApplication.aspx.cs:178`, `PermitSearch.aspx.cs:50`, `PlanReview.aspx.cs:35` | Critical |
| 2 | **Open authorization** | `Web.config:18` — `<allow users="*" />` | Critical |
| 3 | **SMTP plaintext credentials** | `EmailHelper.cs:37` — hardcoded `"username"/"password"` in comments, no TLS | High |
| 4 | **No CSRF protection** | No `AntiForgeryToken` or equivalent on forms | High |
| 5 | **Session fixation risk** | `Global.asax.cs:22-23` — random GUID as UserId, no real auth | High |

### 7.2 Medium Issues 🟡

| # | Issue | Location | Severity |
|---|---|---|---|
| 6 | **Unencrypted SMTP** | `Web.config:8` — port 25, `EnableSsl = false` | Medium |
| 7 | **Verbose error exposure** | `PermitApplication.aspx.cs:178` — `ex.Message` shown to user | Medium |
| 8 | **No input sanitization** | Query string parameter `permitId` used directly | Medium |
| 9 | **No role-based access** | Dashboard accessible to all users | Medium |
| 10 | **ViewState encryption** | Enabled but adds overhead; should use signed tokens instead | Low |

### 7.3 Modernization Security Targets

- Replace Windows Auth with **ASP.NET Core Identity** + **Microsoft Entra ID**
- Add **RBAC** (Role-Based Access Control) for admin vs. applicant roles
- Use **anti-forgery tokens** on all forms
- Implement **Content Security Policy** headers
- Use **Azure Key Vault** for secrets (SMTP credentials, connection strings)
- Enable **HTTPS-only** with HSTS

---

## 8. Modernization Readiness Assessment

### 8.1 Migration Complexity by Component

| Component | Complexity | Effort | Risk | Notes |
|---|---|---|---|---|
| **Database Schema** | 🟢 Low | 1-2 days | Low | Clean schema → EF Core model scaffolding |
| **Stored Procedures** | 🟡 Medium | 3-5 days | Medium | Extract business logic to C# services |
| **Data Access Layer** | 🟡 Medium | 2-3 days | Low | Replace static classes with DI services + EF Core |
| **Master Page → Layout** | 🟢 Low | 0.5 day | Low | Direct mapping to Blazor `MainLayout.razor` |
| **CSS/Styling** | 🟢 Low | 1 day | Low | Reusable; optionally adopt a component library |
| **Default.aspx (Home)** | 🟢 Low | 0.5 day | Low | Simple grid display |
| **Dashboard.aspx** | 🟢 Low | 1 day | Low | Stats + grids |
| **PermitSearch.aspx** | 🟡 Medium | 2 days | Medium | ObjectDataSource paging, FormView, session state |
| **InspectionSchedule.aspx** | 🟢 Low | 1 day | Low | Simple form + grid |
| **PlanReview.aspx** | 🟢 Low | 1 day | Low | Form + history grid |
| **PermitApplication.aspx** | 🔴 High | 3-5 days | Medium | 4-step wizard, ViewState, validators, MultiView |
| **User Controls** | 🟢 Low | 0.5 day | Low | Simple → Blazor components |
| **Email Service** | 🟢 Low | 1 day | Low | Replace with Azure Communication Services |
| **Authentication/Authorization** | 🟡 Medium | 2-3 days | Medium | Implement Identity + Entra ID |
| **Configuration** | 🟢 Low | 0.5 day | Low | `Web.config` → `appsettings.json` |

### 8.2 Overall Readiness Score

| Dimension | Score (1-5) | Notes |
|---|---|---|
| **Code Modularity** | 2/5 | Static classes, no interfaces, tight coupling |
| **Architecture Clarity** | 3/5 | Clear separation of pages but no layers |
| **Database Portability** | 4/5 | Clean schema, standard SQL Server |
| **Business Logic Isolation** | 2/5 | Split between C# and stored procedures |
| **Test Coverage** | 1/5 | Zero tests, untestable architecture |
| **Deployment Automation** | 1/5 | No CI/CD, no containerization |
| **Documentation** | 3/5 | Code is self-documenting, schema has comments |
| **Codebase Size** | 5/5 | Small (~2,500 LOC) — very manageable |

**Overall Readiness: 2.6 / 5** — The small codebase is the biggest asset. The fundamental architecture requires a full rewrite, but the limited scope makes this achievable in a focused sprint.

### 8.3 Estimated Migration Effort

| Phase | Effort | Description |
|---|---|---|
| **Phase 1:** Project setup & EF Core models | 2-3 days | SDK-style project, EF Core scaffolding from schema |
| **Phase 2:** Domain services & data access | 3-5 days | Extract business logic from SPs, implement repositories |
| **Phase 3:** UI rewrite (Blazor/MVC) | 5-8 days | Rewrite 6 pages + 2 controls as components |
| **Phase 4:** Auth & security | 2-3 days | Identity, Entra ID, RBAC |
| **Phase 5:** Email & integrations | 1-2 days | Azure Communication Services |
| **Phase 6:** Testing & validation | 3-5 days | Unit tests, integration tests, UAT |
| **Total Estimated** | **16-26 days** | ~3-5 weeks for a single developer |

---

## 9. Recommended Target Architecture

### 9.1 Technology Stack

| Current | Target | Rationale |
|---|---|---|
| .NET Framework 4.8 | **.NET 9** | LTS, cross-platform, performance |
| ASP.NET Web Forms | **Blazor Server** | Component model, rich interactivity, server-side rendering |
| Static data access classes | **EF Core 9 + Repository pattern** | ORM, migrations, testability |
| DataTable returns | **Strongly-typed models** | Type safety, IntelliSense, serialization |
| `Web.config` | **`appsettings.json` + Azure Key Vault** | Modern config, secrets management |
| Windows Auth | **ASP.NET Core Identity + Microsoft Entra ID** | Cloud-native auth, MFA support |
| System.Net.Mail | **Azure Communication Services** | Managed email, delivery tracking |
| SQL Server LocalDB | **Azure SQL Database** | Managed, scalable, geo-redundant |
| IIS Express | **Azure App Service** | PaaS, auto-scaling, deployment slots |
| No CI/CD | **GitHub Actions** | Automated build, test, deploy |
| No tests | **xUnit + bUnit + Playwright** | Unit, component, and E2E tests |

### 9.2 Proposed Solution Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Azure App Service                         │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                    Blazor Server App (.NET 9)              │  │
│  │                                                           │  │
│  │  ┌─────────────┐  ┌──────────────┐  ┌────────────────┐  │  │
│  │  │   Pages/     │  │  Components/ │  │   Shared/      │  │  │
│  │  │  Home        │  │  AddressLkup │  │  MainLayout    │  │  │
│  │  │  Dashboard   │  │  PermitHeader│  │  NavMenu       │  │  │
│  │  │  PermitApp   │  │  DataGrid    │  │  ErrorBoundary │  │  │
│  │  │  Search      │  │  WizardStep  │  │                │  │  │
│  │  │  Inspections │  └──────────────┘  └────────────────┘  │  │
│  │  │  PlanReview  │                                         │  │
│  │  └──────┬──────┘                                         │  │
│  │         │ Dependency Injection                             │  │
│  │  ┌──────▼──────────────────────────────────────────────┐  │  │
│  │  │              Services Layer                          │  │  │
│  │  │  IPermitService    → PermitService                  │  │  │
│  │  │  IInspectionService → InspectionService             │  │  │
│  │  │  IEmailService     → AzureEmailService              │  │  │
│  │  │  IFeeCalculator    → FeeCalculatorService           │  │  │
│  │  └──────┬──────────────────────────────────────────────┘  │  │
│  │         │ EF Core 9                                       │  │
│  │  ┌──────▼──────────────────────────────────────────────┐  │  │
│  │  │        PermitDbContext (EF Core)                     │  │  │
│  │  │  Entities: Permit, Applicant, Contractor,           │  │  │
│  │  │           Inspection, PlanReview, Fee, ActivityLog   │  │  │
│  │  └──────┬──────────────────────────────────────────────┘  │  │
│  └─────────┼─────────────────────────────────────────────────┘  │
└────────────┼────────────────────────────────────────────────────┘
             │
    ┌────────▼────────┐    ┌─────────────────┐    ┌──────────────┐
    │  Azure SQL DB    │    │  Azure Key Vault │    │ Azure Comms  │
    │  (Permit data)   │    │  (Secrets)       │    │ (Email)      │
    └─────────────────┘    └─────────────────┘    └──────────────┘
```

### 9.3 Entity Model Mapping

| Current DataTable | Target Entity | EF Core Mapping |
|---|---|---|
| `GetRecentPermits()` columns | `Permit` class | `DbSet<Permit>` → `Permits` table |
| `GetUpcomingInspections()` columns | `Inspection` class | `DbSet<Inspection>` → `Inspections` table |
| Applicant DataRow | `Applicant` class | `DbSet<Applicant>` → `Applicants` table |
| Review history rows | `PlanReview` class | `DbSet<PlanReview>` → `PlanReviews` table |
| Fee amounts | `Fee` class | `DbSet<Fee>` → `Fees` table |
| Activity rows | `ActivityLog` class | `DbSet<ActivityLog>` → `ActivityLog` table |
| — | `Contractor` class | `DbSet<Contractor>` → `Contractors` table |

### 9.4 Key Migration Patterns

| Legacy Pattern | Modern Replacement |
|---|---|
| ViewState wizard | Blazor `EditForm` with bound model + stepper component |
| UpdatePanel + ScriptManager | Blazor SignalR real-time updates (automatic) |
| ObjectDataSource + GridView | Blazor `<QuickGrid>` or third-party DataGrid |
| `Response.Write("<script>")` | Blazor `IJSRuntime` or toast notification component |
| `ConfigurationManager.AppSettings` | `IConfiguration` + `IOptions<T>` |
| `Session["key"]` | `AuthenticationStateProvider` + `ProtectedSessionStorage` |
| Static `DataAccess` classes | Interface-based services registered in DI container |
| Master Page + ContentPlaceHolder | `MainLayout.razor` + `@Body` |
| User Controls (.ascx) | Blazor Razor components (`.razor`) with `[Parameter]` |
| ASP.NET Validators | Blazor `EditForm` + `DataAnnotationsValidator` |
| Global.asax lifecycle | `Program.cs` middleware pipeline |
| URL routing via `MapPageRoute` | `@page` directives in Blazor components |

### 9.5 Azure Services Mapping

| Capability | Current | Azure Target |
|---|---|---|
| **Hosting** | IIS Express (local) | Azure App Service (B1+) |
| **Database** | SQL Server LocalDB | Azure SQL Database (S0+) |
| **Authentication** | Windows Auth (simulated) | Microsoft Entra ID |
| **Email** | SMTP (plaintext) | Azure Communication Services |
| **Secrets** | `Web.config` plaintext | Azure Key Vault |
| **Monitoring** | `Debug.WriteLine()` | Application Insights |
| **CI/CD** | None | GitHub Actions |
| **Session/Cache** | SQL Server session state | Azure Cache for Redis |

---

## Appendix A: File Inventory

| # | File Path | Type | LOC |
|---|---|---|---|
| 1 | `RiverdalePermitSystem.Web/Default.aspx` | ASPX Page | 55 |
| 2 | `RiverdalePermitSystem.Web/Default.aspx.cs` | Code-Behind | 42 |
| 3 | `RiverdalePermitSystem.Web/Default.aspx.designer.cs` | Designer | ~15 |
| 4 | `RiverdalePermitSystem.Web/Global.asax` | Global | 1 |
| 5 | `RiverdalePermitSystem.Web/Global.asax.cs` | Code-Behind | 36 |
| 6 | `RiverdalePermitSystem.Web/Web.config` | Config | 56 |
| 7 | `RiverdalePermitSystem.Web/Web.Debug.config` | Transform | ~5 |
| 8 | `RiverdalePermitSystem.Web/Web.Release.config` | Transform | ~5 |
| 9 | `RiverdalePermitSystem.Web/RiverdalePermitSystem.Web.csproj` | Project | 184 |
| 10 | `RiverdalePermitSystem.Web/App_Code/PermitDataAccess.cs` | Data Access | 213 |
| 11 | `RiverdalePermitSystem.Web/App_Code/InspectionDataAccess.cs` | Data Access | 106 |
| 12 | `RiverdalePermitSystem.Web/App_Code/EmailHelper.cs` | Helper | 84 |
| 13 | `RiverdalePermitSystem.Web/MasterPages/Site.Master` | Master Page | 39 |
| 14 | `RiverdalePermitSystem.Web/MasterPages/Site.Master.cs` | Code-Behind | 24 |
| 15 | `RiverdalePermitSystem.Web/MasterPages/Site.Master.designer.cs` | Designer | 17 |
| 16 | `RiverdalePermitSystem.Web/Pages/Dashboard.aspx` | ASPX Page | 63 |
| 17 | `RiverdalePermitSystem.Web/Pages/Dashboard.aspx.cs` | Code-Behind | 44 |
| 18 | `RiverdalePermitSystem.Web/Pages/PermitApplication.aspx` | ASPX Page | 187 |
| 19 | `RiverdalePermitSystem.Web/Pages/PermitApplication.aspx.cs` | Code-Behind | 182 |
| 20 | `RiverdalePermitSystem.Web/Pages/PermitSearch.aspx` | ASPX Page | 138 |
| 21 | `RiverdalePermitSystem.Web/Pages/PermitSearch.aspx.cs` | Code-Behind | 65 |
| 22 | `RiverdalePermitSystem.Web/Pages/InspectionSchedule.aspx` | ASPX Page | 76 |
| 23 | `RiverdalePermitSystem.Web/Pages/InspectionSchedule.aspx.cs` | Code-Behind | 78 |
| 24 | `RiverdalePermitSystem.Web/Pages/PlanReview.aspx` | ASPX Page | 82 |
| 25 | `RiverdalePermitSystem.Web/Pages/PlanReview.aspx.cs` | Code-Behind | 86 |
| 26 | `RiverdalePermitSystem.Web/UserControls/AddressLookup.ascx` | User Control | 7 |
| 27 | `RiverdalePermitSystem.Web/UserControls/AddressLookup.ascx.cs` | Code-Behind | 44 |
| 28 | `RiverdalePermitSystem.Web/UserControls/PermitHeader.ascx` | User Control | 8 |
| 29 | `RiverdalePermitSystem.Web/UserControls/PermitHeader.ascx.cs` | Code-Behind | 30 |
| 30 | `RiverdalePermitSystem.Web/Styles/Site.css` | Stylesheet | 233 |
| 31 | `Database/Schema.sql` | DDL | 157 |
| 32 | `Database/StoredProcedures/PermitProcedures.sql` | Stored Procs | 421 |
| 33 | `RiverdalePermitSystem.sln` | Solution | ~25 |

---

## Appendix B: Risk Register

| ID | Risk | Probability | Impact | Mitigation |
|---|---|---|---|---|
| R1 | ViewState-dependent wizard loses data during migration | Medium | High | Build form model early; test all wizard paths |
| R2 | Business logic in stored procedures missed during extraction | Medium | High | Create comprehensive test suite before extracting |
| R3 | Fee calculation discrepancy between C# and SQL | High | Medium | Consolidate to single C# implementation with unit tests |
| R4 | Session state dependencies cause auth issues | Low | Medium | Map all session keys; implement proper auth early |
| R5 | UpdatePanel async behavior differs from Blazor rendering | Low | Low | Blazor handles this natively; no special handling needed |
| R6 | CSS incompatibilities with Blazor component isolation | Low | Low | Test CSS in Blazor early; adopt scoped CSS if needed |

---

*This analysis was generated as part of the Spec2Cloud modernization methodology. Proceed to Step 3: Target Architecture Specification.*
