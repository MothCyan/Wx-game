using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRecycle : MonoBehaviour, IRecycle
{
    public void OnEnter()
    {
        // 进入对象池时的初始化逻辑
    }

    public void OnExit()
    {
        // 退出对象池时的清理逻辑
    }
}
