using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricitySkillEffect : MonoBehaviour, IAttack
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; }
    public List<GameObject> Balls { get; set; }
    public GameObject BallPrefab; // 旋转球预制体

    [Header("旋转设置")]
    public float OrbitRadius = 2.5f;   // 旋转半径
    public float RotateSpeed = 90f;  // 旋转速度（度/秒）

    private float currentAngle = 0f;
    public void Start()
    {
        
        
        Balls = new List<GameObject>(); // 初始化旋转球列表

        PlayerController.Attack.attacks.Add(this);
        AttackSpeed = 1f;
        CreateBalls();
    }
    public void CreateBalls()
    {
        GameObject ball = PoolManager.Instance.GetObj("Skill12");
        Balls.Add(ball);
    }

    void Update()
    {
        if (Balls == null || Balls.Count == 0) return;

        currentAngle += RotateSpeed * Time.deltaTime;

        float angleStep = 360f / Balls.Count;

        for (int i = 0; i < Balls.Count; i++)
        {
            if (Balls[i] == null) continue;

            float angle = (currentAngle + angleStep * i) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * OrbitRadius;
            float y = Mathf.Sin(angle) * OrbitRadius;

            Balls[i].transform.position = transform.position + new Vector3(x, y, 0f);
        }
    }

    public bool Attack()
    {
        return true;
    }
}

