using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class RoamTimerExpiredCondition : ConditionNode
    {
        // TODO Rework node
        private BaseEnemyBehaviour _enemyBehaviour;
        private float _timerMax;
        private float _timerCurrent = 0;

        public RoamTimerExpiredCondition(BaseEnemyBehaviour enemyBehaviour, float roamTimer, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
            _timerMax = roamTimer;
        }

        public override ENodeState Evaluate()
        {
            if (_timerCurrent >= _timerMax)
            {
                _timerCurrent = 0;
                _enemyBehaviour.SetRoamPosition(Vector2.zero);
                _nodeState = ENodeState.Success;
            }
            else
            {
                _timerCurrent += Time.deltaTime;
                _nodeState = ENodeState.Failure;
            }

            return _nodeState;
        }

        protected override bool Predicate() { return false; }
    }
}