using System;
using UnityEngine;

[Serializable]
public class SkillsData : ITableRow
{
    // 字段名必须与 JSON key 完全一致（大小写）
    public int id;
    public string Name;
    public float BaseHurt;
    public float HurtRange;
    public float AttackSpeed;
    public float Scale;
    public float Cooldownreduction;
    public float Amplification;

    // 实现 ITableRow 接口
    public int Id => id;
}

/// <summary>
/// JsonUtility 不支持泛型包装类，必须为每种数据类型单独定义非泛型 Wrapper
/// </summary>
[Serializable]
public class SkillsDataWrapper
{
    public SkillsData[] array;
}
