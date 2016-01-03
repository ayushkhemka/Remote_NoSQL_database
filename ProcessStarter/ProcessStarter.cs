/////////////////////////////////////////////////////////////////////////
// ProcessStarter.cs - Start specified number of read and write clients   //
// ver 1.1                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This package is the entry point for this project. It accepts on the command line 
 * a number of write and read clients to start. It then starts the specified number of
 * clients, along with the server and WPF Client. The number of write and read clients
 * can be modified in run.bat, but should not exceed 4 due to port conflicts.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.1 : 23 Nov 2015
 * - added finalized code to start all the processes
 * ver 1.0 : 29 Oct 2015
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Project4
{
  class ProcessStarter
  {
		//------------< start write and read clients, command line arguments denote local and remote addresses >
		//------------< and /M is for write client to log messages or not >-------------------------------------
    public bool startClient(int offset, string process)
    {
      process = Path.GetFullPath(process);
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = process,
				Arguments = "/R http://localhost:8080/CommService /L http://localhost:808" + offset.ToString() + "/CommService /M",
				// set UseShellExecute to true to see child console, false hides console
				UseShellExecute = true
			};
      try
      {
        Process p = Process.Start(psi);
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
    }
		
		//--------< start server and wpf client, which do not take arguments >-----------
		public bool startProcess(string process)
		{
			process = Path.GetFullPath(process);
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = process,
				// set UseShellExecute to true to see child console, false hides console
				UseShellExecute = true
			};
			try
			{
				Process p = Process.Start(psi);
				return true;
			}
			catch (Exception ex)
			{
				Console.Write("\n  {0}", ex.Message);
				return false;
			}
		}
#if (TEST_PROCESSSTARTER)
		static void Main(string[] args)
    {
			Console.Title="Starting clients and server";
			ProcessStarter ps = new ProcessStarter();
			//----------< dynamically assign port numbers and start clients and server >----------
			for (int i = 0; i < Int32.Parse(args[0]); i++) ps.startClient((i + 1), "Client/bin/debug/WriteClient.exe");
			for (int i = 0; i < Int32.Parse(args[1]); i++) ps.startClient((i + 6), "Client2/bin/debug/ReadClient.exe");
			ps.startProcess("Server/bin/debug/Server.exe");
			ps.startProcess("WpfClient/bin/Debug/WpfApplication1.exe");
			Utilities.title("\n  Please view the ReadMe file for a detailed explanation of this project", '=');
			Console.ReadKey();
    }
#endif
  }
}