using System.Collections.Generic;

namespace Youregone.BehaviourTrees
{
    public class Composite : Node
    {
        protected List<Node> _childNodes = new List<Node>();

        public Composite(int nodePriority = 0) : base(nodePriority) {}

        public override ENodeState Evaluate() { return ENodeState.Failure; }

        public void AddChildNode(Node node)
        {
            _childNodes.Add(node);
        }
    }

}