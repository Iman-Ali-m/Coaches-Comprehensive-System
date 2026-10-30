# Coaches Comprehensive System (CCS)

A robust, offline-first Windows desktop application designed to manage comprehensive records for sports coaches (specifically tailored for Chess coaches). The system handles personal information, organizational documents, coaching courses, bank details, chess training records, dynamic custom fields, and financial payments.

## 🏗️ Architecture & Technologies

The project is built using a clean, N-Tier architecture to ensure maintainability and separation of concerns:

*   **UI Layer (CCS.App):** Windows Presentation Foundation (WPF) with a custom modern UI, role-based sidebar navigation, and custom window chrome.
*   **Core Layer (CCS.Core):** Contains domain models, constants, global session management, and core utilities.
*   **Data Layer (CCS.Data):** Handles SQLite database connections, schema migrations (`DatabaseInitializer`), and data access using Dapper.
*   **Services Layer (CCS.Services):** Contains business logic, including CRUD operations, file management, auto-backup, Excel/PDF exports, and authentication.
*   **Test Layer (CCS.Tests):** Unit tests using xUnit to ensure database integrity and service logic.

### Technologies Used:
*   **Framework:** .NET Framework 4.8 (Provides support for Windows 7 SP1 and above).
*   **Database:** SQLite (Local file-based, easily portable and backup-able).
*   **ORM:** Dapper (Micro-ORM for high performance).
*   **Reporting:** ClosedXML (Excel generation) and iText7 (PDF generation with Persian/RTL support).
*   **Installer:** Inno Setup (for creating the Windows `Setup.exe`).

## ✨ Key Features

*   **Role-Based Access Control (RBAC):** 
    *   `Admin`: Full access to all features, including user management and system settings.
    *   `Manager`: Full access to records and exports, but restricted from user management.
    *   `Registrar`: Restricted access, only able to register new coaches.
*   **Comprehensive Data Entry:** Detailed forms for personal info, organizational documents, coaching courses, bank accounts, and chess training.
*   **Dynamic Form Builder:** Admins can create custom fields (Text, Number, Date, File, Checkbox, Dropdown, Radio, Multi-select, Template Roundtrip) that automatically appear in the coach registration form.
*   **File Management & ZIP Export:** Upload unlimited documents, attach them to specific records, and export a complete ZIP file per coach organized by folder structure.
*   **Advanced Reporting:** Generate professional PDF resumes, payment status reports, and coaching course lists (both Excel and PDF) for a single coach, a specific group, or all coaches.
*   **Auto-Backup & Restore:** The system automatically creates ZIP backups of the database and files based on user-defined intervals. Manual restore is also supported.

## 🚀 How to Use / Installation

### Prerequisites
*   Windows 7 SP1 or later.
*   .NET Framework 4.8 Runtime installed.
*   .NET SDK (for building from source).
