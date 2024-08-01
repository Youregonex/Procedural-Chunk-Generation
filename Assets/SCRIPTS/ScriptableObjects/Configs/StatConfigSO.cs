using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Configs/Stat Config")]
public class StatConfigSO : ScriptableObject
{
    [field: SerializeField] public List<Stat> StatList { get; private set; }
}
