
public abstract class Node
{
   public enum ENodeState
    {
        Running,
        Success,
        Failure
    }

    public Node(int nodePriority = 0)
    {
        NodePriority = nodePriority;
    }

    protected ENodeState _nodeState;

    public int NodePriority { get; protected set; }
    public ENodeState NodeState => _nodeState;


    public abstract ENodeState Evaluate();
}
