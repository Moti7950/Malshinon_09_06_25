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
        private bool IsCapitalized(string word)
        {
            return !string.IsNullOrEmpty(word) &&
                   char.IsUpper(word[0]) &&
                   word.Skip(1).All(c => char.IsLower(c) || !char.IsLetter(c));
        }

        public Menu()
        {
            //Console.WriteLine(_StartRunCode.GetPersonByName("David", "Levi"));
            //Console.WriteLine(_StartRunCode.GetPersonBySecretCode("EB111"));
            //_StartRunCode.GetReporterStats();
            //Console.WriteLine(_StartRunCode.GetPersonBySecretCode("DM999"));
            //_StartRunCode.InsertNewPerson(new PeopleDB("Shira", "Goldstein", "E4321", 0, 0));
            //Console.WriteLine(_StartRunCode.GetPersonBySecretCode("E4321"));
            //שים לב בחלק השני מכניסים את שם האיש שמדווח!!
            //_StartRunCode.InsertIntelReport(new IntelreportsDB(3, 6, "Suspicious behavior at the northern gate", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
            //_StartRunCode.UpdateReportCount(7);
            //_StartRunCode.UpdateMentionCount(3);
        }
        public List<string> SplitBySpace(string fullName)
        {
            string[] parts = fullName.Split(' ');
            string firstName = parts[0];
            string lastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";

            return new List<string> { firstName, lastName };
        }
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
                Console.WriteLine("❌ Could not extract a full name from the report.");
                return;
            }

            // 3. קבלת מזהים מה־DB
            int reporterId = _StartRunCode.GetPersonIdBySecretCode(inputSecretCode);
            int targetId = _StartRunCode.GetPersonIdByName(firstName, lastName);

            if (reporterId == -1)
            {
                Console.WriteLine("❌ Reporter not found.");
                return;
            }

            if (targetId == -1)
            {
                Console.WriteLine($"❌ Target '{firstName} {lastName}' not found.");
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
        public void startMenu()
        {
            // יצירת חיבור לנתונים כרגע נוצר חיבור אוטמטי יש צורך במערכת חכמה של סיסמאות ולאיפה להתחבר לשרת
            _StartRunCode.Access_TO_DB();
            _StartRunCode.GetTargetStats();
            bool boli = true;
            while (boli)
            {
                Console.WriteLine(" ╔══════════════════════════════╗");
                Console.WriteLine(" ║  Please select your choce    ║");
                Console.WriteLine(" ║          1. name:            ║");
                Console.WriteLine(" ║          2. secret code:     ║");
                Console.WriteLine(" ║          3. Exit:            ║");
                Console.WriteLine(" ╚══════════════════════════════╝");

                string typeConect = Console.ReadLine();

                if (typeConect == "3")
                {
                    Console.WriteLine("Goodbye!");
                    boli = false;
                }
                else if (typeConect == "1")
                {
                    Console.WriteLine("Please enter your first and last name: ");
                    string inputUserName = Console.ReadLine();

                    if (!doesTheUserExist(typeConect, inputUserName))
                    {
                        //לא צריך לבצע הדפסה כפולה 
                        //Console.WriteLine("Please enter your first and last name to create a new user. ");
                        //string inputUserNameNotExist = Console.ReadLine();
                        //שימוש במחלקה לחילוק הרשימה לשם פרטי ושם משפחה
                        List<string> nameParts = SplitBySpace(inputUserName);
                        string firstName = nameParts[0];
                        string lastName = nameParts[1];
                        //יצירת המשתמש 
                        PeopleDB craiteNweUser = new PeopleDB(firstName, lastName);
                        //הכנסת המשתמש החדש ל DB 
                        _StartRunCode.InsertNewPerson(craiteNweUser);
                    }
                    else if (doesTheUserExist(typeConect, inputUserName))
                    {
                        List<string> nameParts = SplitBySpace(inputUserName);
                        string firstName = nameParts[0];
                        string lastName = nameParts[1];

                        Console.WriteLine($"Welcome {firstName} {lastName}");

                        // שליפת הקוד הסודי של המשתמש מתוך ה-DB
                        string secretCode = _StartRunCode.GetSecretCodeByName(firstName, lastName);
                        //_StartRunCode.UpdateReportCount();
                        //_StartRunCode.UpdateMentionCount();

                        if (string.IsNullOrEmpty(secretCode))
                        {
                            Console.WriteLine("❌ Could not find your secret code in the system.");
                        }
                        else
                        {
                            HandleAndSendReport(secretCode);
                        }
                    }
                }
                else if (typeConect == "2")
                {
                    Console.WriteLine("Please enter your secret code:");
                    string secretCode = Console.ReadLine();

                    if (!doesTheUserExist(typeConect, secretCode))
                    {
                        List<string> nameParts = SplitBySpace(secretCode);
                        if (nameParts.Count < 2)
                        {
                            Console.WriteLine("❌ Please enter both first and last name.");
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
                            Console.WriteLine("❌ Could not find a person with this secret code.");
                        }
                        else
                        {
                            Console.WriteLine($"✅ Welcome back, {person.Value.FirstName} {person.Value.LastName}!");
                            HandleAndSendReport(secretCode);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                }

            }
        }
    }
}


