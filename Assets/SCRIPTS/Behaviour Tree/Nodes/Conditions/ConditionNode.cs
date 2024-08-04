
namespace Youregone.BehaviourTrees
{
    public abstract class ConditionNode : Node
    {
        protected ConditionNode(int nodePriority = 0) : base(nodePriority) {}

        public override ENodeState Evaluate() { return Predicate() ? ENodeState.Success : ENodeState.Failure; }

        protected abstract bool Predicate();
    }
}