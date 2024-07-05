using UnityEngine;

[CreateAssetMenu(menuName = "State Data/Roam State Data")]
public class BaseEnemyRoamStateDataSO : ScriptableObject
{
    [field: SerializeField] public float RoamTimeMax { get; private set; }
}
