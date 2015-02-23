using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class Selector : Terminal
    {
        public override String ToString() { return "Selector: %" + GetText(); }

        public Selector(String val)
            : base(val)
        {
        }
    }
}
