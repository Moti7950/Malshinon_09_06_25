using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Google.Protobuf.Compiler;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;

namespace Malshinon_09_06_25
{
    internal class DAL
    {
        private MySqlConnection _conn;
        public string DBName { get; private set; }
        public string server { get; private set; }
        public string user { get; private set; }
        public string password { get; private set; }
        
        public DAL(string DBName, string server = "localhost", string user = "root", string password = "")
        {
            this.server = server;
            this.user = user;
            this.DBName = DBName;
            this.password = password;
        }
        public void Access_TO_DB()
        {
            string connStr = $"server={server};user={user};password={password};database={DBName}";
            this._conn = new MySqlConnection(connStr);
            Console.WriteLine("------------------------------");
            Console.WriteLine("|                            |");
            Console.WriteLine("| Connection object created! |");
            Console.WriteLine("|          /!\\               |");
            Console.WriteLine("------------------------------");
        }
        public bool GetPersonByName(string name, string lastName)
        {
            string query = "SELECT * FROM people WHERE first_name = @first_name AND last_name = @last_name";
            try
            {
                _conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@first_name", name);
                cmd.Parameters.AddWithValue("@last_name", lastName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Catch" + e.Message);
                return false;
            }
            finally
            {
                _conn.Close();
            }
        }
        //same fanc like Get by name!!
        public bool GetPersonBySecretCode(string secret_code)
        {
            string query ="SELECT * FROM people WHERE secret_code =@secret_code";
            try
            {
                _conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@secret_code", secret_code);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Catch" + e.Message);
                return false;
            }
            finally
            {
                _conn.Close();
            }
        }
        public void GetReporterStats()
        {
            List<PeopleDB> GetStatus = new List<PeopleDB>();
            string query = "SELECT first_name, last_name, num_reports FROM people";
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, _conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PeopleDB person = new PeopleDB(
                            reader.GetString("first_name"),
                            reader.GetString("last_name"),
                            null,
                            0,
                            reader.GetInt32("num_reports")
                            );
                        GetStatus.Add(person);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading agents: /!\\ " + ex.ToString());
            }
            finally
            {
                _conn.Close();
            }
            
            foreach (var person in GetStatus)
            {
                Console.WriteLine("╔════════════════════════════════════════════════╗");
                Console.WriteLine("║                 REPORTER INFO                  ║");
                Console.WriteLine("╠════════════════════════════════════════════════╣");
                Console.WriteLine($"║ Name    : {(person.first_name + " " + person.last_name),-36} ║");
                Console.WriteLine($"║ Reports : {person.num_reports,-36} ║");
                Console.WriteLine("╚════════════════════════════════════════════════╝");
            }

        }

        public void GetTargetStats()
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                string query = "SELECT target_id, timestamp FROM intelreports ORDER BY timestamp ASC";
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                Dictionary<int, List<DateTime>> targetTimes = new Dictionary<int, List<DateTime>>();

                while (reader.Read())
                {
                    int targetId = reader.GetInt32("target_id");
                    DateTime time = reader.GetDateTime("timestamp");

                    if (!targetTimes.ContainsKey(targetId))
                        targetTimes[targetId] = new List<DateTime>();

                    targetTimes[targetId].Add(time);
                }

                reader.Close();

                bool anySuspicious = false;

                foreach (var kvp in targetTimes)
                {
                    int targetId = kvp.Key;
                    List<DateTime> times = kvp.Value;
                    times.Sort();

                    bool targetPrinted = false;

                    for (int i = 0; i < times.Count; i++)
                    {
                        int countInWindow = 1;
                        DateTime windowStart = times[i];

                        for (int j = i + 1; j < times.Count; j++)
                        {
                            if ((times[j] - windowStart).TotalMinutes <= 15)
                                countInWindow++;
                            else
                                break;
                        }

                        if (countInWindow >= 3)
                        {
                            if (!targetPrinted) // נדפיס רק פעם אחת לכל יעד
                            {
                                Console.WriteLine("════════════════════════════════════════");
                                Console.WriteLine("⚠️  ALERT: High Activity Detected!");
                                Console.WriteLine($"🎯 Target ID      : {targetId}");
                                Console.WriteLine($"🕒 Time Window    : {windowStart:dd/MM/yyyy HH:mm} ➜ {windowStart.AddMinutes(15):HH:mm}");
                                Console.WriteLine($"📈 Reports Count  : {countInWindow}");
                                Console.WriteLine("════════════════════════════════════════\n");
                                anySuspicious = true;
                                targetPrinted = true;
                            }
                        }
                    }
                }

                if (!anySuspicious)
                {
                    Console.WriteLine("✅ No suspicious target activity detected within 15-minute windows.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("שגיאה בניתוח מטרות: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
        }
        public void InsertNewPerson(PeopleDB addPerson)
        {
            string query = "INSERT INTO people (first_name, last_name, secret_code, num_mentions, num_reports, type) " +
               "VALUES (@first_name, @last_name, @secret_code, @num_mentions, @num_reports, @type)";
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, _conn);

                cmd.Parameters.AddWithValue("@first_name", addPerson.first_name);
                cmd.Parameters.AddWithValue("@last_name", addPerson.last_name);
                cmd.Parameters.AddWithValue("@secret_code", addPerson.secret_code);
                cmd.Parameters.AddWithValue("@num_mentions", 0);
                cmd.Parameters.AddWithValue("@num_reports", 0);
                cmd.Parameters.AddWithValue("@type", "reporter");

                int rowsAffected = cmd.ExecuteNonQuery();

                Console.WriteLine(rowsAffected > 0 ? "Success" : "Not added");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erorr " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
        }

        public void InsertIntelReport(IntelreportsDB addIntel)//!! בדיקה על קבלת האיש המדווח
        {//int reporter_id, int target_id, string text, string timestamp
            string query = "INSERT INTO intelreports (reporter_id, target_id, text, timestamp) " +
                   "VALUES (@reporter_id, @target_id, @text, @timestamp)";
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, _conn);

                cmd.Parameters.AddWithValue("@reporter_id", addIntel.reporter_id);
                cmd.Parameters.AddWithValue("@target_id", addIntel.target_id);
                cmd.Parameters.AddWithValue("@text", addIntel.text);
                cmd.Parameters.AddWithValue("@timestamp", addIntel.timestamp);

                int rowsAffected = cmd.ExecuteNonQuery();

                Console.WriteLine(rowsAffected > 0 ? "Success" : "Not added");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erorr " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
        }
        public void UpdateReportCount(int targetId)
        {
            {
                string query = @"UPDATE people
                                SET num_reports = (SELECT COUNT(*) FROM intelreports WHERE target_id = people.id)
                                WHERE id = @targetId";
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        _conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, _conn);
                    cmd.Parameters.AddWithValue("@targetId", targetId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("╔══════════════════════════════════════╗");
                        Console.WriteLine("║      Report count updated!!          ║");
                        Console.WriteLine("╚══════════════════════════════════════╝");
                    }
                    else
                    {
                        Console.WriteLine("╔══════════════════════════════════════╗");
                        Console.WriteLine("║      No reports found to update /!\\  ║");
                        Console.WriteLine("╚══════════════════════════════════════╝");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error: " + ex.Message);
                }
                finally
                {
                    _conn.Close();
                }
            }
        }
        public void UpdateMentionCount(int reporterId)
        {
            string query = @"UPDATE people p
                            JOIN (SELECT reporter_id, COUNT(*) AS mention_count FROM intelreports WHERE reporter_id = @reporterId GROUP BY reporter_id) r
                            ON p.id = r.reporter_id SET p.num_mentions = r.mention_count WHERE p.id = @reporterId;
                            ";
            try
            {
                if (_conn.State != ConnectionState.Open)
                    _conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@reporterId", reporterId);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("╔══════════════════════════════════════╗");
                    Console.WriteLine("║      Mention counts updated!!        ║");
                    Console.WriteLine("╚══════════════════════════════════════╝");
                }
                else
                {
                    Console.WriteLine("╔══════════════════════════════════════╗");
                    Console.WriteLine("║     No mentions found to update /!\\  ║");
                    Console.WriteLine("╚══════════════════════════════════════╝");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
        }
        //public void CreateAlert()
        //{ }
        public void GetAlerts()
        { }

    }
}
