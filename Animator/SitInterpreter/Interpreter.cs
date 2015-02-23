using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Reflection;
using System.Collections;
using Animator.Lexer;
using Animator.SitInterpreter.Selector;
using Animator.SitInterpreter.Substitutes;


namespace Animator.SitInterpreter
{
    class Interpreter
    {
        private Window win;
        public Storyboard sb;
        private int beginTime = 0;
        public LinkedList<FrameworkElement> controlsToCollapseAfterAnimation = new LinkedList<FrameworkElement>();
        public LinkedList<FrameworkElement> controlsToCollapseBeforeAnimation = new LinkedList<FrameworkElement>();
        private Dictionary<String, FrameworkElement> controlEvolutions = new Dictionary<string, FrameworkElement>();
        private Dictionary<String, Grid> gdParents = new Dictionary<string, Grid>();
        private Dictionary<String, ArrayList> widthsGrid = new Dictionary<String, ArrayList>();
        private Dictionary<String, ArrayList> heightsGrid = new Dictionary<String, ArrayList>();
        private Dictionary<String, int> propertyToAnim = new Dictionary<String, int>();
        public ArrayList animTimer;

        public Interpreter(Tree<Symbol> tree, Window w)
        {
            
            win = w;
            sb = new Storyboard();
            sb.Completed += new EventHandler(removeEmptyGrids);
            animTimer = new ArrayList();
            animTimer.Add(0);
            interpretProgram(tree);
        }

        public void interpretProgram(Tree<Symbol> tree)
        {
            foreach (Tree<Symbol> t in tree.Children())
            {
                Symbol s = t.Element();
                if (isStatement(s))
                    interpretStatement(t);
                else if (isProgram(s))
                    interpretProgram(t);
            }
        }

        public void interpretStatement(Tree<Symbol> tree)
        {
            Tree<Symbol> operation = tree.Child(0);
            Symbol s = operation.Element();

            if (isStmtContract(s))
                interpretStmtContract(operation);
            else if(isStmtRemove(s))
                interpretStmtRemove(operation);
            else if (isStmtChangeBox(s))
                interpretStmtChangeBox(operation);
            else if (isStmtExtend(s))
                interpretStmtExtend(operation);
            else if (isStmtChangeRows(s))
                interpretStmtChangeGridDefinition(operation, true);
            else if (isStmtChangeColumns(s))
                interpretStmtChangeGridDefinition(operation, false);
            else if (isStmtSubstitute(s))
                interpretStmtSubstitute(operation);
            else if (isStmtSet(s))
                interpretStmtSet(operation);
        }

        public void interpretStmtSubstitute(Tree<Symbol> tree)
        {
            String substituteType = tree.Child(3).Element().GetText();
            /*String type = tree.Child(1).Element().GetText(); ;

            if (type.Equals("RadioButtons"))
                substituteRadioButtons(tree);*/

             LinkedList<FrameworkElement> controls = getSelectedControls(tree.Child(1));
             Type typeControls = null;
             foreach (FrameworkElement selectedControl in controls)
             {
                 if (typeControls == null)
                     typeControls = selectedControl.GetType();
                 else
                 {
                     if (typeControls != selectedControl.GetType())
                         throw new InterpreterException("Un des controls sélectionnés par SUBSTITUTE n'a pas le même type que les autres");
                 }
             }

             //Console.WriteLine("Animator.SitInterpreter.Substitutes." + getType(typeControls) + "To" + substituteType);
             Type type = Type.GetType("Animator.SitInterpreter.Substitutes." + getType(typeControls) + "To" + substituteType);
             SubstituteAb s = (SubstituteAb)Activator.CreateInstance(type, new object[] { win, this });
             s.substitute(tree, controls);

        }

        public void interpretStmtSet(Tree<Symbol> tree)
        {
            LinkedList<FrameworkElement> controls = getSelectedControls(tree.Child(1));

            foreach (FrameworkElement selectedControl in controls)
            {
                String identifier = selectedControl.Name;
                String property = tree.Child(2).Element().GetText();
                dynamic value;
                Symbol valueTerminal = tree.Child(4).Child(0).Element();

                // détermine de quel type doit être la propriété à modifier
                if (valueTerminal is MyString)
                    value = valueTerminal.GetText();
                else if (valueTerminal is Constant)
                    value = Double.Parse(valueTerminal.GetText());
                else if (valueTerminal.GetText().Equals("BOOLEAN"))
                {
                    if (tree.Child(4).Child(0).Child(0).Element().GetText().Equals("true"))
                        value = true;
                    else
                        value = false;
                } 
                else
                    throw new InterpreterException("Impossible de déterminer le type de terminal pour SET (string, int ou boolean)");

                FrameworkElement controlEvo = getControlEvolutions(identifier);
                FrameworkElement controlCopy = CloneFrameWorkElement(controlEvo);
                FrameworkElement controlOnUI = (FrameworkElement)win.FindName(controlEvo.Name);

                if (!(controlEvo.Parent is Grid))
                    throw new InterpreterException("Seuls les controles dont le parent est un grid peuvent être modifiés.");

                // disparition ancien control
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addDoubleAnimation(controlEvo.Name, 1, 0, Control.OpacityProperty, 2);
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
                controlsToCollapseAfterAnimation.AddLast(controlEvo);

                // création et insertion du nouveau control avec la propriété modifiée
                DependencyObject dOParent = controlOnUI.Parent;

                Grid parent = (Grid)controlEvo.Parent;
                
                Grid parentOnUI = (Grid)controlOnUI.Parent;
                Grid parentCopy = (Grid)CloneFrameWorkElement(parent);
                parentCopy.Children.Add(controlCopy);
                Grid gdParent = gdParents[identifier];
                int indexColumn = Grid.GetColumn(parentOnUI);
                int indexRow = Grid.GetRow(parentOnUI);
                //Console.WriteLine("2 name: " + controlCopy.Name + " - gparent: " + ((Grid)gdParent).Name);
                insertControlIntoGrid(gdParent, parentCopy, indexRow, indexColumn, false, false);

                controlCopy.Opacity = 0;
                controlsToCollapseBeforeAnimation.AddLast(controlCopy);

                controlCopy.Name += "Bis";
                parentCopy.Name += "Bis";
                win.RegisterName(controlCopy.Name, controlCopy);
                win.RegisterName(parentCopy.Name, parentCopy);

                // modification propriété
                PropertyInfo pi = controlCopy.GetType().GetProperty(property);
                pi.SetValue(controlCopy, value, null);
    
                // affichage nouveau control avec nouvelle propriété
                addObjectAnimation(((Grid)controlCopy.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addObjectAnimation(controlCopy.Name, Visibility.Visible, UIElement.VisibilityProperty, 2);
                addDoubleAnimation(controlCopy.Name, 0, 1, Control.OpacityProperty, 2);
                addObjectAnimation(((Grid)controlCopy.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
                //Console.WriteLine("Nouveau control content: " + (string)(pi.GetValue(controlCopy, null)));
                controlEvolutions[identifier] = controlCopy;

                //double value = (double)(pi.GetValue(win, null));
            }
        }

        public void interpretStmtContract(Tree<Symbol> tree)
        {
            LinkedList<FrameworkElement> controls = getSelectedControls(tree.Child(1));

            foreach (FrameworkElement selectedControl in controls)
            {
                //String identifier = tree.Child(1).Element().GetText();
                String identifier = selectedControl.Name;
                int contractWidth = Int32.Parse(tree.Child(3).Element().GetText());
                int contractHeight = Int32.Parse(tree.Child(4).Element().GetText());

                FrameworkElement controlEvo = getControlEvolutions(identifier);

                double width = Double.IsNaN(controlEvo.Width) ? controlEvo.ActualWidth : controlEvo.Width;
                double height = Double.IsNaN(controlEvo.Height) ? controlEvo.ActualHeight : controlEvo.Height;

                //Console.WriteLine("Contract. Height avant: " + controlEvo.ActualHeight + " - après: " + (height - contractHeight));

                //addColorAnimation(controlEvo.Name, Colors.Red, 2);
                if(controlEvo.Parent!=null)
                    addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addDoubleAnimation(controlEvo.Name, width, width - contractWidth, Control.WidthProperty, 2);
                addDoubleAnimation(controlEvo.Name, height, height - contractHeight, Control.HeightProperty, 2);
                if (controlEvo.Parent != null)
                    addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);

                controlEvo.Width = width - contractWidth;
                controlEvo.Height = height - contractHeight;
            }
        }

        public FrameworkElement getControlEvolutions(String feName)
        {
            FrameworkElement fe2;

            if (controlEvolutions.ContainsKey(feName))
                fe2 = controlEvolutions[feName];
            else
            {
                fe2 = (FrameworkElement)win.FindName(feName);

                DependencyObject dOParent = fe2.Parent;

                if (dOParent is Grid)
                {
                    Grid parent = (Grid)fe2.Parent;
                    fe2 = CloneFrameWorkElement(fe2);
                    Grid parentCopy = (Grid)CloneFrameWorkElement(parent);
                    parentCopy.Children.Add(fe2);
                    gdParents[feName] = (Grid)parent.Parent;
                }
                else if (dOParent is ComboBox)
                {
                    ComboBox parent = (ComboBox)fe2.Parent;
                    fe2 = CloneFrameWorkElement(fe2);
                    ComboBox parentEvo = (ComboBox)getControlEvolutions(parent.Name);
                    parentEvo.Items.Add(fe2);
                }
                else if (dOParent is ListBox)
                {
                    ListBox parent = (ListBox)fe2.Parent;
                    fe2 = CloneFrameWorkElement(fe2);
                    ListBox parentEvo = (ListBox)getControlEvolutions(parent.Name);
                    parentEvo.Items.Add(fe2);
                }
                else if(fe2 is Window)
                {
                    Window w = new Window();
                    w.Height = ((Window)fe2).Height;
                    w.Width = ((Window)fe2).Width;
                    w.Name = fe2.Name;
                    fe2 = w;
                }
   

                controlEvolutions[feName] = fe2;
                //Console.WriteLine("1. name: " + feName + " - gparent: " + ((Grid)parent.Parent).Name);
                
            }
            return fe2;
        }

        public void interpretStmtExtend(Tree<Symbol> tree)
        {
            LinkedList<FrameworkElement> controls = getSelectedControls(tree.Child(1));

            foreach (FrameworkElement selectedControl in controls)
            {
                //String identifier = tree.Child(1).Element().GetText();
                String identifier = selectedControl.Name;
                int contractWidth = Int32.Parse(tree.Child(3).Element().GetText());
                int contractHeight = Int32.Parse(tree.Child(4).Element().GetText());

                FrameworkElement controlEvo = getControlEvolutions(identifier);

                double width = Double.IsNaN(controlEvo.Width) ? controlEvo.ActualWidth : controlEvo.Width;
                double height = Double.IsNaN(controlEvo.Height) ? controlEvo.ActualHeight : controlEvo.Height;

                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addDoubleAnimation(controlEvo.Name, width, width + contractWidth, Control.WidthProperty, 2);
                addDoubleAnimation(controlEvo.Name, height, height + contractHeight, Control.HeightProperty, 2);
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);

                controlEvo.Width = width + contractWidth;
                controlEvo.Height = height + contractHeight;
            }
        }

        public void interpretStmtChangeGridDefinition(Tree<Symbol> tree, bool isRow)
        {
            String identifier = tree.Child(1).Element().GetText();
            LinkedList<Int32> vals = new LinkedList<Int32>();

            Tree<Symbol> t = tree.Child(3);
            Symbol s = t.Child(0).Element();
            while (true)
            {
                vals.AddLast(Int32.Parse(s.GetText()));
                Tree<Symbol> t2 = t;
                if (t.Child(1).Child(0).Element() != Grammar.lambda)
                {
                    t = t.Child(1).Child(0);
                    s = t.Child(0).Element();
                }
                else
                    break;
            }

            Grid grid = (Grid)win.FindName(identifier);
            win.UpdateLayout();

            ArrayList heights = getOriginalDefSize(grid, true);
            ArrayList widths = getOriginalDefSize(grid, false);

            int i = 0;
            foreach (Int32 val in vals)
            {
                double oldV = isRow ?  (Double)heights[i] : (Double)widths[i];
                double newV = (double)val / 100;

                if (isRow) heights[i] = newV;
                else widths[i] = newV;

                addObjectAnimation(grid.Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addGridLengthAnimation( isRow ? (DefinitionBase)grid.RowDefinitions[i] : (DefinitionBase)grid.ColumnDefinitions[i],
                                        oldV,
                                        newV,
                                        isRow ? RowDefinition.HeightProperty : ColumnDefinition.WidthProperty,
                                        1);
                addObjectAnimation(grid.Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
                i++;
            }

        }

        public ArrayList getOriginalDefSize(Grid grid, bool isRow)
        {
            if(isRow && heightsGrid.ContainsKey(grid.Name))
                return heightsGrid[grid.Name];

            if (!isRow && widthsGrid.ContainsKey(grid.Name))
                return widthsGrid[grid.Name];

            int j = 0;
            double sum = 0;
            ArrayList res = new ArrayList();
            IEnumerable<DefinitionBase> defs = isRow ? (IEnumerable<DefinitionBase>)grid.RowDefinitions : (IEnumerable<DefinitionBase>)grid.ColumnDefinitions;

            foreach (DefinitionBase d in defs)
            {
                double value = isRow ? ((RowDefinition)d).Height.Value : ((ColumnDefinition)d).Width.Value;
                sum += value;
                res.Add(value);
                j++;
            }

            if (sum > 1)
            {
                j = 0;
                foreach (DefinitionBase def in defs)
                {
                    Grid tmpGrid = (Grid)getGridChildren(grid, j, false).ElementAt(0);
                    double tmpActual = isRow ? tmpGrid.ActualHeight : tmpGrid.ActualWidth;
                    double actual = isRow ? grid.ActualHeight : grid.ActualWidth;
                    //Console.WriteLine("Changement de " + res[j] + " à " + tmpActual/ actual);
                    res.Insert(j, tmpActual / actual);
                    j++;
                }

            }

            if (isRow)
                heightsGrid[grid.Name] = res;
            else
                widthsGrid[grid.Name] = res;

            return res;
        }


        public void addGridLengthAnimation(DefinitionBase control, double a, double b, Object property, int duration)
        {
            //Console.WriteLine("Val: " + a + " - " + b);
            GridLengthAnimation anim = new GridLengthAnimation();
            anim.From = new GridLength(a, GridUnitType.Star);
            anim.To = new GridLength(b, GridUnitType.Star);
            anim.Duration = TimeSpan.FromSeconds(duration);
            anim.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            if (duration > 0) animTimer.Add(beginTime);
            Storyboard.SetTarget(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));
            sb.Children.Add(anim);
        }

        public void interpretStmtChangeBox(Tree<Symbol> tree)
        {

            LinkedList<FrameworkElement> controls = getSelectedControls(tree.Child(1));

            foreach (FrameworkElement selectedControl in controls)
            {
                String identifier = selectedControl.Name;
                String identifierGrid = tree.Child(3).Element().GetText();

                FrameworkElement controlEvo = getControlEvolutions(identifier);
                FrameworkElement controlCopy = CloneFrameWorkElement(controlEvo);
                FrameworkElement controlOnUI = (FrameworkElement)win.FindName(controlEvo.Name);

                Grid parent = (Grid)controlEvo.Parent;
                Grid parentCopy = (Grid)CloneFrameWorkElement(parent);
                parentCopy.Children.Add(controlCopy);
                Grid gdParent = gdParents[identifier];

                controlCopy.Opacity = 0;
                controlsToCollapseBeforeAnimation.AddLast(controlCopy);

                controlCopy.Name += "Bis";
                parentCopy.Name += "Bis";
                win.RegisterName(controlCopy.Name, controlCopy);
                win.RegisterName(parentCopy.Name, parentCopy);

                // disparition du controle à l'ancien placement
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addDoubleAnimation(controlEvo.Name, 1, 0, Control.OpacityProperty, 1);
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
                controlsToCollapseAfterAnimation.AddLast(controlOnUI);
                // apparition du nouveau control
                addObjectAnimation(((Grid)controlCopy.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addObjectAnimation(controlCopy.Name, Visibility.Visible, UIElement.VisibilityProperty, 2);
                addDoubleAnimation(controlCopy.Name, 0, 1, Control.OpacityProperty, 2);
                addObjectAnimation(((Grid)controlCopy.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);

                // Ajout controlCopy dans le grid de destination
                int row = Int32.Parse(tree.Child(6).Element().GetText()); ;
                int column = Int32.Parse(tree.Child(9).Element().GetText()); ;
                bool insertRow = tree.Child(5).Child(0).Element().GetText().Equals("ROW") ? false : true;
                bool insertColumn = tree.Child(8).Child(0).Element().GetText().Equals("COL") ? false : true;

                Grid grid = (Grid)win.FindName(identifierGrid);
                insertControlIntoGrid(grid, parentCopy, row, column, insertRow, insertColumn);

                controlEvolutions[identifier] = controlCopy;
                gdParents[identifier] = grid;
            }
        }

        public LinkedList<FrameworkElement> getControlByType(Grid root, String type)
        {
            LinkedList<FrameworkElement> res = new LinkedList<FrameworkElement>();

            foreach (FrameworkElement fe in root.Children)
            {
                if (fe is Grid)
                {
                    foreach (FrameworkElement rb in getControlByType((Grid)fe, type))
                    {
                        res.AddLast(rb);
                    }
                }
                else
                {
                    if ((type.Equals("all") && fe.GetType().ToString().Contains("System.Windows.Controls")) || fe.GetType().ToString().Equals("System.Windows.Controls." + type))
                    {
                        //Console.WriteLine("trouvé: " + fe.ToString());
                        res.AddLast(fe);
                    }
                }

            }

            return res;
        }

        public void insertControlIntoGrid(Grid grid, FrameworkElement control, int row, int column, bool insertRow, bool insertColumn)
        {
            if (insertRow)
            {
                
                if (grid.RowDefinitions.Count == 0)
                {
                    RowDefinition rowDef2 = new RowDefinition();
                    grid.RowDefinitions.Add(rowDef2);
                }

                RowDefinition rowDef = new RowDefinition();
                grid.RowDefinitions.Add(rowDef);
                rowDef.Height = GridLength.Auto;
                moveRowDefinitionsForward(grid, row, true);
            }

            if (insertColumn)
            {
                if (grid.ColumnDefinitions.Count == 0)
                {
                    ColumnDefinition colDef2 = new ColumnDefinition();
                    grid.ColumnDefinitions.Add(colDef2);
                }

                ColumnDefinition colDef = new ColumnDefinition();
                grid.ColumnDefinitions.Add(colDef);
                colDef.Width = GridLength.Auto;
                moveRowDefinitionsForward(grid, column, false);
            }
            
            grid.Children.Add(control);
            Grid.SetColumn(control, column);
            Grid.SetRow(control, row);
        }


        public void moveRowDefinitionsForward(Grid grid, int index, bool isRow)
        {
            IEnumerable<DefinitionBase> defs = isRow ? (IEnumerable<DefinitionBase>)grid.RowDefinitions : (IEnumerable<DefinitionBase>)grid.ColumnDefinitions;
            for (int i = defs.Count()-2; i >= index; i--)
            {
                foreach (FrameworkElement fe in getGridChildren(grid, i, isRow))
                {
                    if(!(fe is Grid))
                        continue;

                    if(isRow)
                        Grid.SetRow(fe, i+1);
                    else
                        Grid.SetColumn(fe, i+1);
                }
            }
        }

        public LinkedList<FrameworkElement> getSelectedControls(Tree<Symbol> tree)
        {
            Terminal t = (Terminal)tree.Child(0).Element();

            LinkedList<FrameworkElement> res = new LinkedList<FrameworkElement>(); 
            if (t is Identifier)
            {
                res.AddLast((FrameworkElement)win.FindName(t.GetText()));
            }else if(t is ControlType || t is All)
            {
                
                LinkedList<FrameworkElement> controls = getControlByType((Grid)win.Content, t.GetText());
                foreach (FrameworkElement fe in controls)
                {
                    if (controlEvolutions.ContainsKey(fe.Name) || (!controlEvolutions.ContainsKey(fe.Name) && !controlEvolutions.ContainsValue(fe)))
                        res.AddLast(fe);
                }
            }
            else if (t is Lexer.Selector)
            {
                Terminal param = (Terminal)tree.Child(1).Child(0).Element();
                Type type = Type.GetType("Animator.SitInterpreter.Selector." + t.GetText());
                SelectorAb s = (SelectorAb)Activator.CreateInstance(type, new object[] { param.GetText() });
                //Console.WriteLine(s.options);
                LinkedList<FrameworkElement> controls = s.getControls((Grid)win.Content);
                foreach (FrameworkElement fe in controls)
                {

                    if (controlEvolutions.ContainsKey(fe.Name) || (!controlEvolutions.ContainsKey(fe.Name) && !controlEvolutions.ContainsValue(fe)))
                    {
                        //Console.WriteLine(fe.Name);
                        res.AddLast(fe);
                    }
                }
            }

            return res;
        }

        public void interpretStmtRemove(Tree<Symbol> tree)
        {
            LinkedList<FrameworkElement> controls = getSelectedControls(tree.Child(1));

            foreach (FrameworkElement control in controls)
            {
                String identifier = control.Name;
                FrameworkElement controlEvo = getControlEvolutions(identifier);
                FrameworkElement controlOnUI = (FrameworkElement)win.FindName(controlEvo.Name);
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.GreenYellow), Grid.BackgroundProperty, 0);
                addDoubleAnimation(controlEvo.Name, 1, 0, Control.OpacityProperty, 1);
                addObjectAnimation(((Grid)controlEvo.Parent).Name, new SolidColorBrush(Colors.White), Grid.BackgroundProperty, 0);
                controlsToCollapseAfterAnimation.AddLast(controlOnUI);
            }
        }



        private FrameworkElement CloneFrameWorkElement(FrameworkElement o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                //Console.WriteLine("Propriété: " + pi);
                
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            // Items n'est pas modifiable
            if (o is ListBox)
            {
                ListBox lb = (ListBox)o;
                foreach (Object lbi in lb.Items)
                {
                    if(lbi is ListBoxItem)
                        ((ListBox)p).Items.Add(((ListBoxItem)lbi).Content);
                    else
                        ((ListBox)p).Items.Add(((String)lbi));
                }
            }
            else if (o is ComboBox)
            {
                ComboBox cb = (ComboBox)o;
                foreach (Object cbi in cb.Items)
                {
                    if (cbi is ComboBoxItem)
                        ((ComboBox)p).Items.Add(((ComboBoxItem)cbi).Content);
                    else
                        ((ComboBox)p).Items.Add(((String)cbi));
                }
            }

            return (FrameworkElement)p;
        }

        public void addObjectAnimation(String control, Object value, Object property, int duration)
        {
            ObjectAnimationUsingKeyFrames anim = new ObjectAnimationUsingKeyFrames();
            DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame();
            keyFrame.Value = value;
            keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
            anim.KeyFrames.Add(keyFrame);
            anim.BeginTime =TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            if (duration > 0) animTimer.Add(beginTime);
            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));
            sb.Children.Add(anim);
        }

        public void addThicknessAnimation(String control, double a, double b, double c, double d, int duration)
        {
            ThicknessAnimationUsingKeyFrames anim = new ThicknessAnimationUsingKeyFrames();
            EasingThicknessKeyFrame edkf = new EasingThicknessKeyFrame();
            edkf.Value = new Thickness(a, b, c, d);
            edkf.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
            anim.KeyFrames.Add(edkf);
            anim.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            if (duration > 0) animTimer.Add(beginTime);
            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Control.MarginProperty));
            sb.Children.Add(anim);
        }

        public void addColorAnimation(String control, Color value, int duration)
        {
            ColorAnimationUsingKeyFrames anim = new ColorAnimationUsingKeyFrames();
            EasingColorKeyFrame eckf = new EasingColorKeyFrame();
            eckf.Value = value;
            eckf.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
            anim.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            if (duration > 0) animTimer.Add(beginTime);
            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(SolidColorBrush.ColorProperty));
            sb.Children.Add(anim);
        }

        public void addStringAnimation(String control, String value, Object property, int duration)
        {
            StringAnimationUsingKeyFrames anim = new StringAnimationUsingKeyFrames();
            DiscreteStringKeyFrame edkf = new DiscreteStringKeyFrame();
            edkf.Value = value;
            edkf.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
            anim.KeyFrames.Add(edkf);
            anim.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            if (duration > 0) animTimer.Add(beginTime);
            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));
            sb.Children.Add(anim);
        }

        public String getType(Type t)
        {
            string type = t.ToString();
            String[] tmp = type.Split('.');
            return tmp[tmp.Count() - 1];
        }

        public void removeEmptyGrids(object sender, EventArgs e)
        {
            collapseRemovedControls();
            Grid root = (Grid)win.Content;
            Size size = removeEmptyGridsRec(root);

            //Console.WriteLine(size.width + " - " + size.height);
            //sb = new Storyboard();
            //beginTime = 0;
            //addDoubleAnimation(win.Name, win.Height, win.Height - size.height, Window.HeightProperty, 4);
            //addDoubleAnimation(win.Name, win.Width, win.Width - size.width, Window.WidthProperty, 4);
            //win.Height = win.Height - size.height;
            //win.Width = win.Width - size.width;
            //sb.Begin(win);
        }

        public struct Size
        {
            public Size(int h, int w) { height = h; width = w; }
            public double height;
            public double width;
        }

        public Size removeEmptyGridsRec(Grid g)
        {
            //Console.WriteLine("Check grid: " + g.Name);
            Size size = new Size(0, 0);
            for (int i = 0; i < g.RowDefinitions.Count; i++)
            {
                bool auto = true;
                foreach (FrameworkElement fe in getGridChildren(g, i, true))
                {
                    
                    if (!(fe is Grid))
                    {
                        auto = false;
                        break;
                    }
                    Grid g2 = (Grid)fe;
                    if (!isGridEmpty(g2))
                    {
                        auto = false;
                        break;
                    } 
                }
                if (auto)
                {
                    //Console.WriteLine("Height auto pour la row: " + i);
                    win.UpdateLayout();
                    size.height += g.RowDefinitions.ElementAt(i).ActualHeight - 5;
                    g.RowDefinitions.ElementAt(i).Height = GridLength.Auto;
                    
                }
            }

            for (int i = 0; i < g.ColumnDefinitions.Count; i++)
            {
                bool auto = true;
                foreach (FrameworkElement fe in getGridChildren(g, i, false))
                {
                    if (!(fe is Grid))
                    {
                        auto = false;
                        break;
                    }

                    Grid g2 = (Grid)fe;
                    if (!isGridEmpty(g2))
                    {
                        auto = false;
                        break;
                    }
                }
                if (auto)
                {
                    //Console.WriteLine("Width auto pour la column: " + i);
                    win.UpdateLayout();
                    size.width += g.ColumnDefinitions.ElementAt(i).ActualWidth - 5;
                    g.ColumnDefinitions.ElementAt(i).Width = GridLength.Auto;
                }
            }

            foreach (DependencyObject ddo in g.Children)
            {
                if (ddo is Grid)
                {
                    removeEmptyGridsRec((Grid)ddo);
                }
            }

            return size;
        }

        public bool isGridEmpty(Grid root)
        {
            bool empty = true; 
            foreach (FrameworkElement fe in root.Children)
            {
                if (!(fe is Grid))
                {
                    empty = false;
                    break;
                }

                Grid g = (Grid)fe;
                empty = isGridEmpty(g);

                if (!empty)
                {
                    break;
                }
            }
            
            return empty;
        }
       
        public static IEnumerable<FrameworkElement> getGridChildren(Grid grid, int a, bool isRow)
        {
            int count = VisualTreeHelper.GetChildrenCount(grid);
            for (int i = 0; i < count; i++)
            {
                FrameworkElement child = (FrameworkElement)VisualTreeHelper.GetChild(grid, i);
                int r;
                if(isRow)
                    r = Grid.GetRow(child);
                else
                    r = Grid.GetColumn(child);

                if (r == a)
                {
                    yield return child;
                }
            }
        }

        private void collapseRemovedControls()
        {
            foreach (FrameworkElement fe in controlsToCollapseAfterAnimation)
            {
                fe.Visibility = Visibility.Collapsed;
                DependencyObject p2 = null;
                DependencyObject doParent = fe.Parent;
                if (doParent is Grid)
                {
                    Grid parent = (Grid)doParent;
                    parent.Children.Remove(fe);
                    p2 = parent.Parent;
                    //Console.WriteLine("Remove de: " + fe.Name + " provenant de: " + parent.Name);
                    if (p2 != null)
                    {
                        Grid parent2 = (Grid)p2;
                        parent2.Children.Remove((UIElement)doParent);
                        //Console.WriteLine("Remove de: " + parent.Name + " provenant de: " + parent2.Name);
                    }
                }
                else if (doParent is ComboBox)
                {
                    ComboBox parent = (ComboBox)doParent;
                    //Console.WriteLine("Remove de: " + fe.Name + " provenant de: " + parent.Name);   
                    parent.Items.Remove(fe);
                    p2 = parent.Parent;
                }
            }
        }

        public void addDoubleAnimation(String control, double a, double b, Object property, int duration)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = a;
            anim.To = b;
            anim.Duration = TimeSpan.FromSeconds(duration);
            anim.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            if (duration > 0) animTimer.Add(beginTime);
            Storyboard.SetTargetName(anim, control);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));
            sb.Children.Add(anim);
        }

        public void runAnimation()
        {
            // on cache les nouveaux controls qui apparaitront dans l'animation
            // on ne les cache pas avant pour ne pas modifier leurs propriétés 
            // avant que toute l'animation ne soit spécifiée
            foreach (FrameworkElement fe in controlsToCollapseBeforeAnimation)
            {
                fe.Visibility = Visibility.Collapsed;
            }

            sb.Begin(win, true);
        }

        public Boolean isStmtSubstitute(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTSUBSTITUTE"));
        }

        public Boolean isStmtChangeBox(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTCHANGEBOX"));
        }

        public Boolean isStmtRemove(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTREMOVE"));
        }

        public Boolean isStmtSet(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTSET"));
        }

        public Boolean isStmtExtend(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTEXTEND"));
        }

        public Boolean isStmtMove(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTMOVE"));
        }

        public Boolean isStmtContract(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTCONTRACT"));
        }

        public Boolean isStatement(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STATEMENT"));
        }

        public Boolean isProgram(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("PROGRAM"));
        }

        public Boolean isName(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("NAME"));
        }

        public Boolean isControl(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("CONTROL"));
        }

        public Boolean isValue(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("VALUE"));
        }

        public Boolean isMove(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("MOVE"));
        }

        public Boolean isBoolean(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("BOOLEAN"));
        }

        public Boolean isStmtChangeRows(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTCHANGEROWS"));
        }

        public Boolean isStmtChangeColumns(Symbol s)
        {
            return (s is NotTerminal && s.GetText().Equals("STMTCHANGECOLUMNS"));
        }
    }
}
