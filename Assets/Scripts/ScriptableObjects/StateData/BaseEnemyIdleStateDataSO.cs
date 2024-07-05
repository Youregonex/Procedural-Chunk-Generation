using UnityEngine;

[CreateAssetMenu(menuName = "State Data/Idle State Data")]
public class BaseEnemyIdleStateDataSO : ScriptableObject
{
    [field: SerializeField] public float TimeToStartRoamMin { get; private set; }
    [field: SerializeField] public float TimeToStartRoamMax { get; private set; }
    [field: SerializeField] public Vector2 RoamPositionOffsetMax { get; private set; }
}
