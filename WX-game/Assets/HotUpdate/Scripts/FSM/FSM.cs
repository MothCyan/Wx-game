using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    private State currentState;
    public EnemyType enemyType;
    public StateType currentStateType;
    public Blackboard blackboard = new Blackboard();
    public Action<FSM> OnValueChanged;


    // 移动数据
    [HideInInspector] public Vector3 moveStartPosition;
    [HideInInspector] public float moveDirection = 1f;

    void Start()
    {
        Init();
    }
    public virtual void Init()
    {
        currentState = StateManager.Instance.enemyStates[enemyType][currentStateType];
        currentState.OnEnter(this);
    }
    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate(this);
        }
    }
    public void ChangeState(StateType stateType)
    {
        StateManager.Instance.TryGetState(enemyType, stateType, out State newState);
        if (currentState == newState)
            return;

        currentState?.OnExit(this);
        currentState = newState;
        currentState?.OnEnter(this);
    }
    public void GetBlackboardValue<T>(string key, out T value)
    {
        value = blackboard.GetValue<T>(key);
    }
    public void SetBlackboardValue<T>(string key, T value)
    {
        blackboard.SetValue(key, value);
        OnValueChanged?.Invoke(this);
    }
    
}