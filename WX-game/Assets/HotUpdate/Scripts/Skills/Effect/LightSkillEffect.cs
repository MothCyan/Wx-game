using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSkillEffect : MonoBehaviour, IAttack
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; }
    public float HurtAmplification { get; set; }  // 光柱专属增伤倍率

    private float timer = 0f;
    private Vector3 baseLightScale = Vector3.zero;

    void Start()
    {
        PlayerController.Attack.attacks.Add(this);
        AttackSpeed = 1.5f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= AttackSpeed - Cooldownreduction)
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

        if (baseLightScale == Vector3.zero)
            baseLightScale = Skill8.transform.localScale;

        Skill8.transform.localScale = baseLightScale * (1f + Scale);

        Hurt hurt = Skill8.GetComponent<Hurt>();
        if (hurt != null) hurt.SkillHurtAmplification = HurtAmplification;

        Skill8.transform.position = target.transform.position;
        return true;
    }
}
