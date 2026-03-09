using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSkillEffect : MonoBehaviour, IAttack
{
    
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; }
    private float timer = 0f;

    void Start()
    {
        PlayerController.Attack.attacks.Add(this);
        AttackSpeed = 2f; // 默认间隔，会被表数据覆盖
    }

    void Update()
    {
        if (AttackSpeed <= 0) return; // 防止间隔为0时每帧触发
        timer += Time.deltaTime;
        if (timer >= AttackSpeed - Cooldownreduction)
        {
            if (Attack())
                timer = 0f;
        }
    }

    private Vector3 baseBallScale = Vector3.zero; // 水球 prefab 原始 scale，首次记录

    public bool Attack()
    {
        // target 为空或已被回收（inactive）时跳过
        if (target == null || !target.activeInHierarchy)
            return false;

        // 实例化水球预制体
        GameObject waterBall = PoolManager.Instance.GetObj("Skill1");
        if (waterBall == null) return false;

        // 首次记录 prefab 原始 scale，后续以此为基准，避免累积误差
        if (baseBallScale == Vector3.zero)
            baseBallScale = waterBall.transform.localScale;

        // 每次都从原始 scale 基础上乘以倍率，而不是读取当前 localScale
        waterBall.transform.localScale = baseBallScale * (1f + Scale);

        waterBall.transform.position = target.transform.position;
        return true;
    }
}
