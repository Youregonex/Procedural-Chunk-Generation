using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drop/Drop List")]
public class DropListDataSO : ScriptableObject
{
    [field: SerializeField] public List<ItemDropDataStruct> ItemDropList { get; private set; }
    [field: SerializeField] public bool DropOneFromList { get; private set; }
}
