using UnityEngine;

/// <summary>
/// 光柱2：增大光柱大小（读取 Scale 字段）
/// </summary>
public class LightSkill2 : BaseSkill
{
    public override void SkillEnable()
    {
        LightSkillEffect Effect = (LightSkillEffect)PlayerController.Attack.GetAttack<LightSkillEffect>();
        if (Effect == null) { Debug.LogError("[LightSkill2] 找不到 LightSkillEffect，请先启用光柱技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[LightSkill2] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Scale += itemData.Scale;
    }
}
