using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> _stateDictionary = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> _currentState;
    protected bool _isTransitioningState = false;

    private void Start()
    {
        _currentState.EnterState();
    }

    private void Update()
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

    public void TransitionState(EState stateKey)
    {
        _isTransitioningState = true;
        _currentState.ExitState();
        _currentState = _stateDictionary[stateKey];
        _currentState.EnterState();
        _isTransitioningState = false;
    }

    protected abstract void InitializeStates();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _currentState.OnTriggerEnter2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);
    }
}
