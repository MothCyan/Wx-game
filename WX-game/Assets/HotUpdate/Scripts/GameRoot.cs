using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class GameRoot : BaseSingleton<GameRoot>
{
    public ResourcePackage Package;
    void Start()
    {
        Package = YooAssets.TryGetPackage("MyPackage");
        if (Package == null)
        {
            Debug.LogError("资源包加载失败");
            return;
        }
        else
        {
            Debug.Log("资源包加载成功");
        }
        StartCoroutine(LoadAllTables());
    }

    private IEnumerator LoadAllTables()
    {
        yield return DataManager.Instance.LoadTable<SkillsData>(
            "skill_tbskill",
            json => JsonUtility.FromJson<SkillsDataWrapper>("{\"array\":" + json + "}").array
        );
        Debug.Log("[GameRoot] 所有数据表加载完成");
    }
}
