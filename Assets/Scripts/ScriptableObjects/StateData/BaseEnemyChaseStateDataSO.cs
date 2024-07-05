using UnityEngine;

[CreateAssetMenu(menuName = "State Data/Chase State Data")]
public class BaseEnemyChaseStateDataSO : ScriptableObject
{
    [field: SerializeField] public float AggroRange { get; private set; }
    [field: SerializeField] public float ChaseRange { get; private set; }
    [field: SerializeField] public float CombatRange { get; private set; }
}
