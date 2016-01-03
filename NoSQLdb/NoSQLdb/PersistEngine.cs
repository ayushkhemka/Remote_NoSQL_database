///////////////////////////////////////////////////////////////
// PersistEngine.cs - persist/restore to/from xml files      //
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
 * This package implements PersistEngine which writes to XML files and restores
 * or augments data from XML files into the database.
 *
 * This class contains different definitions for different types of 
 * databases. This is because in order to write or load to/from XML
 * files, you have to know the type of the data. This is the reason
 * we write the KeyType and PayloadType in the XML files along with
 * the database contents.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, DBExtensions.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
 *
 * Build Process:  devenv Project2.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.1 : 20 Nov 15
 * - added error handling
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
using System.Xml.Linq;

namespace Project2
{
	public class PersistEngine
	{
		public void XMLWrite(DBEngine<int, DBElement<int, string>> db, out string pathname)
		{
			pathname = "xmlDoc.xml";
			// The root element is nosqldb or nosqldbLOS, which we can say, is the name
			// of the database of the corresponding type. The declaration is important
			// from an XML parser point of view.
			DBElement<int, string> Val = new DBElement<int, string>();
			XDocument doc = new XDocument(new XElement("nosqldb"));
			doc.Declaration = new XDeclaration("1.0", "utf - 8", "yes");
			XElement keyType = new XElement("keytype", "int");
			XElement payloadType = new XElement("payloadtype", "string");
			doc.Root.Add(keyType);
			doc.Root.Add(payloadType);
			foreach (var key in db.Keys())
			{
				XElement keyNode = new XElement("key", key);
				db.getValue(key, out Val);
				XElement elementNode = new XElement("element");
				elementNode.Add(new XElement("name", Val.name));
				elementNode.Add(new XElement("descr", Val.descr));
				elementNode.Add(new XElement("timeStamp", Val.timeStamp.ToString()));
				XElement childrenNode = new XElement("children"); //since children is List<Key>
				foreach (var item in Val.children)
				{
					childrenNode.Add(new XElement("key", item));
				}
				elementNode.Add(childrenNode);
				elementNode.Add(new XElement("payload", Val.payload)); //since payload is string for this type of database
				doc.Root.Add(keyNode);
				doc.Root.Add(elementNode);
			}
			doc.Save(pathname);
		}

		public void XMLWriteLOS(DBEngine<string, DBElement<string, List<string>>> dbLOS, out string pathnameLOS)
		{
			pathnameLOS = "xmlDocLOS.xml";
			XDocument docLOS = new XDocument(new XElement("nosqldbLOS"));
			docLOS.Declaration = new XDeclaration("1.0", "utf - 8", "yes");
			XElement keyTypeLOS = new XElement("keytype", "string");
			XElement payloadTypeLOS = new XElement("payloadtype", "ListOfString");
			docLOS.Root.Add(keyTypeLOS);
			docLOS.Root.Add(payloadTypeLOS);
			DBElement<string, List<string>> ValLOS = new DBElement<string, List<string>>();
			foreach (var keyLOS in dbLOS.Keys())
			{
				XElement keyNodeLOS = new XElement("key", keyLOS);
				dbLOS.getValue(keyLOS, out ValLOS);
				XElement elementNodeLOS = new XElement("element");
				elementNodeLOS.Add(new XElement("name", ValLOS.name));
				elementNodeLOS.Add(new XElement("descr", ValLOS.descr));
				elementNodeLOS.Add(new XElement("timeStamp", ValLOS.timeStamp.ToString()));

				XElement childrenNodeLOS = new XElement("children");
				foreach (var item in ValLOS.children)
				{
					childrenNodeLOS.Add(new XElement("key", item));
				}
				elementNodeLOS.Add(childrenNodeLOS);

				XElement payloadNodeLOS = new XElement("payload"); //since payload is List<string> for this type of database
				foreach (var item in ValLOS.payload)
				{
					payloadNodeLOS.Add(new XElement("item", item));
				}
				elementNodeLOS.Add(payloadNodeLOS);

				docLOS.Root.Add(keyNodeLOS);
				docLOS.Root.Add(elementNodeLOS);
			}
			docLOS.Save(pathnameLOS);
		}
		/* The restore functions capture elements from the XML files provided.
		 * Key will always be the node preceeding the element node in our file
		 * structure, so the previous node of element is our key for that element.
		 * Then the keys and the corresponding values are extracted and entered into
		 * the corresponding database. If a key is already present in the database,
		 * I have chosen to discard it.
		 */
		public bool XMLRestore(string pathname, DBEngine<int, DBElement<int, string>> db)
		{
			DBElement<int, string> elemString;
			XDocument xdoc = new XDocument();
			try { xdoc = XDocument.Load(pathname); }
			catch (Exception e) { Console.Write("\n  Problem found: {0}\n", e.ToString()); return false; }
			var elem = xdoc.Root.Elements("element"); //extracting all elements from the file
			foreach (XElement i in elem)
			{
				XElement keyNode = (XElement)i.PreviousNode;
				int key = Int32.Parse(keyNode.Value.ToString()); //key is of type int for this database, so we need to cast it
				if (!db.Keys().Contains(key))
				{
					elemString = new DBElement<int, string>();
					elemString.name = i.Element("name").Value.ToString();
					elemString.timeStamp = Convert.ToDateTime(i.Element("timeStamp").Value.ToString()); //the format extracted from the file may or may not be a string
					elemString.descr = i.Element("descr").Value.ToString();
					elemString.payload = i.Element("payload").Value.ToString(); //we know that the payload is string
					elemString.children.Clear();
					var childrenNode = i.Element("children").Elements("key"); //since each child of the element is stored as a <key> in the file
					foreach (var c in childrenNode)
						elemString.children.Add(Int32.Parse(c.Value));
					db.insert(key, elemString);
				}
			}
			return true;
		}

		public bool XMLRestoreLOS(string pathnameLOS, DBEngine<string, DBElement<string, List<string>>> dbLOS)
		{
			DBElement<string, List<string>> elemStringLOS;
			XDocument xdocLOS = new XDocument();
			try { xdocLOS = XDocument.Load(pathnameLOS); }
			catch (Exception e) { Console.Write("\n  Problem found: {0}\n", e.ToString()); return false; }
			var elemLOS = xdocLOS.Root.Elements("element"); //extracting all the elements from the xml file
			foreach (XElement i in elemLOS)
			{
				XElement keyNodeLOS = (XElement)i.PreviousNode;
				string keyLOS = keyNodeLOS.Value.ToString(); //we know that the key is of type string
				if (!dbLOS.Keys().Contains(keyLOS))
				{
					elemStringLOS = new DBElement<string, List<string>>();
					elemStringLOS.name = i.Element("name").Value.ToString();
					elemStringLOS.timeStamp = Convert.ToDateTime(i.Element("timeStamp").Value.ToString()); //the format stored in the file may or may not be a string
					elemStringLOS.descr = i.Element("descr").Value.ToString();
					elemStringLOS.payload = new List<string>() { }; //we know that the payload is List<string>
					elemStringLOS.children.Clear();
					var childrenNode = i.Element("children").Elements("key");
					foreach (var c in childrenNode)
						elemStringLOS.children.Add(c.Value.ToString()); //since each child is a string
					var payloadNode = i.Element("payload").Elements("item");
					foreach (var p in payloadNode)
						elemStringLOS.payload.Add(p.Value.ToString());
					dbLOS.insert(keyLOS, elemStringLOS);
				}
			}
			return true;
		}
	}

#if (TEST_PERSISTENGINE)
    public class TEST_PERSISTENGINE
    {
        private string pathname = "";       // We need to store these variables so that
        private string pathnameLOS = "";    // they can be reused later.
        static void Main(string[] args)
        {
            PersistEngine pe = new PersistEngine();
            TEST_PERSISTENGINE pet = new TEST_PERSISTENGINE();
            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            DBEngine<string, DBElement<string, List<string>>> dbLOS = new DBEngine<string, DBElement<string, List<string>>>();
            DBElement<int, string> elem = new DBElement<int, string>();
            DBElement<string, List<string>> elemLOS = new DBElement<string, List<string>>();
            //Adding DBElement<int, string>
            elem.name = "name";
            elem.descr = "descr";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int>() { 1, 2, 3 });
            elem.payload = "hello";
            db.insert(0, elem);
            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.name = "name1";
            elem1.descr = "descr1";
            elem1.children.AddRange(new List<int>() { 1, 2, 3 });
            elem1.timeStamp = DateTime.Now;
            elem1.payload = "hello1";
            db.insert(1, elem1);
            //Adding DBElement<string, List<string>>
            elemLOS.name = "name2";
            elemLOS.descr = "descr2";
            elem.timeStamp = DateTime.Now;
            elemLOS.payload = new List<string>() { "hello", "world" };
            dbLOS.insert("Two", elemLOS);
            elemLOS = new DBElement<string, List<string>>();
            elemLOS.name = "name3";
            elemLOS.descr = "descr3";
            elemLOS.timeStamp = DateTime.Now;
            elemLOS.children.AddRange(new List<string>() { "One", "Two", "Three" });
            elemLOS.payload = new List<string>() { "fee", "foo", "bar" };
            dbLOS.insert("Three", elemLOS);
            pe.XMLWrite(db, out pet.pathname);
            pe.XMLWriteLOS(dbLOS, out pet.pathnameLOS);
            WriteLine("\n{0}", XDocument.Load(pet.pathname).ToString());
            WriteLine("\n{0}", XDocument.Load(pet.pathnameLOS).ToString());
            pe.XMLRestore("../../xmlRestore.xml", db);
            db.show<int, DBElement<int, string>, string>();
            pe.XMLRestoreLOS("../../xmlRestoreLOS.xml", dbLOS);
            dbLOS.show<string, DBElement<string, List<string>>, List<string>, string>();
        }
    }
#endif
}