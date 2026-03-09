using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : State
{
    // 敌人移动速度
    public float MoveSpeed = 3f;
    
    public void ChooseState(FSM fsm, StateType stateType)
    {

    }

    public void OnEnter(FSM fsm)
    {
        fsm.GetComponent<Collider2D>().enabled = true;
        fsm.OnValueChanged += OnValueChanged;
    }

    public void OnExit(FSM fsm)
    {
        fsm.OnValueChanged -= OnValueChanged;
    }

    public void OnUpdate(FSM fsm)
    {
        // 获取敌人的 Transform
        Transform enemyTransform = fsm.GetComponent<Transform>();
        
        // 获取玩家位置
        Vector2 playerPos = PlayerMove.PlayerPosition;
        
        // 计算朝向玩家的方向
        Vector2 direction = (playerPos - (Vector2)enemyTransform.position).normalized;
        
        // 向玩家移动
        enemyTransform.position += (Vector3)direction * MoveSpeed * Time.deltaTime;

        // 根据移动方向翻转 X 轴
        if (direction.x > 0)
            enemyTransform.localScale = new Vector3(-Mathf.Abs(enemyTransform.localScale.x), enemyTransform.localScale.y, enemyTransform.localScale.z);
        else if (direction.x < 0)
            enemyTransform.localScale = new Vector3(Mathf.Abs(enemyTransform.localScale.x), enemyTransform.localScale.y, enemyTransform.localScale.z);
    }
    public void OnValueChanged(FSM fsm)
    {
        fsm.GetBlackboardValue<float>("HP", out float hp);
        if (hp <= 0)
        {
            fsm.ChangeState(StateType.Dead);
        }
    }
}
