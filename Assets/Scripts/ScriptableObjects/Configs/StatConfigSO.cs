using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Stat Config")]
public class StatConfigSO : ScriptableObject
{
    public List<Stat> stats;
}
