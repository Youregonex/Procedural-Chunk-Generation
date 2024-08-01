using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drop/Drop List")]
public class DropList : ScriptableObject
{
    public List<ItemDropDataStruct> itemDropList;
}
