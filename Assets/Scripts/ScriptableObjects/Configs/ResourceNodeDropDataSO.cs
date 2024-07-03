using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource Node Drop List")]
public class ResourceNodeDropDataSO : ScriptableObject
{
    public List<NodeResourceDropDataStruct> nodeResourceDropDataList;
}
