using System;

public interface IWeapon
{
    public event Action OnAttackStarted;
    public event Action OnAttackFinished;

    public void Attack();
}
