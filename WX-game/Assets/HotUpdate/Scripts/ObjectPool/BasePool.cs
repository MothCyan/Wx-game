using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class BasePool
{
    public string ObjectName;
    public AssetHandle Prefab;
    
    // 对象池队列
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    // 初始化池大小
    public int InitialPoolSize = 10;
    
    // 对象池父物体
    private Transform poolParent;

    // Prefab 的原始 Scale（首个对象实例化后记录）
    private Vector3 prefabOriginalScale = Vector3.one;
    private bool originalScaleRecorded = false;
    
    public BasePool(string objectName, int initialSize = 10)
    {
        ObjectName = objectName;
        InitialPoolSize = initialSize;
        
        // 创建对象池父物体
        GameObject poolObj = new GameObject($"Pool_{objectName}");
        poolParent = poolObj.transform;
        
        // 同步加载预制体（简化处理）
        if (GameRoot.Instance == null || GameRoot.Instance.Package == null)
        {
            Debug.LogError($"[BasePool] GameRoot 或 Package 未初始化，无法加载: {objectName}");
            return;
        }
        Prefab = GameRoot.Instance.Package.LoadAssetAsync<GameObject>(ObjectName);
        
        Debug.Log($"[BasePool] 开始加载预制体: {ObjectName}");
    }
    
    
    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    public GameObject GetObj()
    {
        if (Prefab == null || !Prefab.IsValid)
        {
            Debug.LogWarning($"对象池 {ObjectName} 预制体还未加载完成");
            return null;
        }
        
        // 等待资源加载完成
        if (Prefab.IsDone == false)
        {
            Debug.LogWarning($"对象池 {ObjectName} 资源正在加载中...");
            return null;
        }
        
        GameObject obj;
        
        if (pool.Count > 0)
        {
            // 从池中取出对象
            obj = pool.Dequeue();
        }
        else
        {
            // 池中没有可用对象，创建新的
            obj = CreateNewObject();
        }
        
        obj.SetActive(true);
        
        // 调用 IRecycle 接口的 OnEnter 方法
        IRecycle recycle = obj.GetComponent<IRecycle>();
        if (recycle != null)
        {
            recycle.OnEnter();
        }
        return obj;
    }
    
    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    public void Recycle(GameObject obj)
    {
        if (obj == null) return;
        
        // 先禁用对象，避免在调用 OnExit 时继续执行 Update
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        // 回收时重置 Scale，防止下次取出时残留上次的修改
        obj.transform.localScale = prefabOriginalScale;
        
        // 调用 IRecycle 接口的 OnExit 方法做清理
        IRecycle recycle = obj.GetComponent<IRecycle>();
        if (recycle != null)
        {
            recycle.OnExit();
        }
        
        pool.Enqueue(obj);
    }
    
    /// <summary>
    /// 创建新对象
    /// </summary>
    private GameObject CreateNewObject()
    {
        GameObject obj = Prefab.InstantiateSync();
        obj.transform.SetParent(poolParent);
        obj.SetActive(false);
        // 首次实例化时记录 Prefab 原始 Scale
        if (!originalScaleRecorded)
        {
            prefabOriginalScale = obj.transform.localScale;
            originalScaleRecorded = true;
        }
        return obj;
    }
    
    /// <summary>
    /// 清空对象池
    /// </summary>
    public void Clear()
    {
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            Object.Destroy(obj);
        }
        
        if (poolParent != null)
        {
            Object.Destroy(poolParent.gameObject);
        }
    }
}
