using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    class ParserException : Exception
    {
        public ParserException(string str)
            : base(str)
        {

        }
    }
}
