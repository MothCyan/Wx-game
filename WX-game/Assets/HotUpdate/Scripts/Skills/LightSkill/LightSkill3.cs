using UnityEngine;

/// <summary>
/// 光柱3：减少光柱冷却间隔（读取 Cooldownreduction 字段）
/// </summary>
public class LightSkill3 : BaseSkill
{
    public override void SkillEnable()
    {
        LightSkillEffect Effect = (LightSkillEffect)PlayerController.Attack.GetAttack<LightSkillEffect>();
        if (Effect == null) { Debug.LogError("[LightSkill3] 找不到 LightSkillEffect，请先启用光柱技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[LightSkill3] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Cooldownreduction += itemData.Cooldownreduction;
    }
}
