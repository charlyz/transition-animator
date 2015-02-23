using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Animator.LL1Parser
{
    public class Tree<S>
    {
        // Descendants du noeud
        private LinkedList<Tree<S>> children = new LinkedList<Tree<S>>();
        // Symbol contenu dans le noeud
        private S e;

        public Tree(S e) { this.e = e; }

        public void Add(Tree<S> t) { children.AddFirst(t); }

        public LinkedList<Tree<S>> Children() { return children; }

        public Tree<S> Child(int n) { return children.Count()>n ? children.ElementAt(n) : null; }

        public int NbChildren() { return children.Count; }

        public S Element() { return e; }

        public void SetElement(S e) { this.e = e; }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(e);

            if (!(children.First() == null))
            {
                sb.Append("(");
                sb.Append(children);
                sb.Append(")\n");
            }

            return sb.ToString();
        }
    }
}
