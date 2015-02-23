using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Animator.LL1Parser;
using System.Collections;

namespace Animator.Lexer
{
    public class ReservedWord : Terminal
    {
        /*
           <RESERVED WORD> ::= 	substitute | by | to | rotate | move | of | contract | remove
         *                      extend  | left | top | right | bottom | true | false |radiobuttons
         *                      COL | COLINSERT | ROW | ROWINSERT | CHANGEBOX | WHERE | CHANGEROWS | 
         *                      CHANGECOLUMNS | IN | BY | List  | SET
         */

        public static Terminal RWSUBSTITUTE;
        public static Terminal RWREMOVE;
        public static Terminal RWBY;
        public static Terminal RWTO;
        public static Terminal RWROTATE;
        public static Terminal RWMOVE;
        public static Terminal RWOF;
        public static Terminal RWCONTRACT;
        public static Terminal RWEXTEND;
        public static Terminal RWLEFT;
        public static Terminal RWTOP;
        public static Terminal RWRIGHT;
        public static Terminal RWBOTTOM;
        public static Terminal RWTRUE;
        public static Terminal RWFALSE;
        public static Terminal RWRADIOBUTTONS;
        public static Terminal RWCOL;
        public static Terminal RWCOLINSERT;
        public static Terminal RWROW;
        public static Terminal RWROWINSERT;
        public static Terminal RWCHANGEBOX;
        public static Terminal RWWHERE;
        public static Terminal RWCHANGEROWS;
        public static Terminal RWCHANGECOLUMNS;
        public static Terminal RWIN;
        public static Terminal RWSET;
        public static Dictionary<String, Terminal> RW;
  
	    public override String ToString(){ return "ReservedWord: " + GetText() ;}

	    public ReservedWord(String val) : base(val)
	    {
	    }

        public static void initRW()
        {
            RWSUBSTITUTE = new ReservedWord("SUBSTITUTE");
            RWREMOVE = new ReservedWord("REMOVE");
            RWBY = new ReservedWord("BY");
            RWTO = new ReservedWord("TO");
            RWROTATE = new ReservedWord("ROTATE");
            RWMOVE = new ReservedWord("MOVE");
            RWOF = new ReservedWord("OF");
            RWCONTRACT = new ReservedWord("CONTRACT");
            RWEXTEND = new ReservedWord("EXTEND");
            RWLEFT = new ReservedWord("left");
            RWTOP = new ReservedWord("top");
            RWRIGHT = new ReservedWord("right");
            RWBOTTOM = new ReservedWord("bottom");
            RWTRUE = new ReservedWord("true");
            RWFALSE = new ReservedWord("false");
            RWRADIOBUTTONS = new ReservedWord("RadioButtons");
            RWCOL = new ReservedWord("COL");
            RWCOLINSERT = new ReservedWord("COLINSERT");
            RWROW = new ReservedWord("ROW");
            RWROWINSERT = new ReservedWord("ROWINSERT");
            RWCHANGEBOX = new ReservedWord("CHANGEBOX");
            RWWHERE = new ReservedWord("WHERE");
            RWCHANGEROWS = new ReservedWord("CHANGEROWS");
            RWCHANGECOLUMNS = new ReservedWord("CHANGECOLUMNS");
            RWIN = new ReservedWord("IN");
            RWSET = new ReservedWord("SET");

            RW = new Dictionary<String, Terminal>();
            RW.Add("SUBSTITUTE", RWSUBSTITUTE);
            RW.Add("BY", RWBY);
            RW.Add("TO", RWTO);
            RW.Add("ROTATE", RWROTATE);
            RW.Add("MOVE", RWMOVE);
            RW.Add("OF", RWOF);
            RW.Add("CONTRACT", RWCONTRACT);
            RW.Add("EXTEND", RWEXTEND);
            RW.Add("REMOVE", RWREMOVE);
            RW.Add("left", RWLEFT);
            RW.Add("top", RWTOP);
            RW.Add("right", RWRIGHT);
            RW.Add("bottom", RWBOTTOM);
            RW.Add("true", RWTRUE);
            RW.Add("false", RWFALSE);
            RW.Add("RadioButtons", RWRADIOBUTTONS);
            RW.Add("COL", RWCOL);
            RW.Add("COLINSERT", RWCOLINSERT);
            RW.Add("ROW", RWROW);
            RW.Add("ROWINSERT", RWROWINSERT);
            RW.Add("CHANGEBOX", RWCHANGEBOX);
            RW.Add("WHERE", RWWHERE);
            RW.Add("CHANGEROWS", RWCHANGEROWS);
            RW.Add("CHANGECOLUMNS", RWCHANGECOLUMNS);
            RW.Add("IN", RWIN);
            RW.Add("SET", RWSET);
        }
	
	    public static Terminal Terminal(String s)
	    {
            if (RW.ContainsKey(s))
                return RW[s];
            else
                return null;
	    }
    }
}
