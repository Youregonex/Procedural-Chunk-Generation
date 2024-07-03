using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseStateMachine<EState> where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> _stateDictionary = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> _currentState;
    protected bool _isTransitioningState = false;

    public void Update()
    {
        EState nextStateKey = _currentState.GetNextState();

        if(!_isTransitioningState && nextStateKey.Equals(_currentState.StateKey))
        {
            _currentState.UpdateState();
        }
        else if(!_isTransitioningState)
        {
            TransitionState(nextStateKey);
        }
    }

    public void SetStartingState(EState stateKey)
    {
        _currentState = _stateDictionary[stateKey];
    }

    protected virtual void TransitionState(EState stateKey)
    {
        _isTransitioningState = true;
        _currentState.ExitState();
        _currentState = _stateDictionary[stateKey];
        _currentState.EnterState();
        _isTransitioningState = false;
    }

    public void InitializeState(EState stateKey, BaseState<EState> baseState)
    {
        _stateDictionary.Add(stateKey, baseState);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        _currentState.OnTriggerEnter2D(collision);
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);
    }
}
