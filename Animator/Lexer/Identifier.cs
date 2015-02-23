using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class Identifier : Terminal
    {
        public String ToString(){ return "Identifier: " + GetText() ;}

        public Identifier(String val) : base(val)
        {   
        }
    }
}
