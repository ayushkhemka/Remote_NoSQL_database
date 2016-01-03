/////////////////////////////////////////////////////////////////////////
// ReadRequestParser.cs - Parse XML file for read requests              //
// ver 1.0                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This package takes the XML file specified by the read client and parses
 * it to produce string messages that can be sent to the server. The messages
 * follow the structure defined in the OCD, which makes it easier for the parser
 * at the other end to generate function calls.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.0 : 23 Nov 2015
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Project4
{
	using Util = Project4.Utilities;
	public class ReadRequestParser
	{
		List<string> queries = new List<string>();
		string message;
		string requestType;
		int numMessages;
		string key;
		string type;

		//-----------< break down whole XML file into List<string> messages and send to read client >-----------
		public List<string> parse(string pathname)
		{
			queries.Clear();
			XDocument xdoc = XDocument.Load(pathname);
			requestType = xdoc.Root.Name.ToString() + ","; //type of request, used by server to decode where request came from
			var queryTypes = xdoc.Root.Elements("queryType");
			foreach (var queryType in queryTypes)
			{
				type = queryType.FirstAttribute.Value.ToString() + ","; //type of request
				XElement n = (XElement)queryType.FirstNode;
				numMessages = Int32.Parse(n.Value); //number of messages for that request
				XElement query = queryType.Element("query");
				for (int i = 0; i < numMessages; i++)
				{
					if (queryType.FirstAttribute.Value.ToString() == "interval" || queryType.FirstAttribute.Value.ToString() == "pattern" || queryType.FirstAttribute.Value.ToString() == "string") key = query.Value.ToString(); //no keygen required
					else key = query.Value.ToString() + i.ToString(); //type of keygen for these query types
					message = requestType + type + key; //compile final message for that query
					queries.Add(message.ToString());
				}
			}
			return queries;
		}
#if (TEST_READREQUESTPARSER)
		static void Main(string[] args)
		{
			List<string> messages = new List<string>();
			ReadRequestParser re = new ReadRequestParser();
			try { messages = re.parse("../../../readRequest.xml"); }
			catch { messages= new List<string>() { "File not found" }; }
			foreach (string message in messages)
				Console.Write(message + "\n");
		}
#endif
	}
}