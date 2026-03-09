using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSkillEffect : MonoBehaviour, IAttack
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    private float timer = 0f;
     public float Cooldownreduction { get; set; }

    void Start()
    {
        PlayerController.Attack.attacks.Add(this);
        AttackSpeed = 1.5f;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= AttackSpeed-Cooldownreduction)
        {
            if (Attack())
                timer = 0f;

        }
    }

    public bool Attack()
    {
        if (target == null || !target.activeInHierarchy)
            return false;

        GameObject Skill8 = PoolManager.Instance.GetObj("Skill8");
        if (Skill8 == null) return false;

        // 对象池回收时已重置为原始 Scale，直接在原始基础上增大
        Skill8.transform.localScale *= (1f + Scale);
        Skill8.transform.position = target.transform.position;
        return true;
    }
}
