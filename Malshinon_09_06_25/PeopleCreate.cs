using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon_09_06_25
{
    internal class PeopleCreate
    {
        public int id { get; private set; }
        public int reporter_id { get; private set; }
        public int target_id { get; private set; }
        public string text { get; private set; }
        public string timestamp { get; private set; }
        public PeopleCreate(int id, int reporter_id, int target_id, string text, string timestamp)
        {
            this.id = id;
            this.reporter_id = reporter_id;
            this.target_id = target_id;
            this.text = text;
            this.timestamp = timestamp;
        }
    }
}
