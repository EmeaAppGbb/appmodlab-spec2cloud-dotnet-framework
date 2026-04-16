# Step 4: Component Specifications — Riverdale City Building Permit System

> **Generated:** 2026-04-16 | **Methodology:** Spec2Cloud Component Specification
> **Application:** Riverdale City Building Permit System
> **Current Platform:** ASP.NET Web Forms / .NET Framework 4.8
> **Input:** Step 3 — Architecture Specification

---

## Table of Contents

1. [Web Forms Pages](#1-web-forms-pages)
   - 1.1 [Default.aspx — Home Page](#11-defaultaspx--home-page)
   - 1.2 [Pages/Dashboard.aspx — Administrator Dashboard](#12-pagesdashboardaspx--administrator-dashboard)
   - 1.3 [Pages/PermitApplication.aspx — Permit Application Wizard](#13-pagespermitapplicationaspx--permit-application-wizard)
   - 1.4 [Pages/PermitSearch.aspx — Permit Search & Detail](#14-pagespermitsearchaspx--permit-search--detail)
   - 1.5 [Pages/InspectionSchedule.aspx — Inspection Scheduling](#15-pagesinspectionscheduleaspx--inspection-scheduling)
   - 1.6 [Pages/PlanReview.aspx — Plan Review Management](#16-pagesplanreviewaspx--plan-review-management)
2. [Master Pages](#2-master-pages)
   - 2.1 [MasterPages/Site.Master — Application Shell](#21-masterpagessitemaster--application-shell)
3. [User Controls](#3-user-controls)
   - 3.1 [UserControls/AddressLookup.ascx — Address Autocomplete](#31-usercontrolsaddresslookupascx--address-autocomplete)
   - 3.2 [UserControls/PermitHeader.ascx — Permit Summary Display](#32-usercontrolspermitheaderascx--permit-summary-display)
4. [Data Access Layer](#4-data-access-layer)
   - 4.1 [App_Code/PermitDataAccess.cs — Permit Data Operations](#41-app_codepermitdataccesscs--permit-data-operations)
   - 4.2 [App_Code/InspectionDataAccess.cs — Inspection Data Operations](#42-app_codeinspectiondataccesscs--inspection-data-operations)
5. [Utilities](#5-utilities)
   - 5.1 [App_Code/EmailHelper.cs — Email Notification Service](#51-app_codeemailhelpercs--email-notification-service)
6. [Database Components](#6-database-components)
   - 6.1 [Database/Schema.sql — Database Schema](#61-dabortschemasql--database-schema)
   - 6.2 [Database/StoredProcedures/PermitProcedures.sql — Stored Procedures](#62-dabortstoredprocedures--stored-procedures)
7. [Configuration](#7-configuration)
   - 7.1 [Web.config — Application Configuration](#71-webconfig--application-configuration)
   - 7.2 [Global.asax / Global.asax.cs — Application Lifecycle](#72-globalasax--globalasaxcs--application-lifecycle)
8. [Styling](#8-styling)
   - 8.1 [Styles/Site.css — Application Stylesheet](#81-stylessitecss--application-stylesheet)

---

## 1. Web Forms Pages

### 1.1 Default.aspx — Home Page

| Attribute | Detail |
|---|---|
| **File** | `Default.aspx` + `Default.aspx.cs` |
| **LOC** | 55 (markup) + 42 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web` |
| **Inherits** | `System.Web.UI.Page` |
| **Master Page** | `~/MasterPages/Site.Master` |
| **Page Title** | `"Home"` |

#### Responsibility

Landing page for the Riverdale Building Permit System. Displays quick-action navigation links and the 10 most recently submitted permits in a refreshable grid. Serves as the entry point for all user roles.

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Quick Action: Apply | `HyperLink` | `lnkNewPermit` | Navigates to `~/Pages/PermitApplication.aspx`; styled as `button-primary` |
| Quick Action: Search | `HyperLink` | `lnkSearchPermits` | Navigates to `~/Pages/PermitSearch.aspx`; styled as `button-secondary` |
| Recent Permits Grid | `GridView` | `gvRecentPermits` | Displays recent permits with columns: PermitId, ApplicationDate (MM/dd/yyyy), PropertyAddress, PermitType, Status, EstimatedCost (currency), View button |
| Refresh Button | `Button` | `btnRefresh` | Triggers manual reload of the recent permits grid |
| Last Updated Label | `Label` | `lblLastUpdate` | Shows timestamp of last data refresh |
| Recent Permits Panel | `UpdatePanel` | `upRecentPermits` | Wraps grid for AJAX partial postback; `UpdateMode="Conditional"` |

**Static Content:**
- List of available permit types (6 types: Building New Construction, Building Addition/Alteration, Electrical, Plumbing, Mechanical, Demolition)
- Processing time note (5–10 business days)
- Support contact information (email + phone)

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Page lifecycle | On first load (`!IsPostBack`): calls `LoadRecentPermits()` |
| `LoadRecentPermits` | `private` | Internal | Calls `PermitDataAccess.GetRecentPermits(10)`, binds result to `gvRecentPermits`, updates `lblLastUpdate` with current timestamp |
| `btnRefresh_Click` | `protected` | Button click | Calls `LoadRecentPermits()` to reload grid data |
| `gvRecentPermits_RowCommand` | `protected` | GridView `RowCommand` | On `"ViewDetails"` command: extracts `PermitId` from row cells[0], redirects to `~/Pages/PermitSearch.aspx?permitId={id}` |

#### ViewState Usage

- **None explicit.** GridView relies on default ViewState for maintaining row data across postbacks.

#### Event Handlers

| Event | Handler | Behavior |
|---|---|---|
| `btnRefresh.Click` | `btnRefresh_Click` | Reloads grid via UpdatePanel partial postback |
| `gvRecentPermits.RowCommand` | `gvRecentPermits_RowCommand` | Navigates to detail view on "View" button click |

#### Data Bindings

| Control | Source | Method |
|---|---|---|
| `gvRecentPermits` | `DataTable` from `PermitDataAccess.GetRecentPermits(10)` | Manual `DataSource`/`DataBind()` |

#### Server Controls Used

`GridView`, `UpdatePanel`, `HyperLink` (×2), `Button`, `Label` (×2), `BoundField` (×6), `ButtonField` (×1)

#### Dependencies

| Dependency | Type | Detail |
|---|---|---|
| `PermitDataAccess.GetRecentPermits()` | Static method | Returns DataTable of recent permits |
| `Site.Master` | Master page | Layout, navigation, ScriptManager |

#### Anti-Patterns

1. **Direct cell index access** — `row.Cells[0].Text` in `RowCommand` is brittle; column reordering breaks navigation
2. **Magic number** — Hardcoded `10` for recent permit count; not configurable
3. **No error handling** — `LoadRecentPermits()` has no try/catch; data access failures will show unhandled exception
4. **Full redirect from UpdatePanel** — `Response.Redirect` from within an UpdatePanel causes partial postback issues

#### Modernization Notes

- Replace GridView with Blazor `<table>` component or `QuickGrid` with server-side paging
- Replace UpdatePanel with Blazor Server SignalR diffing (automatic)
- Extract "recent permits count" to configuration/constant
- Replace `Response.Redirect` with `NavigationManager.NavigateTo()`
- Add loading skeleton/spinner for async data fetch

---

### 1.2 Pages/Dashboard.aspx — Administrator Dashboard

| Attribute | Detail |
|---|---|
| **File** | `Pages/Dashboard.aspx` + `Pages/Dashboard.aspx.cs` |
| **LOC** | 63 (markup) + 44 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.Pages` |
| **Inherits** | `System.Web.UI.Page` |
| **Master Page** | `~/MasterPages/Site.Master` |
| **Page Title** | `"Dashboard"` |
| **URL Route** | `/dashboard` (mapped in `RouteConfig`) |

#### Responsibility

Administrative dashboard displaying aggregate permit statistics (total permits, pending review, inspections today, monthly revenue), a recent activity feed, and a permit status breakdown summary. Intended for department managers and clerks.

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Total Permits Card | `Label` | `lblTotalPermits` | Stat card: total permit count |
| Pending Review Card | `Label` | `lblPendingReview` | Stat card: pending review count |
| Inspections Today Card | `Label` | `lblInspectionsToday` | Stat card: today's inspection count |
| Monthly Revenue Card | `Label` | `lblMonthlyRevenue` | Stat card: current month revenue (formatted as currency) |
| Refresh Button | `Button` | `btnRefreshStats` | Triggers reload of all dashboard data |
| Last Updated Label | `Label` | `lblLastUpdate` | Timestamp of last refresh |
| Dashboard Panel | `UpdatePanel` | `upDashboard` | Wraps stat cards for partial postback; `UpdateMode="Conditional"` |
| Recent Activity Grid | `GridView` | `gvRecentActivity` | Columns: Timestamp (MM/dd/yyyy HH:mm), ActivityType, PermitId, Description, UserName |
| Status Summary Grid | `GridView` | `gvStatusSummary` | Columns: Status, Count, TotalValue (currency); wrapped in `UpdatePanel` `upStatusChart` |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Page lifecycle | On first load: calls `LoadDashboardData()` |
| `LoadDashboardData` | `private` | Internal | (1) Gets stats via `PermitDataAccess.GetDashboardStatistics()` → populates 4 stat labels from row[0]. (2) Gets activity via `PermitDataAccess.GetRecentActivity(20)` → binds to `gvRecentActivity`. (3) Gets status summary via `PermitDataAccess.GetPermitsByStatus()` → binds to `gvStatusSummary`. (4) Updates `lblLastUpdate`. |
| `btnRefreshStats_Click` | `protected` | Button click | Calls `LoadDashboardData()` |

#### ViewState Usage

- **None explicit.** Default ViewState maintains GridView state.

#### Event Handlers

| Event | Handler |
|---|---|
| `btnRefreshStats.Click` | `btnRefreshStats_Click` — reloads all data |

#### Data Bindings

| Control | Source | Binding Method |
|---|---|---|
| Stat Labels (×4) | `PermitDataAccess.GetDashboardStatistics()` row[0] | Manual property assignment |
| `gvRecentActivity` | `PermitDataAccess.GetRecentActivity(20)` | Manual `DataSource`/`DataBind()` |
| `gvStatusSummary` | `PermitDataAccess.GetPermitsByStatus()` | Manual `DataSource`/`DataBind()` |

#### Server Controls Used

`UpdatePanel` (×2), `GridView` (×2), `Button`, `Label` (×6), `BoundField` (×10)

#### Dependencies

| Dependency | Type |
|---|---|
| `PermitDataAccess.GetDashboardStatistics()` | Static method |
| `PermitDataAccess.GetRecentActivity()` | Static method |
| `PermitDataAccess.GetPermitsByStatus()` | Static method |
| `Site.Master` | Master page |

#### Anti-Patterns

1. **No access control** — Dashboard is accessible to all users despite showing admin-level data (revenue, all permits)
2. **Fragile DataTable column access** — `stats.Rows[0]["TotalPermits"]` with no null checks on column existence
3. **Unchecked `decimal.Parse`** — `decimal.Parse(stats.Rows[0]["MonthlyRevenue"].ToString())` will throw `FormatException` on unexpected data
4. **No error handling** — Any data access failure in `LoadDashboardData()` crashes the entire page
5. **Magic number** — `20` for activity count is hardcoded

#### Modernization Notes

- Implement RBAC: restrict to Admin/Manager roles
- Replace stat cards with Blazor components using `@inject` services
- Add real-time updates via SignalR for live dashboard
- Replace GridViews with `QuickGrid` or custom Blazor table components
- Add charts/visualizations for status summary (e.g., Radzen chart component)
- Add auto-refresh interval with configurable polling

---

### 1.3 Pages/PermitApplication.aspx — Permit Application Wizard

| Attribute | Detail |
|---|---|
| **File** | `Pages/PermitApplication.aspx` + `Pages/PermitApplication.aspx.cs` |
| **LOC** | 187 (markup) + 182 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.Pages` |
| **Inherits** | `System.Web.UI.Page` |
| **Master Page** | `~/MasterPages/Site.Master` |
| **Page Title** | `"Apply for Permit"` |
| **URL Route** | `/apply` (mapped in `RouteConfig`) |
| **Page Directive** | `EnableViewState="true"` (explicit) |

#### Responsibility

Four-step wizard for submitting new building permit applications. Manages complex form state across steps using ViewState, validates input at each step, calculates fees, and submits the application via data access layer with email confirmation.

#### UI Elements — Step 1: Property Information

| Control | Type | ID | Purpose |
|---|---|---|---|
| Address Lookup | User Control (`AddressLookup.ascx`) | `ucAddress` | Address entry with simulated autocomplete |
| Parcel Number | `TextBox` | `txtParcelNumber` | Parcel number input (max 50 chars) |
| Zoning District | `DropDownList` | `ddlZoning` | Options: R1, R2, C1, I1 |
| Next Button | `Button` | `btnStep1Next` | Validates Step1 group, saves to ViewState, advances wizard |
| Address Validator | `RequiredFieldValidator` | `rfvAddress` | Validates `ucAddress$txtAddress`; `ValidationGroup="Step1"` |
| Parcel Validator | `RequiredFieldValidator` | `rfvParcel` | Validates `txtParcelNumber`; `ValidationGroup="Step1"` |
| Zoning Validator | `RequiredFieldValidator` | `rfvZoning` | Validates `ddlZoning` (InitialValue=""); `ValidationGroup="Step1"` |

#### UI Elements — Step 2: Applicant Details

| Control | Type | ID | Purpose |
|---|---|---|---|
| Applicant Name | `TextBox` | `txtApplicantName` | Full name (max 100 chars) |
| Email | `TextBox` | `txtEmail` | Email address (max 100 chars) |
| Phone | `TextBox` | `txtPhone` | Phone number (max 20 chars) |
| Company | `TextBox` | `txtCompany` | Company name (max 100 chars, optional) |
| License Number | `TextBox` | `txtLicenseNumber` | Contractor license (max 50 chars, optional) |
| Previous Button | `Button` | `btnStep2Prev` | Returns to Step 1; `CausesValidation="false"` |
| Next Button | `Button` | `btnStep2Next` | Validates Step2, saves ViewState, advances |
| Name Validator | `RequiredFieldValidator` | `rfvApplicantName` | `ValidationGroup="Step2"` |
| Email Required | `RequiredFieldValidator` | `rfvEmail` | `ValidationGroup="Step2"` |
| Email Format | `RegularExpressionValidator` | `revEmail` | Pattern: `\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*`; `ValidationGroup="Step2"` |
| Phone Validator | `RequiredFieldValidator` | `rfvPhone` | `ValidationGroup="Step2"` |

#### UI Elements — Step 3: Project Details

| Control | Type | ID | Purpose |
|---|---|---|---|
| Permit Type | `DropDownList` | `ddlPermitType` | 6 types + empty option; `AutoPostBack="true"` triggers fee recalculation |
| Project Description | `TextBox` (MultiLine) | `txtProjectDescription` | 5 rows, max 1000 chars |
| Estimated Cost | `TextBox` | `txtEstimatedCost` | Numeric (max 15 chars) |
| Square Footage | `TextBox` | `txtSquareFootage` | Optional (max 10 chars) |
| Estimated Fee Label | `Label` | `lblEstimatedFee` | Displays calculated fee; initial `"$0.00"` |
| Calculate Fee Button | `Button` | `btnCalculateFee` | Triggers fee calculation; `CausesValidation="false"` |
| Fee Calculation Panel | `UpdatePanel` | `upFeeCalculation` | Wraps fee display; `AsyncPostBackTrigger` on `btnCalculateFee` |
| Previous/Next Buttons | `Button` | `btnStep3Prev` / `btnStep3Next` | Navigation; Next validates `ValidationGroup="Step3"` |
| Type Validator | `RequiredFieldValidator` | `rfvPermitType` | `ValidationGroup="Step3"` |
| Description Validator | `RequiredFieldValidator` | `rfvDescription` | `ValidationGroup="Step3"` |
| Cost Required | `RequiredFieldValidator` | `rfvCost` | `ValidationGroup="Step3"` |
| Cost Range | `RangeValidator` | `rvCost` | Min 1, Max 99999999, Type Double; `ValidationGroup="Step3"` |

#### UI Elements — Step 4: Review & Submit

| Control | Type | ID | Purpose |
|---|---|---|---|
| Review Panel | `Panel` | `pnlReview` | Displays all collected data in read-only labels |
| Review Labels (×10) | `Label` | `lblReviewAddress`, `lblReviewParcel`, `lblReviewZoning`, `lblReviewApplicant`, `lblReviewEmail`, `lblReviewPhone`, `lblReviewPermitType`, `lblReviewDescription`, `lblReviewCost`, `lblReviewFee` | Read-only summary of all wizard steps |
| Submit Message Panel | `Panel` | `pnlSubmitMessage` | Initially `Visible="false"`; shown on successful submit |
| Permit ID | `Label` | `lblPermitId` | Displays generated permit ID after submission |
| Confirmation Email | `Label` | `lblConfirmEmail` | Shows email that will receive confirmation |
| Track Permit Link | `HyperLink` | `lnkTrackPermit` | Links to `~/Pages/PermitSearch.aspx` |
| Previous Button | `Button` | `btnStep4Prev` | Returns to Step 3; hidden after submit |
| Submit Button | `Button` | `btnSubmit` | Submits application; `CausesValidation="false"`; hidden after submit |

#### Wizard Infrastructure

| Control | Type | ID | Purpose |
|---|---|---|---|
| MultiView | `MultiView` | `mvWizard` | Container for 4 `View` panels; `ActiveViewIndex="0"` |
| Step Views | `View` (×4) | `vwStep1`–`vwStep4` | Individual wizard step containers |
| Step Indicators | `HtmlGenericControl` (div, ×4) | `step1Indicator`–`step4Indicator` | Visual step progress; CSS classes: `wizard-step`, `active`, `completed` |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | First load: calls `UpdateWizardSteps()` |
| `UpdateWizardSteps` | `private` | Internal | Sets CSS class on each step indicator based on `mvWizard.ActiveViewIndex`: `"active"` for current, `"completed"` for previous, default for future |
| `btnStep1Next_Click` | `protected` | Button | If valid: `SaveToViewState()`, set `ActiveViewIndex=1`, `UpdateWizardSteps()` |
| `btnStep2Prev_Click` | `protected` | Button | Set `ActiveViewIndex=0`, `UpdateWizardSteps()` |
| `btnStep2Next_Click` | `protected` | Button | If valid: `SaveToViewState()`, set `ActiveViewIndex=2`, `UpdateWizardSteps()` |
| `btnStep3Prev_Click` | `protected` | Button | Set `ActiveViewIndex=1`, `UpdateWizardSteps()` |
| `btnStep3Next_Click` | `protected` | Button | If valid: `SaveToViewState()`, `LoadReviewData()`, set `ActiveViewIndex=3`, `UpdateWizardSteps()` |
| `btnStep4Prev_Click` | `protected` | Button | Set `ActiveViewIndex=2`, `UpdateWizardSteps()` |
| `ddlPermitType_SelectedIndexChanged` | `protected` | AutoPostBack | Calls `CalculateFee()` |
| `btnCalculateFee_Click` | `protected` | Button | Calls `CalculateFee()` |
| `CalculateFee` | `private` | Internal | Parses `txtEstimatedCost.Text` to decimal; if valid, calls `PermitDataAccess.CalculatePermitFee()`, displays result in `lblEstimatedFee`, stores in `ViewState["CalculatedFee"]` |
| `SaveToViewState` | `private` | Internal | Saves all 12 form fields to ViewState keys: `PropertyAddress`, `ParcelNumber`, `Zoning`, `ApplicantName`, `Email`, `Phone`, `Company`, `LicenseNumber`, `PermitType`, `ProjectDescription`, `EstimatedCost`, `SquareFootage` |
| `LoadReviewData` | `private` | Internal | Reads all ViewState keys and populates 10 review labels |
| `btnSubmit_Click` | `protected` | Button | (1) Creates `DataTable` for applicant data with 5 columns (Name/Email/Phone/Company/LicenseNumber), populates from ViewState. (2) Calls `PermitDataAccess.SubmitPermitApplication()` with all collected data. (3) Calls `EmailHelper.SendPermitConfirmation()`. (4) Shows success panel, hides review/buttons. (5) Stores `DataTable` in `Session["SubmittedPermitData"]`. (6) On exception: `Response.Write` alert with `ex.Message` (XSS vulnerability). |

#### ViewState Usage

| Key | Type | Set By | Read By | Purpose |
|---|---|---|---|---|
| `PropertyAddress` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Property address from AddressLookup control |
| `ParcelNumber` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Parcel number |
| `Zoning` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Selected zoning district |
| `ApplicantName` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Applicant full name |
| `Email` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Applicant email |
| `Phone` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Applicant phone |
| `Company` | `string` | `SaveToViewState()` | `btnSubmit_Click` | Company name (optional) |
| `LicenseNumber` | `string` | `SaveToViewState()` | `btnSubmit_Click` | Contractor license (optional) |
| `PermitType` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Selected permit type |
| `ProjectDescription` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Project description text |
| `EstimatedCost` | `string` | `SaveToViewState()` | `LoadReviewData()`, `btnSubmit_Click` | Estimated project cost |
| `SquareFootage` | `string` | `SaveToViewState()` | `btnSubmit_Click` | Square footage (optional) |
| `CalculatedFee` | `decimal` | `CalculateFee()` | `LoadReviewData()` | Computed permit fee |

**Total: 13 ViewState entries** — All form data is round-tripped through encrypted ViewState on every postback.

#### Event Handlers

| Event | Handler | ValidationGroup |
|---|---|---|
| `btnStep1Next.Click` | `btnStep1Next_Click` | `Step1` |
| `btnStep2Prev.Click` | `btnStep2Prev_Click` | None |
| `btnStep2Next.Click` | `btnStep2Next_Click` | `Step2` |
| `btnStep3Prev.Click` | `btnStep3Prev_Click` | None |
| `btnStep3Next.Click` | `btnStep3Next_Click` | `Step3` |
| `btnStep4Prev.Click` | `btnStep4Prev_Click` | None |
| `btnCalculateFee.Click` | `btnCalculateFee_Click` | None |
| `btnSubmit.Click` | `btnSubmit_Click` | None |
| `ddlPermitType.SelectedIndexChanged` | `ddlPermitType_SelectedIndexChanged` | None (AutoPostBack) |

#### Server Controls Used

`MultiView`, `View` (×4), `UpdatePanel`, `TextBox` (×8), `DropDownList` (×3), `Button` (×8), `Label` (×13), `Panel` (×2), `HyperLink`, `RequiredFieldValidator` (×7), `RegularExpressionValidator` (×1), `RangeValidator` (×1), `AsyncPostBackTrigger`, User Control `AddressLookup` (×1)

#### Dependencies

| Dependency | Type | Detail |
|---|---|---|
| `PermitDataAccess.CalculatePermitFee()` | Static method | Fee calculation with permit type + cost |
| `PermitDataAccess.SubmitPermitApplication()` | Static method | Submits permit; returns permit ID |
| `EmailHelper.SendPermitConfirmation()` | Static method | Sends confirmation email |
| `AddressLookup.ascx` | User control | Address entry with simulated autocomplete |
| `Site.Master` | Master page | Layout + ScriptManager |

#### Anti-Patterns

1. **🔴 XSS vulnerability** — `Response.Write($"<script>alert('Error: {ex.Message}');</script>")` in catch block injects unsanitized exception message into JavaScript
2. **Excessive ViewState** — 13 fields round-tripped on every postback; encrypted ViewState adds CPU overhead and payload size
3. **No server-side re-validation on submit** — `btnSubmit` has `CausesValidation="false"`; relies on prior step validation which may be bypassed
4. **DataTable as transfer object** — `DataTable` constructed manually for applicant data instead of a typed model
5. **Session pollution** — `Session["SubmittedPermitData"]` stores DataTable after submit; never cleaned up
6. **No idempotency protection** — Double-click or refresh after submit could cause duplicate submissions
7. **Unchecked `decimal.Parse`** — `ViewState["EstimatedCost"]` parsed without `TryParse` in `LoadReviewData()`
8. **Cross-control validator path** — `rfvAddress` validates `ucAddress$txtAddress` using naming container path string (fragile)

#### Modernization Notes

- Replace MultiView wizard with Blazor `Stepper` component or multi-step form with component state
- Replace ViewState with component-level C# state (`private` fields) in Blazor Server
- Add `[Required]`, `[EmailAddress]`, `[Range]` data annotations on a `PermitApplicationModel` DTO
- Implement anti-forgery tokens and idempotency key for submission
- Replace `Response.Write` error handling with proper Blazor error boundaries
- Extract fee calculation to a domain service behind an interface

---

### 1.4 Pages/PermitSearch.aspx — Permit Search & Detail

| Attribute | Detail |
|---|---|
| **File** | `Pages/PermitSearch.aspx` + `Pages/PermitSearch.aspx.cs` |
| **LOC** | 138 (markup) + 65 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.Pages` |
| **Inherits** | `System.Web.UI.Page` |
| **Master Page** | `~/MasterPages/Site.Master` |
| **Page Title** | `"Search Permits"` |
| **URL Route** | `/search` (mapped in `RouteConfig`) |

#### Responsibility

Multi-criteria search interface for building permits with paginated results. Supports search by permit ID, address, type, and status. Provides inline detail view via FormView and report generation placeholder. Accepts query string parameter `permitId` for deep linking from other pages.

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Permit ID Search | `TextBox` | `txtPermitId` | Permit ID filter (max 50 chars) |
| Address Search | `TextBox` | `txtAddress` | Property address filter (max 200 chars) |
| Permit Type Filter | `DropDownList` | `ddlPermitType` | 6 types + "All Types" option |
| Status Filter | `DropDownList` | `ddlStatus` | 6 statuses + "All Statuses" option (Submitted, Under Review, Approved, Issued, Expired, Rejected) |
| Search Button | `Button` | `btnSearch` | Triggers search |
| Clear Button | `Button` | `btnClear` | Resets all filters; `CausesValidation="false"` |
| Search Data Source | `ObjectDataSource` | `odsPermits` | Binds to `PermitDataAccess.SearchPermits`; paging enabled (`pageSize`/`startRow` parameters); 4 `ControlParameter` bindings |
| Results Grid | `GridView` | `gvPermits` | Bound to `odsPermits`; `AllowPaging="True"`, `PageSize="20"`, pager: `NumericFirstLast` with 10 buttons; columns: PermitId, ApplicationDate, PropertyAddress, PermitType, Status, EstimatedCost, Actions (View + Report buttons via `TemplateField`) |
| Detail Panel | `Panel` | `pnlPermitDetails` | Initially hidden; displays permit detail on "View" action |
| Detail FormView | `FormView` | `fvPermitDetails` | Bound to `odsPermitDetails`; displays: PermitId, Status, ApplicationDate, PropertyAddress, ParcelNumber, PermitType, EstimatedCost, ApplicantName |
| Detail Data Source | `ObjectDataSource` | `odsPermitDetails` | Binds to `PermitDataAccess.GetPermitById`; `SessionParameter` reads `"SelectedPermitId"` from Session |
| Close Detail Button | `Button` | `btnCloseDetails` | Hides detail panel |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | First load: checks `Request.QueryString["permitId"]`; if present, populates `txtPermitId` and auto-triggers search |
| `btnSearch_Click` | `protected` | Button | Calls `gvPermits.DataBind()` (ObjectDataSource handles parameter collection); hides detail panel |
| `btnClear_Click` | `protected` | Button | Clears all search fields, resets dropdowns to index 0, rebinds grid, hides detail panel |
| `gvPermits_RowCommand` | `protected` | GridView | On `"ViewDetails"`: stores `permitId` in `Session["SelectedPermitId"]`, shows detail panel, binds `fvPermitDetails`. On `"GenerateReport"`: writes JavaScript alert (placeholder for Crystal Reports) |
| `gvPermits_PageIndexChanging` | `protected` | GridView | Sets `PageIndex` to `e.NewPageIndex`, calls `DataBind()` |
| `btnCloseDetails_Click` | `protected` | Button | Hides `pnlPermitDetails` |

#### ViewState Usage

- **None explicit.** ObjectDataSource and GridView use default ViewState for paging and parameter persistence.

#### Session Usage

| Key | Type | Set By | Read By |
|---|---|---|---|
| `Session["SelectedPermitId"]` | `string` | `gvPermits_RowCommand` | `odsPermitDetails` (via `SessionParameter`) |

#### Event Handlers

| Event | Handler |
|---|---|
| `btnSearch.Click` | `btnSearch_Click` |
| `btnClear.Click` | `btnClear_Click` |
| `gvPermits.RowCommand` | `gvPermits_RowCommand` |
| `gvPermits.PageIndexChanging` | `gvPermits_PageIndexChanging` |
| `btnCloseDetails.Click` | `btnCloseDetails_Click` |

#### Data Bindings

| Control | Source | Binding Pattern |
|---|---|---|
| `gvPermits` | `odsPermits` → `PermitDataAccess.SearchPermits()` | Declarative `ObjectDataSource` with `ControlParameter` bindings |
| `fvPermitDetails` | `odsPermitDetails` → `PermitDataAccess.GetPermitById()` | Declarative `ObjectDataSource` with `SessionParameter` |

#### Server Controls Used

`GridView`, `FormView`, `ObjectDataSource` (×2), `Panel`, `TextBox` (×2), `DropDownList` (×2), `Button` (×4), `BoundField` (×6), `TemplateField` (×1), `Label` (×8 in template), `HyperLink` (implicit in buttons)

#### Dependencies

| Dependency | Type |
|---|---|
| `PermitDataAccess.SearchPermits()` | Static method (via ObjectDataSource reflection) |
| `PermitDataAccess.GetPermitById()` | Static method (via ObjectDataSource reflection) |
| `Session["SelectedPermitId"]` | Session state |
| `Site.Master` | Master page |

#### Anti-Patterns

1. **🔴 XSS vulnerability** — `Response.Write($"<script>alert('Crystal Report generation for Permit {permitId}...');</script>")` injects unsanitized permit ID into JavaScript
2. **Session-based parameter passing** — Detail view uses `Session["SelectedPermitId"]` which is shared across tabs; opening two search results in different tabs causes conflicts
3. **Simulated search method invocation** — `Page_Load` calls `btnSearch_Click(null, null)` to trigger search from query string, bypassing normal event lifecycle
4. **ObjectDataSource reflection coupling** — `TypeName="PermitDataAccess"` binds via string-based reflection; no compile-time safety
5. **No input sanitization** — Search parameters pass directly to data access without sanitization
6. **Report placeholder** — Crystal Reports integration is a JavaScript alert placeholder

#### Modernization Notes

- Replace ObjectDataSource with Blazor service injection and `@inject IPermitService`
- Replace GridView paging with server-side `QuickGrid` or custom pagination component
- Replace Session-based detail view with URL parameter or component state
- Implement proper search debouncing
- Replace Crystal Reports with SSRS, Power BI Embedded, or custom PDF generation
- Add URL-based search state for bookmarkable searches

---

### 1.5 Pages/InspectionSchedule.aspx — Inspection Scheduling

| Attribute | Detail |
|---|---|
| **File** | `Pages/InspectionSchedule.aspx` + `Pages/InspectionSchedule.aspx.cs` |
| **LOC** | 76 (markup) + 78 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.Pages` |
| **Inherits** | `System.Web.UI.Page` |
| **Master Page** | `~/MasterPages/Site.Master` |
| **Page Title** | `"Inspection Schedule"` |

#### Responsibility

Dual-purpose page: (1) scheduling form for new building inspections (permit ID, type, date, notes) and (2) grid showing upcoming inspections with complete/cancel actions. Used by building inspectors.

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Permit ID | `TextBox` | `txtPermitId` | Permit ID for new inspection (max 50 chars) |
| Inspection Type | `DropDownList` | `ddlInspectionType` | 6 types: Foundation, Framing, Electrical, Plumbing, Mechanical, Final |
| Requested Date | `TextBox` (Date) | `txtRequestedDate` | HTML5 date input (`TextMode="Date"`) |
| Notes | `TextBox` (MultiLine) | `txtNotes` | Optional notes (3 rows, max 500 chars) |
| Schedule Button | `Button` | `btnSchedule` | Submits new inspection; `ValidationGroup="Schedule"` |
| Status Message | `Label` | `lblMessage` | Success/error message display; initially hidden |
| Inspections Grid | `GridView` | `gvInspections` | Columns: InspectionId, PermitId, InspectionType, ScheduledDate, Status, InspectorName, Actions (Complete + Cancel buttons via TemplateField) |
| Inspections Panel | `UpdatePanel` | `upInspections` | Wraps grid; `UpdateMode="Conditional"` |
| Refresh Button | `Button` | `btnRefresh` | Reloads inspection grid |
| Permit ID Validator | `RequiredFieldValidator` | `rfvPermitId` | `ValidationGroup="Schedule"` |
| Type Validator | `RequiredFieldValidator` | `rfvInspectionType` | `ValidationGroup="Schedule"` |
| Date Validator | `RequiredFieldValidator` | `rfvDate` | `ValidationGroup="Schedule"` |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | First load: calls `LoadInspections()` |
| `LoadInspections` | `private` | Internal | Calls `InspectionDataAccess.GetUpcomingInspections()`, binds to `gvInspections` |
| `btnSchedule_Click` | `protected` | Button | If valid: (1) Calls `InspectionDataAccess.ScheduleInspection()` with form values, (2) Shows success message with inspection ID, (3) Clears form, (4) Reloads grid. On exception: shows error in `lblMessage` with `"warning-message"` CSS class. |
| `gvInspections_RowCommand` | `protected` | GridView | On `"CompleteInspection"`: calls `InspectionDataAccess.CompleteInspection(id, "Passed")`, reloads grid. On `"CancelInspection"`: calls `InspectionDataAccess.CancelInspection(id)`, reloads grid. |
| `btnRefresh_Click` | `protected` | Button | Calls `LoadInspections()` |

#### ViewState Usage

- **None explicit.**

#### Event Handlers

| Event | Handler | ValidationGroup |
|---|---|---|
| `btnSchedule.Click` | `btnSchedule_Click` | `Schedule` |
| `gvInspections.RowCommand` | `gvInspections_RowCommand` | None |
| `btnRefresh.Click` | `btnRefresh_Click` | None |

#### Data Bindings

| Control | Source |
|---|---|
| `gvInspections` | `InspectionDataAccess.GetUpcomingInspections()` — manual binding |

#### Server Controls Used

`UpdatePanel`, `GridView`, `TextBox` (×3), `DropDownList`, `Button` (×3), `Label`, `RequiredFieldValidator` (×3), `BoundField` (×6), `TemplateField` (×1)

#### Dependencies

| Dependency | Type |
|---|---|
| `InspectionDataAccess.GetUpcomingInspections()` | Static method |
| `InspectionDataAccess.ScheduleInspection()` | Static method |
| `InspectionDataAccess.CompleteInspection()` | Static method |
| `InspectionDataAccess.CancelInspection()` | Static method |
| `Site.Master` | Master page |

#### Anti-Patterns

1. **Hardcoded inspection result** — `CompleteInspection(id, "Passed")` always passes; no UI to select pass/fail/partial result
2. **No confirmation dialog** — Complete and Cancel actions execute immediately without user confirmation
3. **Unsafe date parsing** — `DateTime.Parse(txtRequestedDate.Text)` without `TryParse`; culture-dependent formatting issues
4. **No permit validation** — Form accepts any permit ID string without verifying the permit exists before scheduling
5. **No access control** — Any user can schedule, complete, or cancel inspections
6. **CSS class mutation** — Error handler changes `lblMessage.CssClass` to `"warning-message"` but never resets to `"success-message"` on subsequent success (stale class persists if no postback resets it — though in practice the label text changes)

#### Modernization Notes

- Add pass/fail/conditional result selection for inspection completion
- Add confirmation modal for destructive actions (cancel)
- Implement real-time date validation (no weekends, per SP business rule)
- Add permit existence check before scheduling
- Restrict to Inspector role
- Add inspector assignment UI (currently auto-assigned in SP)

---

### 1.6 Pages/PlanReview.aspx — Plan Review Management

| Attribute | Detail |
|---|---|
| **File** | `Pages/PlanReview.aspx` + `Pages/PlanReview.aspx.cs` |
| **LOC** | 82 (markup) + 86 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.Pages` |
| **Inherits** | `System.Web.UI.Page` |
| **Master Page** | `~/MasterPages/Site.Master` |
| **Page Title** | `"Plan Review"` |

#### Responsibility

Plan review management for building inspectors/reviewers. Allows loading a permit by ID, viewing its summary (via PermitHeader control), submitting reviews with type, status, comments, and deficiency checklist, and viewing review history.

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Permit ID Input | `TextBox` | `txtPermitId` | Permit ID to review (max 50 chars) |
| Load Button | `Button` | `btnLoadPermit` | Loads permit data and shows review form |
| Permit Header | User Control (`PermitHeader.ascx`) | `ucPermitHeader` | Read-only permit summary display |
| Review Type | `DropDownList` | `ddlReviewType` | 6 types: Structural, Electrical, Plumbing, Mechanical, Fire Safety, Zoning Compliance |
| Reviewer Label | `Label` | `lblReviewer` | Shows current reviewer (Session UserId truncated to 8 chars) |
| Review Status | `DropDownList` | `ddlReviewStatus` | 5 options: Pending, In Progress, Approved, Approved with Conditions, Rejected |
| Comments | `TextBox` (MultiLine) | `txtComments` | Review comments (5 rows, max 2000 chars) |
| Deficiencies | `CheckBoxList` | `cblDeficiencies` | 5 checkboxes: Missing Documentation, Insufficient Details, Building Code Violation, Zoning Issue, Structural Concern |
| Submit Review Button | `Button` | `btnSubmitReview` | Submits the review |
| Review Message | `Label` | `lblReviewMessage` | Success/error message; initially hidden |
| Review History Grid | `GridView` | `gvReviewHistory` | Columns: ReviewDate, ReviewType, ReviewerName, Status, Comments |
| Review Form Panel | `Panel` | `pnlReviewForm` | Wraps entire review form + history; initially hidden (`Visible="false"`) |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | First load: sets `lblReviewer.Text` to `Session["UserId"]` truncated to 8 chars |
| `btnLoadPermit_Click` | `protected` | Button | If permit ID not empty: (1) Calls `PermitDataAccess.GetPermitById()`, (2) If found: sets `ucPermitHeader` properties (PermitId, PropertyAddress, Status), loads review history, shows form panel. (3) If not found: `Response.Write` alert (XSS vector). |
| `LoadReviewHistory` | `private` | Internal | Calls `PermitDataAccess.GetPlanReviewHistory(txtPermitId.Text)`, binds to `gvReviewHistory` |
| `btnSubmitReview_Click` | `protected` | Button | (1) Collects deficiencies from `cblDeficiencies` into semicolon-delimited string. (2) Calls `PermitDataAccess.SubmitPlanReview()` with permit ID, type, reviewer, status, comments, deficiencies. (3) Shows success message. (4) Resets form (status dropdown, comments, checkboxes). (5) Reloads history. On exception: shows error in `lblReviewMessage`. |

#### ViewState Usage

- **None explicit.**

#### Event Handlers

| Event | Handler |
|---|---|
| `btnLoadPermit.Click` | `btnLoadPermit_Click` |
| `btnSubmitReview.Click` | `btnSubmitReview_Click` |

#### Data Bindings

| Control | Source |
|---|---|
| `ucPermitHeader` | Manual property assignment from `PermitDataAccess.GetPermitById()` row |
| `gvReviewHistory` | `PermitDataAccess.GetPlanReviewHistory()` — manual binding |

#### Server Controls Used

`Panel`, `GridView`, `TextBox` (×2), `DropDownList` (×2), `Button` (×2), `Label` (×2), `CheckBoxList`, `BoundField` (×5), User Control `PermitHeader` (×1)

#### Dependencies

| Dependency | Type |
|---|---|
| `PermitDataAccess.GetPermitById()` | Static method |
| `PermitDataAccess.GetPlanReviewHistory()` | Static method |
| `PermitDataAccess.SubmitPlanReview()` | Static method |
| `PermitHeader.ascx` | User control |
| `Session["UserId"]` | Session state |
| `Site.Master` | Master page |

#### Anti-Patterns

1. **🔴 XSS vulnerability** — `Response.Write("<script>alert('Permit not found.');</script>")` — while the string is static here, the pattern encourages XSS in similar usage
2. **GUID-based reviewer identity** — Reviewer name is a truncated random GUID from Session, not a real user identity
3. **Semicolon-delimited deficiencies** — `"MissingDocumentation; InsufficientDetails; "` string concatenation instead of structured data
4. **No review type uniqueness check** — Same reviewer can submit duplicate reviews for the same type
5. **No input validation on submit** — No `ValidationGroup` on `btnSubmitReview`; allows empty submissions
6. **No access control** — Any user can submit plan reviews

#### Modernization Notes

- Replace with Blazor component using `EditForm` + data annotations
- Use real authenticated user identity for reviewer
- Store deficiencies as a `List<string>` or JSON, not semicolon-delimited text
- Add review type uniqueness constraint
- Require mandatory fields (comments, at least one deficiency when rejected)
- Restrict to Plan Reviewer role

---

## 2. Master Pages

### 2.1 MasterPages/Site.Master — Application Shell

| Attribute | Detail |
|---|---|
| **File** | `MasterPages/Site.Master` + `MasterPages/Site.Master.cs` |
| **LOC** | 39 (markup) + 24 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.MasterPages` |
| **Inherits** | `System.Web.UI.MasterPage` |

#### Responsibility

Defines the HTML shell for all pages: document structure, stylesheet reference, global `<form>` tag with ScriptManager, header with branding and user info, navigation bar, content area, and footer. Manages user session display and logout.

#### Layout Structure

```
┌────────────────────────────────────────────────┐
│  .header                                       │
│  ┌─────────────────────┬──────────────────────┐│
│  │ h1: City Council -  │ .user-info:          ││
│  │ Building Permit Sys │ Welcome, {UserId}    ││
│  │                     │ | Role: {UserRole}   ││
│  │                     │ | [Logout]           ││
│  └─────────────────────┴──────────────────────┘│
├────────────────────────────────────────────────┤
│  .navigation                                   │
│  [Home] [Apply for Permit] [Search Permits]    │
│  [Schedule Inspection] [Plan Review] [Dashboard]│
├────────────────────────────────────────────────┤
│  .content                                      │
│  ┌────────────────────────────────────────────┐│
│  │ ContentPlaceHolder: MainContent            ││
│  │ (Page-specific content injected here)      ││
│  └────────────────────────────────────────────┘│
├────────────────────────────────────────────────┤
│  .footer                                       │
│  © 2024 Riverdale City Council | Contact info  │
└────────────────────────────────────────────────┘
```

#### Common Elements

| Element | Type | ID | Purpose |
|---|---|---|---|
| ScriptManager | `ScriptManager` | `ScriptManager1` | Global script manager; `EnablePartialRendering="true"` enables AJAX UpdatePanels across all pages |
| User Name Display | `Label` | `lblUserName` | Shows first 8 chars of `Session["UserId"]` GUID |
| User Role Display | `Label` | `lblUserRole` | Shows `Session["UserRole"]` value |
| Logout Link | `LinkButton` | `lnkLogout` | Triggers session clear and redirect |
| Head Placeholder | `ContentPlaceHolder` | `head` | Allows child pages to inject `<head>` content |
| Main Placeholder | `ContentPlaceHolder` | `MainContent` | Allows child pages to inject body content |

#### Navigation Links

| Link | Target | Display Text |
|---|---|---|
| Home | `~/Default.aspx` | Home |
| Apply | `~/Pages/PermitApplication.aspx` | Apply for Permit |
| Search | `~/Pages/PermitSearch.aspx` | Search Permits |
| Inspections | `~/Pages/InspectionSchedule.aspx` | Schedule Inspection |
| Review | `~/Pages/PlanReview.aspx` | Plan Review |
| Dashboard | `~/Pages/Dashboard.aspx` | Dashboard |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | First load: sets `lblUserName` to first 8 chars of `Session["UserId"]` (or "Guest"), sets `lblUserRole` to `Session["UserRole"]` (or "Guest") |
| `lnkLogout_Click` | `protected` | LinkButton | Calls `Session.Clear()` then `Session.Abandon()`, redirects to `~/Default.aspx` |

#### Dependencies

| Dependency | Type |
|---|---|
| `Session["UserId"]` | Session state (set in `Global.asax.cs`) |
| `Session["UserRole"]` | Session state (set in `Global.asax.cs`) |
| `~/Styles/Site.css` | External stylesheet |

#### Anti-Patterns

1. **Single `<form>` wrapping entire page** — Web Forms requirement but prevents multiple independent forms; all content pages share one form
2. **Truncated GUID as user display** — `Session["UserId"]?.ToString().Substring(0, 8)` — meaningless to users
3. **No role-based navigation** — All 6 links visible to all users regardless of role
4. **No active link highlighting** — Current page not indicated in navigation
5. **Relative CSS path** — `../Styles/Site.css` assumes master page is in a subdirectory; fragile with URL routing
6. **Global ScriptManager** — All pages incur AJAX framework overhead even if they don't use UpdatePanels

#### Modernization Notes

- Replace with Blazor `MainLayout.razor` + `NavMenu.razor` components
- Implement role-based navigation visibility
- Use `NavigationManager` for active link highlighting
- Use `AuthorizeView` for conditional UI rendering
- Replace ScriptManager with Blazor Server SignalR (automatic)
- Use ASP.NET Core Identity for user display name

---

## 3. User Controls

### 3.1 UserControls/AddressLookup.ascx — Address Autocomplete

| Attribute | Detail |
|---|---|
| **File** | `UserControls/AddressLookup.ascx` + `UserControls/AddressLookup.ascx.cs` |
| **LOC** | 7 (markup) + 44 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.UserControls` |
| **Inherits** | `System.Web.UI.UserControl` |

#### Responsibility

Simulated address autocomplete control. Provides a text input with a "Lookup" button that displays a hardcoded list of Riverdale addresses in a dropdown. User selection populates the text input. Designed to be replaced with a real geocoding/address service.

#### Properties

| Property | Type | Access | Implementation |
|---|---|---|---|
| `Address` | `string` | `get/set` | Gets/sets `txtAddress.Text` |

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Address Input | `TextBox` | `txtAddress` | Free text entry for address (max 200 chars) |
| Lookup Button | `Button` | `btnLookup` | Triggers hardcoded address suggestion display; `CausesValidation="false"` |
| Suggestions Panel | `Panel` | `pnlSuggestions` | Dropdown overlay; `Visible="false"` initially; styled with absolute positioning, white background, border, max-height 200px with scroll |
| Suggestions List | `ListBox` | `lstSuggestions` | Displays address suggestions; `AutoPostBack="true"` for selection handling |

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | Empty — no initialization needed |
| `btnLookup_Click` | `protected` | Button | Creates hardcoded `List<string>` of 5 Riverdale addresses, binds to `lstSuggestions`, shows `pnlSuggestions` |
| `lstSuggestions_SelectedIndexChanged` | `protected` | ListBox selection | If selection exists: copies `SelectedValue` to `txtAddress.Text`, hides `pnlSuggestions` |

#### Hardcoded Address Data

```
123 Main Street, Riverdale
456 Oak Avenue, Riverdale
789 Elm Drive, Riverdale
321 Maple Lane, Riverdale
654 Pine Road, Riverdale
```

#### Events (Raised)

None — Parent pages read the `Address` property directly.

#### Used By

| Page | Registration | Instance ID |
|---|---|---|
| `PermitApplication.aspx` | `<%@ Register Src="~/UserControls/AddressLookup.ascx" TagPrefix="uc" TagName="AddressLookup" %>` | `ucAddress` |

#### Dependencies

None (self-contained).

#### Anti-Patterns

1. **Hardcoded data** — 5 static addresses; no integration with geocoding API or address database
2. **Full postback for suggestions** — Button click causes full server round-trip for a lookup that should be client-side
3. **AutoPostBack on selection** — ListBox selection triggers another full postback
4. **Inline CSS** — Suggestion panel uses inline `style` attribute instead of CSS class
5. **No keyboard accessibility** — No ARIA roles or keyboard navigation support
6. **Parent validator coupling** — Parent page references inner control via naming container path `ucAddress$txtAddress`

#### Modernization Notes

- Replace with client-side autocomplete component (e.g., Blazor component with debounced API call)
- Integrate with Azure Maps or Google Places API for real address validation
- Use component events (`EventCallback`) instead of property polling
- Add address validation/normalization
- Implement ARIA roles for accessibility

---

### 3.2 UserControls/PermitHeader.ascx — Permit Summary Display

| Attribute | Detail |
|---|---|
| **File** | `UserControls/PermitHeader.ascx` + `UserControls/PermitHeader.ascx.cs` |
| **LOC** | 8 (markup) + 30 (code-behind) |
| **Namespace** | `RiverdalePermitSystem.Web.UserControls` |
| **Inherits** | `System.Web.UI.UserControl` |

#### Responsibility

Read-only display component showing a permit summary header (ID, address, status). Used as a context banner on pages that operate on a specific permit.

#### Properties

| Property | Type | Access | Implementation |
|---|---|---|---|
| `PermitId` | `string` | `get/set` | Gets/sets `lblPermitId.Text` |
| `PropertyAddress` | `string` | `get/set` | Gets/sets `lblPropertyAddress.Text` |
| `Status` | `string` | `get/set` | Gets/sets `lblStatus.Text` |

#### UI Elements

| Control | Type | ID | Purpose |
|---|---|---|---|
| Permit ID | `Label` | `lblPermitId` | Displays permit identifier |
| Property Address | `Label` | `lblPropertyAddress` | Displays property address |
| Status | `Label` | `lblStatus` | Displays permit status; `Font-Bold="true"` |

**Container:** `<div>` with inline styles — gray background (`#f0f0f0`), padding 15px, bottom margin 20px, border 1px solid `#ccc`.

#### Code-Behind Logic

| Method | Visibility | Trigger | Logic |
|---|---|---|---|
| `Page_Load` | `protected` | Lifecycle | Empty — properties are set by parent page |

#### Events (Raised)

None — Pure display component.

#### Used By

| Page | Registration | Instance ID |
|---|---|---|
| `PlanReview.aspx` | `<%@ Register Src="~/UserControls/PermitHeader.ascx" TagPrefix="uc" TagName="PermitHeader" %>` | `ucPermitHeader` |

#### Dependencies

None (self-contained).

#### Anti-Patterns

1. **Inline CSS** — Container div uses inline `style` attribute instead of CSS class
2. **No data validation** — Properties accept any string with no validation
3. **Direct Label binding** — Properties are thin wrappers around Label.Text; no model binding

#### Modernization Notes

- Replace with Blazor `PermitHeader.razor` component with `[Parameter]` properties
- Use CSS class instead of inline styles
- Consider adding a `Permit` model object as a single parameter instead of 3 string properties
- Add status-based color coding (e.g., green for Approved, red for Rejected)

---

## 4. Data Access Layer

### 4.1 App_Code/PermitDataAccess.cs — Permit Data Operations

| Attribute | Detail |
|---|---|
| **File** | `App_Code/PermitDataAccess.cs` |
| **LOC** | 213 |
| **Class** | `public static class PermitDataAccess` |
| **Namespace** | Global (no namespace) |

#### Responsibility

Central data access class for all permit-related operations. Provides methods for permit CRUD, search, fee calculation, dashboard statistics, plan review history, and status aggregation. Currently implements **simulated data** — all methods return hardcoded/generated DataTable results with comments indicating where stored procedure calls would occur.

#### Connection String

```csharp
private static string ConnectionString
{
    get { return ConfigurationManager.ConnectionStrings["PermitDB"].ConnectionString; }
}
```
- Reads from `Web.config` `<connectionStrings>` section
- Property exists but is **never used** — all methods return simulated data
- Points to: `Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\PermitDB.mdf;Integrated Security=True`

#### Methods

##### 4.1.1 `GetRecentPermits(int count)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Fetch most recent N permits for home page display |
| **Parameters** | `count` (int) — number of permits to return |
| **Returns** | `DataTable` with columns: PermitId (string), ApplicationDate (DateTime), PropertyAddress (string), PermitType (string), Status (string), EstimatedCost (decimal) |
| **SQL (intended)** | `SELECT TOP(@count) ... FROM Permits ORDER BY ApplicationDate DESC` |
| **Implementation** | Generates `count` rows with pattern-based data: sequential IDs (`PERM-2024-100x`), dates offset by `-i` days, cycling permit types and statuses |
| **Called by** | `Default.aspx.cs:LoadRecentPermits()` |

##### 4.1.2 `CalculatePermitFee(string permitType, decimal estimatedCost)` → `decimal`

| Attribute | Detail |
|---|---|
| **Purpose** | Calculate permit fee based on type and estimated cost |
| **Parameters** | `permitType` (string) — permit type code; `estimatedCost` (decimal) — project cost |
| **Returns** | `decimal` — calculated fee amount |
| **SQL (intended)** | `EXEC sp_CalculatePermitFee` (or client-side calculation) |
| **Implementation** | Pure computation: base fee + (estimatedCost × percentage rate). Fee schedule: NewConstruction ($500 + 3%), Addition ($300 + 2.5%), Electrical/Plumbing/Mechanical ($150 + 1.5%), Demolition ($250 + 1%), Default ($100 + 2%) |
| **Business Rule** | `fee = baseFee + (estimatedCost × percentageFee)` |
| **Called by** | `PermitApplication.aspx.cs:CalculateFee()` |
| **⚠️ Duplication** | Same logic exists in `sp_CalculatePermitFee` and `sp_SubmitPermitApplication` stored procedures — triple duplication |

**Fee Schedule:**

| Permit Type | Base Fee | Percentage Rate |
|---|---|---|
| NewConstruction | $500.00 | 3.0% |
| Addition | $300.00 | 2.5% |
| Electrical | $150.00 | 1.5% |
| Plumbing | $150.00 | 1.5% |
| Mechanical | $150.00 | 1.5% |
| Demolition | $250.00 | 1.0% |
| Default | $100.00 | 2.0% |

> **Note:** The C# version lacks the square footage surcharge ($0.50/sqft for NewConstruction/Addition) and zoning surcharge ($200 for C1/I1 districts) present in `sp_CalculatePermitFee`.

##### 4.1.3 `SubmitPermitApplication(string propertyAddress, string parcelNumber, string zoning, string permitType, string description, decimal estimatedCost, DataTable applicantData)` → `string`

| Attribute | Detail |
|---|---|
| **Purpose** | Submit a new permit application |
| **Parameters** | 7 parameters including `applicantData` DataTable with columns: Name, Email, Phone, Company, LicenseNumber |
| **Returns** | `string` — generated permit ID (format: `PERM-2024-{ticks%100000}`) |
| **SQL (intended)** | `EXEC sp_SubmitPermitApplication` (with applicant upsert, fee calculation, activity log) |
| **Implementation** | Returns generated ID only; no actual database interaction |
| **Called by** | `PermitApplication.aspx.cs:btnSubmit_Click()` |

##### 4.1.4 `SearchPermits(string permitId, string address, string permitType, string status, int startRow, int pageSize)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Paginated search with multi-field filtering |
| **Parameters** | 4 filter criteria + paging parameters (`startRow`, `pageSize`) |
| **Returns** | `DataTable` with columns: PermitId, ApplicationDate, PropertyAddress, PermitType, Status, EstimatedCost |
| **SQL (intended)** | `SELECT ... FROM Permits WHERE ... ORDER BY ... OFFSET @startRow ROWS FETCH NEXT @pageSize ROWS ONLY` |
| **Implementation** | Ignores filter criteria; generates `pageSize` rows with sequential IDs offset by `startRow` |
| **Called by** | `PermitSearch.aspx` via `ObjectDataSource` (declarative binding) |

##### 4.1.5 `GetPermitById(string permitId)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Single permit lookup by ID |
| **Parameters** | `permitId` (string) |
| **Returns** | `DataTable` with columns: PermitId, ApplicationDate, PropertyAddress, ParcelNumber, PermitType, Status, EstimatedCost, ApplicantName (always 1 row) |
| **SQL (intended)** | `SELECT ... FROM Permits JOIN Applicants WHERE PermitId = @PermitId` |
| **Implementation** | Returns hardcoded single row (123 Main Street, John Smith, NewConstruction, Under Review) |
| **Called by** | `PermitSearch.aspx` (via ObjectDataSource), `PlanReview.aspx.cs:btnLoadPermit_Click()` |

##### 4.1.6 `GetDashboardStatistics()` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Aggregate dashboard statistics |
| **Parameters** | None |
| **Returns** | `DataTable` with columns: TotalPermits (int), PendingReview (int), InspectionsToday (int), MonthlyRevenue (decimal) |
| **SQL (intended)** | Multiple aggregate queries or dedicated stored procedure |
| **Implementation** | Returns hardcoded: 1247 total, 38 pending, 12 inspections, $125,430.50 revenue |
| **Called by** | `Dashboard.aspx.cs:LoadDashboardData()` |

##### 4.1.7 `GetRecentActivity(int count)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Recent activity log entries for dashboard |
| **Parameters** | `count` (int) — number of entries |
| **Returns** | `DataTable` with columns: Timestamp (DateTime), ActivityType (string), PermitId (string), Description (string), UserName (string) |
| **SQL (intended)** | `SELECT TOP(@count) ... FROM ActivityLog ORDER BY Timestamp DESC` |
| **Implementation** | Generates `count` rows with cycling activity types and usernames, 15-minute intervals |
| **Called by** | `Dashboard.aspx.cs:LoadDashboardData()` |

##### 4.1.8 `GetPermitsByStatus()` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Status group counts with total values for dashboard |
| **Parameters** | None |
| **Returns** | `DataTable` with columns: Status (string), Count (int), TotalValue (decimal) |
| **SQL (intended)** | `SELECT Status, COUNT(*), SUM(EstimatedCost) FROM Permits GROUP BY Status` |
| **Implementation** | Returns 5 hardcoded rows: Submitted(38), Under Review(25), Approved(42), Issued(156), Expired(8) |
| **Called by** | `Dashboard.aspx.cs:LoadDashboardData()` |

##### 4.1.9 `GetPlanReviewHistory(string permitId)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | All plan reviews for a specific permit |
| **Parameters** | `permitId` (string) |
| **Returns** | `DataTable` with columns: ReviewDate (DateTime), ReviewType (string), ReviewerName (string), Status (string), Comments (string) |
| **SQL (intended)** | `SELECT ... FROM PlanReviews WHERE PermitId = @PermitId ORDER BY ReviewDate` |
| **Implementation** | Returns 2 hardcoded rows (Structural Approved, Electrical Pending) regardless of input |
| **Called by** | `PlanReview.aspx.cs:LoadReviewHistory()` |

##### 4.1.10 `SubmitPlanReview(string permitId, string reviewType, string reviewer, string status, string comments, string deficiencies)` → `string`

| Attribute | Detail |
|---|---|
| **Purpose** | Submit a plan review with deficiency tracking |
| **Parameters** | 6 string parameters |
| **Returns** | `string` — generated review ID (format: `REV-2024-{ticks%10000}`) |
| **SQL (intended)** | `EXEC sp_SubmitPlanReview` (with status cascading) |
| **Implementation** | Returns generated ID only; no database interaction |
| **Called by** | `PlanReview.aspx.cs:btnSubmitReview_Click()` |

#### Anti-Patterns

1. **Static class** — Cannot be mocked, injected, or substituted; prevents unit testing
2. **No namespace** — Class is in global namespace; will conflict in larger solutions
3. **DataTable returns** — No type safety; column names are magic strings; no IntelliSense
4. **Unused ConnectionString** — Property exists but is never referenced in any method
5. **Simulated data masking real behavior** — Hardcoded data hides potential database issues and prevents integration testing
6. **Fee calculation duplication** — C# version missing square footage and zoning surcharges present in SP
7. **No connection management** — When real DB calls are added, connection handling must be implemented
8. **No error handling** — No try/catch in any method
9. **DataTable as input parameter** — `SubmitPermitApplication` accepts `DataTable applicantData` instead of typed parameters

#### Modernization Notes

- Replace with interface-based service: `IPermitService` / `PermitService`
- Replace DataTable returns with strongly-typed DTOs/records
- Use Entity Framework Core for data access
- Consolidate fee calculation to single authoritative implementation
- Add proper error handling with custom exceptions
- Register in DI container with scoped lifetime
- Add input validation

---

### 4.2 App_Code/InspectionDataAccess.cs — Inspection Data Operations

| Attribute | Detail |
|---|---|
| **File** | `App_Code/InspectionDataAccess.cs` |
| **LOC** | 106 |
| **Class** | `public static class InspectionDataAccess` |
| **Namespace** | Global (no namespace) |

#### Responsibility

Data access class for all inspection-related operations: retrieving upcoming inspections, scheduling new inspections, completing/cancelling inspections, viewing history, and checking inspector schedules. All methods return **simulated data**.

#### Connection String

```csharp
private static string ConnectionString
{
    get { return ConfigurationManager.ConnectionStrings["PermitDB"].ConnectionString; }
}
```
- Exists but **never used** — all methods return simulated data.

#### Methods

##### 4.2.1 `GetUpcomingInspections()` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Retrieve all upcoming/pending inspections |
| **Parameters** | None |
| **Returns** | `DataTable` with columns: InspectionId (string), PermitId (string), InspectionType (string), ScheduledDate (DateTime), Status (string), InspectorName (string) |
| **SQL (intended)** | `SELECT ... FROM Inspections WHERE ScheduledDate >= GETDATE() AND Status IN ('Scheduled', 'Pending')` |
| **Implementation** | Generates 10 rows with cycling types (Foundation/Framing/Electrical/Plumbing/Final), inspectors, and alternating statuses |
| **Called by** | `InspectionSchedule.aspx.cs:LoadInspections()` |

##### 4.2.2 `ScheduleInspection(string permitId, string inspectionType, DateTime requestedDate, string notes)` → `string`

| Attribute | Detail |
|---|---|
| **Purpose** | Schedule a new building inspection |
| **Parameters** | `permitId`, `inspectionType`, `requestedDate`, `notes` |
| **Returns** | `string` — generated inspection ID (format: `INSP-2024-{ticks%10000}`) |
| **SQL (intended)** | `EXEC sp_ScheduleInspection` (with permit validation, weekend check, auto-assignment, status update) |
| **Implementation** | Returns generated ID only; comments document SP logic: validate permit status, check availability, calculate fees, send email, update status |
| **Called by** | `InspectionSchedule.aspx.cs:btnSchedule_Click()` |

##### 4.2.3 `CompleteInspection(string inspectionId, string result)` → `void`

| Attribute | Detail |
|---|---|
| **Purpose** | Mark an inspection as completed with pass/fail result |
| **Parameters** | `inspectionId`, `result` (e.g., "Passed", "Failed") |
| **Returns** | `void` |
| **SQL (intended)** | `EXEC sp_CompleteInspection` (with status cascade, report generation, notifications) |
| **Implementation** | No-op; comments document SP logic: update status/result, cascade permit status, generate report, send notifications |
| **Called by** | `InspectionSchedule.aspx.cs:gvInspections_RowCommand()` |

##### 4.2.4 `CancelInspection(string inspectionId)` → `void`

| Attribute | Detail |
|---|---|
| **Purpose** | Cancel a scheduled inspection |
| **Parameters** | `inspectionId` |
| **Returns** | `void` |
| **SQL (intended)** | `EXEC sp_CancelInspection` (with schedule cleanup, notifications) |
| **Implementation** | No-op; comments document SP logic: update status, free inspector schedule, notify applicant |
| **Called by** | `InspectionSchedule.aspx.cs:gvInspections_RowCommand()` |

##### 4.2.5 `GetInspectionHistory(string permitId)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Historical inspections for a specific permit |
| **Parameters** | `permitId` |
| **Returns** | `DataTable` with columns: InspectionDate, InspectionType, Result, InspectorName, Comments |
| **SQL (intended)** | `SELECT ... FROM Inspections WHERE PermitId = @PermitId ORDER BY InspectionDate` |
| **Implementation** | Returns 2 hardcoded rows (Foundation Passed, Framing Passed) |
| **Called by** | Not currently called by any page (available for future use) |

##### 4.2.6 `GetInspectorSchedule(string inspectorId, DateTime startDate, DateTime endDate)` → `DataTable`

| Attribute | Detail |
|---|---|
| **Purpose** | Inspector's schedule for a date range |
| **Parameters** | `inspectorId`, `startDate`, `endDate` |
| **Returns** | `DataTable` with columns: InspectionId, ScheduledTime, PermitId, PropertyAddress, InspectionType, Status |
| **SQL (intended)** | `SELECT ... FROM Inspections WHERE InspectorId = @InspectorId AND ScheduledDate BETWEEN @Start AND @End` |
| **Implementation** | Returns empty DataTable with column definitions only |
| **Called by** | Not currently called by any page (available for future use) |

#### Anti-Patterns

Same as `PermitDataAccess.cs`: static class, no namespace, DataTable returns, unused ConnectionString, simulated data, no error handling.

#### Modernization Notes

- Replace with `IInspectionService` / `InspectionService`
- Use EF Core with `Inspection` entity
- Add validation logic currently embedded in `sp_ScheduleInspection` to service layer
- Add real notification integration via service events

---

## 5. Utilities

### 5.1 App_Code/EmailHelper.cs — Email Notification Service

| Attribute | Detail |
|---|---|
| **File** | `App_Code/EmailHelper.cs` |
| **LOC** | 84 |
| **Class** | `public static class EmailHelper` |
| **Namespace** | Global (no namespace) |

#### Responsibility

Email notification service for the permit system. Provides methods for sending permit confirmation, inspection confirmation, review completion, and permit issuance notifications. Currently **simulated** — all methods log to `Debug.WriteLine` instead of sending actual emails.

#### Configuration Properties

| Property | Type | Source | Value |
|---|---|---|---|
| `SmtpServer` | `string` | `ConfigurationManager.AppSettings["SmtpServer"]` | `smtp.riverdalecity.gov` |
| `SmtpPort` | `int` | `ConfigurationManager.AppSettings["SmtpPort"]` | `25` (default) |
| `EmailFrom` | `string` | `ConfigurationManager.AppSettings["EmailFrom"]` | `permits@riverdalecity.gov` |

#### Methods

##### 5.1.1 `SendPermitConfirmation(string recipientEmail, string applicantName, string permitId)` → `void`

| Attribute | Detail |
|---|---|
| **Purpose** | Send HTML-formatted permit submission confirmation email |
| **Parameters** | `recipientEmail`, `applicantName`, `permitId` |
| **Implementation** | Logs to `Debug.WriteLine`; contains commented-out code showing full `System.Net.Mail` implementation: `MailMessage` construction, HTML body via `GetPermitConfirmationBody()`, `SmtpClient` with `NetworkCredential` |
| **Email Template** | HTML body with: greeting, "application received" message, permit ID, review time estimate, footer |
| **Called by** | `PermitApplication.aspx.cs:btnSubmit_Click()` |

##### 5.1.2 `SendInspectionConfirmation(string recipientEmail, string inspectionId, DateTime scheduledDate, string inspectionType)` → `void`

| Attribute | Detail |
|---|---|
| **Purpose** | Confirm inspection scheduling |
| **Implementation** | `Debug.WriteLine` only — no HTML template |
| **Called by** | Not currently called (intended for `InspectionSchedule.aspx.cs`) |

##### 5.1.3 `SendReviewCompletedNotification(string recipientEmail, string permitId, string reviewStatus, string comments)` → `void`

| Attribute | Detail |
|---|---|
| **Purpose** | Notify applicant of plan review completion |
| **Implementation** | `Debug.WriteLine` only |
| **Called by** | Not currently called (intended for `PlanReview.aspx.cs`) |

##### 5.1.4 `SendPermitIssuedNotification(string recipientEmail, string permitId, DateTime expirationDate)` → `void`

| Attribute | Detail |
|---|---|
| **Purpose** | Notify applicant of permit issuance with expiration date |
| **Implementation** | `Debug.WriteLine` only |
| **Called by** | Not currently called (intended for permit issuance workflow) |

#### Private Helper Methods

##### `GetPermitConfirmationBody(string applicantName, string permitId)` → `string`

Returns HTML email body with string interpolation. Template includes heading, greeting, confirmation text, permit ID, review timeline, and city department signature.

#### Anti-Patterns

1. **Static class** — Cannot be mocked for testing; hard dependency in callers
2. **Hardcoded credentials pattern** — Commented code shows `new NetworkCredential("username", "password")` — credential storage anti-pattern
3. **No SSL/TLS** — `smtp.EnableSsl = false` in commented code; port 25 is unencrypted
4. **Swallowed exceptions** — `SendPermitConfirmation` catches all exceptions and only logs to `Debug.WriteLine`; caller has no idea email failed
5. **No namespace** — Global namespace
6. **String interpolation in HTML** — Template uses direct string interpolation without HTML encoding (potential XSS in email body)
7. **`SmtpClient` disposal** — Commented code creates `SmtpClient` without `using` statement (implements `IDisposable`)

#### Modernization Notes

- Replace with `IEmailService` interface with DI registration
- Use Azure Communication Services or SendGrid for email delivery
- Store SMTP credentials in Azure Key Vault
- Implement Razor email templates (e.g., via FluentEmail or custom template engine)
- Add email queuing (outbox pattern) for reliability
- Add retry logic for transient failures
- HTML-encode all user data in email templates

---

## 6. Database Components

### 6.1 Database/Schema.sql — Database Schema

| Attribute | Detail |
|---|---|
| **File** | `Database/Schema.sql` |
| **LOC** | 157 |
| **Database** | `RiverdalePermitDB` |
| **Target** | SQL Server 2019+ (currently LocalDB) |

#### Tables

##### 6.1.1 `Applicants` — Permit Applicants

| Column | Type | Constraints | Description |
|---|---|---|---|
| `ApplicantId` | `INT IDENTITY(1,1)` | `PRIMARY KEY` | Auto-incrementing ID |
| `Name` | `NVARCHAR(100)` | `NOT NULL` | Full name |
| `Email` | `NVARCHAR(100)` | `NOT NULL` | Email address |
| `Phone` | `NVARCHAR(20)` | `NOT NULL` | Phone number |
| `Company` | `NVARCHAR(100)` | `NULL` | Company name (optional) |
| `LicenseNumber` | `NVARCHAR(50)` | `NULL` | Contractor license (optional) |
| `CreatedDate` | `DATETIME` | `DEFAULT GETDATE()` | Record creation timestamp |

**Indexes:** `IX_Applicants_Email` on `Email`

**Relationships:** Referenced by `Permits.ApplicantId` (1:N)

**Sample Data:** 4 rows (John Smith, Mary Johnson, Bob Williams, Sarah Davis)

##### 6.1.2 `Contractors` — Licensed Contractors

| Column | Type | Constraints | Description |
|---|---|---|---|
| `ContractorId` | `INT IDENTITY(1,1)` | `PRIMARY KEY` | Auto-incrementing ID |
| `CompanyName` | `NVARCHAR(200)` | `NOT NULL` | Business name |
| `LicenseNumber` | `NVARCHAR(50)` | `NOT NULL UNIQUE` | Unique license identifier |
| `InsuranceExpiry` | `DATE` | `NULL` | Insurance expiration date |
| `Rating` | `DECIMAL(3,2)` | `NULL` | Contractor rating (0.00–9.99) |
| `ContactEmail` | `NVARCHAR(100)` | `NOT NULL` | Contact email |
| `ContactPhone` | `NVARCHAR(20)` | `NOT NULL` | Contact phone |
| `IsActive` | `BIT` | `DEFAULT 1` | Active status flag |

**Indexes:** `IX_Contractors_LicenseNumber` on `LicenseNumber`

**Relationships:** Referenced by `Permits.ContractorId` (1:N, optional)

**Sample Data:** 3 rows (Premier Builders, Riverside Construction, Elite Electrical)

##### 6.1.3 `Permits` — Building Permits (Central Entity)

| Column | Type | Constraints | Description |
|---|---|---|---|
| `PermitId` | `NVARCHAR(50)` | `PRIMARY KEY` | Application-generated permit ID |
| `ApplicationDate` | `DATETIME` | `NOT NULL DEFAULT GETDATE()` | Submission timestamp |
| `PropertyAddress` | `NVARCHAR(200)` | `NOT NULL` | Property location |
| `ParcelNumber` | `NVARCHAR(50)` | `NOT NULL` | Parcel identifier |
| `PermitType` | `NVARCHAR(50)` | `NOT NULL` | Type code |
| `Status` | `NVARCHAR(50)` | `NOT NULL DEFAULT 'Submitted'` | Workflow status |
| `ApplicantId` | `INT` | `NOT NULL, FK` | References `Applicants` |
| `ContractorId` | `INT` | `NULL, FK` | References `Contractors` (optional) |
| `EstimatedCost` | `DECIMAL(18,2)` | `NOT NULL` | Project cost estimate |
| `SquareFootage` | `INT` | `NULL` | Project size (optional) |
| `ZoningDistrict` | `NVARCHAR(10)` | `NULL` | Zoning code |
| `ProjectDescription` | `NVARCHAR(MAX)` | `NULL` | Full description |
| `IssuedDate` | `DATETIME` | `NULL` | Permit issuance date |
| `ExpirationDate` | `DATETIME` | `NULL` | Permit expiration date |
| `CompletedDate` | `DATETIME` | `NULL` | Project completion date |
| `CreatedBy` | `NVARCHAR(100)` | `DEFAULT SYSTEM_USER` | Creating user |
| `CreatedDate` | `DATETIME` | `DEFAULT GETDATE()` | Record creation |
| `ModifiedDate` | `DATETIME` | `DEFAULT GETDATE()` | Last modification |

**Indexes:** `IX_Permits_Status`, `IX_Permits_ApplicationDate`, `IX_Permits_PropertyAddress`

**Foreign Keys:** `FK_Permits_Applicants` (ApplicantId → Applicants), `FK_Permits_Contractors` (ContractorId → Contractors)

**Status Values (from application):** Submitted, Under Review, Approved, Issued, Under Inspection, Certificate of Occupancy Issued, Inspection Failed - Corrections Required, Review Rejected - Resubmit Required, Expired

**Sample Data:** 5 rows with varying types and statuses

##### 6.1.4 `PlanReviews` — Plan Review Records

| Column | Type | Constraints | Description |
|---|---|---|---|
| `ReviewId` | `NVARCHAR(50)` | `PRIMARY KEY` | Generated review ID |
| `PermitId` | `NVARCHAR(50)` | `NOT NULL, FK` | References `Permits` |
| `ReviewerId` | `NVARCHAR(100)` | `NOT NULL` | Reviewer identifier |
| `ReviewType` | `NVARCHAR(50)` | `NOT NULL` | Review category |
| `Status` | `NVARCHAR(50)` | `NOT NULL DEFAULT 'Pending'` | Review status |
| `Comments` | `NVARCHAR(MAX)` | `NULL` | Reviewer comments |
| `Deficiencies` | `NVARCHAR(MAX)` | `NULL` | Semicolon-delimited deficiency list |
| `ReviewDate` | `DATETIME` | `DEFAULT GETDATE()` | Review submission date |
| `DueDate` | `DATETIME` | `NULL` | Review due date |
| `CompletedDate` | `DATETIME` | `NULL` | Completion date |

**Indexes:** `IX_PlanReviews_PermitId`, `IX_PlanReviews_Status`

**Foreign Keys:** `FK_PlanReviews_Permits` (PermitId → Permits)

##### 6.1.5 `Inspections` — Building Inspections

| Column | Type | Constraints | Description |
|---|---|---|---|
| `InspectionId` | `NVARCHAR(50)` | `PRIMARY KEY` | Generated inspection ID |
| `PermitId` | `NVARCHAR(50)` | `NOT NULL, FK` | References `Permits` |
| `InspectorId` | `NVARCHAR(100)` | `NOT NULL` | Assigned inspector |
| `InspectionType` | `NVARCHAR(50)` | `NOT NULL` | Inspection category |
| `ScheduledDate` | `DATETIME` | `NOT NULL` | Scheduled date/time |
| `CompletedDate` | `DATETIME` | `NULL` | Actual completion date |
| `Result` | `NVARCHAR(50)` | `NULL` | Pass/Fail/Partial |
| `Status` | `NVARCHAR(50)` | `NOT NULL DEFAULT 'Scheduled'` | Inspection status |
| `Comments` | `NVARCHAR(MAX)` | `NULL` | Inspector comments |
| `Photos` | `NVARCHAR(MAX)` | `NULL` | Photo references/paths |
| `CreatedDate` | `DATETIME` | `DEFAULT GETDATE()` | Record creation |

**Indexes:** `IX_Inspections_PermitId`, `IX_Inspections_ScheduledDate`, `IX_Inspections_InspectorId`

**Foreign Keys:** `FK_Inspections_Permits` (PermitId → Permits)

##### 6.1.6 `Fees` — Permit Fees & Payments

| Column | Type | Constraints | Description |
|---|---|---|---|
| `FeeId` | `INT IDENTITY(1,1)` | `PRIMARY KEY` | Auto-incrementing ID |
| `PermitId` | `NVARCHAR(50)` | `NOT NULL, FK` | References `Permits` |
| `FeeType` | `NVARCHAR(50)` | `NOT NULL` | Fee category |
| `Amount` | `DECIMAL(18,2)` | `NOT NULL` | Fee amount |
| `PaidDate` | `DATETIME` | `NULL` | Payment date |
| `PaymentMethod` | `NVARCHAR(50)` | `NULL` | Payment method |
| `TransactionId` | `NVARCHAR(100)` | `NULL` | Payment transaction reference |
| `CreatedDate` | `DATETIME` | `DEFAULT GETDATE()` | Record creation |

**Indexes:** `IX_Fees_PermitId`

**Foreign Keys:** `FK_Fees_Permits` (PermitId → Permits)

##### 6.1.7 `ActivityLog` — System Activity Audit Trail

| Column | Type | Constraints | Description |
|---|---|---|---|
| `LogId` | `INT IDENTITY(1,1)` | `PRIMARY KEY` | Auto-incrementing ID |
| `Timestamp` | `DATETIME` | `DEFAULT GETDATE()` | Event timestamp |
| `ActivityType` | `NVARCHAR(100)` | `NOT NULL` | Activity category |
| `PermitId` | `NVARCHAR(50)` | `NULL, FK` | Related permit (optional) |
| `Description` | `NVARCHAR(MAX)` | `NULL` | Activity description |
| `UserName` | `NVARCHAR(100)` | `NOT NULL` | User who performed action |

**Indexes:** `IX_ActivityLog_Timestamp`, `IX_ActivityLog_PermitId`

**Foreign Keys:** `FK_ActivityLog_Permits` (PermitId → Permits)

#### Entity Relationships

```
Applicants (1) ──────► (N) Permits (N) ◄────── (1) Contractors (optional)
                              │
                ┌─────────────┼─────────────┐
                │             │             │
                ▼             ▼             ▼
           PlanReviews   Inspections      Fees
              (N)           (N)           (N)
                              │
                              ▼
                         ActivityLog (N)
```

#### Anti-Patterns

1. **Application-generated PKs** — `Permits`, `PlanReviews`, `Inspections` use application-generated string IDs instead of database-generated IDs (potential for collisions with tick-based generation)
2. **No CHECK constraints** — Status and type columns accept any string; no enumeration enforcement
3. **NVARCHAR(MAX) for structured data** — `Deficiencies` and `Photos` stored as unstructured text
4. **Missing UpdatedBy** — `ModifiedDate` exists on `Permits` but no `ModifiedBy` for audit trail
5. **SYSTEM_USER for CreatedBy** — Uses SQL Server login identity, not application user identity
6. **Destructive schema script** — `DROP DATABASE IF EXISTS` at top; running on wrong server is catastrophic

#### Modernization Notes

- Add CHECK constraints for Status and Type enumerations
- Use GUID or IDENTITY for primary keys; generate in database
- Normalize deficiencies into a junction table
- Add `ModifiedBy`, `IsDeleted` (soft delete), `RowVersion` (concurrency) columns
- Use database migrations (EF Core Migrations) instead of raw SQL scripts
- Consider Azure SQL with elastic pool for production

---

### 6.2 Database/StoredProcedures/PermitProcedures.sql — Stored Procedures

| Attribute | Detail |
|---|---|
| **File** | `Database/StoredProcedures/PermitProcedures.sql` |
| **LOC** | 421 |
| **Procedures** | 5 |

#### 6.2.1 `sp_SubmitPermitApplication`

| Attribute | Detail |
|---|---|
| **Parameters** | `@PermitId`, `@PropertyAddress`, `@ParcelNumber`, `@PermitType`, `@EstimatedCost`, `@ZoningDistrict`, `@ProjectDescription`, `@ApplicantName`, `@ApplicantEmail`, `@ApplicantPhone` |
| **Returns** | `0` on success, `-1` on validation failure |
| **Transaction** | Yes — `BEGIN TRANSACTION` / `COMMIT` / `ROLLBACK` in `TRY`/`CATCH` |

**Business Rules:**
1. **Minimum cost validation** — NewConstruction permits require `EstimatedCost >= $10,000`
2. **Applicant upsert** — Finds existing applicant by email; creates new if not found
3. **Fee calculation** — Duplicates fee logic: base fee by type + percentage × cost
4. **Activity logging** — Inserts "Application Submitted" entry into `ActivityLog`

**Operations:** INSERT Applicant (conditional) → INSERT Permit → INSERT Fee → INSERT ActivityLog

#### 6.2.2 `sp_CalculatePermitFee`

| Attribute | Detail |
|---|---|
| **Parameters** | `@PermitType`, `@EstimatedCost`, `@SquareFootage` (optional), `@ZoningDistrict` (optional), `@CalculatedFee OUTPUT` |
| **Returns** | `0`; fee via OUTPUT parameter |
| **Transaction** | No (read-only calculation) |

**Business Rules:**
1. **Base fee** — 6 permit types + default ($100–$500)
2. **Percentage fee** — By type (1%–3%)
3. **Square footage surcharge** — $0.50/sqft for NewConstruction and Addition (if provided)
4. **Zoning surcharge** — $200 for C1 (Commercial) and I1 (Industrial) districts

**⚠️ Key Difference from C#:** The stored procedure includes square footage and zoning surcharges that the C# `CalculatePermitFee()` does not. This means fees calculated client-side (displayed in wizard) may differ from fees stored in the database.

#### 6.2.3 `sp_ScheduleInspection`

| Attribute | Detail |
|---|---|
| **Parameters** | `@PermitId`, `@InspectionType`, `@RequestedDate`, `@InspectorId` (optional), `@Notes` (optional), `@InspectionId OUTPUT` |
| **Returns** | `0` on success; `@InspectionId` via OUTPUT |
| **Transaction** | Yes |

**Business Rules:**
1. **Permit validation** — Must exist and be in "Issued" or "Under Inspection" status
2. **Weekend check** — No scheduling on Saturday (7) or Sunday (1)
3. **Auto-assignment** — If no inspector specified, assigns inspector with fewest inspections on requested date
4. **ID generation** — `INSP-{year}-{random}` format
5. **Status cascade** — Updates permit status to "Under Inspection"
6. **Activity logging** — Records "Inspection Scheduled" entry

#### 6.2.4 `sp_CompleteInspection`

| Attribute | Detail |
|---|---|
| **Parameters** | `@InspectionId`, `@Result`, `@Comments` (optional) |
| **Returns** | `0` on success |
| **Transaction** | Yes |

**Business Rules:**
1. **Status update** — Marks inspection as "Completed" with result and completion date
2. **Comment append** — Appends new comments to existing (with newline separator)
3. **Permit status cascade:**
   - Passed + Final inspection → "Certificate of Occupancy Issued" + CompletedDate set
   - Passed + non-Final → "Issued"
   - Failed → "Inspection Failed - Corrections Required"
4. **Activity logging** — Records "Inspection Completed" entry

**⚠️ Bug:** Uses `@TempInspection` table variable in `OUTPUT INTO` clause but declares it as `#TempInspection` temp table at end of script — these are different objects and the SP will fail at runtime.

#### 6.2.5 `sp_SubmitPlanReview`

| Attribute | Detail |
|---|---|
| **Parameters** | `@PermitId`, `@ReviewType`, `@ReviewerId`, `@Status`, `@Comments` (optional), `@Deficiencies` (optional), `@ReviewId OUTPUT` |
| **Returns** | `0` on success; `@ReviewId` via OUTPUT |
| **Transaction** | Yes |

**Business Rules:**
1. **ID generation** — `REV-{year}-{random}` format
2. **Permit status cascade:**
   - If review Approved AND no remaining Pending/InProgress/Rejected reviews → permit "Approved"
   - If review Rejected → permit "Review Rejected - Resubmit Required"
3. **Activity logging** — Records "Plan Review Submitted" entry

#### Stored Procedure Anti-Patterns

1. **Business logic in database** — Fee calculation, status transitions, and validation are embedded in SQL; cannot be unit tested without database
2. **Duplicated fee logic** — Fee calculation exists in `sp_SubmitPermitApplication`, `sp_CalculatePermitFee`, and `PermitDataAccess.CalculatePermitFee()`
3. **`sp_` prefix** — Microsoft recommends against `sp_` prefix for user procedures (reserved for system procedures, causes extra catalog lookup)
4. **Table variable bug** — `sp_CompleteInspection` references `@TempInspection` but script creates `#TempInspection`
5. **RAISERROR + RETURN pattern** — Uses both `RAISERROR` (legacy) and `THROW` (modern) in same procedures
6. **No parameter validation** — Most parameters not validated for NULL or empty
7. **SYSTEM_USER for audit** — Records SQL login, not application user

#### Modernization Notes

- Move all business logic to C# domain/service layer
- Replace stored procedures with EF Core queries and commands
- Use single source of truth for fee calculation
- Implement status state machine in C# with explicit transitions
- Use database migrations for schema changes
- Remove `sp_` prefix from procedure names

---

## 7. Configuration

### 7.1 Web.config — Application Configuration

| Attribute | Detail |
|---|---|
| **File** | `Web.config` |
| **LOC** | 56 |

#### Connection Strings

| Name | Provider | Connection String | Purpose |
|---|---|---|---|
| `PermitDB` | `System.Data.SqlClient` | `Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=\|DataDirectory\|\PermitDB.mdf;Integrated Security=True` | Primary database (LocalDB with file-attached .mdf) |

#### App Settings

| Key | Value | Purpose |
|---|---|---|
| `SmtpServer` | `smtp.riverdalecity.gov` | SMTP mail server hostname |
| `SmtpPort` | `25` | SMTP port (unencrypted) |
| `EmailFrom` | `permits@riverdalecity.gov` | Sender email address |
| `ReportPath` | `~/Reports/` | Report template directory (unused) |
| `ValidationSettings:UnobtrusiveValidationMode` | `None` | Disables unobtrusive validation (uses classic Web Forms validation) |

#### System.Web Settings

| Setting | Value | Purpose |
|---|---|---|
| `compilation debug` | `true` | Debug mode enabled (should be false in production) |
| `compilation targetFramework` | `4.8` | .NET Framework 4.8 |
| `httpRuntime targetFramework` | `4.8` | Runtime target |
| `httpRuntime maxRequestLength` | `10240` | Max upload size: 10 MB |
| `authentication mode` | `Windows` | Windows Authentication (IIS Integrated) |
| `authorization` | `<allow users="*" />` | All users allowed (no restrictions) |
| `sessionState mode` | `SQLServer` | SQL Server-backed session state |
| `sessionState sqlConnectionString` | `Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True` | Session database connection |
| `sessionState cookieless` | `false` | Session uses cookies |
| `sessionState timeout` | `30` | 30-minute session timeout |
| `pages enableViewState` | `true` | ViewState enabled globally |
| `pages enableViewStateMac` | `true` | ViewState MAC validation enabled |
| `pages viewStateEncryptionMode` | `Always` | ViewState encrypted on every page |
| `customErrors mode` | `RemoteOnly` | Custom errors for remote users |
| `customErrors defaultRedirect` | `~/Error.aspx` | Error page (does not exist in project) |

#### System.WebServer Settings

| Setting | Value | Purpose |
|---|---|---|
| `validation validateIntegratedModeConfiguration` | `false` | Suppresses IIS integrated mode warnings |
| ScriptModule | Registered in `<modules>` and `<handlers>` | Supports AJAX framework (ScriptManager/UpdatePanel) |
| `ExtensionlessUrlHandler-Integrated-4.0` | Registered | Enables extensionless URL routing |

#### Runtime Assembly Bindings

| Assembly | Redirect | Purpose |
|---|---|---|
| `System.Web.Extensions` | `1.0.0.0-4.0.0.0` → `4.0.0.0` | AJAX framework compatibility |
| `System.Web.Extensions.Design` | `1.0.0.0-4.0.0.0` → `4.0.0.0` | Design-time AJAX support |

#### Anti-Patterns

1. **🔴 Debug mode in production config** — `compilation debug="true"` impacts performance and security
2. **🔴 Open authorization** — `<allow users="*" />` bypasses all access control
3. **Plaintext SMTP** — Port 25, no SSL/TLS configuration
4. **LocalDB attachment** — `AttachDbFilename` is development-only; not suitable for production
5. **Missing Error.aspx** — `customErrors defaultRedirect="~/Error.aspx"` references non-existent page
6. **Global ViewState encryption** — CPU overhead on every page even when ViewState is minimal
7. **No HTTPS enforcement** — No `requireSSL` on session cookies or `httpOnlyCookies`

#### Modernization Notes

- Replace with `appsettings.json` / `appsettings.{Environment}.json`
- Move connection strings to Azure Key Vault or managed identity
- Use ASP.NET Core Identity + Microsoft Entra ID for authentication
- Configure HTTPS/HSTS enforcement
- Use environment-based configuration (`IConfiguration`)
- Replace SQL Server session with Redis or cookie-based session

---

### 7.2 Global.asax / Global.asax.cs — Application Lifecycle

| Attribute | Detail |
|---|---|
| **File** | `Global.asax` + `Global.asax.cs` |
| **LOC** | 1 + 36 |
| **Namespace** | `RiverdalePermitSystem.Web` |
| **Classes** | `Global : HttpApplication`, `RouteConfig` |

#### Application Lifecycle Events

| Event | Method | Logic |
|---|---|---|
| `Application_Start` | `Application_Start` | Calls `RouteConfig.RegisterRoutes(RouteTable.Routes)` to register 3 URL routes |
| `Application_Error` | `Application_Error` | Gets `Server.GetLastError()`, logs to `Debug.WriteLine` — no persistent logging, no error recovery |
| `Session_Start` | `Session_Start` | Sets `Session["UserRole"] = "Applicant"` and `Session["UserId"] = Guid.NewGuid().ToString()` — simulated authentication |

#### Route Configuration (`RouteConfig`)

| Route Name | URL Pattern | Physical Path |
|---|---|---|
| `PermitApplication` | `/apply` | `~/Pages/PermitApplication.aspx` |
| `PermitSearch` | `/search` | `~/Pages/PermitSearch.aspx` |
| `Dashboard` | `/dashboard` | `~/Pages/Dashboard.aspx` |

> **Note:** Only 3 of 6 pages have friendly URLs. Default.aspx, InspectionSchedule.aspx, and PlanReview.aspx are accessed via direct file paths only.

#### Anti-Patterns

1. **Simulated authentication** — Every new session gets a random GUID "user" and hardcoded "Applicant" role
2. **Debug-only error logging** — `Debug.WriteLine` output is lost in production
3. **No error recovery** — `Application_Error` logs but doesn't clear the error or redirect
4. **Incomplete route coverage** — Only 3 of 6 pages have friendly URLs

#### Modernization Notes

- Replace with `Program.cs` / `Startup.cs` in ASP.NET Core
- Use ASP.NET Core routing with attribute routing or minimal APIs
- Implement proper middleware pipeline for error handling
- Use ASP.NET Core Identity for user session initialization
- Add structured logging via `ILogger` / Application Insights

---

## 8. Styling

### 8.1 Styles/Site.css — Application Stylesheet

| Attribute | Detail |
|---|---|
| **File** | `Styles/Site.css` |
| **LOC** | 234 |

#### Responsibility

Single CSS file providing all visual styling for the application. Implements a government/municipal visual identity with blue color scheme, form-based layouts, grid styling, and responsive elements.

#### CSS Architecture

| Category | Classes/Selectors | Purpose |
|---|---|---|
| **Reset** | `*` | Universal box-sizing reset, zero margin/padding |
| **Typography** | `body` | Arial, 14px, #333 text, #f4f4f4 background |
| **Header** | `.header`, `.header h1`, `.user-info` | Dark blue (#003366) header bar with absolute-positioned user info |
| **Navigation** | `.navigation`, `.navigation a`, `.navigation a:hover` | Medium blue (#0055aa) nav bar; white links with hover darkening |
| **Content Area** | `.content` | White background, 20px padding, min-height 500px, box shadow |
| **Footer** | `.footer` | Dark blue footer, centered white text |
| **Headings** | `h2` | Blue with bottom border accent |
| **Form Sections** | `.form-section`, `.form-section h3` | Light gray containers with borders |
| **Form Layout** | `.form-row`, `.form-row label`, `.form-row input/select/textarea` | Label (200px fixed width, bold) + input (400px) layout |
| **Buttons** | `.button-primary`, `.button-primary:hover`, `.button-secondary` | Blue primary, gray secondary; white text, 10px/20px padding |
| **Wizard** | `.wizard-steps`, `.wizard-step`, `.wizard-step.active`, `.wizard-step.completed` | Flexbox step indicators; blue active, green completed |
| **Grid/Table** | `table.grid`, `table.grid th`, `table.grid td`, `table.grid tr:hover` | Blue header, striped hover, full-width collapse |
| **Messages** | `.error-message`, `.success-message`, `.warning-message` | Red inline errors; green/yellow boxed messages |
| **Dashboard** | `.dashboard-stats`, `.stat-card`, `.stat-card h3`, `.stat-card .number` | CSS Grid (4 columns), centered cards with large numbers |
| **Misc** | `.updatepanel` | Min-height 200px for AJAX panels |

#### Color Palette

| Color | Hex | Usage |
|---|---|---|
| Dark Blue | `#003366` | Header, footer, headings, stat numbers |
| Medium Blue | `#0055aa` | Navigation, buttons, form headings, grid headers |
| Dark Hover Blue | `#003d7a` | Button/nav hover states |
| Yellow | `#ffcc00` | User info links |
| Green | `#66bb66` | Completed wizard steps |
| Light Gray | `#f4f4f4` | Page background |
| White | `#ffffff` | Content area, navigation text |
| Section Gray | `#f9f9f9` | Form sections, stat cards |
| Border Gray | `#ddd` | Borders, separators |
| Success Green | `#d4edda` / `#155724` | Success message background/text |
| Warning Yellow | `#fff3cd` / `#856404` | Warning message background/text |
| Error Red | `red` | Error message text |

#### Anti-Patterns

1. **No responsive design** — Fixed-width labels (200px) and inputs (400px); no media queries; breaks on mobile
2. **No CSS methodology** — No BEM, SMACSS, or other naming convention
3. **Single monolithic file** — All styles in one file; no componentization
4. **Inline styles in HTML** — Several controls use inline `style` attributes that override CSS (AddressLookup, PermitHeader)
5. **No CSS variables** — Colors are hardcoded throughout; theme changes require find/replace
6. **`!important` absence** — While not inherently anti-pattern, the `*` reset can cause specificity issues
7. **No dark mode support** — Single color scheme only

#### Modernization Notes

- Adopt CSS framework (Bootstrap 5, Tailwind CSS) or design system
- Use CSS custom properties for theming
- Implement responsive design with mobile-first approach
- Use CSS Modules or scoped CSS in Blazor components
- Split into component-level CSS files
- Add `prefers-color-scheme` media query for dark mode
- Consider government design system (e.g., USWDS) for accessibility compliance

---

## Appendix A: Component Dependency Matrix

| Component | PermitDataAccess | InspectionDataAccess | EmailHelper | Session State | ViewState | Site.Master |
|---|---|---|---|---|---|---|
| **Default.aspx** | ✅ GetRecentPermits | — | — | — | — | ✅ |
| **Dashboard.aspx** | ✅ GetDashboardStatistics, GetRecentActivity, GetPermitsByStatus | — | — | — | — | ✅ |
| **PermitApplication.aspx** | ✅ CalculatePermitFee, SubmitPermitApplication | — | ✅ SendPermitConfirmation | ✅ SubmittedPermitData | ✅ 13 keys | ✅ |
| **PermitSearch.aspx** | ✅ SearchPermits, GetPermitById | — | — | ✅ SelectedPermitId | — | ✅ |
| **InspectionSchedule.aspx** | — | ✅ GetUpcomingInspections, ScheduleInspection, CompleteInspection, CancelInspection | — | — | — | ✅ |
| **PlanReview.aspx** | ✅ GetPermitById, GetPlanReviewHistory, SubmitPlanReview | — | — | ✅ UserId | — | ✅ |
| **Site.Master** | — | — | — | ✅ UserId, UserRole | — | N/A |
| **AddressLookup.ascx** | — | — | — | — | — | — |
| **PermitHeader.ascx** | — | — | — | — | — | — |

---

## Appendix B: Security Vulnerability Inventory

| # | Vulnerability | Type | Location | Severity |
|---|---|---|---|---|
| 1 | XSS via `Response.Write` | Reflected XSS | `PermitApplication.aspx.cs:178` | 🔴 Critical |
| 2 | XSS via `Response.Write` | Reflected XSS | `PermitSearch.aspx.cs:50` | 🔴 Critical |
| 3 | XSS via `Response.Write` | Reflected XSS | `PlanReview.aspx.cs:35` | 🟡 Medium (static string) |
| 4 | No authentication enforcement | Broken AuthN | `Web.config:18`, `Global.asax.cs:22` | 🔴 Critical |
| 5 | No authorization checks | Broken AuthZ | All pages | 🔴 Critical |
| 6 | No anti-forgery tokens | CSRF | All forms | 🔴 High |
| 7 | Unencrypted SMTP | Cleartext transport | `Web.config:7-9`, `EmailHelper.cs` | 🟡 Medium |
| 8 | Debug mode enabled | Information exposure | `Web.config:14` | 🟡 Medium |
| 9 | Missing error page | Information exposure | `Web.config:26` (Error.aspx missing) | 🟡 Medium |
| 10 | Swallowed email exceptions | Silent failure | `EmailHelper.cs:43-47` | 🟢 Low |

---

## Appendix C: Anti-Pattern Summary

| Category | Count | Key Anti-Patterns |
|---|---|---|
| **Security** | 10 | XSS (×3), no auth, no authZ, no CSRF, plaintext SMTP, debug mode |
| **Architecture** | 8 | Static classes, no DI, no interfaces, DataTable coupling, no namespaces, duplicated fee logic |
| **State Management** | 5 | Excessive ViewState (13 keys), session pollution, session-based parameter passing, simulated auth |
| **Error Handling** | 5 | No try/catch, swallowed exceptions, Debug.WriteLine logging, Response.Write alerts, missing error page |
| **Data Access** | 6 | Unused ConnectionString, simulated data, magic strings, fragile column access, no input validation |
| **UI/UX** | 6 | No responsive design, inline CSS, no accessibility, hardcoded data, no confirmation dialogs |
| **Database** | 5 | Business logic in SPs, `sp_` prefix, table variable bug, no CHECK constraints, destructive schema script |

**Total unique anti-patterns identified: 45**

---

*End of Component Specifications — Step 4*
