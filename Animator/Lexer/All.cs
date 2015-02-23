using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class All : Terminal
    {
        public override String ToString() { return "all"; }

        public All()
            : base("all")
        {
        }
    }
}
