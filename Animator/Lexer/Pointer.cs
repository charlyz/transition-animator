using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class Pointer
    {
	    // Positions du pointeur dans le fichier
	    private int row, col;
	    // Fichier d'entrée
	    private StreamReader input;
	    // Caractère actuel
	    private int c;
	
	    public Pointer(StreamReader sr)
	    {
		    input = sr;
		    c = sr.Read();
		    row = 1;
		    col = 1;
	    }
	
	    public char GetChar()
	    {
		    return (char)c;
	    }
	
	    public char Remove()
	    {	
		    if((char)c == '\n')
		    {
			    row++;
			    col = 1;
		    }
		    else
			    col++;
		
		    char old = (char)c;
            c = input.Read();
		    return old;
	    }
	
	    public bool HasNext()
	    {
		    return (c != -1);
	    }
	
	    public int GetCurrentCol()
	    {
		    return col;
	    }
	
	    public int GetCurrentRow()
	    {
		    return row;
	    }
	
	    public void Close()
	    {
		    try 
		    {
			    input.Close(); 
		    }
		    catch(IOException e)
		    {
                throw new ParserException("Impossible de fermer le fichier: " + input);
		    }
	    }
    }
}
