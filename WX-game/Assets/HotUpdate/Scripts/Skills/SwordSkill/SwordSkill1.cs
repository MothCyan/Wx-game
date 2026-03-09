using UnityEngine;

/// <summary>
/// 圣剑1：增加圣剑百分之10的伤害（读取 Amplification 字段叠加到全局增伤）
/// </summary>
public class SwordSkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[SwordSkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        PlayerData.Instance.AttackPowerAmplification += itemData.Amplification;
    }
}
