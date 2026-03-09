using UnityEngine;

/// <summary>
/// 雷球2：增加雷球环绕速度（读取 Amplification 字段）
/// </summary>
public class ElectricitySkill2 : BaseSkill
{
    public override void SkillEnable()
    {
        ElectricitySkillEffect Effect = (ElectricitySkillEffect)PlayerController.Attack.GetAttack<ElectricitySkillEffect>();
        if (Effect == null) { Debug.LogError("[ElectricitySkill2] 找不到 ElectricitySkillEffect，请先启用雷球技能"); return; }

        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[ElectricitySkill2] 找不到技能数据，SkillData.id={Data.id}"); return; }

        Effect.RotateSpeed += itemData.Amplification;
    }
}
