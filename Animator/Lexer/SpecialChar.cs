using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;

namespace Animator.Lexer
{
    public class SpecialChar : Terminal
    {

  	    /* <SPECIAL SYMBOLS> ::= 	 . | # | " | ; | , | @ | % | ( | )
  	    */
  
	    // Comparateurs conditionnels
	    //public final static Terminal SCPPE = new SpecialChar("<=");

	    // Caractères spéciaux
        public static Terminal SCDOT;
        public static Terminal SCSQUARE;
        public static Terminal SCGUILLEMET;
        public static Terminal SCPOINTVIR;
        public static Terminal SCNEG;
        public static Terminal SCVIRGULE;
        public static Terminal SCAT;
        public static Terminal SCPERCENT;
        public static Terminal SCOPENPARENTHESE;
        public static Terminal SCCLOSEPARENTHESE;
	
	    // Opérateurs arithmétiques
	    //public final static Terminal SCMUL = new SpecialChar("*");
	
	    // Caractère spécial pour définir la fin du fichier
	    public static Terminal EOF;
  
      public String ToString(){ return "SpecialChar: " + GetText() ;}

      public SpecialChar(String val) : base(val)
      { 
      }

      public static void initSC()
      {
          SCDOT = new SpecialChar(".");
          SCSQUARE = new SpecialChar("#");
          SCGUILLEMET = new SpecialChar("\"");
          SCPOINTVIR = new SpecialChar(";");
          SCNEG = new SpecialChar("-");
          SCVIRGULE = new SpecialChar(",");
          SCAT = new SpecialChar("@");
          SCPERCENT = new SpecialChar("%");
          EOF = new SpecialChar("EOF");
          SCOPENPARENTHESE = new SpecialChar("(");
          SCCLOSEPARENTHESE = new SpecialChar(")");
      }
  
      public static Terminal Terminal(char c)
      {
	      switch(c)
	      {
	  	    case '.': return SCDOT;
	  	    case '#': return SCSQUARE;
	  	    case '"': return SCGUILLEMET;
	  	    case ';': return SCPOINTVIR;
            case '-': return SCNEG;
            case ',': return SCVIRGULE;
            case '@': return SCAT;
            case '%': return SCPERCENT;
            case '(': return SCOPENPARENTHESE;
            case ')': return SCCLOSEPARENTHESE;
	      }
	      return null;
      }
  
    }
}
