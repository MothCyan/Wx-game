using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePool : MonoBehaviour
{
    public string ObjectName;
    public GameObject Prefab;
    void Awake()
    {
        //Prefab = HotUpdateLoad.Package.LoadAssetAsync<GameObject>(ObjectName).InstantiateSync();
    }
}
