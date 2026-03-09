using System;
using UnityEngine;

/// <summary>
/// 对应 demo_tbitem.json 的数据结构
/// 字段名必须与 json 的 key 完全一致（大小写敏感）
/// </summary>
[Serializable]
public class TbItem : ITableRow
{
    public int id;
    public string name;
    public string desc;
    public int count;

    // 实现 ITableRow 接口
    public int Id => id;
}

[Serializable]
public class TbItemWrapper
{
    public TbItem[] array;
}
