using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class ControlType : Terminal
    {
        public override String ToString() { return "ControlType: @" + GetText(); }

        public ControlType(String val) : base(val)
        {
        }
    }
}
