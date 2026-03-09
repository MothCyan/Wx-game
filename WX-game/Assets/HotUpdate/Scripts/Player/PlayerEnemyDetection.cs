using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
[Preserve]
public class PlayerEnemyDetection : MonoBehaviour
{
    public PlayerAttack attack;

    
    // 检测范围
    public float DetectionRadius = 10f;
    
    // 检测间隔（优化性能，不用每帧检测）
    public float DetectionInterval = 0.2f;
    
    private float detectionTimer = 0f;
    
    void Start()
    {
        attack = GetComponent<PlayerAttack>();
    }
    
    void Update()
    {
        detectionTimer += Time.deltaTime;
        
        if (detectionTimer >= DetectionInterval)
        {
            detectionTimer = 0f;
            FindNearestEnemy();
        }
    }
    
    void FindNearestEnemy()
    {
        // 在范围内查找所有带 "Enemy" 标签的物体
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, DetectionRadius);
        
        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider2D col in enemies)
        {
            if (col.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = col.gameObject;
                }
            }
        }
        
        // 设置最近的敌人为攻击目标
        if (nearestEnemy != null)
        {
            attack.TargetPosition = nearestEnemy;
            
        }
        else
        {
            attack.TargetPosition = null;
        }
    }
    
    // 在编辑器中显示检测范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }
}
