using UnityEngine;

[CreateAssetMenu(menuName = "State Data/Attack State Data")]
public class BaseEnemyAttackStateDataSO : ScriptableObject
{
    [field: SerializeField] public float AttackRangeMin { get; private set; }
    [field: SerializeField] public float AttackRangeMax { get; private set; }
    [field: SerializeField] public float AttackDelayMin { get; private set; }
    [field: SerializeField] public float AttackDelayMax { get; private set; }
}
