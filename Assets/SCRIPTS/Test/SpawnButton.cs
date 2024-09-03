using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    public ItemDataSO ButtonItemDataSO { get; private set; }

    public void SetItemDataSO(ItemDataSO itemDataSO)
    {
        ButtonItemDataSO = itemDataSO;
    }
}
