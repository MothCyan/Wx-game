using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtText : MonoBehaviour, IRecycle
{
   
    public void ResRecycle()
    {
        PoolManager.Instance.RecycleObj("HurtText", this.gameObject);
    }

    public void OnEnter()
    {
       Invoke("ResRecycle", 1.5f);
    }

    public void OnExit()
    {
        
    }
}
