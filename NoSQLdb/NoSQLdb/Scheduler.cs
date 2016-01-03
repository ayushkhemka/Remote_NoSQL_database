///////////////////////////////////////////////////////////////
// Scheduler.cs - Automatic persisting of database contents  //
// Ver 1.0                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Author:      Ayush Khemka, aykhemka@syr.edu, 538044584    //
// Source:      Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package runs a simple timer and after a specific time
 * time interval, persists contents to XML files. This process
 * will carry on until a key is pressed.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: UtilityExtensions.cs
 *
 * Build is based on TimerDemo.cs given by
 * Dr. James Fawcett as a starter package for
 * Project #2
 *
 * Build Process:  devenv Project2.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 07 Oct 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Project2
{
    public class Scheduler
    {

        public Timer schedular { get; set; } = new Timer();

        public Scheduler(int interval)
        {
            schedular.Interval = interval;
            schedular.AutoReset = true;

            // Note use of timer's Elapsed delegate, binding to subscriber lambda
            // This delegate is invoked when the internal timer thread has waited
            // for the specified Interval.

            schedular.Elapsed += (object source, ElapsedEventArgs e) =>
            {

                Console.Write("\n  Persisted to XML files at {0}", e.SignalTime);
            };
        }
#if (TEST_SCHEDULER)
		static void Main(string[] args)
        {
            "Demonstrate Timer - needed for scheduled persistance in Project #2".title('=');
            Console.Write("\n\n  press any key to exit\n");

            Scheduler td = new Scheduler(100);
            td.schedular.Enabled = true;
            Console.ReadKey();
            Console.Write("\n\n");
        }
#endif
    }
}