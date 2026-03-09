using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

/// <summary>
/// 通用数据管理器：加载 JSON 文件并按 id 缓存，供任意对象查询
/// 使用方式：
///   1. 在游戏初始化时调用 DataManager.Instance.LoadTable("demo_tbitem", callback)
///   2. 任意对象调用 DataManager.Instance.GetData<TbItem>(1001) 获取数据
/// </summary>
public class DataManager : BaseSingleton<DataManager>
{
    // key: 表名，value: (key: id, value: 反序列化后的对象)
    private Dictionary<string, Dictionary<int, object>> tables= new Dictionary<string, Dictionary<int, object>>();

    /// <summary>
    /// 加载一张 JSON 表（协程版）
    /// tableName: YooAsset 中的资源名，例如 "demo_tbitem"
    /// parseFunc: 将 json 字符串解析为数组的函数，用于绕开 JsonUtility 泛型限制
    ///   例如：json => JsonUtility.FromJson&lt;SkillsDataWrapper&gt;("{\"array\":" + json + "}").array
    /// </summary>
    public IEnumerator LoadTable<T>(string tableName, System.Func<string, T[]> parseFunc) where T : ITableRow
    {
        if (tables.ContainsKey(tableName))
        {
            Debug.Log($"[DataManager] {tableName} 已加载，跳过");
            yield break;
        }

        AssetHandle handle = GameRoot.Instance.Package.LoadAssetAsync<TextAsset>(tableName);
        yield return handle;

        if (handle.AssetObject == null)
        {
            Debug.LogError($"[DataManager] 加载失败: {tableName}");
            yield break;
        }

        string json = (handle.AssetObject as TextAsset).text;
        T[] rows = parseFunc(json);

        if (rows == null || rows.Length == 0)
        {
            Debug.LogError($"[DataManager] {tableName} 解析结果为空，请检查 JSON 格式和数据类定义");
            yield break;
        }

        var dict = new Dictionary<int, object>();
        foreach (var row in rows)
            dict[row.Id] = row;

        tables[tableName] = dict;
        Debug.Log($"[DataManager] {tableName} 加载完成，共 {rows.Length} 条");
    }

    /// <summary>
    /// 按 id 获取数据，找不到返回 default
    /// </summary>
    public T GetData<T>(string tableName, int id) where T : class
    {
        if (!tables.TryGetValue(tableName, out var dict))
        {
            Debug.LogWarning($"[DataManager] 表 {tableName} 未加载");
            return null;
        }
        if (!dict.TryGetValue(id, out object obj))
        {
            Debug.LogWarning($"[DataManager] {tableName} 中找不到 id={id}");
            return null;
        }
        return obj as T;
    }

    /// <summary>
    /// 判断某张表是否已加载
    /// </summary>
    public bool IsLoaded(string tableName) => tables.ContainsKey(tableName);
}

/// <summary>所有数据行必须实现此接口，提供 Id 字段</summary>
public interface ITableRow
{
    int Id { get; }
}

/// <summary>解析顶层 JSON 数组的工具类</summary>
public static class JsonHelper
{
    public static T[] FromJsonArray<T>(string json)
    {
        // JsonUtility 不支持泛型包装类，改用 Newtonsoft.Json
        // 如果没有引入 Newtonsoft.Json，则通过手动拼接 wrapper 对象
        // 注意：此方法要求 T 必须是 [Serializable] 且只有公共字段
        string wrapped = "{\"array\":" + json + "}";
        var wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(wrapped);
        if (wrapper == null || wrapper.array == null)
        {
            Debug.LogError($"[JsonHelper] 解析失败，请确保数据类标记了 [Serializable] 且字段为公共字段（非属性）");
            return new T[0];
        }
        return wrapper.array;
    }

    [System.Serializable]
    private class JsonArrayWrapper<T>
    {
        public T[] array;
    }
}
