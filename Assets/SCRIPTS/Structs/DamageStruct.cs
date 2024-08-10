using UnityEngine;

public struct DamageStruct
{
    public GameObject damageSender;
    public EFactions senderFaction;
    public float damageAmount;
    public float knockbackForce;

    public DamageStruct(GameObject damageSender, EFactions senderFaction, float damageAmount, float knockbackForce)
    {
        this.damageSender = damageSender;
        this.senderFaction = senderFaction;
        this.damageAmount = damageAmount;
        this.knockbackForce = knockbackForce;
    }
}
