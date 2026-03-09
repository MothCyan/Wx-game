using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : State
{
    
    public void ChooseState(FSM fsm, StateType stateType)
    {

    }

    public void OnEnter(FSM fsm)
    {
        fsm.GetComponent<Collider2D>().enabled = false;
        fsm.StartCoroutine(DeadRoutine(fsm));
    }

    private IEnumerator DeadRoutine(FSM fsm)
    {
        yield return new WaitForSeconds(0f);
        OnExit(fsm);
    }

    public void OnExit(FSM fsm)
    {
        // 回收敌人对象
        
        PoolManager.Instance.RecycleObj("NormalEnemy",fsm.gameObject);
        
    }

    public void OnUpdate(FSM fsm)
    {

    }
}
