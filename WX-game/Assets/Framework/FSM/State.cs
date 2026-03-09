using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    public abstract void OnEnter(FSM fsm);
    public abstract void OnUpdate(FSM fsm);
    public abstract void OnExit(FSM fsm);
    public abstract void ChooseState(FSM fsm, StateType stateType);
}

public enum StateType
{
    None,
    Idle,
    Move,
    Attack,
    Dead
}
public enum EnemyType
{
    None,
    NormalEnemy,
    Bat
}
