using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class MoveToRoamPositionNode : BehaviourNode
    {
        private BaseEnemyBehaviour _enemyBehaviour;

        public MoveToRoamPositionNode(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        public override ENodeState Evaluate()
        {
            if (Vector2.Distance(_enemyBehaviour.transform.position, _enemyBehaviour.CurrentRoamPosition) >= .1f)
            {
                _enemyBehaviour.SetMovementDirection(_enemyBehaviour.CurrentRoamPosition - (Vector2)_enemyBehaviour.transform.position);
                _enemyBehaviour.SetAimPosition(_enemyBehaviour.CurrentRoamPosition);
            }
            else
            {
                _enemyBehaviour.SetRoamPosition(Vector2.zero);
                _enemyBehaviour.SetMovementDirection(Vector2.zero);
            }

            _nodeState = ENodeState.Running;
            return _nodeState;
        }
    }

}