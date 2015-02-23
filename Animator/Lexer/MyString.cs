using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class MyString : Terminal
    {
        public override String ToString() { return "String: " + GetText(); }

        public MyString(String val) : base(val)
        {
        }
    }
}
