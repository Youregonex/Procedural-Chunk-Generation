using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Config/Stat Config")]
public class StatConfigSO : ScriptableObject
{
    public List<Stat> stats;
}
