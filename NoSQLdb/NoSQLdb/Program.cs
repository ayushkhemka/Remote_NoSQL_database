using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
    class TestExec
    {
        private DBEngine<int, string> db = new DBEngine<int, string>();
        void TestR2()
        {
            "Demonstrating Requirements #2".title('-');
            WriteLine();
        }
        static void main (string[] args)
        {
            TestExec exec = new TestExec();
            "Demonstrating Project #2 Requirements".title('=');
            exec.TestR2();
            Write("\n\n");
        }
    }
}
