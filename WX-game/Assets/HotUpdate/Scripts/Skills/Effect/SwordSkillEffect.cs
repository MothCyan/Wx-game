using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillEffect : MonoBehaviour, IAttack
{
    
    public GameObject target { get; set; }
    public float AttackSpeed { get; set; }
    public float Scale { get; set; }
    private float timer = 0f;
     public float Cooldownreduction { get; set; }

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
        Skill9.transform.localScale = new Vector3(Skill9.transform.localScale.x + Skill9.transform.localScale.x * Scale, Skill9.transform.localScale.y + Skill9.transform.localScale.y * Scale, 1);
        Skill9.transform.position = new Vector2(target.transform.position.x, target.transform.position.y + 2f);
        return true;

    }
}
