using UnityEngine;

/// <summary>
/// 圣剑2：增加圣剑百分之20的范围（读取 Scale 字段）
/// </summary>
public class SwordSkill2 : BaseSkill
{
    public override void SkillEnable()
    {
        SwordSkillEffect Effect = (SwordSkillEffect)PlayerController.Attack.GetAttack<SwordSkillEffect>();
        if (Effect == null) { Debug.LogError("[SwordSkill2] 找不到 SwordSkillEffect，请先启用圣剑技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[SwordSkill2] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Scale += itemData.Scale;
    }
}
