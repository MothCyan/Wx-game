using UnityEngine;

/// <summary>
/// 圣剑1：增加圣剑百分之10的伤害（修改 SwordSkillEffect.HurtAmplification）
/// </summary>
public class SwordSkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        SwordSkillEffect Effect = (SwordSkillEffect)PlayerController.Attack.GetAttack<SwordSkillEffect>();
        if (Effect == null) { Debug.LogError("[SwordSkill1] 找不到 SwordSkillEffect，请先启用圣剑技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[SwordSkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.HurtAmplification += itemData.Amplification;
    }
}
