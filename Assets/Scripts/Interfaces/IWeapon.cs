using System;

public interface IWeapon
{
    public event EventHandler OnAttackStarted;
    public event EventHandler OnAttackFinished;

    public void Attack();
}
