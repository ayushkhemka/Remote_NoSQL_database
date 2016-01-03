///////////////////////////////////////////////////////////////
// DFactory.cs - define immutable noSQL database                       //
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
 * This package implements DBFactory<Key, Value, Data> where Value
 * is the DBElement<key, Data> type.
 *
 * This class contains the immutable database we need to create as per
 * requirement 8, which says that we need to create a new immutable
 * database from the list of keys returned by a query.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, QueryEngine.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
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
	public class DBFactory<Key, Value, Data>
	{
		private Dictionary<Key, Value> dbStore;
		static QueryEngine<Key, Value, Data> qe = new QueryEngine<Key, Value, Data>();

		/* This constructor takes the given inputs, creates a new instance
		 * of the Dictionary, and adds the values to it.
		 * There is nothing more you can do to it except for calling the
		 * constructor. There is an additional showDBF() function, which
		 * can be used to show the immutable database if needed.
		 */
		public DBFactory(List<Key> keys, DBEngine<Key, Value> db)
		{
			dbStore = new Dictionary<Key, Value>();
			Value value;
			foreach (Key key in keys)
			{
				if (!db.Keys().Contains(key))
				{ }
				db.getValue(key, out value);
				dbStore[key] = value;
			}
		}

		private bool getValue(Key key, out Value val)
		{
			if (dbStore.Keys.Contains(key))
			{
				val = dbStore[key];
				return true;
			}
			val = default(Value);
			return false;
		}
		private IEnumerable<Key> Keys()
		{
			return dbStore.Keys;
		}

		public void showDBF(DBFactory<Key, Value, Data> db) //Source: DBExtensions.cs
		{
			foreach (Key key in db.Keys())
			{
				Value value;
				db.getValue(key, out value);
				DBElement<Key, Data> elem = value as DBElement<Key, Data>;
				Write("\n\n  -- key = {0} --\n", key);
				Console.WriteLine("Name: {0}", elem.name);
				Console.WriteLine("Description: {0}", elem.descr);
				Console.WriteLine("Timestamp: {0}", elem.timeStamp.ToString());
				Console.WriteLine("Children: {0}", qe.ToString(elem.children));
				//Console.WriteLine("Payload: {0}", qe.ToString(payload));  //-- Can't cast Data as Generic List 
			}
		}
	}
}
#if (TEST_DBFACTORY)
    class TEST_DBFACTORY{
    static void Main(string[] args)
        {
            "Testing DBFactory Package".title('=');
            WriteLine();

            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "Name";
            elem.descr = "Description";
            elem.timeStamp = DateTime.Now;
            elem.children.Add(1);
            elem.payload = "Payload";
            db.insert(1, elem);
            DBFactory<int, DBElement<int, string>, string> dbFact = new DBFactory<int, DBElement<int, string>, string>(new List<int> { 1 }, db);
            dbFact.showDBF(dbFact);
        }
    }
}
#endif