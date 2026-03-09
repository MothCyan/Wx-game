using UnityEngine;

/// <summary>
/// 雷球3：增加一个环绕自身的雷球（调用 CreateBalls 新增一颗）
/// </summary>
public class ElectricitySkill3 : BaseSkill
{
    public override void SkillEnable()
    {
        ElectricitySkillEffect Effect = (ElectricitySkillEffect)PlayerController.Attack.GetAttack<ElectricitySkillEffect>();
        if (Effect == null) { Debug.LogError("[ElectricitySkill3] 找不到 ElectricitySkillEffect，请先启用雷球技能"); return; }

        Effect.CreateBalls();
    }
}
