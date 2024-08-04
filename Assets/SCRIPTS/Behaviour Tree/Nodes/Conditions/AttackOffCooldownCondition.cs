
namespace Youregone.BehaviourTrees
{
    public class AttackOffCooldownCondition : ConditionNode
    {
        private BaseEnemyBehaviour _enemyBehaviour;

        public AttackOffCooldownCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        protected override bool Predicate()
        {
            return _enemyBehaviour.GetAttackCooldown() <= 0;
        }
    }
}