using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.Lexer;

namespace Animator.LL1Parser
{
    public class Grammar 
    {
	    public static Terminal lambda;
        public static Identifier identifier;
        public static Constant constant;
        public static MyString myString;
        public static ControlType control;
        public static All all;
        public static Selector selector;
        public static MyProperty myProperty;
        public static NotTerminal program;

        private LinkedList<Rule> rules;
        private LinkedList<Rule> extendedRules;
        private LinkedList<Terminal> terminalList;
        private LinkedList<NotTerminal> notTerminalList;
        private Dictionary<NotTerminal, NotTerminal> extendedSymbols = new Dictionary<NotTerminal, NotTerminal>();

        public Grammar(LinkedList<Terminal> t, LinkedList<NotTerminal> nt, LinkedList<Rule> r)
	    {
		    terminalList = t;
		    notTerminalList = nt;
		    rules = r;
	    }

        public static void initGrammar()
        {
            lambda = new Terminal("lambda");
            identifier = new Identifier("identifier");
            constant = new Constant("constant");
            myString = new MyString("string");
            control = new ControlType("control");
            selector = new Selector("selector");
            myProperty = new MyProperty("property");
            all = new All();
            program = new NotTerminal("PROGRAM");
        }
	
	    public void ComputeExtendedRules()
	    {	
		    Dictionary<String, NotTerminal> notTerminalNames = new Dictionary<String, NotTerminal>();
		
		    // Program' -> lambda
		    LinkedList<Symbol> right = new LinkedList<Symbol>();
		    right.AddLast(lambda);
		    NotTerminal program2 = new NotTerminal(Grammar.program.GetText()+"'");
		    extendedRules = new LinkedList<Rule>(rules);
		    extendedRules.AddLast(new Rule(program2, right));
		    // Program est son propre symbole étendu
		    extendedSymbols.Add(program, program2);
		

		    // Pour chaque règle, on va créer un symbole étendu
		    // du symbole de gauche.
		    // Y  -> alpha X beta
		    // X2 -> beta Y2
		    foreach(Rule p in rules)
		    {
			    right = new LinkedList<Symbol>(p.Right());
			    NotTerminal left = p.Left();
                //System.Console.WriteLine("Rule - " + p);
			    while(right.Count()>0)
			    {
				    Symbol symbol = right.First();
                    right.RemoveFirst();
				
				    if(symbol is NotTerminal)
				    {
					    NotTerminal X = (NotTerminal)symbol;
					
					    // Si on a une règle du style X' -> X', on ne l'ajoute pas
					    if(X == left && right.Count()<1) 
						    continue;
					
					    String X2_Name = X.GetText()+"'";
					    String Y2_Name = left.GetText()+"'";

                        NotTerminal X2;
                        if (!notTerminalNames.ContainsKey(X2_Name))
					    {
						    // Le non terminal étendu X' n'existe pas, on le crée
						    X2 = new NotTerminal(X2_Name);
						    // On l'ajoute à la map qui établit la relation name <--> symbole
						    notTerminalNames.Add(X2_Name, X2);
						    // On ajoute ce nouveau non terminal à la liste des non terminaux
						    notTerminalList.AddLast(X2);
					    }else{
                            X2 = notTerminalNames[X2_Name];    
                        }

					    NotTerminal Y2;
                        if (!notTerminalNames.ContainsKey(Y2_Name))
					    {
						    // Le non terminal étendu X' n'existe pas, on le crée
						    Y2 = new NotTerminal(Y2_Name);
						    // On l'ajoute à la map qui établit la relation name --> symbole
						    notTerminalNames.Add(Y2_Name, Y2);
						    // On ajoute ce nouveau non terminal à la liste des non terminaux
						    notTerminalList.AddLast(Y2);
					    }else{
                            Y2 = notTerminalNames[Y2_Name];
                        }

					    // On crée la partie de droite de la règle définissant X2
					    LinkedList<Symbol> X2_Right = new LinkedList<Symbol>(right);
					    X2_Right.AddLast(Y2);
					    // On ajoute enfin la nouvelle règle créée
					    extendedRules.AddLast(new Rule(X2, X2_Right));
					    // On définit la relation Symbole --> Symbole étendu
                        //System.Console.WriteLine("NewRule - " + new Rule(X2, X2_Right));
                        //System.Console.WriteLine("Sym --> SymEtend - " + X + " -> " + X2);
                        if(!extendedSymbols.ContainsKey(X))
					        extendedSymbols.Add(X, X2);
				    }
			    }
		    }
	    }
	
	    public HashSet<Terminal> MergeSets(LinkedList<Symbol> l)
	    {
		    HashSet<Terminal> res = new HashSet<Terminal>();
		
		    foreach(Symbol s in l)
		    {
			    // Si l'ensemble P1 de "s" est vide c'est qu'il n'a pas encore été défini.
			    // On retourne un ensemble vide.
			    if(s.P1().Count()<1)
				    return new HashSet<Terminal>();
			
			    // Si l'ensemble P1 de "s" possède un lambda, on ajoute cet ensemble au résultat
			    // et on passe au symbole suivant. En effet, p1(s) renvoit les premiers terminaux 
			    // possibles. Si parmis ceux ci se trouvent un lambda, on va prendre en compte les
			    // éléments de p1(symbol) du symbol suivant.
			    if(s.P1().Contains(lambda))
				    res.UnionWith(s.P1());
			    // Il n'y a pas de lambda dans l'ensemble p1 de "s", on stoppe la boucle et 
			    // on enlève les lambda du résultat. Il y a un lambda dans le résutat que 
			    // si tous les ensembles P1 à droite d'une règle en possèdent un.
			    else
			    {
				    res.Remove(lambda);
				    res.UnionWith(s.P1());
				    break;
			    }
		    }
		    return res;
	    }
	
	    public void ComputeP1()
	    {
		    /*
		     * Pour cet algorithme, on va utiliser le concept d'itération du point fixe.
		     * C'est à dire que chaque ensemble P1 d'un symbole dépend des ensembles P1
		     * des symboles qui le dérivent. Etant donné qu'il n'y pas un ordre des symboles 
		     * pour calculer les ensembles, on tente de calculer tous les ensembles P1 de 
		     * tous les symboles, si les informations sont insuffisantes on ne fait rien et on
		     * passe au symbole suivant. Quand on a tenté de calculer les ensembles P1 
		     * de tous les symboles, on recommence l'opération jusqu'à ce que, petit à petit,
		     * tous les symboles soient calculés. 
		     */

		    // Avant de commencer, on définit la valeur des ensembles P1
		    // des terminaux. Ceux ci ont comme unique élément eux meme.
		    foreach(Terminal t in terminalList)
		    {	
			    t.P1().Add(t);
		    }
		
		    bool b = true;
		
		    while(b)
		    {	
			    b = false;
			
			    // Pour chaque symbole de gauche d'une règle, on va tenter de définir
			    // son ensemble P1.
			    foreach(Rule r in extendedRules)
			    {
                    int numElem = r.Left().P1().Count();
                    r.Left().P1().UnionWith(MergeSets(r.Right()));
				    if(numElem < r.Left().P1().Count())
					    b = true;
			    }
		    }
            /*foreach (Rule r in extendedRules)
            {
                System.Console.WriteLine("Rule: " + r);
                System.Console.Write("Left.P1: ");
                foreach (Terminal t in r.Left().P1())
                    System.Console.Write(t + " - ");
                System.Console.Write("\r\n");
            }*/
	    }
	
	    /**
	     * pré: CreateAugmentedGrammar d'abord
	     * @param s
	     */
	    public void ComputeS1()
	    {	
		    /*
		     * Rappel: En augmentant la grammaire, on a ajouté un
		     * certain nombre de règles de la manière suivante:
		     * Y  -> alpha X beta
		     * X2 -> beta Y2
		     * 
		     * Celà va permettre de connaitre les symboles pouvant
		     * apparaitre "après" chaque symbole présent "à la base".
		     * Le principe est simple, on définit l'ensemble s1 de S comme
		     * étant égal à l'ensemble p1 de S' (que l'on connait déjà). 
		     * 
		     */
            foreach (KeyValuePair<NotTerminal, NotTerminal> entry in extendedSymbols) 
			    entry.Key.S1().UnionWith(entry.Value.P1());
            /*foreach (Rule r in rules)
            {
                System.Console.WriteLine("Rule: " + r);
                System.Console.Write("Left.S1: ");
                foreach (Terminal t in r.Left().S1())
                    System.Console.Write(t + " - ");
                System.Console.Write("\r\n");
            }*/
	    }
	
	    public LinkedList<Rule> Rules() {return rules;}
        public LinkedList<Terminal> Terminals() { return terminalList; }
        public LinkedList<NotTerminal> NotTerminals() { return notTerminalList; }
    }
}
