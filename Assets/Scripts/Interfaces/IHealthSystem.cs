using System;

public interface IHealthSystem
{
    public event Action<IImpactStruct> OnDamageTaken;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    public void TakeDamage(IImpactStruct impactStruct);
}
