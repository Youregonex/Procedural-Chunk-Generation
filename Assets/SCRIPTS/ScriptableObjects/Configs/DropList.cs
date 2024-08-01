using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drop/Drop List")]
public class DropList : ScriptableObject
{
    [field: SerializeField] public List<ItemDropDataStruct> ItemDropList { get; private set; }
}
