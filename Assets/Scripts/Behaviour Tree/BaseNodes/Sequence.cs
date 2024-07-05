using System.Collections.Generic;

public class Sequence : Node
{
    public Sequence(List<Node> childNodes, int nodePriority = 0) : base(nodePriority)
    {
        _childNodes = childNodes;
    }


    protected List<Node> _childNodes = new List<Node>();

    public override ENodeState Evaluate()
    {
        bool isAnyChildRunning = false;

        foreach(Node node in _childNodes)
        {
            switch(node.Evaluate())
            {
                case ENodeState.Running:

                    isAnyChildRunning = true;

                    break;

                case ENodeState.Success:
                    break;

                case ENodeState.Failure:

                    _nodeState = ENodeState.Failure;
                    return _nodeState;

                default:
                    break;
            }
        }

        _nodeState = isAnyChildRunning ? ENodeState.Running : ENodeState.Success;
        return _nodeState;
    }
}
