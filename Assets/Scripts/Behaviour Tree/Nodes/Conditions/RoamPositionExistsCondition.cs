using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class RoamPositionExistsCondition : Node
    {
        public RoamPositionExistsCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        private BaseEnemyBehaviour _enemyBehaviour;

        public override ENodeState Evaluate()
        {
            _nodeState = _enemyBehaviour.CurrentRoamPosition != Vector2.zero ? ENodeState.Success : ENodeState.Failure;

            return _nodeState;
        }
    }

}