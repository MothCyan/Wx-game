using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSkillEffect : MonoBehaviour, IAttack
{
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    public float Cooldownreduction { get; set; } = 0.5f;

    private float timer = 0f;

    void Start()
    {
        PlayerController.Attack.attacks.Add(this);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= AttackSpeed-Cooldownreduction)
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
        Skill32.transform.localScale = new Vector3(Skill3.transform.localScale.x + Skill3.transform.localScale.x * Scale, Skill3.transform.localScale.y + Skill3.transform.localScale.y * Scale, 1);
        Skill3.transform.localScale = new Vector3(Skill3.transform.localScale.x + Skill3.transform.localScale.x * Scale, Skill3.transform.localScale.y + Skill3.transform.localScale.y * Scale, 1);
        Skill32.transform.position = transform.position;
        Skill3.transform.position = transform.position;
        Skill32.transform.rotation = Quaternion.Euler(0, 0, 230);
        return true;

    }
}
