using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillData", menuName = "ScriptableObjects/Skill Data", order = 51)]
public class SkillData : ScriptableObject
{
    public int id;
    public Sprite icon;
    public string skillName;
    public string description;
    public bool CanExtractInStart;
    /// <summary>被 Enable 后不可再被抽取（适用于一次性技能）</summary>
    public bool OnlyEnableOnce;
    public SkillType skillType;
    public SkillData[] PreliminaryDatas = new SkillData[5];
}
public enum SkillType
{
    通用,
    远程,
    近程,
    普攻
}
