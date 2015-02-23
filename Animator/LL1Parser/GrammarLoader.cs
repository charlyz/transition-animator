using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Animator.LL1Parser
{
    public class GrammarLoader 
{
	private Dictionary<String, Terminal> terminalMap;
	private Dictionary<String, NotTerminal> notTerminalMap;
	private LinkedList<Rule> rules;
	private StreamReader sr;
	private HashSet<NotTerminal> allLeftNotTerminal = new HashSet<NotTerminal>();
	
	public GrammarLoader(String f) 
	{	
		terminalMap = new Dictionary<String, Terminal>();
		notTerminalMap = new Dictionary<String, NotTerminal>();
		rules = new LinkedList<Rule>();
		
		// On ajoute les balises prédéfinies.
		terminalMap.Add(Grammar.lambda.GetText(), Grammar.lambda);
		terminalMap.Add(Grammar.identifier.GetText(), Grammar.identifier);
		terminalMap.Add(Grammar.constant.GetText(), Grammar.constant);
        terminalMap.Add(Grammar.myString.GetText(), Grammar.myString);
        terminalMap.Add(Grammar.control.GetText(), Grammar.control);
        terminalMap.Add(Grammar.selector.GetText(), Grammar.selector);
        terminalMap.Add(Grammar.all.GetText(), Grammar.all);
        terminalMap.Add(Grammar.myProperty.GetText(), Grammar.myProperty);
		notTerminalMap.Add(Grammar.program.GetText(), Grammar.program);
		
		sr = File.OpenText(f);
	}

	public Grammar Load() 
	{
		String line;

		while((line = sr.ReadLine())!= null)
		{
            //System.Console.WriteLine(line);
			if(line.Length == 0 || line.StartsWith("//"))
				continue;
			// On sépare left et right
			String[] tmp = Regex.Split(line.Trim(), "::=");
			if(tmp.Length != 2)
                throw new ParserException("Erreur de syntaxe de la grammaire: sigle ::= manquant");
			
			// On récupère left, si left est Terminal, on provoque une erreur
			Symbol left = nextSymbol(tmp[0].Replace("\t", "").Trim()); // \t = TAB
			if(!(left is NotTerminal))
                throw new ParserException("Erreur de syntaxe de la grammaire: un symbole de gauche est terminal");
			
			// On sépare toutes les dérivations possibles de right
            String[] tokens = tmp[1].Split('|');
			int i = 0;
			LinkedList<Symbol> right = new LinkedList<Symbol>();
			
			// Pour chaque dérivation, on récupère les symboles
			while(i<tokens.Length)
			{
				String[] sym = tokens[i].Replace("\t", "").Split(' ');
				int j = 0;
				
				// On récupère chaque symbole et on l'ajoute à une liste
				while(j<sym.Length)
				{	
					if(sym[j].Trim().Length>2)
						right.AddLast(nextSymbol(sym[j]));
					j++;
				}

				// Si right est vide, il y a un probleme.
				if(right.Count()<1)
                    throw new ParserException("Erreur de syntaxe de la grammaire: un symbole de droite est manquant");
				
				// On crée la règle, left et right sont correctes.
				rules.AddLast(new Rule((NotTerminal)left, right));
				allLeftNotTerminal.Add((NotTerminal)left);
				right = new LinkedList<Symbol>();
				i++;
			}
		}
		
		// On vérifie que tous les non terminaux soient joignables.
		checkGrammar();
        //Console.WriteLine("Taille Terminals Grammar Loader: " + terminalMap.Values.Count());
        //foreach (Terminal t in terminalMap.Values)
            //Console.WriteLine(t);
		return new Grammar(new LinkedList<Terminal>(terminalMap.Values), new LinkedList<NotTerminal>(notTerminalMap.Values), rules);
	}

	/**
	 * pre: balise != null && balise.length > 2
	 * @param balise
	 * @return
	 */
	private Symbol nextSymbol(String balise) 
	{		
		char c1 = balise[0];
		char cn = balise[balise.Length-1];

		String name = balise.Substring(1, balise.Length-2);
        //System.Console.WriteLine(name + " - " + c1 +"  - " + cn);
		// La balise est un terminal
		if(c1 == '\'' && cn == '\'')
		{
			Terminal symbol;
			// Si le symbole ne se trouve pas dans la map des terminal, on l'ajoute.
            if (!terminalMap.ContainsKey(name))
			{
				symbol = new Terminal(name);
				terminalMap.Add(name, symbol);
            }else
            {
                symbol = terminalMap[name];
            }
			return symbol;
		}
		// La balise est un non terminal
		else if(c1 == '<' && cn == '>')
        {
            
			NotTerminal symbol;
			// Si le symbole ne se trouve pas dans la map des non terminal, on l'ajoute.
            if (!notTerminalMap.ContainsKey(name))
            {
                symbol = new NotTerminal(name);
                notTerminalMap.Add(name, symbol);
            }else
            {
                symbol = notTerminalMap[name];
            }
			return symbol;
		}
		else
			// La balise n'est pas contenue entre '' et <> donc est invalide
            throw new ParserException("Erreur dans la syntaxe de la grammaire: la balise '" + balise + "' n'est pas entre '' ou <>");
	}
	
	/**
	 * pré: grammaire déjà chargée
	 */
	public void checkGrammar()
	{
		// On parcourt toutes les règles pour vérifier que les non terminaux
		// à droite aient tous une dérivation.
		foreach(Rule r in rules)
		{
			foreach(Symbol s in r.Right())
			{
				if(s is NotTerminal)
				{
					// On vérifie que le non terminal "s" situé à droite d'une règle
					// possède une dérivation.
					if(!allLeftNotTerminal.Contains(s))
                        throw new ParserException("Le non terminal " + (NotTerminal)s + " n'a pas de dérivation.");
				}
			}
			
		}
	}

}
}
