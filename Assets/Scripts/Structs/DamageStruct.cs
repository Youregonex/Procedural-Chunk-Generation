using UnityEngine;

public struct DamageStruct : IImpactStruct
{
    public GameObject damageSender;
    public EFactions senderFaction;
    public float damageAmount;
    public float knockbackForce;
}
