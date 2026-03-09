using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : BaseSingleton<PlayerData>
{
    public float AttackspeedAmplification;
    public float AttackPowerAmplification;
    public float MoveSpeedAmplification;
    public float RemoteskillsAmplification;
    public float ShortrangeSkillAmplification;
    public float NormalAttackAmplification;

    public void SetAttackSpeed(float value) => AttackspeedAmplification += value;
    public void SetAttackPower(float value) => AttackPowerAmplification += value;
    public void SetMoveSpeed(float value) => MoveSpeedAmplification += value;
    public void SetRemoteSkills(float value) => RemoteskillsAmplification += value;
    public void SetShortrangeSkill(float value) => ShortrangeSkillAmplification += value;
    public void SetNormalAttack(float value) => NormalAttackAmplification += value;
    public void Reset()
    {
        AttackspeedAmplification = 0;
        AttackPowerAmplification = 0;
        MoveSpeedAmplification = 0;
        RemoteskillsAmplification = 0;
        ShortrangeSkillAmplification = 0;
        NormalAttackAmplification = 0;
    }

    
}
