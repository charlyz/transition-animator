using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    public  class Terminal : Symbol
    {
        public int line;
        public int col;
        //Index courant dans la liste des non terminaux de la grammaire
        public static int TerminalIndex = 0;
  
        public Terminal(String s) : base(s)
        {
	        ptIndex = TerminalIndex;
	        TerminalIndex++;
            //Console.WriteLine("NewTerminal: " + s + " - " + ptIndex);
        }

        public override String ToString()
        {
	       return "'" + text + "'";
        }
  
    }
}
