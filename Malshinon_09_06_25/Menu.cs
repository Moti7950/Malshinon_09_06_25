using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon_09_06_25
{
    internal class Menu
    {
        private DAL _StartRunCode = new DAL("malshinon");

        private Dictionary<string, string> adminList = new Dictionary<string, string>
        {
            { "admin", "admin" }, // דוגמה למשתמש אדמין
            // ניתן להוסיף משתמשים נוספים כאן
        };
    private bool IsCapitalized(string word)
        {
            return !string.IsNullOrEmpty(word) &&
                   char.IsUpper(word[0]) &&
                   word.Skip(1).All(c => char.IsLower(c) || !char.IsLetter(c));
        }

        public Menu()
        { }
        public void Login()
        {

        }
        public List<string> SplitBySpace(string fullName)
        {
            string[] parts = fullName.Split(' ');
            string firstName = parts[0];
            string lastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";

            return new List<string> { firstName, lastName };
        }
        //הפונקציה הזו מקבלת קובץ טקסט ומחלצת את השם לפי אות גדולה שים לב שהתרגום רק לשפה העברית
        public List<string> ExtractFullNameFromText(string text)
        {
            string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            for (int i = 0; i < words.Length - 1; i++)
            {
                string word1 = words[i].Trim(new char[] { '.', ',', ';', '!', '?' });
                string word2 = words[i + 1].Trim(new char[] { '.', ',', ';', '!', '?' });

                if (IsCapitalized(word1) && IsCapitalized(word2))
                {
                    return new List<string> { word1, word2 }; // מצאנו שם פרטי ומשפחה
                }
            }

            return new List<string> { "", "" }; // לא נמצא
        }

        public void HandleAndSendReport(string inputSecretCode)
        {
            // 1. קבלת טקסט חופשי מהמשתמש
            Console.WriteLine("Enter full report text:");
            string text = Console.ReadLine();

            // 2. חילוץ השם מהטקסט (שתי מילים שמתחילות באות גדולה)
            List<string> nameParts = ExtractFullNameFromText(text);
            string firstName = nameParts[0];
            string lastName = nameParts[1];

            // בדיקה אם הצלחנו לחלץ שם
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Could not extract a full name from the report.");
                return;
            }

            // 3. קבלת מזהים מה־DB
            int reporterId = _StartRunCode.GetPersonIdBySecretCode(inputSecretCode);
            int targetId = _StartRunCode.GetPersonIdByName(firstName, lastName);

            if (reporterId == -1)
            {
                Console.WriteLine("User not found.");
                Console.WriteLine(" No user found create a new user");
                PeopleDB createA_TemporaryUser = new PeopleDB(firstName, lastName);
                _StartRunCode.InsertNewPerson(createA_TemporaryUser);
                return;
            }

            if (targetId == -1)
            {
                Console.WriteLine($"Target '{firstName} {lastName}' not found.");
                return;
            }

            // 4. הכנסת הדיווח
            _StartRunCode.InsertIntelReport(new IntelreportsDB(
                reporterId,
                targetId,
                text,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            ));

            Console.WriteLine($"Report saved. Target: {firstName} {lastName} (ID: {targetId})");
        }
        public bool doesTheUserExist(string userExist, string inputUserName)
        {
            //צריך לבדוק איך להכניס נכון את הפרטים בשביל להשתמש במערכות לדוגמא רווח יפריד את השם הפרטי והשם המשפחה
            switch (userExist)
            {
                case "1":

                    //שימוש במתודה של חילוק הרשימה לשם פרטי ושם משפחה
                    List<string> nameParts = SplitBySpace(inputUserName);
                    string firstName = nameParts[0];
                    string lastName = nameParts[1];

                    if (_StartRunCode.GetPersonByName(firstName, lastName))
                    {
                        // קריאה לפונקציה של קבלת נתונים מהמשתמש
                        return true;
                    }
                    else
                    {
                        //במקרה שהמשתמש לא קיים אשמור את הנתונים במשתנה ואצור משתמש חדש
                        Console.WriteLine("No user found!");
                        return false;
                    }

                case "2":
                    //לבדוק למה יצרתי את ההדפסה הזו לבדוק למה לא הכניס שם משפחה
                    Console.WriteLine("Please enter your secret code: ");
                    string inputUsersecretcode = Console.ReadLine();
                    _StartRunCode.GetPersonBySecretCode(inputUsersecretcode);
                    return true;

                default:
                    Console.WriteLine("Please enter a valid value!");
                    return false;

            }
        }
        public static bool adminUsersAcceess(string adminUser, string password,Dictionary<string,string> usersDict)
        {
            if (usersDict.ContainsKey(adminUser))
            {
                if (usersDict[adminUser] == password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        public void startMenu()
        {
            //תחילת חיבור לשרת, כרגע לא מבקש שם שרת
            _StartRunCode.Access_TO_DB();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           Welcome to the Suspicious Activity Report System   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ This system allows you to:                                   ║");
            Console.WriteLine("║   • Report a suspicious person                               ║");
            Console.WriteLine("║   • Log in using your secret code                            ║");
            Console.WriteLine("║   • Or log in using your first and last name                 ║");
            Console.WriteLine("║   • Create new user profiles                                 ║");
            Console.WriteLine("║                                                              ║");
            Console.WriteLine("║       Thank you for choosing our service!                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            bool boli = true;
            while (boli)
            {
                Console.WriteLine(" ╔══════════════════════════════╗");
                Console.WriteLine(" ║  Please select your choce    ║");
                Console.WriteLine(" ║          1. name:            ║");
                Console.WriteLine(" ║          2. secret code:     ║");
                Console.WriteLine(" ║          3. Admin Access:    ║");
                Console.WriteLine(" ║          4. Exit:            ║");
                Console.WriteLine(" ╚══════════════════════════════╝");

                string typeConect = Console.ReadLine();
                //בדיקה אם המשתמש קיים במערכת לפי השם הפרטי והשם משפחה שלו
                if (typeConect == "1")
                {
                    Console.WriteLine("Please enter your first and last name separated by a space: ");
                    string inputUserName = Console.ReadLine();
                    List<string> nameParts = SplitBySpace(inputUserName);

                    string firstName = "";
                    string lastName = "";

                    if (nameParts.Count >= 2)
                    {
                        firstName = nameParts[0];
                        lastName = nameParts[1];

                        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                        {
                            Console.WriteLine("Error: Both first and last name must not be empty.");
                            Console.WriteLine();
                            Console.WriteLine("Please enter your first name:");
                            firstName = Console.ReadLine();
                            Console.WriteLine("Please enter your last name:");
                            lastName = Console.ReadLine();
                        }
                    }

                    else
                    {
                        Console.WriteLine("Error: You must enter both first and last name separated by a space.");
                        Console.WriteLine();
                        Console.WriteLine("Please enter your first name:");
                        firstName = Console.ReadLine();
                        Console.WriteLine("Please enter your last name:");
                        lastName = Console.ReadLine();
                    }
                    //במקרה שהמשתמש לא קיים במערכת
                    if (!doesTheUserExist(typeConect, inputUserName))
                    {
                        //יצירת המשתמש 
                        PeopleDB craiteNweUser = new PeopleDB(firstName, lastName);
                        //הכנסת המשתמש החדש ל DB 
                        _StartRunCode.InsertNewPerson(craiteNweUser);
                    }
                    //במקרה שהמשתמש קיים במערכת
                    else if (doesTheUserExist(typeConect, inputUserName))
                    {
                        Console.WriteLine($"Welcome {firstName} {lastName}");

                        Console.WriteLine("Hi test ");
                        // שליפת הקוד הסודי של המשתמש מתוך ה-DB
                        string secretCode = _StartRunCode.GetSecretCodeByName(firstName, lastName);

                        Console.WriteLine(secretCode);

                        if (string.IsNullOrEmpty(secretCode))
                        {
                            Console.WriteLine("Could not find your secret code in the system.");
                        }
                        else
                        {
                            HandleAndSendReport(secretCode);
                        }
                    }
                }
                //בדיקה אם המשתמש קיים במערכת לפי הקוד הסודי שלו
                else if (typeConect == "2")
                {
                    Console.WriteLine("Please enter your secret code:");
                    string secretCode = Console.ReadLine();

                    if (!doesTheUserExist(typeConect, secretCode))
                    {
                        List<string> nameParts = SplitBySpace(secretCode);
                        if (nameParts.Count < 2)
                        {
                            Console.WriteLine("Please enter both first and last name.");
                            return;
                        }
                        string firstName = nameParts[0];
                        string lastName = nameParts[1];

                        PeopleDB createNewUser = new PeopleDB(firstName, lastName);
                        _StartRunCode.InsertNewPerson(createNewUser);
                    }

                    else
                    {
                        var person = _StartRunCode.GetPersonBySecretCode(secretCode);

                        if (person == null)
                        {
                            Console.WriteLine("Could not find a person with this secret code.");
                        }
                        else
                        {
                            Console.WriteLine($"Welcome back, {person.Value.FirstName} {person.Value.LastName}!");
                            HandleAndSendReport(secretCode);
                        }
                    }
                }
                //תפריט של מנהל
                else if (typeConect == "3")
                {
                    Console.WriteLine("Please enter your Admin user: ");
                    string adminUser = Console.ReadLine();
                    Console.WriteLine("Please enter your Admin password: ");
                    string passwors = Console.ReadLine();
                    adminUsersAcceess(adminUser, passwors, adminList);
                    if (adminUsersAcceess(adminUser, passwors, adminList))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("══════════════════════════════════════════════");
                        Console.WriteLine($"        👤 Admin Panel - Welcome {adminUser}");
                        Console.WriteLine("══════════════════════════════════════════════");

                        // הגדרת צבע לטקסט של התפריט
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════╗");
                        Console.WriteLine("║ 📋  Please choose from menu:   ║");
                        Console.WriteLine("╠════════════════════════════════╣");
                        Console.WriteLine("║ 1. 👥 Get all users            ║");
                        Console.WriteLine("║ 2. 📝 Get all reports          ║");
                        Console.WriteLine("║ 3. 🎯 Get target stats         ║");
                        Console.WriteLine("║ 4. ❌ Exit                     ║");
                        Console.WriteLine("╚════════════════════════════════╝");

                        // החזרת הצבע המקורי
                        Console.ResetColor();

                        Console.Write("\nEnter your choice: ");
                        string adminChoice = Console.ReadLine();
                        //בדיקה אם המשתמש בחר באופציה 1
                        switch (adminChoice)
                        {
                            case "1":
                                //שליפת כל המשתמשים
                                //_StartRunCode.GetAllUsers();
                                Console.WriteLine("Not active at the moment");
                                break;
                            case "2":
                                //שליפת כל הדיווחים
                                //_StartRunCode.GetAllReports();
                                Console.WriteLine("Not active at the moment");
                                break;
                            case "3":
                                //שליפת הסטטיסטיקות של היעד
                                _StartRunCode.GetTargetStats();
                                break;
                            case "4":
                                Console.WriteLine("Goodbye!");
                                Console.WriteLine("Thank you for choosing our service!  😉 ");
                                boli = false;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
                                break;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Invalid admin credentials.");
                    }
                }
                //כרגע לא יצרתי את המערכת של האדמין כי אין לי צורך בה
                else if (typeConect == "4")
                {
                    Console.WriteLine("Goodbye!");
                    Console.WriteLine("Thank you for choosing our service!  😉 ");
                    boli = false;
                }
                //בדיקה אם המשתמש הכניס ערך לא תקין
                else
                {
                    Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                }

            }
        }
    }
}


