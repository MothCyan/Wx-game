using UnityEngine;

/// <summary>
/// 水魔法1：增加水魔法百分之20伤害（修改 WaterSkillEffect.HurtAmplification）
/// </summary>
public class WaterSkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        WaterSkillEffect Effect = (WaterSkillEffect)PlayerController.Attack.GetAttack<WaterSkillEffect>();
        if (Effect == null) { Debug.LogError("[WaterSkill1] 找不到 WaterSkillEffect，请先启用水魔法技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[WaterSkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.HurtAmplification += itemData.Amplification;
    }
}
