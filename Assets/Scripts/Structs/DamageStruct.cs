using UnityEngine;

public struct DamageStruct : IImpactStruct
{
    public GameObject damageSender;
    public float damageAmount;
    public float knockbackForce;
}
