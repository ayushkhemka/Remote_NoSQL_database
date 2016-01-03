/////////////////////////////////////////////////////////////////////////
// MessageMaker.cs - Construct ICommService Messages                   //
// ver 1.2                                                             //
// Author: Ayush Khemka, 538044584, aykhemka@syr.edu									 //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis, Project #4    //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This is a placeholder for application specific message construction
 *
 * Additions to C# Console Wizard generated code:
 * - references to ICommService and Utilities
 */
/*
 * Maintenance History:
 * --------------------
 * ver 1.2 : 3 Nov 2015
 * - changed namespace
 * ver 1.0 : 29 Oct 2015
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4
{
  public class MessageMaker
  {
    public static int msgCount { get; set; } = 0;
    public Message makeMessage(string fromUrl, string toUrl)
    {
      Message msg = new Message();
      msg.fromUrl = fromUrl;
      msg.toUrl = toUrl;
      msg.content = String.Format("\n  message #{0}", ++msgCount);
			return msg;
    }
#if (TEST_MESSAGEMAKER)
    static void Main(string[] args)
    {
      MessageMaker mm = new MessageMaker();
      Message msg = mm.makeMessage("fromFoo", "toBar");
      Utilities.showMessage(msg);
      Console.Write("\n\n");
    }
#endif
  }
}
