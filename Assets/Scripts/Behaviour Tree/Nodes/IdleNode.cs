using UnityEngine;

public class IdleNode : Node
{
    public IdleNode(BaseEnemyBehaviour enemyBehaviour, float timeBetweenRoamMax, Vector2 roamPositionOffsetMax, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
        _timeBetweenRoamMax = timeBetweenRoamMax;
        _timeBetweenRoamCurrent = _timeBetweenRoamMax;
        _roamPositionOffsetMax = roamPositionOffsetMax;
    }

    private BaseEnemyBehaviour _enemyBehaviour;

    private Vector2 _roamPositionOffsetMax;
    private float _timeBetweenRoamMax;
    private float _timeBetweenRoamCurrent;
    private bool _isWaiting = true;

    public override ENodeState Evaluate()
    {
        Debug.Log($"{this} node active");
        _enemyBehaviour.SetMovementDirection(Vector2.zero);

        if(_isWaiting)
        {
            _timeBetweenRoamCurrent -= Time.deltaTime;

            if(_timeBetweenRoamCurrent <= 0)
            {
                _isWaiting = false;
                _timeBetweenRoamCurrent = _timeBetweenRoamMax;
                _enemyBehaviour.SetRoamPosition(GetRandomRoamPosition());
            }
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
