using UnityEngine;

/// <summary>
/// 水魔法3：减少水魔法0.2秒的释放间隔（读取 Cooldownreduction 字段）
/// </summary>
public class WaterSkill3 : BaseSkill
{
    public override void SkillEnable()
    {
        WaterSkillEffect Effect = (WaterSkillEffect)PlayerController.Attack.GetAttack<WaterSkillEffect>();
        if (Effect == null) { Debug.LogError("[WaterSkill3] 找不到 WaterSkillEffect，请先启用水魔法技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[WaterSkill3] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Cooldownreduction += itemData.Cooldownreduction;
    }
}
