/////////////////////////////////////////////////////////////////////////
// RequestEngine.cs - Parse write requests from write clients           //
// ver 1.0                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This package parses the write requests coming from the write clients.
 * It figures out the request type, stores appropriate variables, and makes
 * function calls to NoSQLdb. It then responds to the server with the result
 * of the operation.
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
using System.IO;
using Project2;

namespace Project4
{
	public class RequestEngine
	{
		private string[] content;
		private string message;
		private DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
		private DBElement<string, List<string>> editElement = new DBElement<string, List<string>>();
		private DBEngine<string, DBElement<string, List<string>>> db = new DBEngine<string, DBElement<string, List<string>>>();
		private string type;
		private string key;
		private string pathname;

		//------------< used to provide the current db to QueryRequestEngine >------------
		public DBEngine<string, DBElement<string, List<string>>> getDB()
		{
			return db;
		}

		//-------------< Fish out the type and appropriate variables from string message >-----------
		public bool parseRequest(string msg, out string reply)
		{
			elem = new DBElement<string, List<string>>();
			int IndexOfComma = msg.IndexOf(",");
			message = msg.Substring(IndexOfComma + 1); //take out the initial write from the message
			content = message.Split(',');
			int start; //to indicate start of a child/payload index
			int end; //to indicate end of a child/payload index
			type = content[0];
			key = content[1];
			elem.name = content[2];
			elem.descr = content[3];
			elem.timeStamp = DateTime.Now;
			elem.children.Clear();
			int count = Int32.Parse(content[4].ToString());
			start = 5; end = count + start;
			while (start < end && count != 0) //start from index 5 and go up to (5 + number of children)
			{
				elem.children.Add(content[start]);
				start++;
			}
			if (count == 0) start = ++start; //if no children, move ahead a place for payload, else stay at end
			else start = end;
			elem.payload = new List<string>() { };
			count = Int32.Parse(content[start].ToString());
			start++;
			end = count + start;
			while (start < end && count != 0) //start at (5 + number of children) or 7, and go up to end
			{
				elem.payload.Add(content[start]);
				start++;
			}
			if (call(type)) //make function call based on type and create appropriate response message
			{
				if (type == "persist") reply = type + " to " + Path.GetFullPath(pathname) + " succeeded\n";
				else if (type == "restore") reply = type + " from " + Path.GetFullPath(elem.name) + " succeeded\n";
				else reply = type + " " + key + " succeeded\n";
				return true;
			}
			if (type == "persist") reply = type + " to " + Path.GetFullPath(pathname) + " failed\n";
			else if (type == "restore") reply = type + " from " + Path.GetFullPath(elem.name) + " failed\n";
			else reply = type + " " + key + " failed\n";
			return false;
		}

		//-------------< call the appropriate package from NoSQLdb based on request type >-----------
		public bool call(string request)
		{
			switch (request)
			{
				case "add":
					if(db.insert(key, elem)) return true; //this will fail if multiple write clients write the same key to the db
					break;
				case "edit":
					if (db.remove(key, out editElement)) //if no element present, editing cannot happen
					{
						editElement.name = elem.name;
						editElement.descr = elem.descr;
						editElement.timeStamp = DateTime.Now;
						editElement.children.Clear();
						editElement.children = elem.children;
						editElement.payload = elem.payload;
						if (db.insert(key, editElement)) return true;
					}
					break;
				case "delete":
					if (db.remove(key, out elem)) return true;
					break;
				case "persist":
					PersistEngine pe = new PersistEngine();
					pe.XMLWriteLOS(db, out pathname); // this will always return true, since it will always write out
					return true;                      // something, even if it's blank, to a file
				case "restore":
					pe = new PersistEngine();
					if(pe.XMLRestoreLOS(elem.name, db)) return true;
					break;
				default: return false;
			}
			return false;
		}
#if (TEST_REQUESTENGINE)

		//-----------------< test stub >-------------
		static void Main(string[] args)
		{
			string reply;
			RequestEngine re = new RequestEngine();
			re.parseRequest("write,add,key0,Name 1,descr 1,0,,1,Hello", out reply);
			Console.WriteLine(reply);
			re.parseRequest("write,edit,key0,Name 2 (edited),descr 2 (edited),1,Three,1,World", out reply);
			Console.WriteLine(reply);
			Console.WriteLine();
		}
#endif
	}
}
