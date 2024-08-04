
namespace Youregone.BehaviourTrees
{
    public abstract class BehaviourNode : Node
    {
        protected BehaviourNode(int nodePriority = 0) : base(nodePriority) {}

        public override ENodeState Evaluate() { return ENodeState.Failure; }
    }

}