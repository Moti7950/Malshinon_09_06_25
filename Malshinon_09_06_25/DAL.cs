using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

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
            Console.WriteLine("Connection object created!");
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
            string query = $"SELECT * FROM people WHERE secret_code={"secret_code"}";
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
                return false;
            }
            finally
            {
                _conn.Close();
            }
        }
        public string GetReporterStats()
        {
            List<DAL> GetStatus = new List<DAL>();
            string query = $"SELECT first_name, last_name, num_reports FROM people ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, _conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DAL a = new DAL(
                        reader.GetString("first_name"),
                        reader.GetString("last_name"),
                        reader.GetString("num_reports")
                    );
                    GetStatus.Add(a);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading agents: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
            
            StringBuilder sb = new StringBuilder();
            foreach (DAL person in GetStatus)
            {
                sb.AppendLine(person.ToString());
            }

            return sb.ToString();

        }
            

        public void GetTargetStats()
        { }
        public void InsertNewPerson()
        {

          
        }
        public void InsertIntelReport()
        { }
        public void UpdateReportCount()
        { }
        public void UpdateMentionCount()
        { }
        public void CreateAlert()
        { }
        public void GetAlerts()
        { }

    }
}
