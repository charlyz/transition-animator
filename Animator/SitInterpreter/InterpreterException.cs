using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    class InterpreterException : Exception
    {
        public InterpreterException(string str)
            : base(str)
        {

        }
    }
}
