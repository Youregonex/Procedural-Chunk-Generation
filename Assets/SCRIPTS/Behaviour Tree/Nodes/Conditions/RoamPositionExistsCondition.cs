using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class RoamPositionExistsCondition : ConditionNode
    {
        private BaseEnemyBehaviour _enemyBehaviour;

        public RoamPositionExistsCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        protected override bool Predicate()
        {
            return _enemyBehaviour.CurrentRoamPosition != Vector2.zero;
        }
    }
}