using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillEffect : MonoBehaviour, IAttack
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; }
    public float HurtAmplification { get; set; }  // 圣剑专属增伤倍率

    private float timer = 0f;
    private Vector3 baseSwordScale = Vector3.zero;

    void Start()
    {
        PlayerController.Attack.attacks.Add(this);
        AttackSpeed = 2.5f;
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

        GameObject Skill9 = PoolManager.Instance.GetObj("Skill9");
        if (Skill9 == null) return false;

        // 记录原始 scale，基于原始值计算，避免累积
        if (baseSwordScale == Vector3.zero)
            baseSwordScale = Skill9.transform.localScale;
        Skill9.transform.localScale = baseSwordScale * (1f + Scale);

        // 将专属增伤写入 Hurt 组件
        Hurt hurt = Skill9.GetComponent<Hurt>();
        if (hurt != null) hurt.SkillHurtAmplification = HurtAmplification;

        Skill9.transform.position = new Vector2(target.transform.position.x, target.transform.position.y + 2f);
        return true;
    }
}
