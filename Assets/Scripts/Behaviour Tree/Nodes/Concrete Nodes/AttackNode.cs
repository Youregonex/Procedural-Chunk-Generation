using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class AttackNode : Node
    {
        public AttackNode(BaseEnemyBehaviour enemyBehaviour, float attackDelayMin, float attackDelayMax, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
            _attackDelayMin = attackDelayMin;
            _attackDelayMax = attackDelayMax;

            _attackDelayCurrent = Random.Range(_attackDelayMin, _attackDelayMax);
        }

        private BaseEnemyBehaviour _enemyBehaviour;
        private float _attackDelayMin;
        private float _attackDelayMax;
        private float _attackDelayCurrent;

        public override ENodeState Evaluate()
        {
            if (_attackDelayCurrent > 0)
            {
                _attackDelayCurrent -= Time.deltaTime;
            }
            else
            {
                _enemyBehaviour.SetMovementDirection(Vector2.zero);
                _enemyBehaviour.SetAimPosition(_enemyBehaviour.GetCurrentTargetTransform().position);
                _enemyBehaviour.TriggerAttack();
                _attackDelayCurrent = Random.Range(_attackDelayMin, _attackDelayMax);
            }

            _nodeState = ENodeState.Running;
            return _nodeState;
        }
    }
}
