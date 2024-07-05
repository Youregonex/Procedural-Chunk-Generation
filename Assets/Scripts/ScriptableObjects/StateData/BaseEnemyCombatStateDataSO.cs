using UnityEngine;

[CreateAssetMenu(menuName = "State Data/Combat State Data")]
public class BaseEnemyCombatStateDataSO : ScriptableObject
{
    [field: SerializeField] public float AttackRangeMax { get; private set; }
    [field: SerializeField] public float AttackRangeMin { get; private set; }
    [field: SerializeField] public float AggroRange { get; private set; }
    [field: SerializeField] public float ChaseRange { get; private set; }
    [field: SerializeField] public float CombatRange { get; private set; }
}
