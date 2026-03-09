using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastBullet1 : BaseSkill
{
    public override void SkillEnable()
    {

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData != null)
        {
            PlayerData.Instance.AttackspeedAmplification += itemData.Amplification;
        }
    }
}
