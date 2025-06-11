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

        public void HandleAndSendReport(string inputSecretCode)
        {
            // יצירת הדיווח הזמני
            Console.WriteLine("Enter reporter ID:");
            int reporterId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter target ID:");
            int targetId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter report text:");
            string text = Console.ReadLine();

            TempReport report = new TempReport(reporterId, targetId, text);

            // ניתוח - לדוגמה:
            //if (text.Length < 5)
            //{
            //    Console.WriteLine("Text too short, not saving.");
            //    return;
            //}

            // שליחה ל־DB
            _StartRunCode.InsertIntelReport(new IntelreportsDB(
                report.reporter_id,
                report.target_id,
                report.text,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            ));

            Console.WriteLine("✔ Report saved.");
        }

        public bool doesTheUserExist(string userExist)
        {
            //צריך לבדוק איך להכניס נכון את הפרטים בשביל להשתמש במערכות לדוגמא רווח יפריד את השם הפרטי והשם המשפחה
            switch (userExist)
            {
                case "1":
                    Console.WriteLine("Please enter your name: ");
                    string inputUserName = Console.ReadLine();
                    //שימוש במתודה של חילוק הרשימה לשם פרטי ושם משפחה
                    List<string> nameParts = SplitBySpace(inputUserName);
                    string firstName = nameParts[0];
                    string lastName = nameParts[1];

                    if (_StartRunCode.GetPersonByName(firstName, lastName))
                    {
                        //בחירת מטרה לפי שם או קוד שים לב יש פה חזרתיות בקוד!!!
                        Console.WriteLine($"Welcome {firstName} {lastName}");
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
            Console.WriteLine(" ╔══════════════════════════════╗");
            Console.WriteLine(" ║  Please select your choce    ║");
             Console.WriteLine(" ║           1. name:           ║");
             Console.WriteLine(" ║          2. secret code:     ║");
             Console.WriteLine(" ╚══════════════════════════════╝");
            string typeConect = Console.ReadLine();
            if (!doesTheUserExist(typeConect))
            {
                //לא צריך לבצע הדפסה כפולה 
                Console.WriteLine("Please enter your name ");
                string inputUserNameNotExist = Console.ReadLine();
                //שימוש במחלקה לחילוק הרשימה לשם פרטי ושם משפחה
                List<string> nameParts = SplitBySpace(inputUserNameNotExist);
                string firstName = nameParts[0];
                string lastName = nameParts[1];
                //יצירת המשתמש 
                PeopleDB craiteNweUser = new PeopleDB(firstName, lastName);
                _StartRunCode.InsertNewPerson(craiteNweUser);
            }
            else if (doesTheUserExist(typeConect))
            {
                Console.WriteLine("Please enter your secret code ");
                string inputSecretCode = Console.ReadLine();
                HandleAndSendReport(inputSecretCode);
            }

        }
    }
}
