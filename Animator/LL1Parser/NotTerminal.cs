using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    public class NotTerminal : Symbol
    {
	    // Index courant dans la liste des non terminaux de la grammaire
	    public static int NotTerminalIndex = 0;
	
        public NotTerminal(String s) : base(s)
        {
    	    ptIndex = NotTerminalIndex;
    	    NotTerminalIndex++;
        }

        public override bool Equals(Object o)
        {
    	    return (o is NotTerminal && base.Equals(o));
        }
    
        public override String ToString(){return "<"+GetText()+">";}
    }
}
