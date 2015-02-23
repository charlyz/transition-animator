using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.Lexer;

namespace Animator.LL1Parser
{
    
    public class Parser 
    {
	    // Tableau à deux dimensions permettant de choisir une règle à executer
	    // en fonction du non terminal au dessus la pile et du terminal lu.
	    private Rule[,] parsingTable;
	    // Grammaire qui est utilisée pour accepter ou non le code source.
	    private Grammar G;
	    // Chemin du fichier contenant la spécification BNF de la grammaire.
        private String grammarPath;
	    // Chemin du fichier contenant le code source à parser.
	    private String inputPath;
	    // Analyseur lexical qui fournit au fur et à mesure chaque symbole du code source.
	    private Lex lex;
	    // Relation entre les objets Terminal du lexer et les objets Terminal de la grammaire
	    // pour permettre l'utilisation de la table de parsing
	    private Dictionary<Terminal, int> symbolToIndex = new Dictionary<Terminal, int>();
	    // Relation entre un non terminal et toutes les règles de dérivation de ce dernier.
	    private Dictionary<NotTerminal, HashSet<Rule>> allLeftNotTerminals;
	
	    /**
	     * Constructeur de la classe.
	     * @param s: Chemin du fichier source à parser.
	     */
	    public Parser(String s, String g)
        {
            inputPath = s;
            grammarPath = g;
            ReservedWord.initRW();
            SpecialChar.initSC();
            Grammar.initGrammar();
        }
	
	    /**
	     * Crée la relation "Objet Terminal du Lexer" -> "Index dans la parsing table"
	     * dans la HashMap SymbolToIndex
	     * 
	     * @post SymbolToIndex modifié
	     *
	     */
	    public void GenerateSymbolToIndex()
	    {
		    /*
		     * Il n'existe aucun lien entre les objets Terminal du lexer et les objets
		     * Terminal de la grammaire. Cette relation est importante pour pouvoir utiliser
		     * la table de parsing qui a été créée à partir des terminaux de la grammaire.
		     * 
		     * Etape 1: Mettre tous les terminaux provenant du Lexer dans une HashMap TerminalsLexer
		     * 
		     * Etape 2: Pour chaque terminal de TerminalsLexer, chercher son correspondant dans
		     * G.Terminals() pour connaitre son index dans la table de parsing
		     * 
		     */
		    Dictionary<String, Terminal> terminalsLexer = new Dictionary<String, Terminal>();

		    terminalsLexer.Add(Grammar.constant.GetText(), Grammar.constant);
		    terminalsLexer.Add(Grammar.identifier.GetText(), Grammar.identifier);
            terminalsLexer.Add(Grammar.myString.GetText(), Grammar.myString);
            terminalsLexer.Add(Grammar.control.GetText(), Grammar.control);
            terminalsLexer.Add(Grammar.selector.GetText(), Grammar.selector);
            terminalsLexer.Add(Grammar.all.GetText(), Grammar.all);
            terminalsLexer.Add(Grammar.myProperty.GetText(), Grammar.myProperty);
            terminalsLexer.Add(Grammar.lambda.GetText(), Grammar.lambda);
            terminalsLexer.Add("SUBSTITUTE", ReservedWord.RWSUBSTITUTE);
		    terminalsLexer.Add("BY", ReservedWord.RWBY);
		    terminalsLexer.Add("ROTATE", ReservedWord.RWROTATE);
		    terminalsLexer.Add("MOVE", ReservedWord.RWMOVE);
		    terminalsLexer.Add("OF", ReservedWord.RWOF);
		    terminalsLexer.Add("CONTRACT", ReservedWord.RWCONTRACT);
            terminalsLexer.Add("EXTEND", ReservedWord.RWEXTEND);
            terminalsLexer.Add("REMOVE", ReservedWord.RWREMOVE);
		    terminalsLexer.Add("left", ReservedWord.RWLEFT);
		    terminalsLexer.Add("top", ReservedWord.RWTOP);
		    terminalsLexer.Add("right", ReservedWord.RWRIGHT);
		    terminalsLexer.Add("bottom", ReservedWord.RWBOTTOM);
            terminalsLexer.Add("TO", ReservedWord.RWTO);
		    terminalsLexer.Add("true", ReservedWord.RWTRUE);
		    terminalsLexer.Add("false", ReservedWord.RWFALSE);
            terminalsLexer.Add("RadioButtons", ReservedWord.RWRADIOBUTTONS);
            terminalsLexer.Add("COL", ReservedWord.RWCOL);
            terminalsLexer.Add("COLINSERT", ReservedWord.RWCOLINSERT);
            terminalsLexer.Add("ROW", ReservedWord.RWROW);
            terminalsLexer.Add("ROWINSERT", ReservedWord.RWROWINSERT);
            terminalsLexer.Add("CHANGEBOX", ReservedWord.RWCHANGEBOX);
            terminalsLexer.Add("WHERE", ReservedWord.RWWHERE);
            terminalsLexer.Add("CHANGEROWS", ReservedWord.RWCHANGEROWS);
            terminalsLexer.Add("CHANGECOLUMNS", ReservedWord.RWCHANGECOLUMNS);
            terminalsLexer.Add("IN", ReservedWord.RWIN);
            terminalsLexer.Add("SET", ReservedWord.RWSET);

		    terminalsLexer.Add(".", SpecialChar.SCDOT);
		    terminalsLexer.Add("#", SpecialChar.SCSQUARE);
		    terminalsLexer.Add("\"", SpecialChar.SCGUILLEMET);
		    terminalsLexer.Add(";", SpecialChar.SCPOINTVIR);
            terminalsLexer.Add(",", SpecialChar.SCVIRGULE);
            terminalsLexer.Add("@", SpecialChar.SCAT);
            terminalsLexer.Add("%", SpecialChar.SCPERCENT);
            terminalsLexer.Add("(", SpecialChar.SCOPENPARENTHESE);
            terminalsLexer.Add(")", SpecialChar.SCCLOSEPARENTHESE);

		    terminalsLexer.Add("EOF", SpecialChar.EOF);
		
		    symbolToIndex = new Dictionary<Terminal, int>();

		    // Pour chaque terminal de la grammaire, on cherche son correspondant
		    // dans TerminalsLexer
		    foreach(Terminal t in G.Terminals())
		    {
                //Console.WriteLine(t.GetText());
			    symbolToIndex.Add(terminalsLexer[t.GetText()], t.Index());
		    }
	    }
	
	    /**
	     * Renvoit l'arbre syntaxique du code source contenu dans InputPath.
	     * Si une exception survient dans le chargement de la grammaire, pendant
	     * l'analyse lexicale ou pendant l'analyse syntaxique, l'analyse est stoppée.
	     * 
	     * @return "Analyse lexicale terminée" s'affiche à l'écran si le code source
	     * est syntaxiquement correct à la grammaire ou une exception est déclenchée
	     * s'il y a une erreur.
	     * 
	     * @post G initialisé, Lex initialisé, SymbolToIndex intialisé, ParseTable
	     * initialisé
	     * 
	     * @throws Exception
	     */
	    public Tree<Symbol> Parse()
	    {
		    /*
		     * Cette méthode vérifie que le code source est syntaxiquement
		     * correct par rapport à la grammaire.
		     * 
		     * Etape 1: Chargement de l'analyseur lexical
		     * Etape 2: Chargement de la grammaire.
		     * Etape 3: Création de la grammaire étendue.
		     * Etape 4: Calcul des ensembles P1 et S1.
		     * Etape 5: Vérification que la grammaire est bien LL1.
		     * Etape 6: Génération des relations objets Terminal du Lexer -> Index dans la parsing table
		     * 			(Les index sont calculés à la création d'un Terminal)
		     * Etape 7: Génération de la table de parsing
		     * Etape 8: Analyse syntaxique (+ génération de l'arbre syntaxique)	 
		     * */

		    // Analyse Lexicale.
		    // Le fichier contenant le code source sera traduit en 
		    // une série d'objets du package Slip.mylexer.* représentant des "Terminal" 
		    // dont le dernier est EOF signifiant qu'on a lu tous les symboles. 
		    // On utilisera plus bas la méthode nextTerminal() qui renvoit 
		    // le prochain symbole à lire.
		    // Si un symbole n'est pas reconnu par l'analyseur lexical, une 
		    // exception se déclenche (par exemple ').System.Console.WriteLine("Analyse syntaxique...");
            lex = new Lex(inputPath);

		    // Chargement de la grammaire
		    GrammarLoader GL = new GrammarLoader(grammarPath);
		    // Le fichier contenant la grammaire BNF du langage est
		    // traduit en une série de symboles de type Slip.myLL1parser.Terminal
		    // et Slip.myLL1parser.NotTerminal.
		    // Si la syntaxe BNF n'est pas respectée ou si un Non Terminal ne 
		    // possède pas au moins une dérivation, une exception se déclenche.
		    // NB: Pour charger une grammaire BNF plus facilement, celle ci
		    // doit respecter certaines normes spécifiées dans la classe GrammarLoader.
		    G = GL.Load();
		    // Augmentation de la grammaire selon ce schéma:
		    // Pour toute règle:
		    // Y  -> alpha X beta
		    // on crée
		    // X2 -> beta Y2
		    // où alpha, beta sont des terminaux.
		    G.ComputeExtendedRules();
		    // Calcul des ensembles P1 de chaque symbole en utilisant
		    // l'algorithme du point fixe.
		    G.ComputeP1();
		    // Calcul des ensembles S1 de chaque symbole "de base".
		    // Tous les ensembles P1 des symboles "prime" deviennent les 
		    // ensembles S1 de leur correspondant "de base".
		    G.ComputeS1();
		
		    // Vérification que la grammaire est bien LL1.
		    // Condition 1: Pour chaque non terminal X, il ne peut pas exister de 
		    // dérivations différentes o et o' de X telle que l'intersection de 
		    // p1(o) et p1(o') est non vide. 
		    CheckCondition1();
		    // Condition 2: Pour chaque non terminal X, si p1(X) contient lambda alors l'intersection
		    // de p1(X) et s1(X) doit etre vide.
		    CheckCondition2();

		    // On crée une relation entre un objet Terminal renvoyé par le Lexer 
		    // et son index dans la table de parsing. En effet, la table de parsing
		    // est générée à partir d'objet Terminal de GrammarLoader.
		    GenerateSymbolToIndex();
		
		    // Génération de la table de parsing.
		    // Pour chaque règle "de base":
		    // Soit A -> alpha, pour chaque élément "a" de p1(alpha) 
		    // on définit table[A, a] : A -> alpha.
		    //
		    // Si lambda est contenu dans p1(alpha), on va aller voir AUSSI dans 
		    // l'ensemble s1(alpha) qui sont les terminaux pouvant se trouver après A
		    // puisque p1(alpha) peut etre vide. On fera donc:
		    // Pour tout b dans s1(A): table[A, b] : A -> alpha
		    GenerateParsingTable();
		
		    // Pile qui sert à savoir ce qui est attendu comme symbole provenant du lexer
		    Stack<Tree<Symbol>> stack = new Stack<Tree<Symbol>>();
		    // Premier terminal du code source
		    Terminal t = lex.NextTerminal();
		    // Ajout d'un élément à la pile, le symbole <PROGRAM>
		    // On encapsule l'élément dans un arbre qui servira à la traduction
		    // en langage interne. Pour l'algorithme, il ne sert à rien.
		    Tree<Symbol> root = new Tree<Symbol>(Grammar.program);
		    stack.Push(root);
		
		    //System.Console.WriteLine("Analyse syntaxique...");
		    // Début de l'algorithme
		    while(stack.Count()>0)
		    {
			    // Récupération du symbole au sommet de la pile
			    Tree<Symbol> tree = stack.Pop();
			    Symbol x = tree.Element();

                //Console.WriteLine("Symbole attendu: " + x);
                //Console.WriteLine("Symbole en cours: " + t);
                //Console.WriteLine("Symboles restants sur la pile: " + stack);
                //Console.WriteLine();
			
			    // Si le symbole attendu est non terminal, on va vérifier 
			    // que le symbole en cours est identique à ce symbole
			    // ou qu'il est égal à lambda (auquels cas on passera au symbole
			    // suivant de la pile).
			    if(x is Terminal)
			    {
				    if((x == Grammar.identifier && t is Identifier) ||
				       (x == Grammar.constant && t is Constant)     ||
                       (x == Grammar.control && t is ControlType) ||
                       (x == Grammar.myString && t is MyString) ||
                       (x == Grammar.selector && t is Selector) ||
                       (x == Grammar.all && t is All) ||
                       (x == Grammar.myProperty && t is MyProperty) ||
				       (x.GetText().Equals(t.GetText())))
				    {
                        // On remplace le terminal "générique" par celui créé dans le lexer.
                        tree.SetElement(t);
					    // Terminal lu correspond au Terminal attendu, on lit le prochain Terminal.
					    t = lex.NextTerminal();
				    }					
				    else if(x != Grammar.lambda)
				    {
                        //Console.WriteLine(t + ": " + (t.GetType()));
					    throw new ParserException("Symbole " + t + " incorrect. Attendu: " + x);
				    }
			    }
			    // Si le symbole attendu est non terminal, on va récupérer la règle
			    // à appliquer en fonction de celui-ci et du symbole en cours.
			    else if(x is NotTerminal)
			    { 		
				    int index;
				    // On récupère l'index du symbole en cours dans la parsing table
				    if(t is Identifier)
					    index = symbolToIndex[Grammar.identifier];
				    else if(t is Constant)
					    index = symbolToIndex[Grammar.constant];
                    else if (t is MyString)
                        index = symbolToIndex[Grammar.myString];
                    else if (t is ControlType)
                        index = symbolToIndex[Grammar.control];
                    else if (t is All)
                        index = symbolToIndex[Grammar.all];
                    else if (t is Selector)
                        index = symbolToIndex[Grammar.selector];
                    else if (t is MyProperty)
                        index = symbolToIndex[Grammar.myProperty];
				    else
					    index = symbolToIndex[t];
				
				    // Récupération de la règle
				    Rule r;
				    if((r = parsingTable[x.Index(), index]) == null)
                        throw new ParserException("Pas de règle pour " + t.GetText() + " (" + t.GetType() + "). x = " + x.GetText() + " (" + x.GetType() + ")");
				
				    // On ajoute sur la pile tous les symboles présents dans
				    // la règle à exécuter.
				    // On les ajoute dans l'ordre inverse pour qu'on puisse les
				    // retirer dans le bon ordre avec la pile.
				    LinkedList<Symbol> right = r.Right();
				    int i = right.Count()-1; 

				    while(i>-1)
				    {	
					    // On ajoute des enfants à l'arbre contenant le symbole attendu.
					    // Ces enfants contiennent les futurs symboles attendus de la pile.
					    // A la fin de l'algorithme, on aura donc un arbre syntaxique du code source.
					    Tree<Symbol> tmp = new Tree<Symbol>(right.ElementAt(i));
					    tree.Add(tmp);
					    stack.Push(tmp);
					    i--;
				    }
			    }
		    }
		
		    // Si le dernier symbole du lexer est le symbole de fin du fichier. On
		    // renvoit l'arbre syntaxique. Sinon on provoque une exception.
		    if(t == SpecialChar.EOF)
		    {	
			    //System.Console.WriteLine("Analyse syntaxique terminée.");
			    return root;
		    }
		    else
                throw new ParserException("Dernier symbole différent de EOF.");	
	    }
	
	    /**
	     * Vérifie que la première condition pour qu'une grammaire soit LL1 
	     * est correcte.
	     * 
	     * @pre Les ensembles P1 de chaque symbole de la grammaire G doivent être calculés.
	     */
	    public void CheckCondition1()
	    {
		    /*
		     * Cette méthode vérifie que la grammaire G respecte la première condition
		     * LL1 à savoir pour chaque non terminal X, il ne peut pas exister de 
		     * dérivations différentes o et o' de X telle que l'intersection de 
		     * p1(o) et p1(o') est non vide. En pratique celà pose problème car
		     * il y aura une incohérence lors de la génération de la table de parsing.
		     * 
		     * Le principe est simple on crée une relation NotTerminal -> Set of Derivations
		     * Ensuite, pour chaque non terminal, on vérifie qu'aucun
		     * ensemble p1 de ses dérivations ne possède un meme terminal.
		     */
		
		    // Génération de la relation NotTerminal -> Ensemble de dérivations
		    if(allLeftNotTerminals == null)
			    GenerateAllLeftNotTerminal();

		    // Soit x -> Y1 | Y2 | .. | Yn
		    // Pour chaque dérivation Yi, on vérifie que l'intersection de p1(Yi) et p1(Yj),
		    // où j>i, est vide.
		    foreach(KeyValuePair<NotTerminal, HashSet<Rule>> entry in allLeftNotTerminals) 
		    {
			    NotTerminal x = entry.Key;
			    HashSet<Rule> Y = entry.Value;
                //System.Console.WriteLine("NonTer: " + x);
			    // Pour chaque Yi
			    int i = 1;
			    foreach(Rule r in Y)
			    {
				    HashSet<Terminal> P1Yi = G.MergeSets(r.Right());
                    //System.Console.WriteLine("R1: " + r);
                    int j = 1;
				    foreach(Rule r2 in Y)
				    {
                        // Pour chaque Yj où j>i
                        if (j <= i)
                        {
                            j++;
                            continue;
                        }

                        //System.Console.WriteLine("R2: " + r2);
					    HashSet<Terminal> P1Yj = G.MergeSets(r2.Right());
					
					    // Intersection de p1(Bi) et p1(Bj) vide ?
					    foreach(Terminal t in P1Yj)
					    {	
						    if(P1Yi.Contains(t))
                                throw new ParserException("Le non terminal '" + x + "' possède deux dérivations ayant des éléments de p1 identiques: "
													    + " " + t);
					    }
				    }
				    i++;
			    }
		    }
	    }
	
	    /**
	     * Génère la relation NotTerminal -> Ensemble de ses dérivations
	     * dans la HashMap AllLeftNotTerminals.
	     * 
	     * @post AllLeftNotTerminals modifié
	     *
	     */
	    public void GenerateAllLeftNotTerminal()
	    {
		    allLeftNotTerminals = new Dictionary<NotTerminal, HashSet<Rule>>();
		    // On parcourt toutes les règles pour récupérer toutes les
		    // dérivations de chaque non terminal.
		    foreach(Rule r in G.Rules())
		    {
			    // TerminalRules est l'ensemble des dérivations du non terminal r.left
		        HashSet<Rule> terminalRules;
		    
		        // Si le nonterminal r.left n'a pas encore de dérivation, on 
		        // crée un ensemble dans lequel on stockera les dites dérivations.
                if (!allLeftNotTerminals.ContainsKey(r.Left()))
                {

                    allLeftNotTerminals.Add(r.Left(), (terminalRules = new HashSet<Rule>()));
                }
                else{
                    terminalRules = allLeftNotTerminals[r.Left()];
                }
			
		        terminalRules.Add(r);
		    }
	    }
	
	    /**
	     * Vérifie que la deuxième condition pour qu'une grammaire soit LL1 est correct.
	     * 
	     * @pre Les ensembles p1 de chaque symboles de la grammaire G doivent etre calculés
	     * @pre Les ensembles s1 de chaque symbole de la grammaire G non étendue doivent etre calculés
	     */
	    public void CheckCondition2()
	    {
		    /*
		     * Cette méthode vérifie que la deuxième condition pour qu'une grammaire soit 
		     * LL1 est remplie à savoir: 
		     * Pour chaque non terminal x, si p1(x) contient lambda alors l'intersection
		     * de p1(x) et s1(x) doit etre vide.
		     * 
		     * Le principe est simple, on récupère chaque non terminal x de la grammaire G,
		     * on vérifie si l'ensemble P1(x) contient lambda. Si oui, on vérifie, pour chaque 
		     * symbole de p1(x) qu'il ne se trouve pas dans s1(x).
		     */
		
		    // On parcourt tous les non terminal de la grammaire
		    foreach(NotTerminal x in G.NotTerminals())
		    {	
			    // On vérifie que p1(x) ne contient pas lambda
			    if(x.P1().Contains(Grammar.lambda))
			    {	
				    HashSet<Terminal> P1x = x.P1();
				    //System.out.println(x + ": " + P1x);
				    // Lambda se trouve dans p1(x), on parcourt donc l'ensemble p1(x)
				    // et on vérifie qu'aucun symbole de p1(x) ne se trouve dans s1(x).
				    foreach(Terminal t in P1x)
				    {
					    if(x.S1().Contains(t))
                            throw new ParserException("L'intersection de l'ensemble s1(x) et p1(Yi) du non terminal: '" + x + "' est non vide: "
												    + " " + t);
				    }
			    }
		    }
	    }
	
	    /**
	     * Génère la table de parsing de la grammaire G
	     * dans le tableau à deux dimensions ParsingTable
	     * 
	     * @pre Les ensembles p1 de chaque symboles de la grammaire G doivent etre calculés
	     * @pre Les ensembles s1 de chaque symbole de la grammaire G non étendue doivent etre calculés
	     */
	    public void GenerateParsingTable()
	    {
		    /*
		     * Comme fait au cours, on va créer un tableau en deux dimensions
		     * de taille NonTerminalList.size() et TerminalList.size().
		     * Le but de ce tableau est de savoir quel règle choisir en fonction
		     * du non terminal courant sur la stack et le symbol lu dans le code 
		     * source. 
		     * 
		     * Soit A -> alpha, pour chaque élément "a" de p1(alpha) 
		     * on définit table[A, a] : A -> alpha.
		     * 
		     * Si lambda est contenu dans p1(alpha), on va aller voir AUSSI dans 
		     * l'ensemble s1(alpha) qui sont les terminaux pouvant se trouver après A
		     * puisque p1(alpha) peut etre vide. On fera donc:
		     * Pour tout b dans s1(A): table[A, b] : A -> alpha
		     */
		
		    //Rule[,] table = new Rule[G.NotTerminals().Count(), G.Terminals().Count()];
            //Console.WriteLine("Taille table: " + G.NotTerminals().Count() + " et " + G.Terminals().Count());
            Rule[,] table = new Rule[NotTerminal.NotTerminalIndex+1, Terminal.TerminalIndex+1];
		    // Parcours des règles
		    foreach(Rule r in G.Rules())
		    {
			
			    // On définit A -> alpha
			    NotTerminal A = r.Left();
			    LinkedList<Symbol> alpha = r.Right();
			
			    // On récupère l'ensemble p1(alpha)
			    // On est certain d'avoir un résultat car tous les ensembles
			    // p1 ont été calculés précédement.
			    bool isLambda = false;
			
			    // Pour chaque "premier terminal" pouvant etre obtenu en dérivant A
			    // On crée table[A, a] : A -> aplha sauf si "a" == Lambda
			    foreach(Terminal a in G.MergeSets(alpha))
			    {
				    if(a == Grammar.lambda)
					    isLambda = true;
				    else{
                        //Console.WriteLine(A + " (" + A.Index() + ") - " + a+ " (" + a.Index() + ")");
					    table[A.Index(), a.Index()] = r;
					    //System.out.println("table[" + A.getText() + ", " + a.getText() + "] = " + r);
				    }
			    }
			
			    // Si on a découvert un lambda dans p1(alpha), on va aussi
			    // ajouter les éléments de s1(A) à la liste.
			    if(isLambda)
			    {
				    // Pour tous les terminaux "b" suivant A on fait:
				    // table[A, b] : A -> alpha
				    foreach(Terminal b in A.S1())
				    {
					    table[A.Index(), b.Index()] = r;
					    //System.out.println("table[" + A.getText() + ", " + b.getText() + "] = " + r);
				    }
			    }
		    }
		
		    parsingTable = table;
	    }
    }

}
