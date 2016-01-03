/////////////////////////////////////////////////////////////////////////
// PerfTest.cs - Calculate and store performance measurement results   //
// ver 1.0                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This package calculates and stores the average processing times for each request type.
 * The server provides it with the processing time of each request when they are processed.
 * This package simply calculates the new average based on the new reading and stores it in
 * a Dictionary<string, double>, to be produced either in tabular form for server, or line by
 * line for WPF.
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

namespace Project4
{
	public class PerfTest
	{
		//---------< store a dictionary and counters for each request type >----------
		private Dictionary<string, double> perfTable;
		int addCount, editCount, delCount, persistCount, restoreCount, valueCount, childCount, patternCount, stringCount, intCount;
		public PerfTest()
		{
			//----------< initialization >---------------
			perfTable = new Dictionary<string, double>();
			perfTable["add"] = 0;
			perfTable["edit"] = 0;
			perfTable["delete"] = 0;
			perfTable["persist"] = 0;
			perfTable["restore"] = 0;
			perfTable["value"] = 0;
			perfTable["children"] = 0;
			perfTable["pattern"] = 0;
			perfTable["string"] = 0;
			perfTable["interval"] = 0;
			addCount = 0; editCount = 0; delCount = 0; persistCount = 0; restoreCount = 0; valueCount = 0; childCount = 0; patternCount = 0; stringCount = 0; intCount = 0;
		}

		public void add(string type, double elapsedTime)
		{
			//-----------< calculate the new average coressponding to the request type and store it >-----------
			switch (type)
			{
				case "add":
					++addCount;
					perfTable["add"] = (perfTable["add"] + elapsedTime) / addCount;
					break;
				case "edit":
					++editCount;
					perfTable["edit"] = (perfTable["edit"] + elapsedTime) / editCount;
					break;
				case "delete":
					++delCount;
					perfTable["delete"] = (perfTable["delete"] + elapsedTime) / delCount;
					break;
				case "persist":
					++persistCount;
					perfTable["persist"] = (perfTable["persist"] + elapsedTime) / persistCount;
					break;
				case "restore":
					++restoreCount;
					perfTable["restore"] = (perfTable["restore"] + elapsedTime) / restoreCount;
					break;
				case "value":
					++valueCount;
					perfTable["value"] = (perfTable["value"] + elapsedTime) / valueCount;
					break;
				case "children":
					++childCount;
					perfTable["children"] = (perfTable["children"] + elapsedTime) / childCount;
					break;
				case "pattern":
					++patternCount;
					perfTable["pattern"] = (perfTable["pattern"] + elapsedTime) / patternCount;
					break;
				case "string":
					++stringCount;
					perfTable["string"] = (perfTable["string"] + elapsedTime) / stringCount;
					break;
				case "interval":
					++intCount;
					perfTable["interval"] = (perfTable["interval"] + elapsedTime) / intCount;
					break;
				default: break;
			}
		}

		public string printTimes()
		{
			//------------< display results in tabular form for display at server >-----------
			StringBuilder accum = new StringBuilder();
			string reply;
			accum.Append("\n  Type\t\tProcessing Time (us/req)");
			accum.Append("\n==================================================\n");
			foreach (string type in perfTable.Keys)
			{
				if (type == "add" || type == "edit" || type == "value") reply="\n  "+type+"\t\t"+ String.Format("{0:#0.000}", perfTable[type]);
				else reply = "\n  " + type + "\t" + String.Format("{0:#0.000}", perfTable[type]); //Source: http://stackoverflow.com/questions/16466645/format-number-with-3-trailing-decimal-places-a-decimal-thousands-separator-and
				accum.Append(reply);
			}
			return accum.ToString();
		}

		public string printTimesWpf(string type)
		{
			//-----------< display results line by line for display at either clients, but majorly WPF >----------
			string reply = type + " operation took: " + String.Format("{0:#0.000}", perfTable[type]) + " microseconds per request";
			return reply;
		}

#if (TEST_PERFTEST)
		//-------------< test stub >-----------
		static void Main(string[] args)
		{
			HiResTimer hrt = new HiResTimer();
			PerfTest pt = new PerfTest();
			for (int j = 0; j < 10; j++)
			{
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("add", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("edit", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("delete", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("persist", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("restore", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("value", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("children", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("pattern", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("string", hrt.ElapsedMicroseconds);
				}
				hrt.Start();
				for (int i = 0; i < 10; i++)
				{
					hrt.Stop();
					pt.add("interval", hrt.ElapsedMicroseconds);
				}
			}
			pt.printTimes();
		}
#endif
	}
}
