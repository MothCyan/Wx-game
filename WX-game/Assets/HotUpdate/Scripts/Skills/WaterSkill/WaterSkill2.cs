using UnityEngine;

/// <summary>
/// 水魔法2：增大水球大小（读取 Scale 字段）
/// </summary>
public class WaterSkill2 : BaseSkill
{
    public override void SkillEnable()
    {
        WaterSkillEffect Effect = (WaterSkillEffect)PlayerController.Attack.GetAttack<WaterSkillEffect>();
        if (Effect == null) { Debug.LogError("[WaterSkill2] 找不到 WaterSkillEffect，请先启用水魔法技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[WaterSkill2] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Scale += itemData.Scale;
    }
}
