using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemDataSO : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [TextArea(5, 50)]
    public string Description;
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Transform Prefab { get; private set; }
    [field: SerializeField] public int MaxStackSize { get; private set; }
}
