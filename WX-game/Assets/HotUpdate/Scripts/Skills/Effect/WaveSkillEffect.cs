using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSkillEffect : MonoBehaviour, IAttack
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; } = 0.5f;
    public float HurtAmplification { get; set; }  // 浪潮魔法专属增伤倍率

    private float timer = 0f;
    private Vector3 baseWaveScale = Vector3.zero;

    void Start()
    {
        PlayerController.Attack.attacks.Add(this);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= AttackSpeed - Cooldownreduction)
        {
            timer = 0f;
            Attack();
        }
    }

    public bool Attack()
    {
        if (!PoolManager.Instance.CanGetObj) return false;
        GameObject Skill3 = PoolManager.Instance.GetObj("Skill3");
        GameObject Skill32 = PoolManager.Instance.GetObj("Skill3");
        if (Skill3 == null || Skill32 == null) return false;

        if (baseWaveScale == Vector3.zero)
            baseWaveScale = Skill3.transform.localScale;

        Vector3 newScale = baseWaveScale * (1f + Scale);
        Skill3.transform.localScale = newScale;
        Skill32.transform.localScale = newScale;

        // 写入专属增伤
        Hurt hurt3 = Skill3.GetComponent<Hurt>();
        if (hurt3 != null) hurt3.SkillHurtAmplification = HurtAmplification;
        Hurt hurt32 = Skill32.GetComponent<Hurt>();
        if (hurt32 != null) hurt32.SkillHurtAmplification = HurtAmplification;

        Skill32.transform.position = transform.position;
        Skill3.transform.position = transform.position;
        Skill32.transform.rotation = Quaternion.Euler(0, 0, 230);
        return true;
    }
}
