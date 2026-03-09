using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
[Preserve]
public class PlayerAttack : MonoBehaviour
{
    public GameObject TargetPosition;
    public GameObject FirePosition;
    public List<IAttack> attacks = new List<IAttack>(10);
    
    // 攻击速度（每秒发射的子弹数量）
    public float AttackSpeed = 1f;
    
    // 攻击计时器
    private float attackTimer = 0f;
    public string AttackType = "NormalBullet";
    
    void Attack()
    {
        // 检查是否有目标
        if (TargetPosition == null)
        {
            return;
        }
        foreach (IAttack a in attacks)
        {
            a.target = TargetPosition;
        }
        
        // 实例化子弹预制体
        GameObject bullet = PoolManager.Instance.GetObj(AttackType);

        if (bullet == null)
        {
            Debug.LogWarning("子弹对象池还未准备好");
            return;
        }

    
        
        // 设置子弹位置
        bullet.transform.position = FirePosition.transform.position;
        
        // 计算朝向目标的方向（2D，在 XY 平面）
        Vector2 direction = (TargetPosition.transform.position - transform.position).normalized;
        
        // 计算角度并设置 Z 轴旋转（减90度使 transform.up 指向目标）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public IAttack GetAttack<T>() where T : IAttack
    {
        foreach (var attack in attacks)
        {
            if (attack is T)
                return attack;
        }
        return default;
    }
    
    
    
    void Update()
    {

        // 累加计时器
        attackTimer += Time.deltaTime;
        
        // 计算发射间隔：基础间隔 / 攻速倍率，倍率越大间隔越短
        float finalAttackSpeed = AttackSpeed * (1f + PlayerData.Instance.AttackspeedAmplification);
        
        // 攻速为负数时不发射
        if (finalAttackSpeed <= 0) return;
        
        float attackInterval = 1f / finalAttackSpeed;
        
        // 当计时器达到发射间隔时，发射子弹
        if (attackTimer >= attackInterval)
        {
            Attack();
            attackTimer = 0f; // 重置计时器
        }
        
        
    }
}
