using System.Collections;
using UnityEngine;

/// <summary>
/// DataManager 使用示例：挂到任意 GameObject 上，填入 id 即可查到对应数据
/// </summary>
public class DataTest : MonoBehaviour
{
    public int itemId = 1001;

    IEnumerator Start()
    {
        // 如果表还没加载，先加载
        if (!DataManager.Instance.IsLoaded("demo_tbitem"))
            yield return DataManager.Instance.LoadTable<TbItem>(
                "demo_tbitem",
                json => JsonUtility.FromJson<TbItemWrapper>("{\"array\":" + json + "}").array
            );

        // 按 id 查数据
        TbItem item = DataManager.Instance.GetData<TbItem>("demo_tbitem", itemId);
        if (item != null)
            Debug.Log($"id={item.id}  名称={item.name}  描述={item.desc}  数量={item.count}");
    }
}
