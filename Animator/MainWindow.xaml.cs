using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Animator.LL1Parser;
using System.Xml.Xsl;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using Animator.SitInterpreter;
using Microsoft.Win32;
using System.Reflection;
using Animator.SitInterpreter.Selector;
using System.Threading;
using System.Windows.Media.Animation;
using System.Timers;



namespace Animator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        bool isUsi = false;
        bool isSit = false;
        String usiPath;
        String sitPath;
        Window win;
        Window win1;
        Window win2;
        Interpreter inter;
        System.Timers.Timer clock;
        public static int elapsedTime = 0;

		public MainWindow()
		{
			this.InitializeComponent();
            //directLoad();
            //Type type = typeof(SelectName);
            /*Type type = Type.GetType("Animator.SitInterpreter.Selector.SelectName");
            SelectorAb s = (SelectorAb)Activator.CreateInstance(type, new object[] { "test" });
            Console.WriteLine(s.options);*/
            /*Type listType = typeof(List<string>);
            List<string> instance = (List<string>)Activator.CreateInstance(listType);*/
		}

        public void directLoad()
        {
            try
            {
                String path = "C:\\Users\\charly\\Documents\\Visual Studio 2010\\Projects\\Animator\\Animator\\";
                //String path = "C:\\Users\\charly\\Documents\\Expression\\Blend 4\\Projects\\Animator\\Animator\\";

                //Parser test = new Parser(path + "Tests\\lex\\poll.sit", path + "SyntaxeConcrete.txt");
                //Parser test = new Parser(path + "Tests\\lex\\pollTest.sit", path + "SyntaxeConcrete.txt");
                //Parser test = new Parser(path + "Tests\\lex\\box.sit", path + "SyntaxeConcrete.txt");
                Parser test = new Parser(path + "Tests\\Final\\Interface1.sit", path + "SyntaxeConcrete.txt");
                //Parser test = new Parser(path + "Tests\\Final\\Interface2.sit", path + "SyntaxeConcrete.txt");

                XslTransform xsltransform = new XslTransform();
                xsltransform.Load(path + "Tests\\xsl\\test2\\Transform.xslt");
                //xsltransform.Transform(path + "Tests\\xsl\\test2\\Box.usi", path + "Tests\\xsl\\test2\\Box.xaml", null);
                //xsltransform.Transform(path + "Tests\\xsl\\test2\\Poll.usi", path + "Tests\\xsl\\test2\\Poll.xaml", null);
                //xsltransform.Transform(path + "Tests\\xsl\\test2\\PollTest.usi", path + "Tests\\xsl\\test2\\PollTest.xaml", null);
                xsltransform.Transform(path + "Tests\\Final\\Interface1.usi", path + "Tests\\Final\\Interface11.xaml", null);
                //xsltransform.Transform(path + "Tests\\Final\\Interface2.usi", path + "Tests\\Final\\Interface21.xaml", null);

                XslTransform xsltransform2 = new XslTransform();
                xsltransform2.Load(path + "Tests\\xsl\\test2\\Transform2.xslt");
                //xsltransform2.Transform(path + "Tests\\xsl\\test2\\Box.xaml", path + "Tests\\xsl\\test2\\Box2.xaml", null);
                //xsltransform2.Transform(path + "Tests\\xsl\\test2\\Poll.xaml", path + "Tests\\xsl\\test2\\Poll2.xaml", null);
                //xsltransform2.Transform(path + "Tests\\xsl\\test2\\PollTest.xaml", path + "Tests\\xsl\\test2\\PollTest2.xaml", null);
                xsltransform2.Transform(path + "Tests\\Final\\Interface11.xaml", path + "Tests\\Final\\Interface12.xaml", null);
                //xsltransform2.Transform(path + "Tests\\Final\\Interface21.xaml", path + "Tests\\Final\\Interface22.xaml", null);

                //StreamReader stringReader = new StreamReader(path + "Tests\\xsl\\test2\\Box2.xaml");
                //StreamReader stringReader = new StreamReader(path + "Tests\\xsl\\test2\\Poll2.xaml");
                //StreamReader stringReader = new StreamReader(path + "Tests\\xsl\\test2\\PollTest2.xaml");
                StreamReader stringReader = new StreamReader(path + "Tests\\Final\\Interface12.xaml");
                //StreamReader stringReader = new StreamReader(path + "Tests\\Final\\Interface22.xaml");
                XmlReader xmlReader = XmlReader.Create(stringReader);
                win = (Window)XamlReader.Load(xmlReader);
                win.Show();
                /*PropertyInfo pi = win.GetType().GetProperty("Height");
                pi.SetValue(win, (Double)1500, null);
                double value = (double)(pi.GetValue(win, null));
                Console.WriteLine(value);*/
                win.Topmost = true;

                inter = new Interpreter(test.Parse(), win);

                if (clock != null) clock.Stop();
                clock = new System.Timers.Timer(1000);
                clock.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                clock.Start();

                inter.runAnimation();

                /*Button btnOk = (Button)win.FindName(inter.getControlEvolutions("button_1").Name);

                if (btnOk != null)
                    btnOk.Click += new RoutedEventHandler(button1_Click);*/
            }
            catch (ParserException e)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Parser: " + e.Message+ "\n");
            }catch(InterpreterException e)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Interpreter: " + e.Message + "\n");
            }
            catch (Exception e)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Erreur inconnue: " + e.Message + "\n");
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("OK");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.Filter = "UsiXML (*.usi)|*.usi";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBlock lab = (TextBlock)this.FindName("usiLabel");
                lab.Text = "OK";
                Button butSitFile = (Button)this.FindName("butSitFile");
                butSitFile.IsEnabled = true;
                isUsi = true;
                usiPath = dlg.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.Filter = "Sit (*.sit)|*.sit";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBlock lab = (TextBlock)this.FindName("sitLabel");
                lab.Text = "OK";
                Button butLaunchAnim = (Button)this.FindName("butLaunchAnim");
                butLaunchAnim.IsEnabled = true;
                isSit = true;
                sitPath = dlg.FileName;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("enter");
            if (!isUsi || !isSit)
                return;
            //Console.WriteLine("enter2");

            try
            {
                Parser test = new Parser(sitPath, "SyntaxeConcrete.txt");

                XslTransform xsltransform = new XslTransform();
                xsltransform.Load("Transform.xslt");
                String xaml = "1.xaml";
                xsltransform.Transform(usiPath, xaml, null);

                XslTransform xsltransform2 = new XslTransform();
                xsltransform2.Load("Transform2.xslt");
                String xaml2 = "2.xaml";
                xsltransform2.Transform(xaml, xaml2, null);

                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Fichier UsiXML transformé en XAML." + "\n");

                StreamReader stringReader = new StreamReader(xaml2);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                if (win != null)
                    win.Close();
                win = (Window)XamlReader.Load(xmlReader);
                stringReader.Close();
                win.Show();
                win.Topmost = true;

                textDebug.AppendText("Interface XAML chargée." + "\n");

                Tree<Symbol> parse = test.Parse();

                textDebug.AppendText("Analyse syntaxique du fichier Sit terminée." + "\n");

                inter = new Interpreter(parse, win);

                textDebug.AppendText("Animation créée." + "\n");

                if (clock != null) clock.Stop();
                clock = new System.Timers.Timer(1000);
                elapsedTime = 0;
                clock.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                clock.Start();
                
                inter.runAnimation();

                Button butPrec = (Button)this.FindName("butPrec");
                butPrec.IsEnabled = true;
                Button butPlay = (Button)this.FindName("butPlay");
                butPlay.IsEnabled = true;
                Button butNext = (Button)this.FindName("butNext");
                butNext.IsEnabled = true;
            }
            catch (ParserException e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Parseur: " + e2.Message + "\n");
            }
            catch (InterpreterException e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Interpreteur: " + e2.Message + "\n");
            }
            catch (Exception e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Erreur inconnue: " + e2.Message + "\n");
            }
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            Button butPlay = (Button)this.FindName("butPlay");
            if (butPlay.Content.Equals("Pause"))
            {
                inter.sb.Pause(win);
                clock.Stop();
                butPlay.Content = "Play";
                Button butPrec = (Button)this.FindName("butPrec");
                butPrec.IsEnabled = false;
                Button butNext = (Button)this.FindName("butNext");
                butNext.IsEnabled = false;
            }
            else
            {
                inter.sb.Resume(win);
                clock.Start();
                butPlay.Content = "Pause";
                Button butPrec = (Button)this.FindName("butPrec");
                butPrec.IsEnabled = true;
                Button butNext = (Button)this.FindName("butNext");
                butNext.IsEnabled = true;
            }

        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            int offset = 0;
            for(int i = 0; i<inter.animTimer.Count;i++)
            {
                if (elapsedTime > (int)inter.animTimer[i])
                {
                    if (i + 2 >= inter.animTimer.Count)
                        offset = (int)inter.animTimer[i];
                    else
                    {
                        offset = (int)inter.animTimer[i + 2];
                    }
                }
            }
            //Console.WriteLine("Elapsed time: " + elapsedTime + " - offset: " +  offset);
            elapsedTime = offset;
            inter.sb.Seek(win, TimeSpan.FromSeconds(offset), TimeSeekOrigin.BeginTime);
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            elapsedTime++;
            //Console.WriteLine("The Elapsed event was raised at {0}", elapsedTime);
        }

        private void butPrec_Click(object sender, RoutedEventArgs e)
        {
            int offset = 0;
            for (int i = 0; i < inter.animTimer.Count; i++)
            {
                if (elapsedTime > (int)inter.animTimer[i])
                {
                    if (i - 1 <= 0)
                        offset = (int)inter.animTimer[i];
                    else
                    {
                        offset = (int)inter.animTimer[i - 2];
                    }
                }
            }
            //Console.WriteLine("Elapsed time: " + elapsedTime + " - offset: " + offset);
            elapsedTime = offset;
            inter.sb.Seek(win, TimeSpan.FromSeconds(offset), TimeSeekOrigin.BeginTime);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                XslTransform xsltransform = new XslTransform();
                xsltransform.Load("Transform.xslt");
                xsltransform.Transform("Interface1.usi", "Interface11.xaml", null);
                XslTransform xsltransform2 = new XslTransform();
                xsltransform2.Load("Transform2.xslt");
                xsltransform2.Transform("Interface11.xaml", "Interface12.xaml", null);
                StreamReader stringReader = new StreamReader("Interface12.xaml");
                XmlReader xmlReader = XmlReader.Create(stringReader);
                win1 = (Window)XamlReader.Load(xmlReader);
                win1.Show();
                win1.Topmost = true;

                xsltransform.Load("Transform.xslt");
                xsltransform.Transform("Interface2.usi", "Interface21.xaml", null);
                xsltransform2 = new XslTransform();
                xsltransform2.Load("Transform2.xslt");
                xsltransform2.Transform("Interface21.xaml", "Interface22.xaml", null);
                stringReader = new StreamReader("Interface22.xaml");
                xmlReader = XmlReader.Create(stringReader);
                win2 = (Window)XamlReader.Load(xmlReader);
                win2.Show();
                win2.Topmost = true;

                Button buttonLancerEx3 = (Button)this.FindName("lancerex3");
                buttonLancerEx3.IsEnabled = true;

                ListBox lb = (ListBox)win1.FindName("listbox_component_19");

                if (lb != null)
                    lb.SelectionChanged += new SelectionChangedEventHandler(ListBox1_SelectionChanged);
            }
            catch (ParserException e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Parser: " + e2.Message + "\n");
            }
            catch (InterpreterException e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Interpreter: " + e2.Message + "\n");
            }
            catch (Exception e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Erreur inconnue: " + e2.Message + "\n");
                System.Console.WriteLine(e2.Message);
                System.Console.WriteLine(e2.StackTrace);
            }
        }

        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ListBox lb = (ListBox)win1.FindName("listbox_component_19");
            String name = (string)(((ListBoxItem)lb.SelectedValue).Content);

            TextBox nom = (TextBox)win2.FindName("nomA");
            RadioButton rbHomme = (RadioButton)win2.FindName("radiobuttonHomme");
            RadioButton rbFemme = (RadioButton)win2.FindName("radiobuttonFemme");
            RadioButton rbAucun = (RadioButton)win2.FindName("radiobuttonAucun");
            RadioButton rbOui = (RadioButton)win2.FindName("radiobuttonOui");
            RadioButton rbNon = (RadioButton)win2.FindName("radiobuttonNon");
            TextBox nationalite = (TextBox)win2.FindName("nationaliteA");

            if (name.Equals("Marc"))
            {
                nom.Text = "Marc";
                rbHomme.IsChecked = true;
                rbFemme.IsChecked = false;
                rbAucun.IsChecked = false;
                rbOui.IsChecked = true;
                rbNon.IsChecked = false;
                nationalite.Text = "Francaise";
            }
            else if (name.Equals("Charles"))
            {
                nom.Text = "Charles";
                rbHomme.IsChecked = true;
                rbFemme.IsChecked = false;
                rbAucun.IsChecked = false;
                rbOui.IsChecked = false;
                rbNon.IsChecked = true;
                nationalite.Text = "Belge";
            }
            else if (name.Equals("Francois"))
            {
                nom.Text = "Francois";
                rbHomme.IsChecked = true;
                rbFemme.IsChecked = false;
                rbAucun.IsChecked = false;
                rbOui.IsChecked = true;
                rbNon.IsChecked = false;
                nationalite.Text = "Francaise";
            }
            else if (name.Equals("Elise"))
            {
                nom.Text = "Elise";
                rbHomme.IsChecked = false;
                rbFemme.IsChecked = true;
                rbAucun.IsChecked = false;
                rbOui.IsChecked = false;
                rbNon.IsChecked = true;
                nationalite.Text = "Francaise";
            }
            else if (name.Equals("Eleonore"))
            {
                nom.Text = "Eleonore";
                rbHomme.IsChecked = false;
                rbFemme.IsChecked = false;
                rbAucun.IsChecked = true;
                rbOui.IsChecked = false;
                rbNon.IsChecked = true;
                nationalite.Text = "Turc";
            }
        }

        private void newComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ComboBox lb = (ComboBox)win1.FindName("newComboBox");
            String name = (string)(lb.SelectedValue);

            TextBox nom = (TextBox)win2.FindName("nomA");
            ComboBox type = (ComboBox)win2.FindName("newComboBoxType");
            CheckBox etudiant = (CheckBox)win2.FindName("newCheckBoxEtudiant");
            TextBox nationalite = (TextBox)win2.FindName("nationaliteA");

            if (name.Equals("Marc"))
            {
                nom.Text = "Marc";
                type.SelectedIndex = 0;
                etudiant.IsChecked = true;
                nationalite.Text = "Francaise";
            }
            else if (name.Equals("Charles"))
            {
                nom.Text = "Charles";
                type.SelectedIndex = 0;
                etudiant.IsChecked = false;
                nationalite.Text = "Belge";
            }
            else if (name.Equals("Francois"))
            {
                nom.Text = "Francois";
                type.SelectedIndex = 0;
                etudiant.IsChecked = true;
                nationalite.Text = "Francaise";
            }
            else if (name.Equals("Elise"))
            {
                nom.Text = "Elise";
                type.SelectedIndex = 1;
                etudiant.IsChecked = false;
                nationalite.Text = "Francaise";
            }
            else if (name.Equals("Eleonore"))
            {
                nom.Text = "Eleonore";
                type.SelectedIndex = 2;
                etudiant.IsChecked = false;
                nationalite.Text = "Turc";
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Parser test1 = new Parser("Interface1.sit", "SyntaxeConcrete.txt");
                Interpreter inter1 = new Interpreter(test1.Parse(), win1);
                inter1.runAnimation();

                Parser test2 = new Parser("Interface2.sit", "SyntaxeConcrete.txt");
                Interpreter inter2 = new Interpreter(test2.Parse(), win2);
                inter2.runAnimation();

                ComboBox cb = (ComboBox)win1.FindName("newComboBox");

                cb.SelectionChanged += new SelectionChangedEventHandler(newComboBox_SelectionChanged);
            }
            catch (ParserException e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Parser: " + e2.Message + "\n");
            }
            catch (InterpreterException e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Interpreter: " + e2.Message + "\n");
            }
            catch (Exception e2)
            {
                if (win != null)
                    win.Close();
                RichTextBox textDebug = (RichTextBox)this.FindName("textDebug");
                textDebug.AppendText("Erreur inconnue: " + e2.Message + "\n");
                System.Console.WriteLine(e2.Message);
                System.Console.WriteLine(e2.StackTrace);
            }
            
        }

	}


}