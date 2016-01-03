/////////////////////////////////////////////////////////////////////////
// WriteRequestParser.cs - Parse XML files for write client             //
// ver 1.0                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This package takes the XML file specified by the write client
 * and breaks it down into messages to be sent to the server based
 * on the message structure defined in the OCD. It generates one message
 * for each query, and sends it multiple times based on the number of messages.
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
	public class WriteRequestParser
	{
		List<string> queries = new List<string>();
		string message;
		string requestType;
		int numMessages;
		string key;
		string type;
		string name=" ";
		string descr=" ";
		DateTime timeStamp = DateTime.Now;
		string childCount=" ";
		List<string> childrenNode = new List<string>() { };
		string children=" ";
		string payloadCount;
		List<string> payloadNode = new List<string>() { };
		string payload=" ";
		
		//-----------< parse XML fifle >-------------
		public List<string> parse(string pathname)
		{
			queries.Clear();
			XDocument xdoc = XDocument.Load(pathname);
			requestType = xdoc.Root.Name.ToString() + ","; //type of request, used by server to decide where it came from
			var queryTypes = xdoc.Root.Elements("queryType");
			foreach (var queryType in queryTypes)
			{
				type = queryType.FirstAttribute.Value.ToString() + ","; //type of the write request
				XElement n = (XElement)queryType.FirstNode;
				numMessages = Int32.Parse(n.Value); //number of messages for that type
				XElement query = queryType.Element("query");
				for (int i = 0; i < numMessages; i++)
				{
					if (queryType.FirstAttribute.Value.ToString() == "add")
						key = "key" + i.ToString() + ","; //keygen to add keys
					else
						key = query.Element("key").Value.ToString() + i.ToString() + ",";
					if (queryType.FirstAttribute.Value.ToString() == "restore")
						name = query.Element("name").Value.ToString() + ","; //no keygen needed here
					else
						name = query.Element("name").Value.ToString() + i.ToString() + ","; //keygen for name
					descr = query.Element("descr").Value.ToString() + i.ToString() + ","; //keygen of description
					var children_ = query.Element("children").Elements();
					childrenNode.Clear();
					foreach (var child in children_)
						childrenNode.Add("key" + (i+1).ToString() + child.Value.ToString());
					childCount = childrenNode.Count.ToString() + ","; //keygen for children
					children = Util.ToString(childrenNode) + ",";
					payloadNode.Clear();
					var payload_ = query.Element("payload").Elements();
					foreach (var item in payload_)
						payloadNode.Add("item" + (i+1).ToString() + item.Value.ToString()); //keygen for payload
					payloadCount = payloadNode.Count.ToString() + ",";
					payload = Util.ToString(payloadNode);
					message = requestType + type + key + name + descr + childCount + children + payloadCount + payload; //generate final message
					queries.Add(message.ToString());
				}
			}
			return queries;
		}
#if (TEST_WRITEREQUESTPARSER)
		static void Main(string[] args)
		{
			List<string> messages = new List<string>();
			WriteRequestParser re = new WriteRequestParser();
			messages = re.parse("../../../writeRequest.xml");
			foreach(string message in messages)
			Console.Write(message + "\n");
		}
#endif
	}
}
