using UnityEngine;

namespace Youregone.BehaviourTrees
{
    public class IdleToRoamNode : Node
    {
        public IdleToRoamNode(BaseEnemyBehaviour enemyBehaviour, float timeBetweenRoamMin, float timeBetweenRoamMax, Vector2 roamPositionOffsetMax, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
            _timeToStartRoamMin = timeBetweenRoamMin;
            _timeToStartRoamMax = timeBetweenRoamMax;
            _roamPositionOffsetMax = roamPositionOffsetMax;

            _timeToStartRoamCurrent = Random.Range(_timeToStartRoamMin, _timeToStartRoamMax);
        }

        private BaseEnemyBehaviour _enemyBehaviour;

        private Vector2 _roamPositionOffsetMax;
        private float _timeToStartRoamMax;
        private float _timeToStartRoamMin;
        private float _timeToStartRoamCurrent;

        public override ENodeState Evaluate()
        {
            _enemyBehaviour.SetMovementDirection(Vector2.zero);

            _timeToStartRoamCurrent -= Time.deltaTime;

            if (_timeToStartRoamCurrent <= 0)
            {
                _timeToStartRoamCurrent = Random.Range(_timeToStartRoamMin, _timeToStartRoamMax);
                _enemyBehaviour.SetRoamPosition(GetRandomRoamPosition());
            }

            _nodeState = ENodeState.Running;
            return _nodeState;
        }

        private Vector2 GetRandomRoamPosition()
        {
            Vector2 randomOffset = new Vector2(Random.Range(-_roamPositionOffsetMax.x, _roamPositionOffsetMax.x),
                                               Random.Range(-_roamPositionOffsetMax.y, _roamPositionOffsetMax.y));

            return (Vector2)_enemyBehaviour.transform.position + randomOffset;
        }

    }

}