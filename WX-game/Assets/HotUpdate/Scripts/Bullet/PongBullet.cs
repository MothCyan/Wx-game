using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBullet : MonoBehaviour, IRecycle
{
    // 子弹移动速度
    public float Speed = 10f;

    // 子弹存活时间（自动销毁）

    private float timer = 0f;
    void Update()
    {
        // 向前移动（图片朝右，使用 transform.right 作为前进方向）
        transform.position += transform.right * Speed * Time.deltaTime;

        // 计时器
        timer += Time.deltaTime;
        
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        GameObject pong=PoolManager.Instance.GetObj("Pong1");
        pong.transform.position = transform.position;

    }
}
