using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon_09_06_25
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DAL StartRunCode = new DAL("malshinon");
            StartRunCode.Access_TO_DB();
            Console.WriteLine(StartRunCode.GetPersonByName("David", "Levi"));
        }
    }
}
