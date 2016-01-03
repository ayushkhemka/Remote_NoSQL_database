/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// ver 2.4                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.4 : 23 Nov 2015
 * - added the functionalities to take requests from clients and
 *   pass it on to the correct parser based on where it came from
 * - record the times to process each type of request, required
 *   for stress testing the server
 * - send performance results to WPF client when requested
 * - final release
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2;

namespace Project4
{
	using Util = Utilities;

	class Server
	{
		string address { get; set; } = "localhost";
		string port { get; set; } = "8080";

		//----< quick way to grab ports and addresses from commandline >-----

		public void ProcessCommandLine(string[] args)
		{
			if (args.Length > 0)
			{
				port = args[0];
			}
			if (args.Length > 1)
			{
				address = args[1];
			}
		}

		static void Main(string[] args)
		{
			RequestEngine re = new RequestEngine();
			QueryRequestEngine qre = new QueryRequestEngine();
			DBEngine<string, DBElement<string, List<string>>> db = new DBEngine<string, DBElement<string, List<string>>>();
			Util.verbose = false;
			Server srvr = new Server();
			srvr.ProcessCommandLine(args);

			Console.Title = "Server";
			Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
			Console.Write("\n ====================================================\n");

			Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
			//Sender sndr = new Sender();
			Receiver rcvr = new Receiver(srvr.port, srvr.address);

			// - serviceAction defines what the server does with received messages
			// - This serviceAction just announces incoming messages and echos them
			//   back to the sender.  
			// - Note that demonstrates sender routing works if you run more than
			//   one client.
			HiResTimer hrt = new HiResTimer();
			PerfTest pt = new PerfTest();
			Action serviceAction = () =>
			{
				Message msg = null;
				while (true)
				{
					msg = rcvr.getMessage();   // note use of non-service method to deQ messages
																		 //insert();
					int length = msg.content.IndexOf(",") + 1;
					string from = "";
					if (msg.content != "connection start message")
						from = msg.content.Substring(0, length); //figure out where the request came from
          string[] msgList=msg.content.Split(',');
					Console.Write("\n  Received message:");
					Console.Write("\n  Request type is: {0}", from); //denotes where the request came from
					Console.Write("\n  sender is {0}", msg.fromUrl);
					Console.Write("\n  content is {0}\n", msg.content);
					string reply="";
					//------------< do processing based on the request type >------------
					if (from == "write,")
					{
						hrt.Start();
						re.parseRequest(msg.content, out reply);
						hrt.Stop();
						pt.add(msgList[1], hrt.ElapsedMicroseconds);
						msg.content=reply;
					}
					else if (from == "read,")
					{
						hrt.Start();
						db = re.getDB();
						qre.parseRequest(db, msg.content, out reply);
						hrt.Stop();
						pt.add(msgList[1], hrt.ElapsedMicroseconds);
						msg.content = reply;
					}
					else if (from == "perf,")
					{
						reply = pt.printTimesWpf(msgList[1]);
						msg.content = reply;
					}
					if (msg.content == "connection start message")
					{
						continue; // don't send back start message
					}
					if (msg.content == "done")
					{
						Console.Write("\n  client has finished\n");
						Console.WriteLine(pt.printTimes());
						continue;
					}
					if (msg.content == "closeServer")
					{
						Console.Write("received closeServer");
						break;
					}
					// swap urls for outgoing message
					Util.swapUrls(ref msg);

#if (TEST_WPFCLIENT)
					/////////////////////////////////////////////////
					// The statements below support testing the
					// WpfClient as it receives a stream of messages
					// - for each message received the Server
					//   sends back 1000 messages
					// Use the code to test the server

					/*int count = 0;
					for (int i = 0; i < 1000; ++i)
					{
						Message testMsg = new Message();
						testMsg.toUrl = msg.toUrl;
						testMsg.fromUrl = msg.fromUrl;
						testMsg.content = String.Format("test message #{0}", ++count);
						Console.Write("\n  sending testMsg: {0}", testMsg.content);
						sndr.sendMessage(testMsg);
					}*/
					Message testMsg = new Message();
					testMsg.toUrl = msg.toUrl;
					testMsg.fromUrl = msg.fromUrl;
					testMsg.content = String.Format("test message");
					Console.Write("\n  sending testMsg: {0}", testMsg.content);
					sndr.sendMessage(testMsg);
#else
					/////////////////////////////////////////////////
					// Use the statement below for normal operation
					sndr.sendMessage(msg);
					//re.db.show<string, DBElement<string, List<string>>, List<string>, string>();
#endif
				}
			};

			if (rcvr.StartService())
			{
				rcvr.doService(serviceAction); // This serviceAction is asynchronous,
			}                                // so the call doesn't block.
			Util.waitForUser();
		}
	}
}