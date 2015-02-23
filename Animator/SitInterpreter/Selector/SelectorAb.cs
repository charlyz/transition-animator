using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Animator.SitInterpreter.Selector
{
    public abstract class SelectorAb
    {
        public string options;

        public SelectorAb(string opt)
        {
            this.options = opt;
        }

        abstract public LinkedList<FrameworkElement> getControls(Grid root);
    }
}
