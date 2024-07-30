using System.Collections.Generic;
using System.Linq;

namespace Youregone.BehaviourTrees
{
    public class PrioritySelector : Selector
    {
        public PrioritySelector(List<Node> childNodes, int nodePriority = 0) : base(childNodes, nodePriority) { }

        public override ENodeState Evaluate()
        {
            _childNodes = _childNodes.OrderByDescending(node => node.NodePriority).ToList();

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
