
namespace Youregone.BehaviourTrees
{
    public class Inverter : Node
    {
        public Inverter(Node node, int nodePriority = 0) : base(nodePriority)
        {
            _node = node;
        }

        protected Node _node;

        public override ENodeState Evaluate()
        {
            switch (_node.Evaluate())
            {
                case ENodeState.Running:

                    _nodeState = ENodeState.Running;

                    break;

                case ENodeState.Success:

                    _nodeState = ENodeState.Failure;

                    break;

                case ENodeState.Failure:

                    _nodeState = ENodeState.Success;

                    break;
                default:
                    break;
            }

            return _nodeState;
        }
    }
}
