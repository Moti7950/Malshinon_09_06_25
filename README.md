# 🕵️ Malshinon - Community Intelligence Reporting System

**Author:** Moti Kopshitz  
**Date:** 09/06/2025  
**Repository:** [GitHub - Malshinon_09_06_25](https://github.com/Moti7950/Malshinon_09_06_25.git)

---

## 📌 Overview

Malshinon is a console-based community intelligence simulation system.  
It allows users to log in by name or secret code, report suspicious individuals, and stores all reports in a structured MySQL database. The system analyzes the data and automatically highlights individuals who receive multiple reports within a short time frame.

---

## 🧭 System Functionality

### 🔹 Main Menu

When the application starts, the user is presented with:

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
- Otherwise, an error message is shown.

#### Option 2: Login by Secret Code
- The user provides a secret code.
- If the code exists, the user is identified and welcomed.
- If not, the system attempts to extract the name from the input and creates a new user.
- The user is then prompted to enter a free-text report as in Option 1.

#### Option 3: Exit
- Cleanly closes the application.

---

## 🔍 Report Analysis

After submitting a report, the system:
- Saves the report with a timestamp in the `intelreports` table.
- Updates the number of mentions and reports in the `people` table.
- Checks whether the same target has received 3 or more reports within a 15-minute window.
- If so, it displays a **red alert box** with report details.

---

## 🧱 Database Schema

The system requires a MySQL database named `malshinon` with the following schema:

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
| `TempReport.cs`     | Temporary data structure for holding report data. |
| `App.config`        | Holds MySQL connection string. |

---

## 🚀 Features Summary

- Dual login method (name or secret code)
- Automatic registration of new users
- Secret code generation
- Full name extraction from free-text
- Report storing and target tracking
- Suspicious activity detection
- Colored alert system in console

---

## 🧪 Example Alert Output

```
╔════════════════════════════════════════════╗
║          ⚠️  ALERT: SUSPICIOUS ACTIVITY    ║
╠════════════════════════════════════════════╣
║ 🎯 Target ID     : 6                       ║
║ 🕒 Time Window   : 10/06/2025 20:22 ➜ 20:37║
║ 📈 Reports Count : 3                       ║
╚════════════════════════════════════════════╝
```

---

## 💻 Running the System

Run the project using:

```bash
dotnet run
```

Make sure MySQL is active and the database `malshinon` is configured with the correct schema and accessible credentials.