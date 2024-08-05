using UnityEngine;

public class GameObjectUniqueID : MonoBehaviour
{
    [field: SerializeField] public string ID { get; private set; }

    private void Awake()
    {
        GenerateID();
    }

    private void GenerateID()
    {
        ID = $"{transform.position.sqrMagnitude}-{gameObject.name}-{transform.GetSiblingIndex()}";
    }
}
