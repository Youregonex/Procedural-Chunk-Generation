using Youregone.Utilities;

namespace Youregone.BehaviourTrees
{
    public class TargetInRangeCondition : ConditionNode
    {
        private BaseEnemyBehaviour _enemyBehaviour;
        private float _range;

        public TargetInRangeCondition(BaseEnemyBehaviour enemyBehaviour, float range, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
            _range = range;
        }

        protected override bool Predicate()
        {
            return Utility.InRange(_range,
                                   _enemyBehaviour.transform.position,
                                   _enemyBehaviour.CurrentTargetTransform.position);
        }
    }
}