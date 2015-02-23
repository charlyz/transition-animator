using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class Constant : Terminal
    {

        public override String ToString() { return "Constant: " + GetText(); }

        public Constant(String val) : base(val)
        { 
        }
    }
}
