using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// 引入 TextMeshPro textMeshPro;

public abstract class BaseEnemy : MonoBehaviour, IRecycle, IEnemySetHP
{
    public EnemyData enemyData;
    public float CurrentHP = 3;
    public FSM fSM;

    private SpriteRenderer spriteRenderer;
    private Material flashMaterial;
    private Material originalMaterial;
    private Coroutine flashCoroutine;

    public void OnEnter()
    {
        // 初始化敌人数据
        CurrentHP = enemyData.MaxHP;
        // 缓存 SpriteRenderer 和闪白 Material
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            flashMaterial = new Material(Shader.Find("Custom/SpriteFlashWhite"));
        }
        // 初始化 FSM
        fSM = GetComponent<FSM>();
        fSM.SetBlackboardValue<float>("HP", CurrentHP);
        fSM.enemyType = enemyData.EnemyType;
        fSM.ChangeState(StateType.Move);
    }

    public void SubHp(float hp, Hurt hurt)
    {
        
        if(hurt != null)
        {
            if(hurt.SkillType == SkillType.普攻)
            {
                hp *= 1 + PlayerData.Instance.AttackPowerAmplification;
                hp *= 1 + PlayerData.Instance.NormalAttackAmplification;
            }
            else if(hurt.SkillType == SkillType.近程)
            {
                hp *= 1 + PlayerData.Instance.AttackPowerAmplification;
                hp *= 1 + PlayerData.Instance.ShortrangeSkillAmplification;
            }
            else if(hurt.SkillType == SkillType.远程)
            {
                hp *= 1 + PlayerData.Instance.AttackPowerAmplification;
                hp *= 1 + PlayerData.Instance.RemoteskillsAmplification;
            }
        }
        else
        {
            CurrentHP -= hp;
        }
            
        int damage = (int)hp;
        CurrentHP -= damage;
        if (fSM == null)
        {
            Debug.LogWarning($"敌人 {enemyData.EnemyName} 没有 FSM 组件");
        }
        fSM.SetBlackboardValue<float>("HP", CurrentHP);
        GameObject hurtText = PoolManager.Instance.GetObj("HurtText");
        float textScale = Mathf.Clamp(1f + (damage / 30f), 1f, 1.5f);
        hurtText.transform.localScale = new Vector3(textScale, textScale, 1);
        hurtText.transform.position = transform.position;

        hurtText.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

        // 触发闪白
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashWhite());
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null && flashMaterial != null)
        {
            flashMaterial.SetTexture("_MainTex", spriteRenderer.sprite.texture);
            flashMaterial.SetFloat("_FlashAmount", 0.8f);
            spriteRenderer.material = flashMaterial;
        }

        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
            spriteRenderer.material = originalMaterial;
    }

    public void OnExit()
    {
        if (spriteRenderer != null)
            spriteRenderer.material = originalMaterial;
    }

    public void EnemySubHP(float hp, Hurt hurt = null)
    {
        SubHp(hp, hurt);
    }
}
