﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Animator.SitInterpreter.Substitutes
{
    class RadioButtonToCheckBox : SubstituteAb
    {
        public RadioButtonToCheckBox(Window w, Interpreter i)
            : base(w, i)
        {

        }

        private long generateId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        public override void substitute(Tree<Symbol> tree, LinkedList<FrameworkElement> controls)
        {
            //Console.WriteLine("Enter substitute");

            String gridName = tree.Child(5).Element().GetText();
            String controlName = tree.Child(7).Element().GetText();
            int row = Int32.Parse(tree.Child(11).Element().GetText()); ;
            int column = Int32.Parse(tree.Child(14).Element().GetText());
            bool insertRow = tree.Child(10).Child(0).Element().GetText().Equals("ROW") ? false : true;
            bool insertColumn = tree.Child(13).Child(0).Element().GetText().Equals("COL") ? false : true;

            CheckBox cb = new CheckBox();
            cb.Name = controlName;
            Grid grid = (Grid)win.FindName(gridName);


            if (controls.Count != 2)
                throw new Exception("Il est seulement possible de substituer deux RadioButton par une CheckBox");

            foreach (FrameworkElement selectedControl in controls)
            {
                String identifier = selectedControl.Name;
                RadioButton rb2 = (RadioButton)interpreter.getControlEvolutions(identifier);

                if (rb2.IsChecked == true && rb2.Content.Equals("Oui"))
                {
                    cb.IsChecked = true;
                    //cb.Content = rb2.Content;
                }

                interpreter.addObjectAnimation(((Grid)rb2.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                interpreter.addDoubleAnimation(rb2.Name, 1, 0, Control.OpacityProperty, 2);
                interpreter.addObjectAnimation(((Grid)rb2.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
                RadioButton rbOnUI = (RadioButton)win.FindName(rb2.Name);
                interpreter.controlsToCollapseAfterAnimation.AddLast(rbOnUI);

            }

            Grid parent = new Grid();
            parent.Name = cb.Name + "Parent";
            parent.Children.Add(cb);
            win.RegisterName(cb.Name, cb);
            win.RegisterName(parent.Name, parent);

            interpreter.insertControlIntoGrid(grid, parent, row, column, insertRow, insertColumn);
            interpreter.controlsToCollapseBeforeAnimation.AddLast(cb);
            interpreter.addObjectAnimation(((Grid)cb.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
            interpreter.addObjectAnimation(cb.Name, Visibility.Visible, UIElement.VisibilityProperty, 2);
            interpreter.addDoubleAnimation(cb.Name, 0, 1, Control.OpacityProperty, 2);
            interpreter.addObjectAnimation(((Grid)cb.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
            win.UpdateLayout();
        }

        public LinkedList<RadioButton> getRadioButtonsFromGroupName(Grid root, String name)
        {
            LinkedList<RadioButton> res = new LinkedList<RadioButton>();

            foreach (FrameworkElement fe in root.Children)
            {
                if (fe is Grid)
                {
                    foreach (RadioButton rb in getRadioButtonsFromGroupName((Grid)fe, name))
                    {
                        res.AddLast(rb);
                    }
                }
                else
                {
                    if (fe is RadioButton)
                    {
                        if (((RadioButton)fe).GroupName.Equals(name))
                            res.AddLast((RadioButton)fe);
                    }
                }

            }

            return res;
        }
    }
}
