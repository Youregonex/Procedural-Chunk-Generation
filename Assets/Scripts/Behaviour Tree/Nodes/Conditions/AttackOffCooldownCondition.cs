
namespace Youregone.BehaviourTrees
{
    public class AttackOffCooldownCondition : Node
    {
        public AttackOffCooldownCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        private BaseEnemyBehaviour _enemyBehaviour;

        public override ENodeState Evaluate()
        {
            _nodeState = _enemyBehaviour.GetAttackCooldown() <= 0 ? ENodeState.Success : ENodeState.Failure;

            return _nodeState;
        }
    }

}