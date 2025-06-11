using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon_09_06_25
{
    internal class TempReport
    {
        public int reporter_id { get; set; }
        public int target_id { get; set; }
        public string text { get; set; }

        public TempReport(int reporterId, int targetId, string text)
        {
            this.reporter_id = reporterId;
            this.target_id = targetId;
            this.text = text;
        }


    }

}
