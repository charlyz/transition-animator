using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;
using System.Windows;

namespace Animator.SitInterpreter.Substitutes
{
    abstract class SubstituteAb
    {
        public Window win;
        public Interpreter interpreter;

        public SubstituteAb(Window win, Interpreter inter)
        {
            this.win = win;
            this.interpreter = inter;
        }

        abstract public void substitute(Tree<Symbol> tree, LinkedList<FrameworkElement> controls);
    }
}
