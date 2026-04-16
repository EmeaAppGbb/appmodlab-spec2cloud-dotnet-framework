# Step 6: Phased Migration Plan — Riverdale City Building Permit System

> **Generated:** 2026-04-16 | **Methodology:** Spec2Cloud Phased Migration Plan
> **Application:** Riverdale City Building Permit System
> **Migration:** ASP.NET Web Forms / .NET Framework 4.8 → .NET 9 / Blazor Server / EF Core / Azure
> **Input:** Step 5 — Modern Architecture Design
> **Estimated Total Duration:** 14–18 weeks (1 developer) / 8–10 weeks (2 developers)

---

## Table of Contents

1. [Migration Overview](#1-migration-overview)
2. [Phase 0 — Preparation](#2-phase-0--preparation)
3. [Phase 1 — Foundation](#3-phase-1--foundation)
4. [Phase 2 — Data Layer](#4-phase-2--data-layer)
5. [Phase 3 — Core Pages](#5-phase-3--core-pages)
6. [Phase 4 — Shared Components](#6-phase-4--shared-components)
7. [Phase 5 — Integration](#7-phase-5--integration)
8. [Phase 6 — Testing](#8-phase-6--testing)
9. [Phase 7 — Deployment](#9-phase-7--deployment)
10. [Cross-Phase Risk Register](#10-cross-phase-risk-register)
11. [Dependency Graph](#11-dependency-graph)

---

## 1. Migration Overview

### 1.1 Migration Strategy

This migration follows a **clean-room rewrite** strategy rather than an in-place upgrade. The legacy ASP.NET Web Forms application (6 pages, 3 static data-access classes, 2 user controls, 5 stored procedures) is small enough that a rewrite into Clean Architecture with .NET 9 / Blazor Server is more efficient than incremental modernization.

### 1.2 Guiding Principles

| Principle | Description |
|---|---|
| **Inside-Out Build Order** | Build Domain → Application → Infrastructure → Presentation; inner layers are testable before outer layers exist |
| **Feature Parity First** | Achieve functional equivalence with the legacy system before adding new capabilities |
| **Continuous Verification** | Each phase ends with a verification gate — automated tests and/or manual review confirming the phase deliverables |
| **No Big Bang** | Each phase produces a working increment; pages are migrated one at a time |
| **Parallel Workstreams** | Data layer and UI work can overlap once the foundation is in place |

### 1.3 Phase Timeline Summary

| Phase | Name | Duration | Depends On | Key Deliverable |
|---|---|---|---|---|
| **0** | Preparation | 1 week | — | Dev environment, tooling, legacy audit complete |
| **1** | Foundation | 1.5–2 weeks | Phase 0 | Solution structure, domain entities, DbContext, Blazor layout shell |
| **2** | Data Layer | 2–3 weeks | Phase 1 | EF Core migrations, all services implemented, stored procedures replaced |
| **3** | Core Pages | 3–4 weeks | Phase 2 | All 6 .aspx pages migrated to .razor components |
| **4** | Shared Components | 1–1.5 weeks | Phase 1 | AddressLookup, PermitHeader, StatusBadge, and utility components |
| **5** | Integration | 2–3 weeks | Phases 3, 4 | Entra ID auth, Azure Communication Services, Key Vault, App Insights |
| **6** | Testing | 2–3 weeks | Phases 3, 4, 5 | Unit, integration, component, and E2E tests at target coverage |
| **7** | Deployment | 1.5–2 weeks | Phase 6 | Azure infrastructure, CI/CD pipelines, production cutover |

> **Note:** Phases 3 and 4 can run in parallel. Phase 6 testing is cumulative — unit tests are written alongside Phases 1–3, with dedicated hardening in this phase.

---

## 2. Phase 0 — Preparation

**Duration:** 1 week
**Goal:** Establish the development environment, audit the legacy system, and confirm migration scope.

### 2.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 0.1 | **Install .NET 9 SDK** | Install .NET 9 SDK and verify `dotnet --version` on all developer machines | SDK installed, verified |
| 0.2 | **Set up development database** | Provision a local SQL Server (or Docker container) and run `Database/Schema.sql` to create the legacy schema with seed data | Working local DB with test data |
| 0.3 | **Run legacy application** | Build and run the existing Web Forms app (`MSBuild` + IIS Express) to establish a behavioral baseline | Screenshots / notes of each page's behavior |
| 0.4 | **Document legacy behavior** | For each of the 6 pages, document: inputs, outputs, validation rules, data flow, edge cases, and known bugs | Legacy behavior specification (checklist) |
| 0.5 | **Audit stored procedures** | Map each SP's business logic to the target Clean Architecture layer per Step 5 §4.2 | SP-to-layer mapping verified |
| 0.6 | **Set up source control** | Create a new branch (`feature/blazor-migration`) from `main`; configure branch protection rules | Branch ready for development |
| 0.7 | **Configure IDE tooling** | Install recommended VS / VS Code extensions: C# Dev Kit, Blazor tooling, EF Core Power Tools, EditorConfig | Consistent dev environment |
| 0.8 | **Create Azure resource group** | Provision an Azure resource group (`rg-riverdale-dev`) for the development environment | Resource group exists |

### 2.2 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Legacy DB schema drift from `Schema.sql` | Medium | Medium | Compare running DB vs. script; reconcile differences before proceeding |
| Missing behavioral documentation for edge cases | High | Medium | Record all edge cases during manual testing; flag unknowns for Phase 3 |
| .NET 9 SDK compatibility issues on dev machines | Low | Low | Use `global.json` to pin SDK version; provide Docker-based dev container as fallback |

### 2.3 Success Criteria

- [ ] .NET 9 SDK installed and `dotnet new blazorserver` runs successfully
- [ ] Legacy application runs locally with all 6 pages functional
- [ ] Behavioral baseline documented for every page and stored procedure
- [ ] Development branch created and CI pipeline stub configured
- [ ] Azure dev resource group provisioned

---

## 3. Phase 1 — Foundation

**Duration:** 1.5–2 weeks
**Goal:** Create the solution structure, domain entities, EF Core DbContext, and Blazor Server application shell with layout.

### 3.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 1.1 | **Create solution structure** | Create the 4-project Clean Architecture solution per Step 5 §5.1: `Domain`, `Application`, `Infrastructure`, `Web` plus 4 test projects | `.sln` with 8 projects, correct references |
| 1.2 | **Define domain enums** | Create all 9 enumerations (`PermitType`, `PermitStatus`, `InspectionType`, etc.) in `Domain/Enums/` | Enums compiled and tested |
| 1.3 | **Define domain entities** | Create all 7 entity classes (`Permit`, `Applicant`, `Contractor`, `PlanReview`, `Inspection`, `Fee`, `ActivityLogEntry`) with properties and business rules | Entities with constructors, validation |
| 1.4 | **Create value objects** | Implement `DateRange` value object in `Domain/ValueObjects/` | Value object with equality |
| 1.5 | **Implement domain services** | Build `PermitFeeCalculator` (consolidating triple-duplicated fee logic) and `PermitStatusMachine` (state transitions) | Domain services with unit tests |
| 1.6 | **Define application interfaces** | Create `IPermitService`, `IPlanReviewService`, `IInspectionService`, `IEmailNotificationService`, `ICurrentUserService` in `Application/Interfaces/` | Interface contracts defined |
| 1.7 | **Create DTOs** | Define all 11 DTOs per Step 5 §2.3 with data annotations | DTOs with validation attributes |
| 1.8 | **Create validators** | Implement `PermitApplicationValidator`, `InspectionScheduleValidator`, `PlanReviewSubmissionValidator` | FluentValidation validators |
| 1.9 | **Configure PermitDbContext** | Create `PermitDbContext` with 7 `DbSet<T>` properties; create Fluent API `IEntityTypeConfiguration<T>` for each entity | DbContext compiles, generates initial migration |
| 1.10 | **Create initial EF Core migration** | Run `dotnet ef migrations add InitialCreate` and verify schema matches legacy `Schema.sql` | Migration that produces equivalent schema |
| 1.11 | **Set up Blazor Server shell** | Create `Program.cs` with DI registration, `MainLayout.razor` (from `Site.Master`), `NavMenu.razor`, and `App.razor` | Blazor app renders layout with navigation |
| 1.12 | **Create placeholder pages** | Add empty `.razor` pages for all 6 routes (`/`, `/permits/apply`, `/permits/search`, `/inspections`, `/reviews`, `/dashboard`) | Routes resolve and render placeholders |
| 1.13 | **Set up CSS framework** | Migrate `Styles/Site.css` to the modern app; add a CSS framework (Bootstrap 5 or equivalent) | Styled layout matching legacy visual design |
| 1.14 | **Write domain unit tests** | Unit tests for `PermitFeeCalculator` (all 6 permit types × zoning combinations) and `PermitStatusMachine` (all valid/invalid transitions) | ≥95% domain layer coverage |

### 3.2 Deliverables

```
RiverdalePermitSystem.sln
├── src/
│   ├── RiverdalePermitSystem.Domain/           ✅ Entities, enums, services, value objects
│   ├── RiverdalePermitSystem.Application/      ✅ Interfaces, DTOs, validators
│   ├── RiverdalePermitSystem.Infrastructure/   ✅ DbContext, configurations, initial migration
│   └── RiverdalePermitSystem.Web/              ✅ Blazor shell with layout and placeholder pages
└── tests/
    └── RiverdalePermitSystem.Domain.Tests/     ✅ Fee calculator + status machine tests
```

### 3.3 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Domain model doesn't match legacy data exactly | Medium | High | Compare EF Core migration output SQL with `Schema.sql` column-by-column; adjust entity mappings |
| Fee calculation discrepancies between C# and SP implementations | High | High | Test all 24 fee combinations (6 types × 4 zoning) against legacy SP output; use SP results as oracle |
| Status machine missing edge-case transitions | Medium | Medium | Map every status change in the 5 stored procedures; create a transition matrix and test exhaustively |
| Blazor Server project template version mismatch | Low | Low | Pin template version via `global.json`; use `dotnet new blazor --interactivity Server` |

### 3.4 Success Criteria

- [ ] Solution builds with zero warnings on `dotnet build`
- [ ] All 7 domain entities compile with correct relationships
- [ ] `PermitFeeCalculator` passes all 24 fee-combination tests matching legacy SP output
- [ ] `PermitStatusMachine` passes all valid/invalid transition tests
- [ ] EF Core `InitialCreate` migration produces schema equivalent to `Schema.sql`
- [ ] Blazor app starts and renders the layout shell with navigation to all 6 routes
- [ ] Domain test coverage ≥ 95%

---

## 4. Phase 2 — Data Layer

**Duration:** 2–3 weeks
**Goal:** Replace all 5 stored procedures and 3 static data-access classes with EF Core–based service implementations.

### 4.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 2.1 | **Implement `PermitService`** | Replace `PermitDataAccess` (11 static methods) with EF Core queries; integrate `PermitFeeCalculator` for fee logic | All `IPermitService` methods implemented |
| 2.2 | **Migrate `sp_SubmitPermitApplication`** | Applicant upsert, fee calculation, permit creation, activity logging — all in `PermitService.SubmitPermitApplicationAsync()` | SP logic fully replaced |
| 2.3 | **Migrate `sp_CalculatePermitFee`** | Already implemented in `PermitFeeCalculator` (Phase 1); wire up via `PermitService.CalculatePermitFeeAsync()` | Service delegates to domain calculator |
| 2.4 | **Implement `InspectionService`** | Replace `InspectionDataAccess` (6 static methods) with EF Core queries; add auto-assignment logic | All `IInspectionService` methods implemented |
| 2.5 | **Migrate `sp_ScheduleInspection`** | Permit validation, no-weekend check, auto-assign inspector, status update, activity log | SP logic fully replaced |
| 2.6 | **Migrate `sp_CompleteInspection`** | Result-based status transitions (Final+Passed→CO, Failed→Corrections), activity log | SP logic fully replaced |
| 2.7 | **Implement `PlanReviewService`** | Replace `PermitDataAccess.GetPlanReviewHistory()` and `SubmitPlanReview()` with EF Core | All `IPlanReviewService` methods implemented |
| 2.8 | **Migrate `sp_SubmitPlanReview`** | Review status cascading (all approved → Permit Approved, any rejected → Rejected) | SP logic fully replaced |
| 2.9 | **Implement `DependencyInjection.cs`** | Register all services, DbContext, domain services in `IServiceCollection` extension | Clean DI setup in `Program.cs` |
| 2.10 | **Seed test data** | Create `HasData()` seed in entity configurations or a separate `DataSeeder` class matching legacy simulated data | Consistent test data for development |
| 2.11 | **Write integration tests** | Test each service method against EF Core InMemory or SQLite provider; verify query correctness | ≥80% infrastructure layer coverage |

### 4.2 Stored Procedure Migration Checklist

| Stored Procedure | Target Service | Business Logic Destination | Status |
|---|---|---|---|
| `sp_SubmitPermitApplication` | `PermitService` | Validator (min cost) + Domain (fees) + Infrastructure (upsert, persist) | ☐ |
| `sp_CalculatePermitFee` | `PermitService` → `PermitFeeCalculator` | Domain (pure calculation) | ☐ |
| `sp_ScheduleInspection` | `InspectionService` | Validator (permit check) + Domain (weekday) + Infrastructure (auto-assign) | ☐ |
| `sp_CompleteInspection` | `InspectionService` → `PermitStatusMachine` | Domain (state transitions) + Infrastructure (persist) | ☐ |
| `sp_SubmitPlanReview` | `PlanReviewService` → `PermitStatusMachine` | Domain (cascading status) + Infrastructure (persist) | ☐ |

### 4.3 Static Class Replacement Map

| Legacy Static Class | Methods | Replacement Service | Notes |
|---|---|---|---|
| `PermitDataAccess` | `GetRecentPermits`, `GetPermitById`, `SearchPermits`, `SubmitPermitApplication`, `CalculatePermitFee`, `GetDashboardStatistics`, `GetRecentActivity`, `GetPermitsByStatus`, `GetPlanReviewHistory`, `SubmitPlanReview`, `GetContractors` | `PermitService`, `PlanReviewService` | 11 methods → split across 2 services |
| `InspectionDataAccess` | `GetUpcomingInspections`, `ScheduleInspection`, `CompleteInspection`, `CancelInspection`, `GetInspectionHistory`, `GetInspectorSchedule` | `InspectionService` | 6 methods → 1 service |
| `EmailHelper` | `SendPermitConfirmation`, `SendInspectionConfirmation`, `SendReviewCompleted`, `SendPermitIssued` | `AzureEmailNotificationService` | Deferred to Phase 5 (stub in Phase 2) |

### 4.4 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| EF Core query performance vs. stored procedures | Medium | Medium | Use compiled queries for hot paths (search, dashboard stats); benchmark against legacy response times |
| Transaction scope differences (SP `BEGIN TRAN` vs. `SaveChangesAsync`) | Medium | High | Map each SP's transaction boundary; use explicit `IDbContextTransaction` where multi-entity consistency is required |
| DataTable-to-DTO mapping misses edge cases | Medium | Medium | Write comparison tests: run legacy SP, capture `DataTable` output, verify DTO equivalent matches |
| Seed data inconsistencies | Low | Low | Use the same seed values as legacy simulated data; verify counts match |

### 4.5 Success Criteria

- [ ] All 5 stored procedures fully replaced by C# service methods
- [ ] All 3 static data-access classes replaced by DI-registered services
- [ ] Zero direct ADO.NET or `DataTable` usage in the new codebase
- [ ] Integration tests pass for all service methods with EF Core InMemory/SQLite
- [ ] `EmailHelper` replaced with stub implementation (actual integration in Phase 5)
- [ ] `dotnet ef database update` creates a working database from migrations
- [ ] Infrastructure test coverage ≥ 80%

---

## 5. Phase 3 — Core Pages

**Duration:** 3–4 weeks
**Goal:** Migrate all 6 `.aspx` Web Forms pages to `.razor` Blazor Server components with full feature parity.

### 5.1 Migration Order

Pages are migrated in dependency order — simpler read-only pages first, complex write pages last:

| Order | Legacy Page | Blazor Component | Complexity | Est. Days |
|---|---|---|---|---|
| 1 | `Pages/Dashboard.aspx` (107 LOC) | `Pages/Admin/Dashboard.razor` | Low | 2–3 |
| 2 | `Default.aspx` (97 LOC) | `Pages/Home.razor` | Low | 2 |
| 3 | `Pages/PermitSearch.aspx` (203 LOC) | `Pages/Permits/Search.razor` | Medium | 3–4 |
| 4 | `Pages/InspectionSchedule.aspx` (154 LOC) | `Pages/Inspections/Schedule.razor` | Medium | 3–4 |
| 5 | `Pages/PlanReview.aspx` (168 LOC) | `Pages/Reviews/PlanReview.razor` | Medium | 3–4 |
| 6 | `Pages/PermitApplication.aspx` (369 LOC) | `Pages/Permits/Apply.razor` | High | 5–7 |

### 5.2 Page Migration Details

#### 5.2.1 Dashboard (`Dashboard.aspx` → `Dashboard.razor`)

| Aspect | Legacy | Modern |
|---|---|---|
| **Data loading** | `GetDashboardStatistics()` → `DataTable` in `Page_Load` | `@inject IPermitService` → `await GetDashboardStatisticsAsync()` in `OnInitializedAsync` |
| **Statistics display** | 4 Labels bound to DataTable columns | Bind to `DashboardStatisticsDto` properties |
| **Activity feed** | GridView with 5 columns | `QuickGrid<ActivityLogDto>` with sortable columns |
| **Status breakdown** | GridView with status/count/value | `QuickGrid<StatusSummaryDto>` or chart component |
| **Authorization** | None (all users see dashboard) | `@attribute [Authorize(Policy = "AdminOnly")]` |
| **Refresh** | Manual page reload | Optional auto-refresh timer (`Timer` + `InvokeAsync(StateHasChanged)`) |

#### 5.2.2 Home Page (`Default.aspx` → `Home.razor`)

| Aspect | Legacy | Modern |
|---|---|---|
| **Welcome section** | Static HTML with user name from session | `AuthenticationStateProvider` → display name from claims |
| **Recent permits** | GridView bound to `GetRecentPermits()` | `QuickGrid<PermitSummaryDto>` |
| **Quick links** | 6 hardcoded hyperlinks | `NavLink` components with role-based visibility |
| **Navigation** | `Response.Redirect` | `NavigationManager.NavigateTo()` |

#### 5.2.3 Permit Search (`PermitSearch.aspx` → `Search.razor`)

| Aspect | Legacy | Modern |
|---|---|---|
| **Search form** | TextBox + DropDownLists + Button | `EditForm` with `PermitSearchCriteria` model binding |
| **Results grid** | ObjectDataSource + GridView with paging | `QuickGrid<PermitSummaryDto>` with `ItemsProvider` for server-side paging |
| **Detail view** | Session → FormView with SessionParameter | Route parameter `/permits/search/{PermitId}` or dialog component |
| **Pagination** | GridView `AllowPaging` + `PageIndexChanging` | `QuickGrid` built-in virtual scrolling or `PaginationState` |

#### 5.2.4 Inspection Schedule (`InspectionSchedule.aspx` → `Schedule.razor`)

| Aspect | Legacy | Modern |
|---|---|---|
| **Upcoming list** | GridView bound to `GetUpcomingInspections()` | `QuickGrid<InspectionSummaryDto>` |
| **Schedule form** | 4 fields + RequiredFieldValidators + Button | `EditForm` + `DataAnnotationsValidator` + `InspectionScheduleDto` |
| **Complete/Cancel** | GridView command buttons | Inline buttons with confirmation dialog (`ConfirmDialog.razor`) |
| **Validation** | Client-side ASP.NET validators only | Server-side FluentValidation + `EditForm` validation display |

#### 5.2.5 Plan Review (`PlanReview.aspx` → `PlanReview.razor`)

| Aspect | Legacy | Modern |
|---|---|---|
| **Review history** | GridView bound to `GetPlanReviewHistory()` | `QuickGrid<PlanReviewHistoryDto>` |
| **Submit form** | DropDownLists + TextBox + CheckBoxList + Button | `EditForm` with `PlanReviewSubmissionDto`; `InputSelect` for enums; `InputCheckbox` loop for deficiencies |
| **Deficiencies** | CheckBoxList → semicolon-delimited string | `List<string>` with checkbox binding |
| **Reviewer ID** | `Session["UserId"]` (GUID) | `ICurrentUserService.GetCurrentUserIdAsync()` |

#### 5.2.6 Permit Application (`PermitApplication.aspx` → `Apply.razor`)

This is the most complex page (369 LOC) with a 4-step wizard:

| Step | Legacy Controls | Modern Controls |
|---|---|---|
| **Step 1: Applicant Info** | TextBoxes + RequiredFieldValidators + RegularExpressionValidator | `EditForm` step 1: `InputText` with `[Required]`, `[EmailAddress]`, `[Phone]` |
| **Step 2: Property Details** | TextBoxes + DropDownLists + AddressLookup.ascx | `EditForm` step 2: `InputText`, `InputSelect<PermitType>`, `AddressLookup.razor` |
| **Step 3: Fee Calculation** | Labels + btnCalculateFee + UpdatePanel | Bind to `PermitApplicationDto`; `@onclick` → `PermitService.CalculatePermitFeeAsync()` |
| **Step 4: Review & Submit** | Summary labels + btnSubmit | Read-only summary display; submit with idempotency guard |
| **Wizard navigation** | `MultiView` + `ActiveViewIndex` | `WizardStep` enum + `switch` block or stepper component |
| **Form state** | 13 ViewState entries | Private component fields (`PermitApplicationModel _model`) |
| **Post-submit** | Session storage + confirmation panel | Navigate to `/permits/search/{permitId}` with success message |

### 5.3 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Permit Application wizard complexity leads to bugs | High | High | Decompose into sub-components (one per step); test each step in isolation with bUnit |
| ViewState-to-component-state data loss on navigation | Medium | Medium | Use `[SupplyParameterFromQuery]` and `NavigationManager` to preserve state in URLs where possible |
| GridView features missing in QuickGrid | Low | Medium | QuickGrid covers sorting, paging, templates; for advanced needs, use MudBlazor DataGrid |
| UpdatePanel real-time behavior differences | Medium | Low | Blazor Server diffing is functionally equivalent; test user-perceived latency |
| CSS/layout regressions vs. legacy appearance | Medium | Low | Side-by-side comparison screenshots; not pixel-perfect but functionally equivalent |

### 5.4 Success Criteria

- [ ] All 6 pages render and function correctly in the Blazor app
- [ ] Permit Application wizard completes all 4 steps and submits successfully
- [ ] Permit Search returns correct results with pagination
- [ ] Inspection Schedule allows scheduling, completing, and cancelling inspections
- [ ] Plan Review allows submission with deficiency checkboxes
- [ ] Dashboard displays correct statistics and activity feed
- [ ] Home page shows recent permits and role-appropriate navigation
- [ ] All forms validate inputs and display validation errors correctly
- [ ] No ViewState, Session, or PostBack patterns remain in the codebase

---

## 6. Phase 4 — Shared Components

**Duration:** 1–1.5 weeks
**Goal:** Migrate user controls to reusable Blazor components and create new shared UI components.
**Note:** Phase 4 can run in parallel with Phase 3 (after Phase 1 completes).

### 6.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 4.1 | **Migrate `AddressLookup.ascx`** | Replace hardcoded address list with `AddressLookup.razor` using `EventCallback<string>` for parent notification; add debounced search input | Reusable component with autocomplete UX |
| 4.2 | **Migrate `PermitHeader.ascx`** | Replace 3 string properties with `[Parameter] PermitDetailDto Permit`; add status color-coding via `StatusBadge.razor` | Reusable permit header component |
| 4.3 | **Create `StatusBadge.razor`** | Color-coded badge for `PermitStatus`, `InspectionStatus`, and `ReviewStatus` enums | Shared status display component |
| 4.4 | **Create `ConfirmDialog.razor`** | Modal confirmation dialog replacing `window.confirm()` and `__doPostBack` patterns | Reusable confirmation component |
| 4.5 | **Create `LoadingSpinner.razor`** | Loading indicator for async operations replacing UpdatePanel loading state | Shared loading component |
| 4.6 | **Create `UserInfo.razor`** | User name, role display, and logout link from `AuthenticationStateProvider` | Header user info component |
| 4.7 | **Create `Toast.razor`** | Toast notification component replacing `Response.Write("<script>alert()")` | Shared notification system |
| 4.8 | **Write bUnit component tests** | Test each shared component: parameter binding, event callbacks, rendering output | ≥80% component coverage |

### 6.2 Component API Contracts

| Component | Parameters | Events | Notes |
|---|---|---|---|
| `AddressLookup.razor` | `string Value`, `string Placeholder` | `EventCallback<string> ValueChanged` | Two-way bindable; debounce 300ms |
| `PermitHeader.razor` | `PermitDetailDto Permit` | — | Display-only; null-safe |
| `StatusBadge.razor` | `string Status`, `string CssClass` | — | Maps status → color automatically |
| `ConfirmDialog.razor` | `string Title`, `string Message`, `bool IsVisible` | `EventCallback OnConfirm`, `EventCallback OnCancel` | Modal overlay |
| `LoadingSpinner.razor` | `bool IsLoading` | — | Conditional render |
| `UserInfo.razor` | — | — | Self-contained; reads `AuthenticationState` |
| `Toast.razor` | — | — | Inject `IToastService`; auto-dismiss |

### 6.3 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Component API changes during Phase 3 page development | Medium | Low | Define interfaces in Phase 4 early; iterate as pages are built |
| AddressLookup debounce introduces async complexity | Low | Low | Use `Timer` or `CancellationTokenSource` pattern; test with bUnit |
| Component styling inconsistencies across pages | Low | Low | Centralize styles in `site.css`; use CSS isolation (`.razor.css`) per component |

### 6.4 Success Criteria

- [ ] All 2 legacy user controls migrated to Blazor components
- [ ] 5 new shared components created and functional
- [ ] All components have documented `[Parameter]` and `EventCallback` contracts
- [ ] bUnit tests pass for each shared component
- [ ] Components integrate correctly into Phase 3 pages
- [ ] No `__doPostBack`, `window.confirm()`, or `Response.Write` patterns remain

---

## 7. Phase 5 — Integration

**Duration:** 2–3 weeks
**Goal:** Integrate Azure services (Entra ID authentication, Azure Communication Services email, Key Vault, Application Insights) and replace legacy infrastructure patterns.

### 7.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 5.1 | **Configure Microsoft Entra ID** | Register app in Entra ID; configure OpenID Connect in `Program.cs`; set up app roles (Applicant, Inspector, Admin) and security groups | Working SSO login/logout |
| 5.2 | **Implement authorization policies** | Create `AdminOnly` and `InspectorOrAdmin` policies; apply `[Authorize]` attributes to pages; add `AuthorizeView` to `NavMenu.razor` | Role-based page access enforced |
| 5.3 | **Implement `EntraIdCurrentUserService`** | Read user identity from `AuthenticationStateProvider` claims; replace all `Session["UserId"]` / `Session["UserRole"]` usage | `ICurrentUserService` fully functional |
| 5.4 | **Implement `AzureEmailNotificationService`** | Replace `EmailHelper` stub with Azure Communication Services SDK; configure sender, templates | Email sending works end-to-end |
| 5.5 | **Configure Azure Key Vault** | Provision Key Vault; store connection strings, Entra ID secrets, email credentials; integrate `AddAzureKeyVault()` in `Program.cs` | Zero secrets in `appsettings.json` |
| 5.6 | **Configure Application Insights** | Add `Serilog.Sinks.ApplicationInsights`; configure structured logging; add custom telemetry for key operations | Logs visible in App Insights portal |
| 5.7 | **Configure Azure Cache for Redis** | Set up distributed session cache; configure output caching for dashboard stats | Redis cache active in non-dev environments |
| 5.8 | **Add health check endpoints** | Register health checks for Azure SQL, Redis, Key Vault; expose `/health` endpoint | Health endpoint returns component status |
| 5.9 | **Implement security middleware** | Add HSTS, HTTPS redirect, CSP, X-Frame-Options, X-Content-Type-Options headers | Security headers verified with scanner |
| 5.10 | **Replace `Global.asax` patterns** | Migrate `Application_Start` (session init) and `Application_Error` (error handling) to `Program.cs` middleware | No `Global.asax` patterns remain |
| 5.11 | **Configure connection string for Azure SQL** | Switch from LocalDB file-attached `.mdf` to Azure SQL connection string with Managed Identity authentication | App connects to Azure SQL |

### 7.2 Authentication Migration Map

| Legacy Pattern | Location | Modern Replacement |
|---|---|---|
| `Session["UserId"] = Guid.NewGuid()` | `Global.asax.cs:Session_Start` | Entra ID `oid` claim |
| `Session["UserName"] = "Test User"` | `Global.asax.cs:Session_Start` | Entra ID `name` claim |
| `Session["UserRole"] = "Applicant"` | `Global.asax.cs:Session_Start` | Entra ID `roles` claim from app registration |
| `lblUserName.Text = Session["UserName"]` | `Site.Master.cs:Page_Load` | `UserInfo.razor` → `AuthenticationStateProvider` |
| `lnkLogout` redirect | `Site.Master.cs` | `/MicrosoftIdentity/Account/SignOut` |
| `Session["SelectedPermitId"]` | `PermitSearch.aspx.cs` | URL route parameter |
| `Session["SubmittedPermitData"]` | `PermitApplication.aspx.cs` | Navigation with query parameter or component state |

### 7.3 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Entra ID app registration misconfiguration | Medium | High | Follow Microsoft identity platform quickstart; test with multiple user accounts and roles |
| Azure Communication Services email delivery failures | Medium | Medium | Configure domain verification; test with real email addresses; implement retry with exponential backoff |
| Key Vault access denied in deployed environment | Medium | High | Grant `Key Vault Secrets User` role to App Service Managed Identity before deployment; test locally with `az login` |
| Redis connection issues in dev environment | Low | Low | Use in-memory cache fallback for local development (`AddDistributedMemoryCache()`) |
| Application Insights data ingestion costs | Low | Medium | Configure sampling rate; set minimum log level to `Information` in production |

### 7.4 Success Criteria

- [ ] Users can log in via Microsoft Entra ID and see their name/role in the header
- [ ] Logout redirects to Entra ID sign-out and back to the app
- [ ] Admin-only pages (Dashboard) return 403 for non-admin users
- [ ] Inspector pages are accessible only to Inspector and Admin roles
- [ ] Email notifications send successfully via Azure Communication Services
- [ ] Connection strings and secrets are sourced from Key Vault (not `appsettings.json`)
- [ ] Application Insights shows request telemetry, dependency calls, and custom events
- [ ] `/health` endpoint returns healthy status for all dependencies
- [ ] Security headers present on all responses (verified with `curl -I` or securityheaders.com)

---

## 8. Phase 6 — Testing

**Duration:** 2–3 weeks
**Goal:** Achieve target test coverage across all layers and validate end-to-end workflows.

### 8.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 6.1 | **Domain unit tests (hardening)** | Complete coverage for all entity business rules, value objects, and edge cases beyond Phase 1 tests | ≥95% domain coverage |
| 6.2 | **Application unit tests** | Test validators (valid + invalid inputs), DTO mapping correctness | ≥90% application coverage |
| 6.3 | **Infrastructure integration tests** | Test all service methods against real database (TestContainers or SQLite); verify query correctness, transaction behavior | ≥80% infrastructure coverage |
| 6.4 | **Blazor component tests (bUnit)** | Test page components: rendering, user interaction, service injection via mocks, form validation display | ≥80% component coverage |
| 6.5 | **End-to-end tests (Playwright)** | Automate critical user journeys: submit permit, search permit, schedule inspection, complete inspection, submit review, view dashboard | All critical paths passing |
| 6.6 | **Legacy parity validation** | Side-by-side comparison: run the same scenarios in legacy and modern apps; verify identical outcomes | Parity report with all scenarios passing |
| 6.7 | **Performance baseline** | Benchmark key operations (page load, search, submit) against legacy response times | Modern app ≤ legacy response times |
| 6.8 | **Security testing** | OWASP ZAP scan or manual security review: XSS, CSRF, injection, auth bypass | Zero high/critical findings |
| 6.9 | **Accessibility audit** | Verify WCAG 2.1 AA compliance: keyboard navigation, screen reader, color contrast | Accessibility report generated |

### 8.2 Test Coverage Targets

| Layer | Project | Target Coverage | Test Framework |
|---|---|---|---|
| Domain | `RiverdalePermitSystem.Domain.Tests` | ≥ 95% | xUnit + FluentAssertions |
| Application | `RiverdalePermitSystem.Application.Tests` | ≥ 90% | xUnit + Moq + FluentAssertions |
| Infrastructure | `RiverdalePermitSystem.Infrastructure.Tests` | ≥ 80% | xUnit + EF Core InMemory + TestContainers |
| Presentation | `RiverdalePermitSystem.Web.Tests` | ≥ 80% | bUnit + xUnit |
| E2E | `RiverdalePermitSystem.E2E.Tests` | Critical paths | Playwright |

### 8.3 Critical E2E Test Scenarios

| # | Scenario | Steps | Expected Outcome |
|---|---|---|---|
| E2E-1 | **Submit new permit** | Login → Apply → Fill 4 steps → Submit | Permit created with `PERM-YYYY-NNNN` ID; fee calculated correctly; confirmation shown |
| E2E-2 | **Search and view permit** | Login → Search → Enter permit ID → View details | Permit details displayed with all fields; applicant info visible |
| E2E-3 | **Schedule inspection** | Login as Inspector → Inspections → Fill form → Schedule | Inspection scheduled; inspector auto-assigned; shows in upcoming list |
| E2E-4 | **Complete inspection (pass)** | Login as Inspector → Find scheduled → Complete with Pass | Status updated; if Final, permit gets CO status |
| E2E-5 | **Submit plan review** | Login as Reviewer → Reviews → Select permit → Submit review | Review recorded; if all approved, permit status updates |
| E2E-6 | **Admin dashboard** | Login as Admin → Dashboard | Statistics displayed; activity feed populated; status breakdown shown |
| E2E-7 | **Authorization enforcement** | Login as Applicant → Navigate to `/dashboard` | 403 Forbidden or redirect to unauthorized page |

### 8.4 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Flaky E2E tests due to async Blazor rendering | High | Medium | Use Playwright auto-waiting; add explicit `WaitForSelectorAsync` for Blazor renders; set reasonable timeouts |
| Legacy parity edge cases discovered late | Medium | Medium | Start parity testing in Phase 3 as each page is completed; don't defer all validation to Phase 6 |
| EF Core InMemory provider behavior differs from SQL Server | Medium | Medium | Use TestContainers with SQL Server Docker image for critical integration tests |
| Test data management complexity | Low | Medium | Use factory pattern for test data; reset database state between tests |

### 8.5 Success Criteria

- [ ] All unit test suites pass (`dotnet test` succeeds)
- [ ] Domain coverage ≥ 95%, Application ≥ 90%, Infrastructure ≥ 80%, Presentation ≥ 80%
- [ ] All 7 critical E2E scenarios pass on Playwright
- [ ] Legacy parity validated: identical outcomes for all documented legacy behaviors
- [ ] Performance: modern app response times ≤ legacy baseline for all key operations
- [ ] Security scan: zero high/critical findings
- [ ] Accessibility: WCAG 2.1 AA compliance for all pages

---

## 9. Phase 7 — Deployment

**Duration:** 1.5–2 weeks
**Goal:** Provision Azure infrastructure, set up CI/CD pipelines, perform staged deployment, and execute production cutover.

### 9.1 Tasks

| # | Task | Description | Output |
|---|---|---|---|
| 7.1 | **Author Bicep templates** | Define Azure resources in `infra/main.bicep`: App Service, Azure SQL, Redis, Key Vault, Communication Services, App Insights, Log Analytics | IaC templates for all environments |
| 7.2 | **Create parameter files** | Define `parameters.dev.json`, `parameters.staging.json`, `parameters.prod.json` with environment-specific SKUs | Parameter files for 3 environments |
| 7.3 | **Provision dev environment** | Deploy Bicep to `rg-riverdale-dev`; verify all resources created correctly | Dev environment running |
| 7.4 | **Configure GitHub Actions CI** | Create `.github/workflows/ci.yml`: checkout → restore → build → test on every PR | CI pipeline passing |
| 7.5 | **Configure GitHub Actions CD** | Create `.github/workflows/cd.yml`: build → test → publish → deploy to staging slot → manual approval → swap to production | CD pipeline ready |
| 7.6 | **Set up deployment slots** | Configure staging slot on Azure App Service; set slot-specific settings (connection strings, App Insights) | Staging slot accessible |
| 7.7 | **Deploy to staging** | Run CD pipeline to staging slot; verify app starts, connects to Azure SQL, health checks pass | Staging deployment successful |
| 7.8 | **Run migration on Azure SQL** | Execute `dotnet ef database update` against Azure SQL in staging; verify schema and seed data | Database schema created in Azure SQL |
| 7.9 | **Staging validation** | Execute full E2E test suite against staging environment; validate all integrations (email, auth, caching) | All tests pass on staging |
| 7.10 | **Data migration (if applicable)** | If legacy production data exists, create and test data migration script from legacy schema to new schema | Data migrated and verified |
| 7.11 | **Production cutover** | Swap staging → production slot; verify health checks; monitor for 30 minutes | Production live and healthy |
| 7.12 | **DNS and certificate** | Configure custom domain and managed TLS certificate on Azure App Service | Custom domain with HTTPS |
| 7.13 | **Monitoring and alerting** | Configure Azure Monitor alerts: CPU > 80%, memory > 80%, response time > 2s, 5xx error rate > 1% | Alerts active |
| 7.14 | **Rollback plan** | Document rollback procedure: slot swap back to previous version; test rollback in staging | Rollback tested and documented |
| 7.15 | **Decommission legacy** | After 2-week parallel run, decommission legacy IIS deployment and LocalDB | Legacy system offline |

### 9.2 Azure Resource Provisioning Checklist

| Resource | Bicep Resource Type | SKU | Environment Config |
|---|---|---|---|
| Resource Group | `Microsoft.Resources/resourceGroups` | — | `rg-riverdale-{env}` |
| App Service Plan | `Microsoft.Web/serverfarms` | Dev: B1, Staging: S1, Prod: P1v3 | Linux, .NET 9 |
| App Service | `Microsoft.Web/sites` | — | System-assigned Managed Identity |
| Azure SQL Server | `Microsoft.Sql/servers` | — | Entra ID admin; firewall rules |
| Azure SQL Database | `Microsoft.Sql/servers/databases` | Dev: Basic, Staging: S1, Prod: S2 | 7-day backup retention |
| Azure Cache for Redis | `Microsoft.Cache/redis` | Dev: Basic C0, Prod: Standard C1 | Entra ID auth |
| Azure Key Vault | `Microsoft.KeyVault/vaults` | Standard | RBAC access model |
| Azure Communication Services | `Microsoft.Communication/communicationServices` | Pay-as-you-go | Email domain verified |
| Application Insights | `Microsoft.Insights/components` | Pay-as-you-go | Connected to Log Analytics |
| Log Analytics Workspace | `Microsoft.OperationalInsights/workspaces` | Pay-as-you-go | 30-day retention |

### 9.3 Deployment Pipeline Stages

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        GitHub Actions Pipeline                          │
│                                                                         │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌─────────────────┐  │
│  │  Build    │───►│  Test    │───►│ Deploy   │───►│ Deploy          │  │
│  │  & Lint   │    │  (all    │    │ Staging  │    │ Production      │  │
│  │          │    │  suites) │    │ (auto)   │    │ (manual gate)   │  │
│  └──────────┘    └──────────┘    └──────────┘    └─────────────────┘  │
│       │               │               │                  │             │
│  dotnet build    dotnet test     Publish +          Slot swap          │
│  dotnet format   + coverage     Deploy to           staging →          │
│                  report         staging slot        production          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 9.4 Rollback Strategy

| Trigger | Action | RTO |
|---|---|---|
| Health check failure post-swap | Automatic slot swap revert | < 2 minutes |
| Critical bug discovered in production | Manual slot swap to previous version via Azure CLI | < 5 minutes |
| Data corruption | Restore Azure SQL from point-in-time backup | < 30 minutes |
| Complete infrastructure failure | Redeploy from Bicep templates + restore DB backup | < 2 hours |

### 9.5 Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Azure SQL connection from App Service fails | Medium | High | Test Managed Identity connectivity before cutover; have connection string fallback |
| Deployment slot swap causes cold start latency | Medium | Medium | Configure slot warm-up; use "Always On" setting |
| Data migration from legacy production DB | Medium | High | Write idempotent migration script; test against copy of production data; plan for maintenance window |
| DNS propagation delay during cutover | Low | Medium | Set low TTL (60s) on DNS records 48 hours before cutover |
| Costs exceed budget | Low | Medium | Use Azure Cost Management alerts; start with minimum SKUs; scale based on actual usage |

### 9.6 Success Criteria

- [ ] All Azure resources provisioned via Bicep (no manual portal clicks in production)
- [ ] CI pipeline runs on every PR and blocks merge on failure
- [ ] CD pipeline deploys to staging automatically on merge to `main`
- [ ] Production deployment requires manual approval gate
- [ ] Health checks pass in production (`/health` returns 200)
- [ ] Application Insights shows request telemetry from production traffic
- [ ] Azure Monitor alerts fire correctly (tested with synthetic load)
- [ ] Rollback tested: slot swap revert completes in < 5 minutes
- [ ] Custom domain with valid TLS certificate
- [ ] Legacy system decommissioned after parallel run

---

## 10. Cross-Phase Risk Register

### 10.1 Top Risks by Severity

| # | Risk | Phase | Likelihood | Impact | Severity | Mitigation |
|---|---|---|---|---|---|---|
| R1 | Fee calculation parity failure (triple-duplicated logic) | 1, 2 | High | High | **Critical** | Exhaustive 24-combination test suite using legacy SP output as oracle |
| R2 | Permit Application wizard complexity | 3 | High | High | **Critical** | Decompose into sub-components; test per-step; allocate 5–7 days |
| R3 | Entra ID configuration errors blocking auth | 5 | Medium | High | **High** | Follow quickstart; test with 3+ user accounts and all roles |
| R4 | Data migration from legacy production | 7 | Medium | High | **High** | Idempotent migration script; test on production copy; maintenance window |
| R5 | EF Core query performance regression | 2 | Medium | Medium | **Medium** | Compiled queries; benchmark vs. legacy; add indexes proactively |
| R6 | Blazor Server SignalR circuit drops | 3, 5 | Medium | Medium | **Medium** | Configure reconnection UI; test with poor network; set appropriate timeouts |
| R7 | Test environment drift from production | 6, 7 | Medium | Medium | **Medium** | Use same Bicep templates for all environments; parameterize SKUs only |
| R8 | Scope creep (adding features during migration) | All | High | Medium | **Medium** | Strict feature-parity-first rule; new features tracked in separate backlog |

### 10.2 Risk Response Plan

| Response | When to Trigger | Action |
|---|---|---|
| **Escalate** | Critical risk materializes (R1, R2) | Pause migration; investigate root cause; involve senior developer |
| **Extend timeline** | Phase exceeds estimate by > 50% | Communicate delay; reassess remaining phases; consider parallelization |
| **Descope** | Scope creep threatens delivery (R8) | Move non-essential features to post-migration backlog |
| **Rollback** | Production deployment fails (R4) | Execute rollback plan per §9.4 |

---

## 11. Dependency Graph

```
Phase 0 (Preparation)
    │
    ▼
Phase 1 (Foundation)
    │
    ├──────────────────────┐
    ▼                      ▼
Phase 2 (Data Layer)    Phase 4 (Shared Components)
    │                      │
    ▼                      │
Phase 3 (Core Pages) ◄────┘
    │
    ▼
Phase 5 (Integration)
    │
    ▼
Phase 6 (Testing)
    │
    ▼
Phase 7 (Deployment)
```

**Key Dependencies:**
- Phase 1 → Phase 2: Domain entities and DbContext must exist before services can be implemented
- Phase 1 → Phase 4: Shared components need the Blazor shell and domain types
- Phase 2 → Phase 3: Pages need working services to bind data
- Phase 4 → Phase 3: Pages consume shared components (but stubs can be used)
- Phases 3 + 4 + 5 → Phase 6: Full test suite requires all features implemented
- Phase 6 → Phase 7: Deployment requires all tests passing

**Parallel Opportunities:**
- Phases 2 and 4 can execute simultaneously after Phase 1
- Phase 3 pages can be developed in parallel by different developers
- Phase 5 integration tasks (5.1–5.3 auth, 5.4 email, 5.6 logging) are independent of each other
- Phase 6 unit tests should be written alongside Phases 1–3, with Phase 6 as a hardening sprint

---

*This phased migration plan was generated as part of the Spec2Cloud modernization methodology. It provides the execution roadmap for migrating the Riverdale City Building Permit System from ASP.NET Web Forms / .NET Framework 4.8 to .NET 9 / Blazor Server / EF Core / Azure. Each phase builds incrementally on the previous, with verification gates ensuring quality at every stage.*
