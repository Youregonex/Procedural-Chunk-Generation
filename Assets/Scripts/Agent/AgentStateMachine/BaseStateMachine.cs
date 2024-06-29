using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> _stateDictionary = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> _currentState;
    protected bool _isTransitioningState = false;

    protected virtual void Start()
    {
        _currentState.EnterState();
    }

    protected virtual void Update()
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

    protected virtual void TransitionState(EState stateKey)
    {
        _isTransitioningState = true;
        _currentState.ExitState();
        _currentState = _stateDictionary[stateKey];
        _currentState.EnterState();
        _isTransitioningState = false;
    }

    protected abstract void InitializeStates();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        _currentState.OnTriggerEnter2D(collision);
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);
    }
}
