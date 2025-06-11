using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon_09_06_25
{
    internal class PeopleDB
    {
        public int id { get; private set; }
        public string first_name { get; private set; }
        public string last_name { get; private set; }
        public string secret_code { get; private set; }
        public List<string> type { get; private set; }
        public int num_mentions { get; private set; }
        public int num_reports { get; private set; }


        public PeopleDB(string first_name, string last_name, string secret_code = null, int num_mentions = 0, int num_reports =0)
        {
            this.first_name = first_name;
            this.last_name = last_name;
            this.secret_code  = secret_code = string.IsNullOrEmpty(secret_code) ? GenerateSecretCode() : secret_code; ;
            this.num_mentions = num_mentions;
            this.num_reports = num_reports;
        }
        public string GenerateSecretCode(int length = 4)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
