using UnityEngine;
using UnityEngine.UI;

public class UIComponentProvider : MonoBehaviour
{
    public static UIComponentProvider Instance;

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
