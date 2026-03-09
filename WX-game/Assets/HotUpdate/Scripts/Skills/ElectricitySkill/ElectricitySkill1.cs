using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雷球1：增加雷球百分之30的伤害（读取 Amplification 字段，对所有球的 BaseHurt 倍增）
/// </summary>
public class ElectricitySkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        ElectricitySkillEffect Effect = (ElectricitySkillEffect)PlayerController.Attack.GetAttack<ElectricitySkillEffect>();
        if (Effect == null) { Debug.LogError("[ElectricitySkill1] 找不到 ElectricitySkillEffect，请先启用雷球技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[ElectricitySkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        foreach (GameObject ball in Effect.Balls)
        {
            if (ball == null) continue;
            Hurt hurt = ball.GetComponent<Hurt>();
            if (hurt != null) hurt.BaseHurt *= (1f + itemData.Amplification);
        }
    }
}
