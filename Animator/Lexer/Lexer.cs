using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class Lexer 
    {
	    /*
	     * Cette classe remplit le rôle d'analyseur lexical. L'analyse utilise
	     * le principe de "la machine à états" pour valider ou un non un code
	     * source. Ci-dessous, les différents états.
	     */
	    // Le Lexer est en attente d'un caractère, n'importe lequel, pour l'analyser.
	    private const int STATE_BEGIN = 0;
	    // Le Lexer est en attente d'un chiffre compris entre 0 et 9.
	    private const int STATE_CATCH_DIGIT = 1;
	    // Le Lexer est en attente d'un caractère.
	    private const int STATE_CATCH_IDENTIFIER = 2;
	    // Le Lexer est en attente de n'importe quel caractère contenu dans un commentaire.
	    private const int STATE_CATCH_COMMENT = 3;
        // Le Lexer est en attente d'un caractère pour former une string.
	    private const int STATE_CATCH_STRING = 4;
        // Le Lexer est en attente d'un caractère pour former un control.
	    private const int STATE_CATCH_CONTROL = 5;
	    // Pointer est un objet qui lit un fichier texte, caractère par caractère
	    private Pointer pointer;
	
	    /**
	     * Constructeur de la classe.
	     * 
	     * @param f: Chemin du fichier source
	     */
	    public Lexer(String f)
	    {
		    pointer = new Pointer(new StreamReader(f));
	    }
	
	    /**
	     * Cette méthode renvoie le prochain symbole lexical du code
	     * source contenu dans le fichier manipulé par pointer.
	     * 
	     * L'algorithme de cette méthode utilise le principe de 
	     * "machine à états". La représentation graphique des états
	     * peut etre consultée dans le rapport.
	     * 
	     * @pre pointeur est différent de null
	     * 
	     * @post La position du pointeur de pointer a avancé d'autant
	     * de caractères que la taille du symbole lexical renvoyé ou
	     * il ne s'est rien passé si la position du pointeur de pointer
	     * est à la fin du fichier.
	     * 
	     * @return un objet Terminal correspondant au prochain Terminal
	     * lu dans le fichier manipulé par pointer ou le Terminal EOF 
	     * si on a parcouru tout le fichier manipulé par pointer.
	     * 
	     */
	    public Terminal nextTerminal()
	    {
		    int state = STATE_BEGIN;
		    StringBuilder res = new StringBuilder();
		    char c;
		
		    try
		    {
			    while(pointer.HasNext())
			    {
				    switch(state)
				    {
					    case STATE_BEGIN:
						
						    removeSpaces();
						    Terminal t;
						    c = pointer.Remove();
						    res.Append(c);

						    switch(c)
						    {
							    // Symbole // ou /
							    case '/':
								    if(pointer.GetChar() == '/')
								    {
									    pointer.Remove();
									    res = new StringBuilder();
									    state = STATE_CATCH_COMMENT;
								    }else
								    {
									    throw new Exception("Caractère inconnu: " + c);
								    }
							    break;
							
							    // Symbole -x
							    case '-':
								    if(Char.IsDigit(pointer.GetChar()))
								    {
									    state = STATE_CATCH_DIGIT;
								    }else
                                        throw new Exception("Caractère '-' non suivi d'un nombre.");
                                    break;


                                // Symbole x.y
							    case '#':
								    state = STATE_CATCH_CONTROL;
                                    break;

                                // Symbole ;
							    case ';':
								    return new SpecialChar(";");
                                    break;

                                // String
							    case '"':
								    state = STATE_CATCH_STRING;
                                    break;
							
							    // Identifier, Constant
							    default:
								    if(Char.IsDigit(c)) state = STATE_CATCH_DIGIT;
								    else if(Char.IsLetter(c)) state = STATE_CATCH_IDENTIFIER;
								    else 
									    {	
										    // Apparement le dernier caractère (qui n'a pas de rapport avec le code) 
										    // de mes fichiers provoque une erreur. J'ai utilisé le 
										    // système D après avoir passer une heure sur le problème ..
										    if(pointer.HasNext())
											    throw new Exception("Caractère inconnu: " + c);
									    }
							    break;
	
						    }	
					    break;

                        case STATE_CATCH_STRING:
						    if(pointer.GetChar() == '"')
                            {
                                state = STATE_BEGIN;
                                return new MyString(res.ToString());

                            }else if(pointer.GetChar() == '\\')
                            {
                                pointer.Remove();
                                if(pointer.GetChar() == '"')
                                {
                                    res.Append('"');
                                    pointer.Remove();
                                }else
                                    res.Append('\\');
                            }
					    break;
					
					    case STATE_CATCH_COMMENT:
						    if(pointer.GetChar() == '\n')
						    {
							    state = STATE_BEGIN;
						    }else
							    pointer.Remove();
					    break;
					
					    case STATE_CATCH_DIGIT:
						    if(Char.IsDigit(pointer.GetChar()) || pointer.GetChar() == '.')
							    res.Append(pointer.Remove());
						    else
						    {
							    state = STATE_BEGIN;
							    return new Constant(res.ToString());
						    }
					    break;
					
					    case STATE_CATCH_IDENTIFIER:
                        case STATE_CATCH_CONTROL:
						    if(Char.IsLetterOrDigit(pointer.GetChar()))
							    res.Append(pointer.Remove());
						    else
						    {	
							    state = STATE_BEGIN;
							    Terminal rw = ReservedWord.Terminal(res.ToString());
							    if(rw != null)
							    {
								    return rw;
							    }else if(state == STATE_CATCH_IDENTIFIER)
								    return new Identifier(res.ToString());
                                else
                                    return new Control(res.ToString());
						    }
					    break;
				    }
			    }
		    }catch(IOException e)
		    {
			    System.Console.WriteLine("Exception lors de l'analyse lexciale du fichier source: " + e);
			    return SpecialChar.EOF;
		    }
		
		    pointer.close();
			
		    return SpecialChar.EOF;
	    }

	    public void removeSpaces() throws IOException 
	    {	
		    while(Character.isWhitespace(pointer.getChar()))
			    pointer.remove();
	    }
	
	    public static void main(String[] arg) throws Exception
	    {
		    Lexer toto = new Lexer(arg[0]);
		    Terminal t;
		      while ((t = toto.nextTerminal()) != SpecialChar.EOF)
		      {
		        System.out.println( t );
		      }
	    }
	
    }
}
