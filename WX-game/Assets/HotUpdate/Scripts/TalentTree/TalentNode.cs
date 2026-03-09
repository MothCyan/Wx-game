using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentNode : MonoBehaviour
{
    public TalentNode[] nextNodes;
    public bool CanEnable;
    public int skillId;
    public string description;
    //攻击速度
    public float AttackspeedAmplification;
    //攻击力
    public float AttackPowerAmplification;
    //移动速度
    public float MoveSpeedAmplification;
    //远程技能加成
    public float RemoteskillsAmplification;
    //近程技能加成
    public float ShortrangeSkillAmplification;
    //普通攻击加成
    public float NormalAttackAmplification;
    void Start()
    {
        CanEnable = PlayerPrefs.GetInt("TalentNode" + skillId, 0) == 1;
    }
    public void EnableEffect()
    {
        foreach (var node in nextNodes)
        {
            node.CanEnable = true;
            PlayerPrefs.SetInt("TalentNode" + node.skillId, 1);
        }
        PlayerData.Instance.SetAttackSpeed(PlayerData.Instance.AttackspeedAmplification + AttackspeedAmplification);
        PlayerData.Instance.SetAttackPower(PlayerData.Instance.AttackPowerAmplification + AttackPowerAmplification);
        PlayerData.Instance.SetMoveSpeed(PlayerData.Instance.MoveSpeedAmplification + MoveSpeedAmplification);
        PlayerData.Instance.SetRemoteSkills(PlayerData.Instance.RemoteskillsAmplification + RemoteskillsAmplification);
        PlayerData.Instance.SetShortrangeSkill(PlayerData.Instance.ShortrangeSkillAmplification + ShortrangeSkillAmplification);
        PlayerData.Instance.SetNormalAttack(PlayerData.Instance.NormalAttackAmplification + NormalAttackAmplification);
    }

}
