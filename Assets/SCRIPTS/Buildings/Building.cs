using UnityEngine;
using System;

public class Building : MonoBehaviour
{
    public event Action<Building> OnBuildingDestruction;

    protected virtual void OnDestroy()
    {
        OnBuildingDestruction?.Invoke(this);
    }
}
