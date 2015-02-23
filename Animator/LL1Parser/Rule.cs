using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    public class Rule
    {
        public NotTerminal left;
        public LinkedList<Symbol> right;

        public Rule(NotTerminal left, LinkedList<Symbol> right)
        {
            this.left = left;
            this.right = right;
        }

        public NotTerminal Left() { return left; }
        public LinkedList<Symbol> Right() { return right; }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(left + " ::=");

            foreach (Symbol sym in right)
            {
                sb.Append(" ");
                sb.Append(sym);
            }

            return sb.ToString();
        }

    }
}
