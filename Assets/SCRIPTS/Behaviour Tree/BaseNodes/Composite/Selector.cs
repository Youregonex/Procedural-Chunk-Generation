using System.Collections.Generic;

namespace Youregone.BehaviourTrees
{
    public class Selector : Composite
    {
        public Selector(int nodePriority = 0) : base(nodePriority) { }

        public Selector(List<Node> childNodes, int nodePriority = 0) : base(nodePriority)
        {
            _childNodes = childNodes;
        }

        public override ENodeState Evaluate()
        {
            foreach (Node node in _childNodes)
            {
                switch (node.Evaluate())
                {
                    case ENodeState.Running:

                        _nodeState = ENodeState.Running;
                        return _nodeState;

                    case ENodeState.Success:

                        _nodeState = ENodeState.Success;
                        return _nodeState;

                    case ENodeState.Failure:
                        break;

                    default:
                        break;
                }
            }

            _nodeState = ENodeState.Failure;
            return _nodeState;
        }
    }
}

