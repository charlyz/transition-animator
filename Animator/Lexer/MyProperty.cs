using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class MyProperty : Terminal
    {
        public String ToString() { return "MyProperty: " + GetText(); }

        public MyProperty(String val)
            : base(val)
        {
        }
    }
}
