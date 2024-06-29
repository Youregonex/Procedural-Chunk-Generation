using System;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum
{
    public BaseState(EState key)
    {
        StateKey = key;
    }

    public EState StateKey { get; private set; }
    public float StateStartTime { get; private set; }
    public float StateCurrentTime => Time.time - StateStartTime;

    public virtual void EnterState()
    {
        StateStartTime = Time.time;
    }

    public virtual void ExitState() { }
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public virtual void OnTriggerEnter2D(Collider2D collision) { }
    public virtual void OnTriggerStay2D(Collider2D collision) { }
    public virtual void OnTriggerExit2D(Collider2D collision) { }
}
