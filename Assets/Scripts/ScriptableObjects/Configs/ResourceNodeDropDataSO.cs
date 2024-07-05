using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Resource Node Drop List")]
public class ResourceNodeDropDataSO : ScriptableObject
{
    public List<NodeResourceDropDataStruct> nodeResourceDropDataList;
}
