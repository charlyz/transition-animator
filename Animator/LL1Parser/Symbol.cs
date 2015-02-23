using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    public class Symbol
    {
        protected String text;
        // Tous les premiers terminaux vers lesquels on peut dériver le symbole this
        private HashSet<Terminal> p1 = new HashSet<Terminal>();
        // Tous les terminaux pouvant suivre le symbole this
        private HashSet<Terminal> s1 = new HashSet<Terminal>();
        // Index dans la parseTable
        public int ptIndex = 0;

        public Symbol(String s)
        {
            text = s;
        }

        public String GetText() { return text; }

        public override bool Equals(Object o){ return o is Symbol && text != null && text.Equals(((Symbol)o).GetText());}

        public HashSet<Terminal> P1() { return p1; }

        public HashSet<Terminal> S1() { return s1; }

        public int Index() { return ptIndex; }
    }
}
