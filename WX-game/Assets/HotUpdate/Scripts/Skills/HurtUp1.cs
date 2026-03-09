using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtUp1 : BaseSkill
{
    public override void SkillEnable()
    {
        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData != null)
        {
            PlayerData.Instance.AttackPowerAmplification += itemData.Amplification;
        }
    }

    
}
