# 🎮 SPEC2CLOUD .NET FRAMEWORK MODERNIZATION 🎮

```
███████╗██████╗ ███████╗ ██████╗██████╗  ██████╗██╗      ██████╗ ██╗   ██╗██████╗ 
██╔════╝██╔══██╗██╔════╝██╔════╝╚════██╗██╔════╝██║     ██╔═══██╗██║   ██║██╔══██╗
███████╗██████╔╝█████╗  ██║      █████╔╝██║     ██║     ██║   ██║██║   ██║██║  ██║
╚════██║██╔═══╝ ██╔══╝  ██║     ██╔═══╝ ██║     ██║     ██║   ██║██║   ██║██║  ██║
███████║██║     ███████╗╚██████╗███████╗╚██████╗███████╗╚██████╔╝╚██████╔╝██████╔╝
╚══════╝╚═╝     ╚══════╝ ╚═════╝╚══════╝ ╚═════╝╚══════╝ ╚═════╝  ╚═════╝ ╚═════╝ 
                                                                                    
             🌆 WEB FORMS TO MODERN 🌆 KILLING VIEWSTATE ONE PAGE AT A TIME 💀
```

## 🕹️ OVERVIEW

**GAME STATUS:** 🟢 READY TO PLAY  
**DIFFICULTY:** ⭐⭐⭐⭐ HARD MODE  
**BOSS BATTLE:** VIEWSTATE MONSTER 👾  

Welcome to the ultimate modernization arcade! This lab takes you on a neon-soaked journey from the dark ages of **ASP.NET Web Forms 4.8** to the bright future of **.NET 9**. Using the legendary **Spec2Cloud** methodology, you'll reverse-engineer a legacy municipal permit system and rebuild it with modern tech — all driven by **auto-generated specifications**! 

🏛️ **Mission:** Modernize "Riverdale City Council" building permit system  
📋 **Method:** SPEC → CODE methodology  
💀 **Enemy:** ViewState, DataSets, Crystal Reports, UpdatePanels  
🎯 **Victory:** Azure-hosted .NET 9 app with Blazor superpowers  

## 🎯 WHAT YOU'LL LEARN

### 🔮 SPEC2CLOUD MASTERY
- 📋 **SPEC GENERATION** — Reverse-engineer legacy codebases into comprehensive specifications
- 🗺️ **BLUEPRINT READY** — Use architecture docs, API contracts, and data models as your modernization guide
- 🔄 **SPEC → CODE** — Drive implementation directly from generated specifications
- 📊 **DECISION TRACKING** — Document modernization choices in the specs themselves

### ⚔️ WEB FORMS BOSS BATTLES
- 💀 **VIEWSTATE ELIMINATED** — Replace 200KB+ ViewState pages with stateless MVC/Blazor
- 🔥 **UPDATEPANEL DESTROYED** — Swap fake AJAX for real Blazor Server interactivity
- 🗄️ **DATASET PURGED** — Convert ADO.NET DataSets to strongly-typed EF Core entities
- 📝 **CRYSTAL REPORTS REPLACED** — Generate PDFs with QuestPDF instead of Crystal Reports
- 🧙 **STORED PROC LOGIC REFACTORED** — Extract business rules into domain services

### 🚀 MODERN TECH UNLOCKED
- ✨ **ASP.NET Core MVC + Blazor** for hybrid rendering
- 💎 **Entity Framework Core 9** with repository pattern
- ☁️ **Azure Services** — App Service, SQL Database, Redis Cache, Blob Storage
- 🔐 **Entra ID Authentication** with role-based access
- 🔍 **Azure AI Search** for permit lookup

## 📦 PREREQUISITES

### 🎮 PLAYER REQUIREMENTS
- 🧠 **Skills:** C# and ASP.NET experience
- 📚 **Knowledge:** Basic Spec2Cloud concepts
- 💪 **Level:** Intermediate-Advanced .NET developer

### 🛠️ EQUIPMENT LOADOUT
- ✅ **.NET Framework 4.8 Developer Pack** (for legacy app)
- ✅ **.NET 9 SDK** (for modernized app)
- ✅ **Visual Studio 2022** (your primary weapon)
- ✅ **SQL Server LocalDB** (database arena)
- ✅ **Azure Subscription** (cloud battleground)
- ✅ **Git** (version control shield)
- ✅ **GitHub Copilot CLI** (your AI companion)

### 💻 INSTALLATION SPELLS
```bash
# Verify .NET installations
dotnet --list-sdks
dotnet --list-runtimes

# Install GitHub Copilot CLI (if not already installed)
gh copilot --version

# Clone the repository
git clone https://github.com/EmeaAppGbb/appmodlab-spec2cloud-dotnet-framework.git
cd appmodlab-spec2cloud-dotnet-framework
```

## 🚀 QUICK START

### 🎲 LEVEL 1: RUN THE LEGACY APP
```bash
# Switch to legacy branch
git checkout legacy

# Restore and run the Web Forms app
cd RiverdalePermitSystem
dotnet restore
dotnet run --project RiverdalePermitSystem.Web
```

🌐 Navigate to `https://localhost:5001` and experience the horror of ViewState! 👻

### 📋 LEVEL 2: GENERATE SPECS
```bash
# Use Spec2Cloud to analyze the legacy codebase
gh copilot suggest "Run Spec2Cloud analysis on the RiverdalePermitSystem solution"

# Review generated specifications
git checkout step-1-spec-generation
# Examine: /specs/architecture.md, /specs/api-contracts/, /specs/data-models/
```

🗺️ **BLUEPRINT READY** — Specifications captured! 🎊

### 🔄 LEVEL 3: MODERNIZE USING SPECS
```bash
# Follow the step-by-step branches
git checkout step-3-data-layer    # Build EF Core entities from data model specs
git checkout step-4-business-logic # Implement services from business rule specs
git checkout step-5-ui-migration   # Build MVC/Blazor from page specs
```

### 🏆 FINAL BOSS: DEPLOY TO AZURE
```bash
git checkout step-6-deploy
# Deploy using provided Bicep templates
gh copilot suggest "Deploy the modernized app to Azure using the infrastructure/main.bicep template"
```

🎉 **VICTORY ACHIEVED** — ViewState monster defeated! 🏅

## 📁 PROJECT STRUCTURE

```
appmodlab-spec2cloud-dotnet-framework/
├── 🎮 README.md                           ← YOU ARE HERE
├── 📚 APPMODLAB.md                         ← Full lab walkthrough
├── 🏛️ RiverdalePermitSystem/              ← Legacy Web Forms app (legacy branch)
│   ├── RiverdalePermitSystem.sln
│   ├── RiverdalePermitSystem.Web/
│   │   ├── Pages/                         ← Web Forms pages (.aspx)
│   │   │   ├── PermitApplication.aspx     ← Multi-step wizard with ViewState 💀
│   │   │   ├── PlanReview.aspx            ← Plan review workflow
│   │   │   ├── InspectionSchedule.aspx    ← Inspector scheduling grid
│   │   │   ├── PermitSearch.aspx          ← GridView + ObjectDataSource
│   │   │   └── Dashboard.aspx             ← UpdatePanels for "AJAX" 🔥
│   │   ├── UserControls/                  ← Reusable .ascx controls
│   │   ├── Reports/                       ← Crystal Reports (.rpt) 📝
│   │   └── App_Code/                      ← DataSet-based data access 🗄️
│   └── Database/                          ← Schema + 40+ stored procedures
├── 🚀 RiverdalePermitSystem.Modern/       ← .NET 9 app (solution branch)
│   ├── RiverdalePermitSystem.Modern.sln
│   ├── src/
│   │   ├── Web/                           ← ASP.NET Core MVC + Blazor
│   │   ├── Core/                          ← Domain models and interfaces
│   │   ├── Infrastructure/                ← EF Core, repositories, services
│   │   └── Shared/                        ← Blazor components
│   └── tests/
├── 📋 specs/                              ← Spec2Cloud generated specs (step-1)
│   ├── architecture.md                    ← System architecture blueprint
│   ├── api-contracts/                     ← API endpoint specifications
│   ├── data-models/                       ← Entity and database specs
│   ├── business-rules/                    ← Business logic documentation
│   └── page-flows/                        ← UI workflow specifications
└── ☁️ infrastructure/                     ← Azure deployment (step-6)
    ├── main.bicep                         ← Infrastructure as Code
    └── parameters.json                    ← Configuration values
```

## 👾 LEGACY STACK (FINAL BOSS DETAILS)

### 🔴 ENEMY ROSTER

| 💀 ENEMY | 🎯 WEAKNESS | 🏆 REPLACEMENT |
|----------|-------------|----------------|
| **ViewState** (200KB+ pages) | Stateless architecture | Blazor Server state management |
| **UpdatePanels** (fake AJAX) | Real interactivity | Blazor components with SignalR |
| **DataSets** (transport objects) | Strong typing | EF Core entities |
| **Stored Proc Logic** (business rules in DB) | Domain-driven design | Domain services in C# |
| **Crystal Reports** (hard-coded connections) | Modern PDF generation | QuestPDF |
| **Session State** (SQL Server storage) | Distributed caching | Azure Redis Cache |
| **Code-Behind** (mixed concerns) | Separation of concerns | MVC pattern + Blazor |

### 🗄️ DATABASE SCHEMA
```
📊 Permits          ← Permit applications and issuance tracking
📋 PlanReviews      ← Architectural/engineering review workflows
🔍 Inspections      ← Scheduled inspections and results
👤 Applicants       ← Property owners and builders
🏗️ Contractors      ← Licensed contractors database
💰 Fees             ← Permit fees and payment tracking
```

### 🧩 LEGACY ANTI-PATTERNS DETECTED
- ⚠️ **Business logic in code-behind event handlers** — No separation of concerns
- ⚠️ **40+ stored procedures with business rules** — Logic trapped in database
- ⚠️ **DataSets stored in session** — Memory bloat and serialization overhead
- ⚠️ **Static helper classes** — Untestable, global state
- ⚠️ **ViewState bloat** — Pages exceed 200KB per request
- ⚠️ **No dependency injection** — Tight coupling everywhere
- ⚠️ **Minimal test coverage** — Changes are risky

## 🎯 TARGET ARCHITECTURE

### 🟢 POWER-UPS UNLOCKED

```
┌─────────────────────────────────────────────────────────────────┐
│  🌐 FRONTEND                                                     │
│  ├── ASP.NET Core MVC (static content)                          │
│  ├── Blazor Server (interactive permit wizard, dashboards)      │
│  └── Razor Components (reusable UI elements)                    │
├─────────────────────────────────────────────────────────────────┤
│  🧠 BUSINESS LAYER                                               │
│  ├── Domain Services (permit validation, fee calculation)       │
│  ├── Repository Pattern (data access abstraction)               │
│  └── Specification Pattern (complex queries)                    │
├─────────────────────────────────────────────────────────────────┤
│  💾 DATA LAYER                                                   │
│  ├── Entity Framework Core 9 (ORM)                              │
│  ├── Strongly-typed entities (no DataSets!)                     │
│  └── Database migrations (code-first approach)                  │
├─────────────────────────────────────────────────────────────────┤
│  ☁️ AZURE SERVICES                                               │
│  ├── App Service (hosting)                                      │
│  ├── Azure SQL Database (data persistence)                      │
│  ├── Azure Redis Cache (distributed caching)                    │
│  ├── Azure Blob Storage (inspection photos)                     │
│  ├── Azure Communication Services (email notifications)         │
│  ├── Azure AI Search (permit search)                            │
│  └── Entra ID (authentication & authorization)                  │
└─────────────────────────────────────────────────────────────────┘
```

### 🔄 SPEC → CODE WORKFLOW

```
┌──────────────────┐      ┌──────────────────┐      ┌──────────────────┐
│  Legacy Codebase │ ───→ │  Spec2Cloud      │ ───→ │  Specifications  │
│  (Web Forms 4.8) │      │  Analysis Tool   │      │  (Markdown Docs) │
└──────────────────┘      └──────────────────┘      └──────────────────┘
                                                              │
                                                              ↓
┌──────────────────┐      ┌──────────────────┐      ┌──────────────────┐
│  Deployed App    │ ←─── │  .NET 9 Build    │ ←─── │  Implementation  │
│  (Azure)         │      │  (MVC + Blazor)  │      │  (Spec-driven)   │
└──────────────────┘      └──────────────────┘      └──────────────────┘
```

**📋 SPEC GENERATED** → **🗺️ BLUEPRINT READY** → **🔄 SPEC → CODE** → **💀 VIEWSTATE ELIMINATED** → **🏆 VICTORY**

## 🎮 LAB WALKTHROUGH USING COPILOT CLI + SPEC2CLOUD

### 🕹️ STAGE 1: EXPLORATION (30 min)
```bash
# Switch to legacy branch and explore the Web Forms horror
git checkout legacy
gh copilot explain "What are the main anti-patterns in this ASP.NET Web Forms application?"
gh copilot suggest "Run the RiverdalePermitSystem locally and test the permit submission workflow"
```

**🎯 Objectives:**
- ✅ Run the legacy Web Forms app
- ✅ Submit a test permit application
- ✅ Schedule an inspection
- ✅ Generate a Crystal Report
- ✅ Observe ViewState size (check dev tools network tab!)

### 🕹️ STAGE 2: SPEC GENERATION (1 hour)
```bash
# Generate specifications using Spec2Cloud
git checkout step-1-spec-generation
gh copilot suggest "Analyze the Web Forms codebase with Spec2Cloud and generate architecture specifications"

# Review generated specs
gh copilot explain "Explain the generated architecture.md specification"
```

**🎯 Objectives:**
- ✅ Run Spec2Cloud analysis on legacy codebase
- ✅ Review architecture specifications
- ✅ Examine API contract definitions
- ✅ Study data model specifications
- ✅ Document business rules found in stored procedures

**📋 SPEC GENERATED** ← *You are here!*

### 🕹️ STAGE 3: SPEC REVIEW & DECISIONS (1 hour)
```bash
git checkout step-2-spec-review
# Review refined specifications with modernization decisions annotated
```

**🎯 Objectives:**
- ✅ Review each specification for completeness
- ✅ Identify modernization patterns (DataSet → Entity, etc.)
- ✅ Document technology choices in specs
- ✅ Map Web Forms pages to MVC/Blazor equivalents

**🗺️ BLUEPRINT READY** ← *You are here!*

### 🕹️ STAGE 4: BUILD DATA LAYER (1.5 hours)
```bash
git checkout step-3-data-layer
gh copilot suggest "Create EF Core entities based on the data model specifications"

# Use Copilot to generate entities from specs
gh copilot chat "Create the Permit entity class based on specs/data-models/permit.md"
gh copilot chat "Create the DbContext with all entities and relationships"
```

**🎯 Objectives:**
- ✅ Create EF Core entities matching data model specs
- ✅ Configure DbContext with relationships
- ✅ Implement repository interfaces
- ✅ Create database migrations
- ✅ Seed test data

**🗄️ DATASET PURGED** ← *Boss defeated!*

### 🕹️ STAGE 5: BUILD BUSINESS LAYER (1.5 hours)
```bash
git checkout step-4-business-logic
gh copilot suggest "Implement domain services based on business rule specifications"

# Extract logic from stored procedures using specs
gh copilot chat "Convert the usp_ValidatePermitApplication stored procedure to a C# domain service using specs/business-rules/permit-validation.md"
```

**🎯 Objectives:**
- ✅ Create domain service interfaces
- ✅ Implement permit validation service
- ✅ Implement fee calculation service
- ✅ Implement inspection scheduling service
- ✅ Add unit tests for business logic

**🧙 STORED PROC LOGIC REFACTORED** ← *Boss defeated!*

### 🕹️ STAGE 6: BUILD UI LAYER (2 hours)
```bash
git checkout step-5-ui-migration
gh copilot suggest "Create MVC controllers and Blazor components based on page specifications"

# Convert Web Forms pages using specs
gh copilot chat "Convert PermitApplication.aspx to an MVC controller and Blazor wizard component using specs/page-flows/permit-application.md"
```

**🎯 Objectives:**
- ✅ Create MVC controllers and views
- ✅ Build Blazor components for interactive elements
- ✅ Implement permit submission wizard (Blazor)
- ✅ Implement inspection scheduling grid (Blazor)
- ✅ Build admin dashboard with real-time updates
- ✅ Replace Crystal Reports with QuestPDF

**💀 VIEWSTATE ELIMINATED** ← *Final boss defeated!*  
**🔥 UPDATEPANEL DESTROYED** ← *Final boss defeated!*  
**📝 CRYSTAL REPORTS REPLACED** ← *Final boss defeated!*

### 🕹️ STAGE 7: AZURE DEPLOYMENT (1 hour)
```bash
git checkout step-6-deploy
gh copilot suggest "Deploy the application to Azure using the Bicep templates in infrastructure/"

# Deploy infrastructure
az deployment group create --resource-group rg-riverdale-permits \
  --template-file infrastructure/main.bicep \
  --parameters infrastructure/parameters.json

# Deploy application
gh copilot suggest "Publish the .NET 9 application to Azure App Service"
```

**🎯 Objectives:**
- ✅ Provision Azure resources (App Service, SQL, Redis, Blob, etc.)
- ✅ Configure Entra ID authentication
- ✅ Deploy application code
- ✅ Run database migrations in Azure SQL
- ✅ Configure Azure Communication Services for emails
- ✅ Test all workflows in production

**🏆 VICTORY ACHIEVED** — Game complete! 🎊

## ⏱️ DURATION

**🎮 TOTAL PLAYTIME:** 6–8 hours

**⏳ STAGE BREAKDOWN:**
- 🕹️ Stage 1: Exploration — 30 min
- 🕹️ Stage 2: Spec Generation — 1 hour
- 🕹️ Stage 3: Spec Review — 1 hour  
- 🕹️ Stage 4: Data Layer — 1.5 hours
- 🕹️ Stage 5: Business Layer — 1.5 hours
- 🕹️ Stage 6: UI Layer — 2 hours
- 🕹️ Stage 7: Azure Deployment — 1 hour

**💡 PRO TIP:** Use GitHub Copilot CLI throughout for maximum efficiency! The `gh copilot suggest` and `gh copilot chat` commands are your power-ups! 🚀

## 📚 RESOURCES

### 🎓 LEARNING MATERIALS
- 📖 [Spec2Cloud Documentation](https://github.com/microsoft/spec2cloud)
- 📖 [ASP.NET Core Migration Guide](https://docs.microsoft.com/aspnet/core/migration/proper-to-2x)
- 📖 [Blazor Server Documentation](https://docs.microsoft.com/aspnet/core/blazor/hosting-models#blazor-server)
- 📖 [Entity Framework Core 9](https://docs.microsoft.com/ef/core/)
- 📖 [QuestPDF Documentation](https://www.questpdf.com/)

### 🛠️ TOOLS & FRAMEWORKS
- 🔧 [GitHub Copilot CLI](https://githubnext.com/projects/copilot-cli)
- 🔧 [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- 🔧 [Visual Studio 2022](https://visualstudio.microsoft.com/)
- 🔧 [Azure CLI](https://docs.microsoft.com/cli/azure/)

### ☁️ AZURE SERVICES
- ☁️ [Azure App Service](https://azure.microsoft.com/services/app-service/)
- ☁️ [Azure SQL Database](https://azure.microsoft.com/services/sql-database/)
- ☁️ [Azure Redis Cache](https://azure.microsoft.com/services/cache/)
- ☁️ [Azure Blob Storage](https://azure.microsoft.com/services/storage/blobs/)
- ☁️ [Azure Communication Services](https://azure.microsoft.com/services/communication-services/)
- ☁️ [Azure AI Search](https://azure.microsoft.com/services/search/)
- ☁️ [Microsoft Entra ID](https://www.microsoft.com/security/business/identity-access/microsoft-entra-id)

### 🎮 RELATED LABS
- 🕹️ [Spec2Cloud Java Spring Boot Modernization](../appmodlab-spec2cloud-java/)
- 🕹️ [Spec2Cloud Node.js Modernization](../appmodlab-spec2cloud-nodejs/)
- 🕹️ [Azure Migration Assessment Lab](../appmodlab-azure-migrate/)

---

## 🏆 ACHIEVEMENTS UNLOCKED

Complete the lab to earn these badges:

- 🥇 **ViewState Slayer** — Eliminated 200KB+ ViewState pages
- 🥇 **UpdatePanel Destroyer** — Replaced fake AJAX with Blazor
- 🥇 **DataSet Purger** — Migrated all DataSets to EF Core entities
- 🥇 **Crystal Crusher** — Replaced Crystal Reports with QuestPDF
- 🥇 **Spec Master** — Generated and used specifications throughout modernization
- 🥇 **Azure Champion** — Successfully deployed to Azure
- 🥈 **Migration Architect** — Completed full modernization workflow
- 🥈 **.NET Legend** — Mastered both .NET Framework and .NET 9

---

## 🎪 GAME OVER... OR JUST BEGINNING?

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                  │
│   ██╗   ██╗██╗ ██████╗████████╗ ██████╗ ██████╗ ██╗   ██╗      │
│   ██║   ██║██║██╔════╝╚══██╔══╝██╔═══██╗██╔══██╗╚██╗ ██╔╝      │
│   ██║   ██║██║██║        ██║   ██║   ██║██████╔╝ ╚████╔╝       │
│   ╚██╗ ██╔╝██║██║        ██║   ██║   ██║██╔══██╗  ╚██╔╝        │
│    ╚████╔╝ ██║╚██████╗   ██║   ╚██████╔╝██║  ██║   ██║         │
│     ╚═══╝  ╚═╝ ╚═════╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝   ╚═╝         │
│                                                                  │
│             🎉 CONGRATULATIONS, SPEC WARRIOR! 🎉                 │
│                                                                  │
│  You've defeated the ViewState monster and modernized a legacy  │
│  .NET Framework application using the Spec2Cloud methodology!   │
│                                                                  │
│         📋 SPEC GENERATED → 🗺️ BLUEPRINT READY →                │
│         🔄 SPEC → CODE → 💀 VIEWSTATE ELIMINATED                │
│                                                                  │
│              ⭐ HIGH SCORE: LEGACY APP MODERNIZED ⭐             │
│                                                                  │
│                   Press START to play again                     │
│                 (Try another modernization lab!)                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

**🎮 Ready Player One?** Let's modernize some legacy code! 🚀

**📋 SPEC GENERATED** | **🗺️ BLUEPRINT READY** | **🔄 SPEC → CODE** | **💀 VIEWSTATE ELIMINATED**

*Built with ❤️ by the GBB App Modernization Team*  
*Powered by GitHub Copilot CLI 🤖 and Spec2Cloud 📋*

---

## 🏆 SOLUTION WALKTHROUGH

The completed lab is available on the **`solution-final`** branch. Each step is tagged so you can check out any point in the walkthrough and see the cumulative output files that were generated.

### 🔀 Viewing the Completed Solution

```bash
# Switch to the finished solution branch
git checkout solution-final

# Or view a specific step by its tag
git checkout step-01-explore-legacy-app
```

### 🏷️ Step Tags & Output Files

Each step has a corresponding Git tag and produces output files in `assets/outputs/`:

| Step | Tag | Title | Output Files |
|------|-----|-------|-------------|
| 1 | `step-01-explore-legacy-app` | Explore Legacy App | `assets/outputs/step-01.txt`, `assets/outputs/step-01-explore-legacy-app.md` |
| 2 | `step-02-spec2cloud-analysis` | Run Spec2Cloud Analysis | `assets/outputs/step-02.txt`, `assets/outputs/step-02-spec2cloud-analysis.md` |
| 3 | `step-03-architecture-spec` | Generate Architecture Spec | `assets/outputs/step-03.txt`, `assets/outputs/step-03-architecture-spec.md` |
| 4 | `step-04-component-specs` | Generate Component Specs | `assets/outputs/step-04.txt`, `assets/outputs/step-04-component-specs.md` |
| 5 | `step-05-modern-architecture` | Design Modern Architecture | `assets/outputs/step-05.txt`, `assets/outputs/step-05-modern-architecture.md` |
| 6 | `step-06-migration-plan` | Generate Migration Plan | `assets/outputs/step-06.txt`, `assets/outputs/step-06-migration-plan.md` |
| 7 | `step-07-scaffold-modern` | Scaffold Modern App | `assets/outputs/step-07.txt`, `RiverdalePermitSystem.Modern/` (full project) |
| 8 | `step-08-validate` | Validate Against Specs | `assets/outputs/step-08.txt`, `assets/outputs/step-08-validation-report.md` |

### 📂 Output File Conventions

- **`step-NN.txt`** — Raw console/session output captured during the step
- **`step-NN-<name>.md`** — Structured Markdown specification or report generated by that step
- Step 7 also produces the entire `RiverdalePermitSystem.Modern/` project tree (Domain, Application, Infrastructure, and Web layers)

### 🔍 Comparing Steps

```bash
# See what changed between any two steps
git diff step-03-architecture-spec..step-04-component-specs

# See only the file names that changed
git diff --name-only step-06-migration-plan..step-07-scaffold-modern

# View the full history from start to finish
git log --oneline step-01-explore-legacy-app..step-08-validate
```
