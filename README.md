# 🚗 DVLD — Driving License Management System

A production-grade, three-tier Windows desktop application that manages the complete driving license lifecycle — from initial application through test scheduling, pass/fail tracking, and automatic license issuance.

Built with C#, WinForms, pure ADO.NET, and stored procedures across a clean layered architecture.

---

## 🎯 Overview

DVLD is a fully fledged workflow engine that enforces real regulatory business rules at every level:

- ❌ Applicants cannot hold duplicate licenses of the same class
- 👁️ ✍️ 🚗 Licenses require passing three sequential tests: vision → written → road
- 📅 Age requirements are validated before any application is accepted
- 📊 Full audit trail with attempt counting and automatic license generation
- 🔒 Enterprise-grade security with asymmetric cryptography session management

---

## ✨ Key Features

| Feature | Description |
|---|---|
| 🪪 First License Issuance Workflow | Multi-stage validation + test sequencing + automatic license generation |
| 📋 Test Management | Vision, written, and street tests tracked independently with attempt history and retake fees |
| 🔄 License Operations | Renewal, replacement (lost/damaged), and international license issuance |
| 👤 Driver & Vehicle Management | Full driver records with license class associations and detainment tracking |
| 🔐 Enterprise Security | Password hashing + Windows Registry session management with asymmetric cryptography |
| ⚙️ Status State Machine | New → Pending → Cancelled → Completed with enforced transitions |

---

## 🛠️ Tech Stack

| Technology | Role |
|---|---|
| C# / .NET Framework 4.8 | Primary language |
| WinForms | Desktop UI layer |
| SQL Server | Relational database (Express or higher) |
| ADO.NET | Data access — handwritten parameterized queries, no ORM |
| Three-Tier Architecture | Presentation → Business Logic → Data |

### 💡 Why Pure ADO.NET?

This project deliberately avoids Entity Framework and ORMs in favor of direct ADO.NET:

- 🎯 Total control over query execution plans
- 🛡️ SQL injection protection via parameterized queries
- 📤 Direct access to output parameters and stored procedure return values
- 👁️ Complete visibility into every database call
- 🚀 Performance that can be measured and optimized per query

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────┐
│         PRESENTATION TIER               │
│         WinForms UI Layer               │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         BUSINESS LOGIC TIER             │
│   Domain Logic · Rules · Validation     │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│           DATA ACCESS TIER              │
│     ADO.NET · Stored Procedures         │
└─────────────────────────────────────────┘
```

Each layer communicates only with the layer directly below it. No cross-layer dependencies.

### 📐 Design Patterns

| Pattern | Usage |
|---|---|
| 📦 Repository | Data tier classes encapsulate all database access (`clsUsersManagmentDataTier`, `clsLicenseDataTier`, `clsApplicationsDataTier`) |
| 🏭 Factory | Static `Find()` methods instantiate domain objects from database rows |
| 📒 Active Record | Business classes combine data and behavior (`clsLicenseManagement`, `clsLocalLicenseApplicationManagement`) |
| 🔁 State Machine | Application status transitions enforced in the business layer — no illegal states permitted |
| ⚡ Transaction Script | License issuance orchestrates multiple database writes atomically |

---

## 🪪 The First License Issuance Flow

This is the core of the system. It enforces a strict, non-skippable workflow:

```
STEP 1 — Validate Prerequisites
  ├── No active license of same class exists
  ├── No pending application already in system
  └── Age requirement met for selected license class

STEP 2 — Vision Test
  ├── Schedule appointment
  ├── Record pass/fail result
  └── Track attempts and fees per attempt

STEP 3 — Written Test
  ├── Only accessible after vision test passes
  ├── Full history preserved
  └── Retake fees configured per test type

STEP 4 — Street/Driving Test
  ├── Only accessible after written test passes
  ├── Final gate before license issuance
  └── Full retake policy and fee tracking

STEP 5 — License Issuance (automatic)
  ├── Triggered when all 3 tests pass
  ├── Expiration date calculated by license class
  ├── Unique license number generated
  ├── Driver record updated
  └── Fee calculated and payment recorded
```

Each test independently tracks: attempt count, pass/fail result, retake fees per attempt, and complete history. Steps cannot be skipped or bypassed — the system enforces the process.

---

## ⚡ Setup & Installation

### 📋 Prerequisites

| Requirement | Version |
|---|---|
| Visual Studio | 2019 or higher |
| .NET Framework | 4.8 |
| SQL Server | Express or higher |

### 🔧 Steps

1. Clone the repository
2. Open SQL Server and restore the `.bak` file from `Database/DVLD_DB_Backup`
3. Name the database exactly `DVLD_02` (case-sensitive)
4. Open `MyDVLD_Project0.sln` in Visual Studio
5. Build projects in this order to resolve dependencies:
   - `MyDVLD_DataTier`
   - `MyDVLD_BusinessTier`
   - `MyDVLD_PeresentationTier`
   - `MyDVLD_Project0`
6. Run `MyDVLD_Project0`

### 🔑 Default Login

On first launch, the app prompts to set up credentials automatically. A pre-configured admin account is available:

| Field | Value |
|---|---|
| Username | `Admin_1` |
| Password | `123123123` |

---

## 💻 Usage Guide

| Action | How |
|---|---|
| Add new driver | People → Add New Person → Create User Account |
| Apply for license | Local Driving License App → New Application → Select License Class → Schedule Tests |
| Record test result | Test Appointments → Take Test → Enter Pass/Fail |
| Issue license | Automatic when all 3 tests pass |
| Renew license | Licenses → Renew → New expiration and fees calculated |
| Replace license | Licenses → Replace (Lost/Damaged) → New license number generated |
| Detain license | Detain License → Enter fine → License marked as blocked |
| International license | International Driving License App |

---

## 📁 Project Structure

```
Full Project/
├── MyDVLD_Project0/            # Entry point and main form
├── MyDVLD_PeresentationTier/   # WinForms UI (users, licenses, tests, people)
├── MyDVLD_BusinessTier/        # Domain logic, validation, and rules
├── MyDVLD_DataTier/            # ADO.NET connections and stored procedures
└── Database/                   # SQL Server backup file (.bak)
```

---

## 🗺️ Roadmap

| Improvement | Purpose |
|---|---|
| ASP.NET Core Web API | Enable multi-user access via web and mobile |
| Audit Logging | Regulatory compliance and accountability |
| Government ID API Integration | Automated identity document verification |
| Biometric Capture | Fingerprint and photo for secure identification |

---

## 🎓 What This Project Demonstrates

- ✅ Three-tier architecture with clean separation of concerns
- ✅ Complex workflow orchestration across the license issuance flow
- ✅ State machine design enforcing application status transitions
- ✅ Raw ADO.NET mastery without ORM abstractions
- ✅ Real-world business rule enforcement simulating regulatory compliance
- ✅ End-to-end full-stack thinking from SQL to UI
- ✅ Enterprise-level security, audit trails, and validation
