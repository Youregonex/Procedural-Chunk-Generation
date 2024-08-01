using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class TargetTooCloseCondition : Node
    {
        private BaseEnemyBehaviour _enemyBehaiour;
        private float _closeRange;

        public TargetTooCloseCondition(BaseEnemyBehaviour enemyBehaviour, float closeRange, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaiour = enemyBehaviour;
            _closeRange = closeRange;
        }

        public override ENodeState Evaluate()
        {
            return Vector2.Distance(_enemyBehaiour.transform.position,
                                    _enemyBehaiour.GetCurrentTargetTransform().position) <= _closeRange ? ENodeState.Success : ENodeState.Failure;
        }
    }

}