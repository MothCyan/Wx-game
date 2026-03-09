using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : BaseSingleton<PoolManager>
{
    /// <summary>
    /// 字典存储所有对象池
    /// </summary>
    private Dictionary<string, BasePool> pools = new Dictionary<string, BasePool>();
    public bool CanGetObj;
    
    void Start()
    {
        StartCoroutine(InitPools());
    }
    
    IEnumerator InitPools()
    {
        // 等待 GameRoot 初始化完成
        while (GameRoot.Instance == null || GameRoot.Instance.Package == null)
        {
            yield return null;
        }
        
        // 预创建一些常用对象池
        yield return CreatePools("NormalBullet");
        yield return CreatePools("NormalEnemy");
        yield return CreatePools("HurtText");
        yield return CreatePools("Skill1");
        yield return CreatePools("Skill3");
        yield return CreatePools("Pong1");
        yield return CreatePools("PongBullet");
        yield return CreatePools("Bat");
        yield return CreatePools("Skill8");
        yield return CreatePools("Skill9");
        yield return CreatePools("Skill12");
        CanGetObj = true;

        Debug.Log("[PoolManager] 对象池初始化完成");
    }
    
    public GameObject GetObj(string objectName)
    {
        if (pools.TryGetValue(objectName, out BasePool pool))
        {
            return pool.GetObj();
        }
        else
        {
            // 对象池不存在且还未初始化完成，不允许临时创建
            Debug.LogWarning($"[PoolManager] 对象池 '{objectName}' 不存在，请在 InitPools 中预注册");
            return null;
        }
    }
    public IEnumerator CreatePools(string Name)
    {
        if(pools.TryGetValue(Name, out BasePool pool))
        {
            yield return null;
        }
        pool = new BasePool(Name);
        pools[Name] = pool;
        yield return null;
    }
    
        
    
    public void RecycleObj(string objectName, GameObject obj)
    {
        if (pools.TryGetValue(objectName, out BasePool pool))
        {
            pool.Recycle(obj);
        }
    }
}
