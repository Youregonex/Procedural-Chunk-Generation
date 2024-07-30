using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class ChaseNode : Node
    {
        public ChaseNode(BaseEnemyBehaviour enemyBehaviour, float minAttackRange, float maxAttackRange, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
            _minAttackRange = minAttackRange;
            _maxAttackRange = maxAttackRange;
        }

        private BaseEnemyBehaviour _enemyBehaviour;
        private float _maxAttackRange;
        private float _minAttackRange;

        public override ENodeState Evaluate()
        {
            float distance = Vector2.Distance(_enemyBehaviour.transform.position, _enemyBehaviour.GetCurrentTargetTransform().position);

            if (distance > _maxAttackRange)
                _enemyBehaviour.SetMovementDirection(_enemyBehaviour.GetCurrentTargetTransform().position - _enemyBehaviour.transform.position);
            else if (distance < _minAttackRange)
                _enemyBehaviour.SetMovementDirection(_enemyBehaviour.transform.position - _enemyBehaviour.GetCurrentTargetTransform().position);

            else
                _enemyBehaviour.SetMovementDirection(Vector2.zero);

            _enemyBehaviour.SetAimPosition(_enemyBehaviour.GetCurrentTargetTransform().position);

            _nodeState = ENodeState.Running;
            return _nodeState;
        }
    }

}