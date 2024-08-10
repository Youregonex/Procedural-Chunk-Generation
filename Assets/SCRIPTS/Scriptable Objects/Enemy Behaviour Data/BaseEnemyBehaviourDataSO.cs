using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data/Behaviour")]
public class BaseEnemyBehaviourDataSO : ScriptableObject
{
    [field: SerializeField] public float AttackRangeMax { get; private set; }
    [field: SerializeField] public float AttackRangeMin { get; private set; }
    [field: SerializeField] public float AggroRange { get; private set; }
    [field: SerializeField] public float ChaseRange { get; private set; }
    [field: SerializeField] public float CombatRange { get; private set; }

    [field: SerializeField] public float AttackDelayMin { get; private set; }
    [field: SerializeField] public float AttackDelayMax { get; private set; }

    [field: SerializeField] public float TimeToStartRoamMax { get; private set; }
    [field: SerializeField] public float TimeToStartRoamMin { get; private set; }
    [field: SerializeField] public Vector2 RoamPositionOffsetMax { get; private set; }
    [field: SerializeField] public float RoamTimeMax { get; private set; }
}
