///////////////////////////////////////////////////////////////
// NoSQLdb.cs - Test Requirements for Project #2            //
// Ver 1.3                                                   //
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
 * This package begins the demonstration of meeting requirements.
 * Every requirement from 2 to 8 have been successfully implemented
 * and demonstrated here. Any notes, if any, to be taken care of are
 * mentioned as per the requirement.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine.cs, DBFactory.cs, 
 *   DBExtensions.cs, PersistEngine.cs, QueryEngine, cs, 
 *   Scheduler.cs, and UtilityExtensions.cs
 *
 * Build Process:  devenv Project2.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.2 : 07 Oct 15
 * - completed the demonstration of every requirement
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Timers;
using static System.Console;

namespace Project2
{
	class NoSQLdb
	{
		// Creating instances of the packages that are going to be referenced.
		// Also defining the Lists<Key> we need to store the results of the query in
		// to be passed on to other packages.

		private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
		private DBEngine<string, DBElement<string, List<string>>> dbLOS = new DBEngine<string, DBElement<string, List<string>>>();
		private string pathname = "";
		private string pathnameLOS = "";
		List<int> Keys = new List<int>();
		List<string> KeysLOS = new List<string>();
		List<int> children = new List<int>();
		List<string> childrenLOS = new List<string>();
		QueryEngine<int, DBElement<int, string>, string> qe = new QueryEngine<int, DBElement<int, string>, string>();
		QueryEngine<string, DBElement<string, List<string>>, List<string>> qeLOS = new QueryEngine<string, DBElement<string, List<string>>, List<string>>();
		DBElement<int, string> Result = new DBElement<int, string>();
		DBElement<string, List<string>> ResultLOS = new DBElement<string, List<string>>();

		//-------------< Following is the demonstration of each requirement, for each database type >----------
		void TestR2()
		{
			//----< Demonstrating the structure of each element of the database. Taking only <int, string> for now >-------
			"Demonstrating Requirement #2".title('-');
			WriteLine();
			"Element Structure (Key=int, Value=DBElement<int, string>)".title('-');
			DBElement<int, string> elem = new DBElement<int, string>();
			elem.name = "Test Element";
			elem.descr = "Description";
			elem.timeStamp = DateTime.Now;
			elem.children.Add(5);
			elem.payload = "Hello!";
			Write(elem.showElement<int, string>());
			WriteLine();
			db.insert(0, elem);
		}
		/*void TestR3()
		{
			//----------< Demonstrating adding and removing of elements >-----------
			"Demonstrating Requirement #3".title();
			WriteLine();
			"Adding <int, string> elements".title('-');
			DBElement<int, string> elem = new DBElement<int, string>();
			elem.name = "Element 1";
			elem.descr = "Test Element (int, string)";
			elem.timeStamp = DateTime.Now;
			elem.children.AddRange(new List<int> { 1, 2, 3 });
			elem.payload = "Element 1's payload. (string)";
			WriteLine(elem.showElement());
			bool p1 = db.insert(1, elem);
			WriteLine();
			DBElement<int, string> elem1 = new DBElement<int, string>();
			elem1.name = "Element 2";
			elem1.descr = "Again <int, string> but no children!";
			elem1.timeStamp = DateTime.Now;
			elem1.children.Clear();
			elem1.payload = "Element 2's payload. (string)";
			WriteLine(elem1.showElement());
			bool p2 = db.insert(2, elem1);
			WriteLine();
			DBElement<int, string> elem2 = new DBElement<int, string>();
			elem2.name = "Element 3";
			elem2.descr = "Test Element <int, string>. Different timeStamp.";
			elem2.children.AddRange(new List<int> { 1, 2, 3 });
			elem2.timeStamp = DateTime.UtcNow;
			elem2.payload = "Element 3's payload. (string)";
			WriteLine(elem2.showElement());
			WriteLine();
			bool p3 = db.insert(3, elem2);
			"Adding <string, List<string>> elements".title('-');
			DBElement<string, List<string>> newelem = new DBElement<string, List<string>>();
			newelem.name = "New elem";
			newelem.descr = "Element 4. <string, List<string>>";
			newelem.timeStamp = DateTime.Now;
			newelem.children.AddRange(new List<string> { "one", "two", "three" });
			newelem.payload = new List<string> { "one", "two", "three" };
			Write(newelem.showElement<string, List<string>, string>());
			bool p4 = dbLOS.insert("Four", newelem);
			Console.WriteLine("\n\nInserting elements... ");
			if (p1 && p2 && p3 && p4)
				Console.WriteLine("All inserts succeeded");
			else
				Console.WriteLine("\n\nAt Least one insert failed");
			"Database".title('-');
			db.show<int, DBElement<int, string>, string>();
			dbLOS.show<string, DBElement<string, List<string>>, List<string>, string>();
			WriteLine();
			"Removing elements".title();
			DBElement<int, string> pay = new DBElement<int, string>();
			db.remove(1, out pay);
			Console.Write("\n\nRemoved element: {0}", pay.name);
			WriteLine();
			db.remove(2, out pay);
			Console.Write("\n\nRemoved element: {0}", pay.name);
			WriteLine();
			db.remove(3, out pay);
			Console.Write("\n\nRemoved element: {0}", pay.name);
			WriteLine();
			"Database".title();
			db.show<int, DBElement<int, string>, string>();
			dbLOS.show<string, DBElement<string, List<string>>, List<string>, string>();
			WriteLine();
		}*/
		void TestR4()
		{
			//----------< Demonstrating editing of items >-------------
			"Demonstrating Requirement #4".title();
			WriteLine();
			"Editing an item".title();
			"Old Database".title();
			db.show<int, DBElement<int, string>, string>();
			dbLOS.show<string, DBElement<string, List<string>>, List<string>, string>();
			WriteLine();
			DBElement<string, List<string>> editElement = new DBElement<string, List<string>>();
			dbLOS.getValue("Four", out editElement);
			editElement.name = "Element 4 (edited)";
			editElement.descr = "New description";
			editElement.timeStamp = DateTime.Now;
			editElement.children.Clear();
			editElement.children.Add("five");
			editElement.payload = new List<string> { "four", "five", "six" };
			editElement.payload.Add("eight");
			dbLOS.insert("Four", editElement);
			"New Database".title();
			db.show<int, DBElement<int, string>, string>();
			dbLOS.show<string, DBElement<string, List<string>>, List<string>, string>();
			WriteLine();
		}
		void TestR5()
		{
			//-----------< Demonstrating writing to XML files and
			//-----------< augmenting from some other files >-----------
			"Demonstrating Requirement #5".title();
			WriteLine();
			"Persisting the data".title('-');
			PersistEngine pe = new PersistEngine();
			pe.XMLWrite(db, out pathname);          // File created as xmlDoc.xml
			pe.XMLWriteLOS(dbLOS, out pathnameLOS); // File created as xmlDocLOS.xml
			WriteLine();
			"File generated for <int, string>".title();
			WriteLine("\n{0}", XDocument.Load(pathname).Declaration);
			WriteLine("{0}", XDocument.Load(pathname).ToString());
			WriteLine();
			"File generated for <string, List<string>>".title();
			WriteLine("\n{0}", XDocument.Load(pathnameLOS).Declaration);
			WriteLine("{0}", XDocument.Load(pathnameLOS).ToString());
			WriteLine();
			"Restoring (or Augmenting) from XML into database".title('-');
			try
			{
				pe.XMLRestore(@"XMLRestore.xml", db);
				pe.XMLRestoreLOS(@"xmlRestoreLOS.xml", dbLOS);
			}
			catch
			{
				Console.WriteLine("\nAt least one file failed to load, please check the paths in code\n");
			}

			db.show<int, DBElement<int, string>, string>();
			dbLOS.show<string, DBElement<string, List<string>>, List<string>, string>();
			WriteLine();
		}
		void TestR6()
		{
			//------< Write to XML files (same as above) after every 1 second. Press any key to stop persisting >--------
			"Demonstrating Requirement #6".title();
			"Press any key to stop".title();
			WriteLine();
			Scheduler sc = new Scheduler(1000);
			sc.schedular.Enabled = true;
			PersistEngine pe = new PersistEngine();
			pe.XMLWrite(db, out pathname);
			pe.XMLWriteLOS(dbLOS, out pathnameLOS);
			Console.ReadKey();
			sc.schedular.Enabled = false; // Setting this will prevent the scheduler to stop executing
			WriteLine();
		}
		/*void TestR7()
		{
			//---------< Demonstrating queries >---------
			"Demonstrating Requirement #7".title();
			WriteLine();
			//--------< Searching for key=0 and key=Four >----------
			int key = 0; string keyLOS = "Four";
			if (qe.getValue(key, db, out Result))
				WriteLine("Value of key \"{2}\" in <int, string> database:\n{0}\n  Payload:  {1}", Result.showMetaData(), Result.payload.ToString(), key);
			else
				WriteLine("Value of key \"{0}\" in <int, string> database: Key not found", key);
			WriteLine();
			if (qeLOS.getValue(keyLOS, dbLOS, out ResultLOS))
				WriteLine("Value of key \"{2}\" in <string, List<string>> database:\n{0}\n  Payload: {1}", ResultLOS.showMetaData(), qeLOS.ToString(ResultLOS.payload), keyLOS);
			else
				WriteLine("Children of key \"{0}\" in <string, List<string>> database: Key not found", keyLOS);
			WriteLine();
			//-----------< Searching for children of key=0 and key=Four >------------
			if (qe.getChildren(key, db, out children))
				WriteLine("Children of key \"{1}\"  in <int, string> database: {0}", qe.ToString(children), key);
			else
				WriteLine("Children of key \"{0}\"  in <int, string> database: Key not found", key);
			if (qeLOS.getChildren(keyLOS, dbLOS, out childrenLOS))
				WriteLine("Children of key \"{1}\": {0} in <string, List<string>> database", qeLOS.ToString(childrenLOS), keyLOS);
			else
				WriteLine("Children of key \"{0}\" in <string, List<string>> database: Key not found", keyLOS);
			WriteLine();
			//------------< Searching for keys starting with 1 and "F" >----------
			key = 1; keyLOS = "F";
			if (qe.searchPattern(key, db, out Keys))
				Console.WriteLine("Set of Keys starting with \"{1}\"  in <int, string> database: {0}", qe.ToString(Keys), key);
			else
				Console.WriteLine("Set of Keys starting with \"{0}\"  in <int, string> database: No Keys found!", key);
			if (qeLOS.searchPattern(keyLOS, dbLOS, out KeysLOS))
				Console.WriteLine("Set of Keys starting with \"{1}\" in <string, List<string>> database: {0}", qeLOS.ToString(KeysLOS), keyLOS);
			else
				Console.WriteLine("Set of Keys starting with \"{0}\" in <string, List<string>> database: No Keys found!", keyLOS);
			WriteLine();
			//----------< Searching for keys whose metadata contain "Descr" and "Test" >------------
			string string1 = "Descr", string2 = "Test";
			if (qe.searchString(string1, db, out Keys))
				Console.WriteLine("Set of Keys containg \"{1}\" in their metadata  for <int, string> database: {0}", qe.ToString(Keys), string1);
			else
				Console.WriteLine("Set of Keys containg \"{0}\" in their metadata  for <int, string> database: No Keys found!", string1);
			if (qeLOS.searchString(string2, dbLOS, out KeysLOS))
				Console.WriteLine("Set of Keys containg \"{1}\" in their metadata for <string, List<string>> database: {0}", qeLOS.ToString(KeysLOS), string2);
			else
				Console.WriteLine("Set of Keys containg \"{0}\" in their metadata for <string, List<string>> database: No Keys found!", string2);
			WriteLine();
			//----------< Searching for keys entered From 01 Oct 2015 to date >-----------
			string1 = "10/1/2015 12:00:00 AM";
			if (qe.searchInterval(string1, "", db, out Keys))
				Console.WriteLine("Set of Keys entered from {1} to date  in <int, string> database: {0}", qe.ToString(Keys), string1);
			else
				Console.WriteLine("Set of Keys entered from {0} to date  in <int, string> database: No Keys found!", string1);
			if (qeLOS.searchInterval(string1, "", dbLOS, out KeysLOS))
				Console.WriteLine("Set of Keys entered from {1} to date  in <string, List<string>> database: {0}", qeLOS.ToString(KeysLOS), string1);
			else
				Console.WriteLine("Set of Keys entered from {0} to date in <string, List<string>> database: No Keys found!", string1);
			WriteLine();
		}*/
		void TestR8()
		{
			//-----------< Run a query and create a new immutable database from the resultant set of keys >------------
			"Demonstrating Requirement #8".title();
			WriteLine();
			List<int> Keys = new List<int>();
			List<string> KeysLOS = new List<string>();
			QueryEngine<int, DBElement<int, string>, string> qe = new QueryEngine<int, DBElement<int, string>, string>();
			QueryEngine<string, DBElement<string, List<string>>, List<string>> qeLOS = new QueryEngine<string, DBElement<string, List<string>>, List<string>>();
			//----------< Searching for keys entered from 01 Oct 2015 till data >-------------
			string string1 = "10/1/2015 12:00:00 AM";
			if (qe.searchInterval(string1, "", db, out Keys))
				Console.WriteLine("Set of Keys entered from {1} to date: {0}", qe.ToString(Keys), string1);
			else
				Console.WriteLine("Set of Keys entered from {0} to date: No Keys found!", string1);
			"Immutable Database created, and can be verified by looking at DBFactory.cs line 58".title();
			WriteLine();
		}
#if (TEST_NOSQLDB)
		public static void Main(string[] args)
        {
            NoSQLdb exec = new NoSQLdb();
            "Demonstrating Project#2 Requirements".title('=');
            WriteLine();
            exec.TestR2();
            //exec.TestR3();
            exec.TestR4();
            exec.TestR5();
            exec.TestR6();
            //exec.TestR7();
            exec.TestR8();
            Write("\n\n");
        }
#endif
    }
}