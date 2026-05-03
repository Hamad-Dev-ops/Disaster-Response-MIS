# 🌊 Smart Disaster Response Management Information System (MIS)

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-12.0-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)](https://www.microsoft.com/en-us/sql-server)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/Hamad-Dev-ops/Disaster-Response-MIS)](https://github.com/Hamad-Dev-ops/Disaster-Response-MIS/stargazers)

## 📋 Overview

A **comprehensive, enterprise-level Disaster Response Management Information System** designed to handle real-time emergency coordination during natural disasters such as floods, earthquakes, and fires. The system supports high-volume transactions, role-based access control, ACID-compliant operations, and provides analytical reporting for decision-makers.

**🎓 Academic Project** – This was developed as a semester database systems project, demonstrating advanced database concepts including normalization (3NF/BCNF), triggers, stored procedures, views, indexing, and performance optimization.

## ✨ Key Features

### 👥 Role-Based Access Control (5 User Roles)
| Role | Responsibilities |
|------|-----------------|
| **Administrator** | Full system control, user management, system statistics |
| **Emergency Operator** | Report incidents, track status, prioritize emergencies |
| **Warehouse Manager** | Manage inventory, allocate resources, add warehouses |
| **Finance Officer** | Handle donations, expenses, financial reporting |
| **Field Officer** | Update incident status, view active incidents |

### 🚨 Emergency Management
- Real-time incident reporting with location (latitude/longitude)
- Disaster type classification (Flood, Earthquake, Fire, Hurricane)
- Severity and priority levels (1-5)
- Status tracking (Reported → Assigned → InProgress → Resolved → Closed)

### 📦 Resource Management
- Warehouse-wise inventory tracking
- Resource allocation with automatic stock deduction
- Dispatch and consumption tracking
- Low stock threshold alerts
- Multi-step approval workflow

### 💰 Financial Management
- Donation and expense tracking
- Budget management per disaster event
- Transaction categorization
- Financial audit trails

### 🤖 Database Automation (Triggers)
- `trg_AfterDispatch` – Auto-deducts inventory after dispatch
- `trg_TeamAssignmentComplete` – Auto-updates team availability
- `trg_PreventNegativeStock` – Prevents inventory from going negative
- `trg_Audit_Incident` – Logs all changes for compliance

### 🔒 ACID Transactions
- Resource allocation with automatic rollback on failure
- Team assignment with status synchronization
- Approval workflow with atomic operations

### 📊 MIS Reports (with Charts & Export)
- Incident Statistics by Location & Severity
- Resource Utilization (Dispatched vs Consumed)
- Response Time Analytics
- Financial Summary (Donations vs Expenses)
- Approval Workflow History
- **Export to Excel** functionality for all reports

---

## 🛠️ Technology Stack

| Category | Technology | Version |
|----------|------------|---------|
| **Backend Framework** | ASP.NET Core MVC | 8.0 |
| **Programming Language** | C# | 12.0 |
| **Database** | Microsoft SQL Server | 2022 / Express |
| **Database Access** | ADO.NET (System.Data.SqlClient) | - |
| **Frontend** | Razor Views, HTML5, CSS3, JavaScript | - |
| **UI Framework** | Bootstrap | 5.3 |
| **Charts** | Chart.js | 4.4 |
| **Notifications** | Toastr.js | - |
| **Icons** | Bootstrap Icons | 1.10 |
| **IDE** | Visual Studio 2022 | - |
| **Database Tool** | SQL Server Management Studio (SSMS) | - |
| **ERD Tool** | ERDPlus | - |

### Database Objects Created
| Object Type | Count |
|-------------|-------|
| Tables | 20+ |
| Stored Procedures | 5 |
| Triggers | 4 |
| Views | 4 |
| Indexes | 7+ |



## 🚀 Installation Guide

### Prerequisites

| Software | Download Link |
|----------|---------------|
| .NET 8 SDK | [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| SQL Server 2022 / Express | [https://www.microsoft.com/sql-server](https://www.microsoft.com/sql-server) |
| SQL Server Management Studio (SSMS) | [https://docs.microsoft.com/ssms](https://docs.microsoft.com/ssms) |
| Visual Studio 2022 | [https://visualstudio.microsoft.com](https://visualstudio.microsoft.com) |
| Git | [https://git-scm.com](https://git-scm.com) |

### Step 1: Clone the Repository

```bash
git clone https://github.com/Hamad-Dev-ops/Disaster-Response-MIS.git
cd Disaster-Response-MIS
