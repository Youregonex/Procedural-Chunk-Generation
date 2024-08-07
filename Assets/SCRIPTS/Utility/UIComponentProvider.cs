using UnityEngine;

public class UIComponentProvider : MonoBehaviour
{
    public static UIComponentProvider Instance { get; private set; }

    [field: Header("Config")]
    [field: SerializeField] public DynamicInventoryDisplay CustomContainerDisplay { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
