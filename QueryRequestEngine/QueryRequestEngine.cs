/////////////////////////////////////////////////////////////////////////
// QueryRequestEngine.cs - Parse read requests and make function calls  //
// ver 1.0                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This package is used to parse the read requests coming from read clients.
 * It figures out the type of the query, stores the criteria in some variable,
 * and then calls QueryEngine from NoSQLdb to make that query. It then converts
 * the response into a suitable string and sends it back to the server.
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
using Project2;

namespace Project4
{
	public class QueryRequestEngine
	{
		QueryEngine<string, DBElement<string, List<string>>, List<string>> qe = new QueryEngine<string, DBElement<string, List<string>>, List<string>>();
		private string[] content;
		private string message;
		private DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
		private string query;
		private string type;
		private List<string> replyList = new List<string>() { };

		//-----------< Fish out the type of query and criteria from the request string >-------------
		public bool parseRequest(DBEngine<string, DBElement<string, List<string>>> db, string msg, out string reply)
		{
			elem = new DBElement<string, List<string>>();
			int IndexOfComma = msg.IndexOf(",");
			message = msg.Substring(IndexOfComma + 1);
			content = message.Split(',');
			type = content[0];
			query = content[1];

			//-----------< make function call and convert response into suitable string >----------
			if (call(db, type))
			{
				if (type == "value") reply = "\n  Value of key " + query + ": " + elem.showElement<string, List<string>, string>();
				else if (type == "children") reply = "\n  Children of key " + query + ": " + Utilities.ToString(replyList);
				else if (type == "pattern") reply = "\n  List of keys starting with \"" + query + "\": " + Utilities.ToString(replyList);
				else if (type == "string") reply = "\n  List of keys containing \"" + query + "\" in their metadata: " + Utilities.ToString(replyList);
				else reply = "\n  List of keys entered from " + query + " to present: " + Utilities.ToString(replyList);
				return true;
			}
			if (type == "value") reply = "\n  Value of key " + query + " not found";
			else if (type == "children") reply = "\n  Children of key " + query + " not found";
			else if (type == "pattern") reply = "\n  List of keys starting with \"" + query + "\" not found";
			else if (type == "string") reply = "\n  List of keys containing \"" + query + "\" in their metadata not found";
			else reply = "\n  List of keys entered from " + query + " to present not found";
			return false;
		}

		//---------< Call QueryEngine based on the query type >-----------
		public bool call(DBEngine<string, DBElement<string, List<string>>> db, string request)
		{
			if (db.Keys() == null) return false;
			switch (request)
			{
				case "value":
					if(qe.getValue(query, db, out elem)) return true;
					break;
				case "children":
					if (qe.getChildren(query, db, out replyList)) return true;
					break;
				case "pattern":
					if (qe.searchPattern(query, db, out replyList)) return true;
					break;
				case "string":
					if (qe.searchString(query, db, out replyList)) return true;
					break;
				case "interval":
					if (qe.searchInterval(query, "", db, out replyList)) return true;
					break;
				default: return false;
			}
			return false;
		}
#if (TEST_QUERYREQUESTENGINE)
		static void Main(string[] args)
		{
			QueryRequestEngine qre = new QueryRequestEngine();
			string reply;
			DBEngine<string, DBElement<string, List<string>>> db = new DBEngine<string, DBElement<string, List<string>>>();
			DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
			elem.name = "name0";
			elem.descr = "descr0";
			elem.children = new List<string>() { "key0", "key1", "key2" };
			elem.timeStamp = DateTime.Now;
			elem.payload = new List<string>() { "item1", "item2", "item3" };
			db.insert("key0", elem);

			elem = new DBElement<string, List<string>>();
			elem.name = "name1";
			elem.descr = "descr1";
			elem.children = new List<string>() { "key3", "key4", "key5" };
			elem.timeStamp = DateTime.Now;
			elem.payload = new List<string>() { "item4", "item5", "item6" };
			db.insert("key1", elem);

			qre.parseRequest(db, "write,value,key0", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,value,key1", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,children,key0", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,children,key1", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,pattern,k", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,pattern,a", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,string,descr0", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,string,descr1", out reply);
			Console.WriteLine(reply);
			qre.parseRequest(db, "write,interval,10/7/1999 12:00:00 AM", out reply);
			Console.WriteLine(reply);
		}
#endif
	}
}