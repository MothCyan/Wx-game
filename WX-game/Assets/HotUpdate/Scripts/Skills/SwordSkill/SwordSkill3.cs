using UnityEngine;

/// <summary>
/// 圣剑3：减少圣剑百分之10的释放间隔（读取 Cooldownreduction 字段）
/// </summary>
public class SwordSkill3 : BaseSkill
{
    public override void SkillEnable()
    {
        SwordSkillEffect Effect = (SwordSkillEffect)PlayerController.Attack.GetAttack<SwordSkillEffect>();
        if (Effect == null) { Debug.LogError("[SwordSkill3] 找不到 SwordSkillEffect，请先启用圣剑技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[SwordSkill3] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Cooldownreduction += itemData.Cooldownreduction;
    }
}
