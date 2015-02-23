using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class Control : Terminal
    {
        public override String ToString() { return "Control: @" + GetText(); }

        public Control(String val) : base(val)
        {
        }
    }
}
