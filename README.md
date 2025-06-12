# 🕵️ Malshinon - Community Intelligence Reporting System

**Author:** Moti Kopshitz  
**Date:** 09/06/2025  
**Repository:** [GitHub - Malshinon_09_06_25](https://github.com/Moti7950/Malshinon_09_06_25.git)

---

## 📌 Overview

Malshinon is a console-based community intelligence simulation system.  
It allows users to log in by name or secret code, report suspicious individuals, and stores all reports in a structured MySQL database.  
The system analyzes the data and automatically highlights individuals who receive multiple reports within a short time frame.  
All actions are logged in a centralized file.

---

## 🧭 System Functionality

### 🔹 Main Menu

```
╔══════════════════════════════╗
║  Please select your choice   ║
║   1. name                    ║
║   2. secret code             ║
║   3. Exit                    ║
╚══════════════════════════════╝
```

#### Option 1: Login by Full Name
- The user enters their first and last name.
- If the user does not exist in the database, they are automatically registered with a secret code.
- If the user already exists, they are welcomed and their secret code is retrieved.
- The user is prompted to enter a free-text report.
- The system extracts two consecutive capitalized words as the target’s name.
- If the target exists in the system, a report is saved to the database.
- After submitting the report:
  - The report is saved to `intelreports`.
  - The target's report count is updated.
  - The reporter's mention count is updated.
  - A log entry is created for each action.

#### Option 2: Login by Secret Code
- The user provides a secret code.
- If the code exists, the user is identified and welcomed.
- If not, the system attempts to extract the name from the input and creates a new user.
- The user is then prompted to enter a free-text report.
- The same post-report logic applies as in Option 1.

#### Option 3: Exit
- Cleanly closes the application.

---

## 📊 Logging System

All critical system actions are logged to a dedicated file with a timestamp.

The system logs:
- Successful logins and failed attempts
- New user registrations
- Report submissions
- Updates to `num_reports` and `num_mentions`
- Detected suspicious activities
- All system errors (try/catch)

Logs are used both for auditing and debugging purposes.

---

## 🔍 Report Analysis

After each submitted report, the system:
- Saves the report with a timestamp in the `intelreports` table.
- Automatically updates:
  - The number of reports for the target
  - The number of mentions for the reporter
- Scans all targets for suspicious behavior:
  - If 3 or more reports are detected on the same target within a 15-minute window, a red alert is displayed.

---

## ⚙️ Log Configuration

### 📁 Customizing Log File Path

By default, logs are saved to:

```
logs/log.txt
```

You can change the log file path dynamically during runtime:

```csharp
logger.SetLogPath("logs/admin_actions.txt");
```

This will automatically create the directory and file if they do not exist.

---

### 🧹 Clearing Log File Content

To erase all content in the log file (without deleting the file itself), use:

```csharp
File.WriteAllText(logger.CurrentLogPath, string.Empty);
```

Alternatively, add this method to the `Logs` class:

```csharp
public void ClearLog()
{
    File.WriteAllText(CurrentLogPath, string.Empty);
}
```

Then you can call:

```csharp
logger.ClearLog();
```

---

## 🧱 Database Schema

The system uses a MySQL database named `malshinon` with the following schema:

```sql
CREATE DATABASE malshinon;
USE malshinon;

CREATE TABLE people (
    id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    secret_code VARCHAR(10),
    num_mentions INT DEFAULT 0,
    num_reports INT DEFAULT 0,
    type VARCHAR(20)
);

CREATE TABLE intelreports (
    id INT AUTO_INCREMENT PRIMARY KEY,
    reporter_id INT NOT NULL,
    target_id INT NOT NULL,
    text TEXT NOT NULL,
    timestamp DATETIME NOT NULL,
    FOREIGN KEY (reporter_id) REFERENCES people(id),
    FOREIGN KEY (target_id) REFERENCES people(id)
);
```

---

## 🗂️ Code Structure

| File                | Description |
|---------------------|-------------|
| `Program.cs`        | Entry point. Initializes and runs the menu. |
| `Menu.cs`           | Handles all user interactions, login flow, and reporting. |
| `DAL.cs`            | Data Access Layer: all database operations (insert, select, update). |
| `PeopleDB.cs`       | Model class representing a person in the system. |
| `IntelreportsDB.cs` | Model class representing a report. |
| `LogDB.cs`          | Placeholder class for logging system actions. |
| `Logs.cs`           | Centralized logger for writing actions to file and displaying them. |
| `TempReport.cs`     | Temporary data structure for holding report data. |
| `App.config`        | Holds MySQL connection string. |

---

## 💻 Running the System

Run the project using:

```bash
dotnet run
```

Make sure MySQL is active and the database `malshinon` is configured with the correct schema and accessible credentials.
