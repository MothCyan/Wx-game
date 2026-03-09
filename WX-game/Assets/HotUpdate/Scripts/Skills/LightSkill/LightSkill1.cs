using UnityEngine;

/// <summary>
/// 光柱1：增加光柱伤害（读取 Amplification 字段叠加到全局增伤）
/// </summary>
public class LightSkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[LightSkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        PlayerData.Instance.AttackPowerAmplification += itemData.Amplification;
    }
}
