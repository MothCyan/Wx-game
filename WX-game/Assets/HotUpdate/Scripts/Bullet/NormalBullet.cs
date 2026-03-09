using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class NormalBullet : MonoBehaviour, IRecycle
{
    // 子弹移动速度
    public float Speed = 10f;

    // 子弹存活时间（自动销毁）

    private float timer = 0f;

    public void OnEnter()
    {
        timer = 0f;
    }

    public void OnExit()
    {
        // OnExit 只做清理工作，不调用回收
        // 清理逻辑（如果需要）
    }
   



    void Update()
    {
        // 向前移动（图片朝右，使用 transform.right 作为前进方向）
        transform.position += transform.right * Speed * Time.deltaTime;

        // 计时器
        timer += Time.deltaTime;
        
    }
}
