using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBulletSkill : BaseSkill
{
    public override void SkillEnable()
    {
        PlayerController.Attack.AttackType="PongBullet";
    }
}
