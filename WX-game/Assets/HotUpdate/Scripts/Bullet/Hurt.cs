using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    public float BaseHurt = 10f;
    public string Name = "NormalBullet";
    public float DestroyTime = 1f;
    public float HurtRange = 1f;
    public int id = 0;
    public SkillType SkillType = SkillType.普攻;
    public HurtType HurtType = HurtType.Recycle;
    public float SkillHurtAmplification = 0f; // 单技能专属增伤倍率（叠加到 SubHp 里）
    void Start()
    {
        if (DestroyTime >= 0)
        {
            Invoke("DestroySelf", DestroyTime);
        }
        if(id!=0)
        {
            SkillsData itemData = DataManager.Instance.GetData<SkillsData>("skill_tbskill", id);
            if(itemData!=null)
            {
                BaseHurt = itemData.BaseHurt;
                HurtRange = itemData.HurtRange;
            }
        }
    }

    private void OnEnable()
    {
        // 每次从对象池取出时重新计时
        CancelInvoke("DestroySelf");
        if (DestroyTime >= 0)
        {
            Invoke("DestroySelf", DestroyTime);
        }
    }

    private void DestroySelf()
    {
        PoolManager.Instance.RecycleObj(Name, this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IEnemySetHP enemy = other.GetComponent<IEnemySetHP>();
        if (enemy != null)
        {
            enemy.EnemySubHP(BaseHurt + Random.Range(-HurtRange, HurtRange + 1f), this);
            if (HurtType == HurtType.Recycle)
            {
                CancelInvoke("DestroySelf"); // 防止双重回收
                PoolManager.Instance.RecycleObj(Name, gameObject);
            }
        }
    }
}
public enum HurtType
{
    Recycle,
    NotRecycle,
}
