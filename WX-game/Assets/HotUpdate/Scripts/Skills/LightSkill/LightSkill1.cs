using UnityEngine;

/// <summary>
/// 光柱1：增加光柱伤害（修改 LightSkillEffect.HurtAmplification）
/// </summary>
public class LightSkill1 : BaseSkill
{
    public override void SkillEnable()
    {
        LightSkillEffect Effect = (LightSkillEffect)PlayerController.Attack.GetAttack<LightSkillEffect>();
        if (Effect == null) { Debug.LogError("[LightSkill1] 找不到 LightSkillEffect，请先启用光柱技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[LightSkill1] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.HurtAmplification += itemData.Amplification;
    }
}
