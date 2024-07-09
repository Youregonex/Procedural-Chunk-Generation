using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability Data")]
public class AbilityDataSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public EAbilityType AbilityType { get; protected set; }
    [field: SerializeField] public float Cooldown { get; private set; }
}
