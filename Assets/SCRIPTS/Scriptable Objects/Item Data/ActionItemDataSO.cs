using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Action Item Data")]
public abstract class ActionItemDataSO : ItemDataSO
{
    public event Action OnActionItemUsed;

    public void OnActionItemUsedInvoke()
    {
        OnActionItemUsed?.Invoke();
    }

    public virtual void Use(AgentCoreBase user)
    {
        OnActionItemUsed?.Invoke();
    }
}
