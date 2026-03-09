using UnityEngine;

/// <summary>
/// 浪潮魔法3：减少浪潮魔法0.3秒释放间隔（读取 Cooldownreduction 字段）
/// </summary>
public class WaveSkill3 : BaseSkill
{
    public override void SkillEnable()
    {
        WaveSkillEffect Effect = (WaveSkillEffect)PlayerController.Attack.GetAttack<WaveSkillEffect>();
        if (Effect == null) { Debug.LogError("[WaveSkill3] 找不到 WaveSkillEffect，请先启用浪潮魔法技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[WaveSkill3] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.Cooldownreduction += itemData.Cooldownreduction;
    }
}
