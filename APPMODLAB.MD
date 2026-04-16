---
title: "Spec2Cloud .NET Framework Modernization"
description: "Modernize ASP.NET Web Forms to .NET 9 using spec-driven approach with Riverdale permit system"
authors: ["marconsilva"]
category: "Spec-Driven Development"
industry: "Government & Public Sector"
services: ["Azure App Service", "Azure SQL Database", "Azure Redis Cache", "Azure Blob Storage"]
languages: [".NET", "C#"]
frameworks: ["ASP.NET Web Forms", "ASP.NET Core", "Blazor", "Entity Framework Core"]
modernizationTools: ["Spec2Cloud"]
agenticTools: []
tags: ["dotnet-framework", "web-forms", "blazor", "modernization", "specifications"]
extensions: ["github.copilot"]
thumbnail: ""
video: ""
version: "1.0.0"
---

## Legacy Application Overview

The Riverdale City Building Permit System is an ASP.NET Web Forms (.NET Framework 4.8) application used by Riverdale City Council for managing building permits, inspections, and plan reviews. The application exhibits classic Web Forms anti-patterns: ViewState, UpdatePanels, DataSets, and stored procedure–driven business logic that must be modernized using the Spec2Cloud methodology.

### Home Page & Navigation

The landing page provides quick action buttons, a recent permits grid, and system information including available permit types and processing times.

![Home Page — Quick actions, recent permits grid, and system information](assets/screenshots/01-home.png)

### Permit Application Wizard

A multi-step wizard (4 steps) guides users through submitting new building permit applications, collecting property information, applicant details, project details, and a review/submit step. This wizard relies heavily on ViewState for state management.

![Permit Application — Four-step wizard with ViewState-dependent state management](assets/screenshots/02-permit-application.png)

### Search & Data Grid

Permits can be searched by ID, address, type, or status. Results display in a paginated GridView with ObjectDataSource binding — a pattern that will be replaced with EF Core queries and Blazor components.

![Permit Search — Paginated GridView with ObjectDataSource binding](assets/screenshots/03-permit-search.png)

### Inspection Scheduling

Inspectors schedule building inspections (Foundation, Framing, Electrical, Plumbing, Mechanical, Final) against existing permits, with a table of upcoming inspections rendered via UpdatePanel for partial page updates.

![Inspection Schedule — UpdatePanel-based scheduling grid for building inspections](assets/screenshots/04-inspection-schedule.png)

### Administrator Dashboard

The admin dashboard shows stat cards (Total Permits, Pending Review, Inspections Today, Monthly Revenue), a recent activity log, and permits-by-status summary — all driven by stored procedures that will be refactored into domain services.

![Dashboard — Admin overview with stat cards, activity log, and stored procedure–driven data](assets/screenshots/05-dashboard.png)

---

## Screenshots

The Riverdale City Building Permit System is an ASP.NET Web Forms (.NET Framework 4.8) application used by Riverdale City Council for managing building permits, inspections, and plan reviews.

### Home Page
The landing page shows quick action buttons, a recent permits grid, and system information including available permit types and processing times.

![Home Page](assets/screenshots/01-home.png)

### Permit Application
A multi-step wizard (4 steps) for submitting new building permit applications. Collects property information, applicant details, project details, and provides a review/submit step.

![Permit Application](assets/screenshots/02-permit-application.png)

### Permit Search
Search permits by ID, address, type, or status. Results display in a paginated grid with View and Report actions per row.

![Permit Search](assets/screenshots/03-permit-search.png)

### Inspection Schedule
Schedule building inspections (Foundation, Framing, Electrical, Plumbing, Mechanical, Final) against existing permits, with a table of upcoming inspections.

![Inspection Schedule](assets/screenshots/04-inspection-schedule.png)

### Administrator Dashboard
Overview dashboard with stat cards (Total Permits, Pending Review, Inspections Today, Monthly Revenue), recent activity log, and permits-by-status summary.

![Dashboard](assets/screenshots/05-dashboard.png)
