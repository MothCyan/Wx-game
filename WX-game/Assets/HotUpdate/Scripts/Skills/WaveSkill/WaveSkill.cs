using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSkill : BaseSkill
{
    public override void SkillEnable()
    {
        WaveSkillEffect Effect = PlayerController.Player.AddComponent<WaveSkillEffect>();
        SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", Data.id);
        if (itemData == null) { Debug.LogError($"[WaveSkill] 找不到技能数据，SkillData.id={Data.id}，请在 Inspector 中设置正确的 id"); return; }
        Effect.AttackSpeed = itemData.AttackSpeed;
        Effect.Cooldownreduction = itemData.Cooldownreduction;
        Effect.Scale = itemData.Scale;

    }
}
