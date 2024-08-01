using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Action Item Data")]
public class ActionItemDataSO : ItemDataSO
{
    public event Action OnActionItemUsed;

    public void OnActionItemUsedInvoke()
    {
        OnActionItemUsed?.Invoke();
    }
}
