# Coaches Comprehensive System (CCS)

A robust, offline-first Windows desktop application designed to manage comprehensive records for sports coaches (specifically tailored for Chess coaches). The system handles personal information, organizational documents, coaching courses, bank details, chess training records, dynamic custom fields, and financial payments with high performance and data integrity.

## Architecture & Technical Methodology

The project is built using a clean, **N-Tier Architecture** to ensure maintainability, scalability, and separation of concerns. The system operates entirely offline and relies on a local, portable database.

### Solution Layers:
1.  **Presentation Layer (`CCS.App`):**
    *   Built with **WPF (Windows Presentation Foundation)**.
    *   Features a custom, modern UI with customized Window Chrome (unified title bars, custom minimize/maximize/close buttons) across all windows.
    *   Implements an interface-driven role-based sidebar navigation.
2.  **Core Layer (`CCS.Core`):**
    *   Contains domain entities (Models), global constants, static session management (`Session.cs`), and utilities (e.g., `Clock`, `AppPaths`).
    *   Acts as the shared foundation for all other projects.
3.  **Data Access Layer (`CCS.Data`):**
    *   Uses **SQLite** as the local, file-based relational database (ensuring easy backup and portability).
    *   Utilizes **Dapper** (a high-performance Micro-ORM) for fast, lightweight data mapping.
    *   Implements a custom, version-controlled migration system (`DatabaseInitializer`) to handle schema updates seamlessly.
4.  **Service Layer (`CCS.Services`):**
    *   Contains all business logic and orchestrates data flow between the UI and the database.
    *   Handles complex operations such as file system management, password hashing (PBKDF2 with SHA256), auto-backup mechanisms, and data exporting.
5.  **Test Layer (`CCS.Tests`):**
    *   Uses **xUnit** for unit and integration testing, ensuring database integrity and business logic reliability.

### Key Technologies & Libraries:
*   **Framework:** .NET Framework 4.8 (Chosen specifically to provide backward compatibility with **Windows 7 SP1** and above, while still supporting modern C# features).
*   **Database:** SQLite (`System.Data.SQLite.Core`).
*   **Reporting & Export:**
    *   **ClosedXML:** For generating complex, RTL-supported Excel (`.xlsx`) reports.
    *   **iText7:** For generating professional, formatted PDF resumes and reports with Persian font support.
*   **Compression:** `System.IO.Compression` for generating ZIP archives of documents and database backups.

## How It Works (System Methodology)

*   **Security & RBAC (Role-Based Access Control):** The system manages three distinct user roles:
    *   `Admin`: Complete access to all modules, user management, and settings. (Default user).
    *   `Manager`: Full access to records, exports, and backups, but cannot manage system users.
    *   `Registrar`: Highly restricted access, allowing only the registration of new coaches.
*   **Dynamic Form Builder:** Admins can create infinite custom fields (Text, Number, Date, File Upload, Checkbox, Radio, Dropdown, Multi-select, and Template Roundtrip). These fields dynamically render and behave correctly on the coach registration page.
*   **File Management:** Uploaded files are stored securely in the local `%LocalAppData%\CoachesComprehensiveSystem\files` directory with unique GUID names, while metadata is stored in SQLite.
*   **Auto-Backup:** A background service checks the last backup timestamp on startup. If the configured interval has passed, it automatically zips the database and file repository into the `backups` folder, keeping only the 5 most recent backups.
*   **Data Export:** The system allows exporting:
    *   One coach, a specific group (by file type), or all coaches.
    *   As a structured ZIP of documents.
    *   As professional PDF/Excel resumes.
    *   As payment status reports or coaching course lists.

##  How to Use & Build

###  Note on missing `bin` and `obj` folders:
You will notice that the `bin` (binaries) and `obj` (intermediate build files) directories are **intentionally excluded** from this repository. It is a standard best practice in software development not to commit compiled binaries to source control. You must compile the project locally to generate them.

### Prerequisites
*   Windows 7 SP1 or later.
*   .NET Framework 4.8 Runtime.
*   .NET SDK (v8.0 or v10.0) installed for building.

