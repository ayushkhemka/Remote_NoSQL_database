/////////////////////////////////////////////////////////////////////////
// MainWindows.xaml.cs - CommService GUI Client                        //
// ver 3.0                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# WPF Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, MakeMessage, Utilities
 * - Added using Project4Starter
 *
 * Note:
 * - This client receives and sends messages.
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
 * ver 3.0 : 23 Nov 2015
 * - changed Xaml for a more compact and efficient design
 * - DB Operations tab sends few messages to NoSQLdb to demonstrate requirements
 * - Performance analysis results can be viewed type by type
 * - final release
 * ver 2.0 : 29 Oct 2015
 * - changed Xaml to achieve more fluid design
 *   by embedding controls in grid columns as well as rows
 * - added derived sender, overridding notification methods
 *   to put notifications in status textbox
 * - added use of MessageMaker in send_click
 * ver 1.0 : 25 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Project4;

namespace WpfApplication1
{
	public partial class MainWindow : Window
	{
		static bool firstConnect = true;
		static Receiver rcvr = null;
		static wpfSender sndr = null;
		string localAddress = "localhost";
		string localPort = "8090";
		string remoteAddress = "localhost";
		string remotePort = "8080";

		/////////////////////////////////////////////////////////////////////
		// nested class wpfSender used to override Sender message handling
		// - routes messages to status textbox
		public class wpfSender : Sender
		{
			TextBox lStat_ = null;  // reference to UIs local status textbox
			System.Windows.Threading.Dispatcher dispatcher_ = null;

			public wpfSender(TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
			{
				dispatcher_ = dispatcher;  // use to send results action to main UI thread
				lStat_ = lStat;
			}
			public override void sendMsgNotify(string msg)
			{
				Action act = () => { lStat_.Text = msg; };
				dispatcher_.Invoke(act);

			}
			public override void sendExceptionNotify(Exception ex, string msg = "")
			{
				Action act = () => { lStat_.Text = ex.Message; };
				dispatcher_.Invoke(act);
			}
			public override void sendAttemptNotify(int attemptNumber)
			{
				Action act = null;
				act = () => { lStat_.Text = String.Format("attempt to send #{0}", attemptNumber); };
				dispatcher_.Invoke(act);
			}
		}
		public MainWindow()
		{
			InitializeComponent();
			lAddr.Text = localAddress;
			lPort.Text = localPort;
			rAddr.Text = remoteAddress;
			rPort.Text = remotePort;
			Title = "Remote NoSQLdb";
			DBOp.IsEnabled = false;       // not allowed to request operations on DB
			perfButton.IsEnabled = false; // or see performance results before establishing connection
		}
		//----< trim off leading and trailing white space >------------------

		string trim(string msg)
		{
			StringBuilder sb = new StringBuilder(msg);
			for (int i = 0; i < sb.Length; ++i)
				if (sb[i] == '\n')
					sb.Remove(i, 1);
			return sb.ToString().Trim();
		}
		//----< indirectly used by child receive thread to post results >----

		public void postRcvMsg(string content)
		{
			TextBlock item = new TextBlock();
			item.Text = trim(content);
			item.FontSize = 16;
			rcvmsgs.Items.Insert(0, item);
		}
		//----< used by main thread >----------------------------------------

		public void postSndMsg(string content)
		{
			TextBlock item = new TextBlock();
			item.Text = trim(content);
			item.FontSize = 16;
			rcvmsgs.Items.Insert(0, item);
		}
		//----< get Receiver and Sender running >----------------------------

		void setupChannel()
		{
			rcvr = new Receiver(localPort, localAddress);
			Action serviceAction = () =>
			{
				try
				{
					Message rmsg = null;
					while (true)
					{
						rmsg = rcvr.getMessage();
						Action act = () => { postRcvMsg(rmsg.content); };
						Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
					}
				}
				catch (Exception ex)
				{
					Action act = () => { rStat.Text = ex.Message; };
					Dispatcher.Invoke(act);
				}
			};
			if (rcvr.StartService())
			{
				rcvr.doService(serviceAction);
			}

			sndr = new wpfSender(rStat, this.Dispatcher);
		}
		//----< set up channel after entering ports and addresses >----------

		private void start_Click(object sender, RoutedEventArgs e)
		{
			localPort = lPort.Text;
			localAddress = lAddr.Text;
			remoteAddress = rAddr.Text;
			remotePort = rPort.Text;

			if (firstConnect)
			{
				firstConnect = false;
				if (rcvr != null)
					rcvr.shutDown();
				setupChannel();
			}
			rStat.Text = "connect setup";
			connect.IsEnabled = false;
			lPort.IsEnabled = false;
			lAddr.IsEnabled = false;
			DBOp.IsEnabled = true;
			perfButton.IsEnabled = true;
		}

		//-----------< Demonstrate operations on NoSQLdb >------------
		private void DBOp_Click(object sender, RoutedEventArgs e)
		{
			DBOp.IsEnabled = false;
			TextBlock item = new TextBlock();
			item.Text = "Sending operations to NoSQLdb, see results in connect window";
			item.FontSize = 16;
			OpBox.Items.Insert(0, item);
			List<string> operation = new List<string>() { "add", "edit", "delete", "persist", "restore", "value", "children", "pattern", "string", "interval" };
			foreach (string op in operation) doOperations(op);
		}
		public void doOperations(string request)
		{
			TextBlock item = new TextBlock();
			item.FontSize = 16;
			Message msg = new Message();
			msg.fromUrl = Utilities.makeUrl(lAddr.Text, lPort.Text);
			msg.toUrl = Utilities.makeUrl(rAddr.Text, rPort.Text);
			//----------< design messages according to message structure and send to server >-----------
			try
			{
				switch (request)
				{
					case "add":
						msg.content = "write,add,keyWpf,nameWpf,descrWpf,2,childWpf1,childWpf2,2,itemWpf1,itemWpf2";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						msg = new Message();
						msg.fromUrl = Utilities.makeUrl(lAddr.Text, lPort.Text);
						msg.toUrl = Utilities.makeUrl(rAddr.Text, rPort.Text);
						msg.content = "write,add,keyWpf1,nameWpf,descrWpf,2,childWpf1,childWpf2,2,itemWpf1,itemWpf2";
						sndr.sendMessage(msg);
						break;
					case "edit":
						msg.content = "write,edit,keyWpf,NameWpf,DescrWpf,2,childWpf1,childWpf2,2,itemWpf1,itemWpf2";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "delete":
						msg.content = "write,delete,keyWpf1,0,0,0,,0,";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "persist":
						msg.content = "write,persist,0,0,0,0,,0,";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "restore":
						msg.content = "write,restore,0,xmlDocLOS.xml,0,0,,0,";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "value":
						msg.content = "read,value,keyWpf";
						item.Text = "Searching for value of specified key";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "children":
						msg.content = "read,children,keyWpf";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "pattern":
						msg.content = "read,pattern,k";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "string":
						msg.content = "read,string,Name";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					case "interval":
						msg.content = "read,interval,10/7/1999 12:00:00 AM";
						if (sndr.sendMessage(msg)) item.Text = "Sending " + msg.content;
						else item.Text = "Sending failed";
						OpBox.Items.Insert(1, item);
						break;
					default: break;
				}
			}
			catch
			{
				item = new TextBlock();
				item.Text = "Some problem with sending a message, see main window";
				item.FontSize = 16;
				OpBox.Items.Insert(0, item);
			}
		}

		private void OpBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		//-------------< request performance statistics for each request type >--------------
		private void perfButton_Click(object sender, RoutedEventArgs e)
		{
			List<string> operation = new List<string>() { "add", "edit", "delete", "persist", "restore", "value", "children", "pattern", "string", "interval" };
			foreach (string op in operation)
			{
				Message msg = new Message();
				msg.fromUrl = Utilities.makeUrl(lAddr.Text, lPort.Text);
				msg.toUrl = Utilities.makeUrl(rAddr.Text, rPort.Text);
				msg.content = "perf," + op;
				sndr.sendMessage(msg);
			}
		}
	}
}