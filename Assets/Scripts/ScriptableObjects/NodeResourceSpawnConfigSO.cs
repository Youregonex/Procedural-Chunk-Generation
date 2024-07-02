using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Node Resource Spawn Config")]
public class NodeResourceSpawnConfigSO : ScriptableObject
{
    public List<ResourceNodeSpawnStruct> nodeResourceSpawn;
}
