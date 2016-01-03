/////////////////////////////////////////////////////////////////////////
// Client1.cs - CommService client sends and receives messages         //
// ver 2.1                                                             //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities
 *
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.1 : 29 Oct 2015
 * - fixed bug in processCommandLine(...)
 * - added rcvr.shutdown() and sndr.shutDown() 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Project2;

namespace Project4
{
  using Util = Utilities;


  ///////////////////////////////////////////////////////////////////////
  // WriteClient class sends and receives messages in this version
  // - commandline format: /L http://localhost:8085/CommService 
  //                       /R http://localhost:8080/CommService
  //   Either one or both may be ommitted

  class WriteClient
  {
    string localUrl { get; set; } = "http://localhost:8081/CommService";
    string remoteUrl { get; set; } = "http://localhost:8080/CommService";
		List<string> request = new List<string>();
		int verbose = 0;
    //----< retrieve urls from the CommandLine if there are any >--------

    public void processCommandLine(string[] args)
    {
      if (args.Length == 0)
        return;
      localUrl = Util.processCommandLineForLocal(args, localUrl);
      remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
			for (int i = 0; i < args.Length; i++)
				if (args[i] == "/M" || args[i]=="/m") verbose = 1;
    }
    static void Main(string[] args)
    {
		WriteRequestParser re = new WriteRequestParser();
		Console.Write("\n  starting CommService client");
      Console.Write("\n =============================\n");

      Console.Title = "Write Client";

      WriteClient clnt = new WriteClient();
      clnt.processCommandLine(args);

      string localPort = Util.urlPort(clnt.localUrl);
      string localAddr = Util.urlAddress(clnt.localUrl);
      Receiver rcvr = new Receiver(localPort, localAddr);
      if (rcvr.StartService())
      {
        rcvr.doService(rcvr.defaultServiceAction());
      }

      Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message

      Message msg = new Message();
      msg.fromUrl = clnt.localUrl;
      msg.toUrl = clnt.remoteUrl;

      Console.Write("\n  sender's url is {0}", msg.fromUrl);
      Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);

      if (!sndr.Connect(msg.toUrl))
      {
        Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
        sndr.shutdown();
        rcvr.shutDown();
        return;
      }
			clnt.request = re.parse("writeRequest.xml");
			HiResTimer hrt = new HiResTimer();
			ulong total = 0;
			foreach(string i in clnt.request)
			{
				msg = new Message();
				msg.fromUrl = clnt.localUrl;
				msg.toUrl = clnt.remoteUrl;
				msg.content = i;
				if(clnt.verbose==1) Console.Write("\n  Sending: {0}", msg.content);
				hrt.Start();
				if (!sndr.sendMessage(msg))
					break;
				hrt.Stop();
				total += hrt.ElapsedMicroseconds;
				Thread.Sleep(100);
			}
			//hrt.Stop();

			/*Use the following for testing the client

			int numMsgs = 5;
			int counter = 0;
			while (true)
			{
				msg.content = "Hello";
				Console.Write("\n  sending {0}", msg.content);
				if (!sndr.sendMessage(msg))
					return;
				Thread.Sleep(100);
				++counter;
				if (counter >= numMsgs)
					break;
			} 
			
			*/
			Console.Write("\n  Total time taken for sending all write messages {0} microseconds\n", total);
			msg = new Message();
			msg.fromUrl = clnt.localUrl;
			msg.toUrl = clnt.remoteUrl;
			msg.content = "done";
      sndr.sendMessage(msg);

      // Wait for user to press a key to quit.
      // Ensures that client has gotten all server replies.
      Util.waitForUser();

			// shut down this client's Receiver and Sender by sending close messages
			try { rcvr.shutDown();
				sndr.shutdown(); }
			catch { Console.Write("\n  Problem with closing sender and/or receiver\n"); }

      Console.Write("\n\n");
    }
  }
}
