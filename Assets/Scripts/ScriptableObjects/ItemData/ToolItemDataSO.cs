using UnityEngine;

[CreateAssetMenu(menuName = "Tool Data")]
public class ToolItemDataSO : ItemDataSO
{
    [field: SerializeField] public Tool ToolPrefab { get; private set; }
    [field: SerializeField] public float AttackRadius { get; private set; }
    [field: SerializeField] public float AttackDamageMin { get; private set; }
    [field: SerializeField] public float AttackDamageMax { get; private set; }
    [field: SerializeField] public float KnockbackForce { get; private set; }
    [field: SerializeField] public float AttackCooldown { get; private set; }
    [field: SerializeField] public EToolType ToolType { get; private set; }
    [field: SerializeField] public int ToolTier { get; private set; }
    [field: SerializeField] public int TicksPerHitMin { get; private set; }
    [field: SerializeField] public int TicksPerHitMax { get; private set; }
}