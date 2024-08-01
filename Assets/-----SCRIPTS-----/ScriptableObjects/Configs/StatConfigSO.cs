using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Configs/Stat Config")]
public class StatConfigSO : ScriptableObject
{
    public List<Stat> stats;
}
