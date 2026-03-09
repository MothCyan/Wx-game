using UnityEngine;

/// <summary>
/// 浪潮魔法1：增加浪潮魔法百分之20伤害（修改 WaveSkillEffect.HurtAmplification）
/// </summary>
public class WaveSkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        WaveSkillEffect Effect = (WaveSkillEffect)PlayerController.Attack.GetAttack<WaveSkillEffect>();
        if (Effect == null) { Debug.LogError("[WaveSkill1] 找不到 WaveSkillEffect，请先启用浪潮魔法技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[WaveSkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.HurtAmplification += itemData.Amplification;
    }
}
