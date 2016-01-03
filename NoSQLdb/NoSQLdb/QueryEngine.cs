///////////////////////////////////////////////////////////////
// QueryEngine.cs - execute queries                           //
// Ver 1.0                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Lenovo g505s, AMD A8, Windows 10            //
// Author:      Ayush Khemka, 538044584                      //
//              (315) 560 - 6783, aykhemka@syr.edu           //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements QueryEngine<Key, Value, Data> where Value
 * is the DBElement<key, Data> type.
 *
 * This class contains the functions needed to execute the queries
 * supported by this database.
 * Queries supported:
 * 1. Value of a specified key.
 * 2. Children of a specified key.
 * 3. All keys starting with a specific digit or character.
 * If -999 or * is passed, all the keys will be returned.
 * 4. All keys containing a specified string in their metadata.
 * 5. All keys entered from a given time and date to a given
 * time and date. If no end date is given, the current date and
 * time will be taken as the end date.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and DBExtensions.cs
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
using static System.Console;

namespace Project2
{
	public class QueryEngine<Key, Value, Data>
	{
		private StringBuilder accum = new StringBuilder();

		//Overloaded function to convert given list to string
		public string ToString(List<Key> elem) //Source: DBExtensions.cs
		{
			accum.Clear();
			if (elem.Count > 0)
			{
				bool first = true;
				foreach (Key key in elem)
				{
					if (first)
					{
						accum.Append(String.Format("{0}", key.ToString()));
						first = false;
					}
					else
						accum.Append(String.Format(", {0}", key.ToString()));
				}
			}
			return accum.ToString();
		}
		public bool getValue(Key key, DBEngine<Key, Value> db, out Value elemResult)
		{
			//----------< Returns the value of a specified key >-------------------
			if (db.Keys().Contains(key))
			{
				db.getValue(key, out elemResult);
				return true;
			}
			elemResult = default(Value);
			return false;
		}
		public bool getChildren(Key key, DBEngine<Key, DBElement<Key, Data>> db, out List<Key> children)
		{
			//--------------< Returns the list of children for a specified key in a specified db >------------
			children = new List<Key>() { };
			DBElement<Key, Data> Result = new DBElement<Key, Data>();
			if (db.Keys().Contains(key))
			{
				children.Clear();
				db.getValue(key, out Result);
				foreach (var child in Result.children)
					children.Add(child);
				return true;
			}
			Result = default(DBElement<Key, Data>);
			children = default(List<Key>);
			return false;
		}

		public bool searchPattern(Key key, DBEngine<Key, Value> db, out List<Key> resultKeys)
		{
			//-----------< Searched for keys starting with "Key". If default Keys are specified
			//-----------< which are -999 for int and "*" for string, the set of all keys for the
			//-----------< given db will be returned >-----------------------------------------
			resultKeys = db.Keys().ToList<Key>();
			List<Key> Keys = db.Keys().ToList<Key>();
			bool flag = false;
			if ((key.ToString().CompareTo("*") == 0) || (key.ToString().CompareTo("-999") == 0))
				flag = true;
			else
			{
				resultKeys.Clear();
				foreach (var item in Keys)
				{
					string itemString = item.ToString();
					char firstChar = itemString.ToCharArray()[0];
					if (key.ToString().CompareTo(firstChar.ToString()) == 0)
					{
						resultKeys.Add((Key)item);
						flag = true;
					}
				}
			}
			if (flag)
				return true;
			else
				return false;
		}

		public bool searchString(string pattern, DBEngine<Key, DBElement<Key, Data>> db, out List<Key> resultKeys)
		{
			//------------< Returns set of keys whose metadata contain the given string >-------------
			resultKeys = db.Keys().ToList<Key>();
			resultKeys.Clear();
			List<Key> Keys = db.Keys().ToList<Key>();
			DBElement<Key, Data> elem = new DBElement<Key, Data>();
			bool flag = false;
			foreach (Key key in Keys)
			{
				db.getValue(key, out elem);
				if ((elem.name.ToString().IndexOf(pattern) >= 0) || (elem.descr.ToString().IndexOf(pattern) >= 0) || (elem.timeStamp.ToString().IndexOf(pattern) >= 0))
				{
					resultKeys.Add(key);
					flag = true;
				}
				else
					foreach (var child in elem.children) //search each child for the string
					{
						if (child.ToString().IndexOf(pattern) >= 0)
						{
							resultKeys.Add(key);
							flag = true;
						}
					}
			}
			if (flag)
				return true;
			return false;
		}

		public bool searchInterval(string startDate, string EndDate, DBEngine<Key, DBElement<Key, Data>> db, out List<Key> resultKeys)
		{
			//----------< Returns the set of keys whose DateTimes are more
			//----------< than the given start date and less than the given
			//----------< end date. Default for end date is DateTime.Now >--------------
			resultKeys = db.Keys().ToList<Key>();
			resultKeys.Clear();
			DateTime start = Convert.ToDateTime(startDate); //Source: https://msdn.microsoft.com/en-us/library/cc165448.aspx
			DateTime end;
			if (EndDate == "")
				end = DateTime.Now;
			else
				end = Convert.ToDateTime(EndDate);
			bool flag = false;
			List<Key> Keys = db.Keys().ToList<Key>();
			DBElement<Key, Data> elem = new DBElement<Key, Data>();
			foreach (Key key in Keys)
			{
				db.getValue(key, out elem);
				if ((elem.timeStamp >= start) && (elem.timeStamp <= end))
				{
					resultKeys.Add(key);
					flag = true;
				}
			}
			if (flag)
				return true;
			return false;
		}
	}
}
#if(TEST_QUERYENGINE)
    public class TEST_QUERYENGINE
    {
        private static List<int> children = new List<int>();
        private static List<string> childrenLOS = new List<string>();
        private static List<int> Keys = new List<int>();
        private static List<string> KeysLOS = new List<string>();
        static void Main(string[] args)
        {
            QueryEngine<int, DBElement<int, string>, string> qe = new QueryEngine<int, DBElement<int, string>, string>();
            QueryEngine<string, DBElement<string, List<string>>, List<string>> qeLOS = new QueryEngine<string, DBElement<string, List<string>>, List<string>>();
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBElement<int, string> elem = new DBElement<int, string>();
            DBElement<int, string> Result = new DBElement<int, string>();
            
            elem.name = "Test";
            elem.descr = "Test descr";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3 });
            elem.payload = "Test Payload";
            db.insert(0, elem);

            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.name = "Test 1";
            elem1.descr = "Test descr 1";
            elem1.timeStamp = DateTime.Now;
            elem1.children.AddRange(new List<int> { 4, 5, 6 });
            elem1.payload = "Test Payload 1";
            db.insert(1, elem1);

            DBEngine<string, DBElement<string, List<string>>> dbLOS = new DBEngine<string, DBElement<string, List<string>>>();
            DBElement<string, List<string>> elemLOS = new DBElement<string, List<string>>();
            DBElement<string, List<string>> ResultLOS = new DBElement<string, List<string>>();
            elemLOS.name = "Test";
            elemLOS.descr = "Test descr";
            elemLOS.timeStamp = DateTime.Now;
            elemLOS.children = new List<string> { "One", "Two", "Three" };
            elemLOS.payload = new List<string> { "Test Payload", "Payload" };
            dbLOS.insert("One", elemLOS);

            DBElement<string, List<string>> elemLOS1 = new DBElement<string, List<string>>();
            elemLOS1.name = "Test 2";
            elemLOS1.descr = "Test descr 2";
            elemLOS1.timeStamp = DateTime.Now;
            elemLOS1.children = new List<string> { "Four", "Five", "Six" };
            elemLOS1.payload = new List<string> { "Test Payload 2", "Payload 2" };
            dbLOS.insert("Two", elemLOS1);

            if (qe.getValue(0, db, out Result))
                WriteLine("Value of key 0:\n{0}\n  Payload:  {1}", Result.showMetaData(), Result.payload.ToString());
            else
                WriteLine("Key not found");
            WriteLine();
            if (qeLOS.getValue("One", dbLOS, out ResultLOS))
                WriteLine("Value of key:\n{0}\n  Payload: {1}", ResultLOS.showMetaData(), qeLOS.ToString(ResultLOS.payload));
            else
                WriteLine("Key not found");
            WriteLine();

            if (qe.getChildren(0, db, out children))
                WriteLine("Children of specified key: {0}", qe.ToString(children));
            else
                WriteLine("Key not found");
            if (qeLOS.getChildren("One", dbLOS, out childrenLOS))
                WriteLine("Children of specified key: {0}", qeLOS.ToString(childrenLOS));
            else
                WriteLine("Key not found");
            WriteLine();

            if (qe.searchPattern(0, db, out Keys))
                Console.WriteLine("Set of Keys: {0}", qe.ToString(Keys));
            else
                Console.WriteLine("No Keys found!");
            if (qeLOS.searchPattern("O", dbLOS, out KeysLOS))
                Console.WriteLine("Set of Keys: {0}", qeLOS.ToString(KeysLOS));
            else
                Console.WriteLine("No Keys found!");
            WriteLine();

            if (qe.searchString("descr", db, out Keys))
                Console.WriteLine("Set of Keys: {0}", qe.ToString(Keys));
            else
                Console.WriteLine("\nNo Keys found!");
            if (qeLOS.searchString("Test", dbLOS, out KeysLOS))
                Console.WriteLine("Set of Keys: {0}", qeLOS.ToString(KeysLOS));
            else
                Console.WriteLine("No Keys found!");
            WriteLine();

            if (qe.searchInterval("10/7/1999 12:00:00 AM", "", db, out Keys))
                Console.WriteLine("Set of Keys: {0}", qe.ToString(Keys));
            else
                Console.WriteLine("\nNo Keys found!");
            if (qeLOS.searchInterval("10/7/1999 12:00:00 AM", "", dbLOS, out KeysLOS))
                Console.WriteLine("Set of Keys: {0}", qeLOS.ToString(KeysLOS));
            else
                Console.WriteLine("No Keys found!");
            WriteLine();
        }
    }
}
#endif