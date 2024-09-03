using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Tool Item Data")]
public class ToolItemDataSO : MeleeWeaponItemDataSO
{
    [field: SerializeField] public EToolType ToolType { get; private set; }
    [field: SerializeField] public int ToolTier { get; private set; }
    [field: SerializeField] public int TicksPerHitMin { get; private set; }
    [field: SerializeField] public int TicksPerHitMax { get; private set; }
}
