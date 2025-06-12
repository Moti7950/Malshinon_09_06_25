using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon_09_06_25
{
    internal class LogDB
    {
        public string user_name { get; private set; }
        public string action { get; private set; }

        public LogDB(string user_name, string action)
        {
            this.user_name = user_name;
            this.action = action;
        }
    }
}
