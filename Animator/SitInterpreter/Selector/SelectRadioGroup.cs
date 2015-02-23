﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Animator.SitInterpreter.Selector
{
    public class SelectRadioGroup : SelectorAb
    {

        public SelectRadioGroup(String opt)
            : base(opt)
        {

        }

        public override LinkedList<FrameworkElement> getControls(Grid root)
        {
            LinkedList<FrameworkElement> res = new LinkedList<FrameworkElement>();

            foreach (FrameworkElement fe in root.Children)
            {
                if (fe is Grid)
                {
                    foreach (FrameworkElement rb in getControls((Grid)fe))
                    {
                        res.AddLast(rb);
                    }
                }
                else
                {
                    //Console.WriteLine(fe.Name + " et " + options);
                    if (fe is RadioButton)
                    {
                        if (((RadioButton)fe).GroupName.Contains(options))
                        {
                            //Console.WriteLine("trouvé: " + fe.ToString());
                            res.AddLast(fe);
                        }
                    }
                }

            }

            return res;
        }
    }
}
