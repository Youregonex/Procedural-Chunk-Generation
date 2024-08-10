using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Node Resource Spawn Config")]
public class NodeResourceSpawnConfigSO : ScriptableObject
{
    [field: SerializeField] public List<ResourceNodeSpawnStruct> NodeResourceSpawn { get; private set; }
}
