
namespace Youregone.BehaviourTrees
{
    public class TargetExistsCondition : ConditionNode
    {
        private BaseEnemyBehaviour _enemyBehaviour;

        public TargetExistsCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        protected override bool Predicate()
        {
            return _enemyBehaviour.CurrentTargetTransform != null;
        }
    }
}